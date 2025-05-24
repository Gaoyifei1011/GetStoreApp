using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.System.Threading;
using Windows.Web.Http;

namespace GetStoreApp.Services.Download
{
    /// <summary>
    /// Aria2 下载服务
    /// </summary>
    public static class Aria2Service
    {
        private static readonly string aria2FilePath = Path.Combine(InfoHelper.AppInstalledLocation, "Mile.Aria2.exe");
        private static readonly string defaultAria2Arguments = "-c --enable-rpc=true --rpc-allow-origin-all=true --rpc-listen-all=true --rpc-listen-port=6300 --stop-with-process={0} -D";
        private static readonly string rpcServerLink = "http://127.0.0.1:6300/jsonrpc";
        private static string aria2Arguments;
        private static ThreadPoolTimer downloadSchedulerTimer;

        private static SemaphoreSlim Aria2SemaphoreSlim { get; set; } = new(1, 1);

        private static Dictionary<string, string> Aria2DownloadDict { get; } = [];

        public static string Aria2ConfPath { get; } = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Aria2.conf");

        public static event Action<DownloadProgress> DownloadProgress;

        /// 初始化Aria2配置文件
        /// </summary>
        public static void InitializeAria2Conf()
        {
            try
            {
                // 原配置文件存在且新的配置文件不存在，拷贝到指定目录
                if (!File.Exists(Aria2ConfPath))
                {
                    byte[] mileAria2 = ResourceService.GetEmbeddedData("Files/Assets/Embed/Mile.Aria2.conf");
                    FileStream fileStream = new(Aria2ConfPath, FileMode.Create);
                    fileStream.Write(mileAria2, 0, mileAria2.Length);
                    fileStream.Flush();
                    fileStream.Close();
                }

                // 使用自定义的配置文件目录
                aria2Arguments = string.Format("--conf-path=\"{0}\" --stop-with-process={1} -D", Aria2ConfPath, Environment.ProcessId);
            }
            //  发生异常时，使用默认的参数
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Aria2 config fileStream save failed.", e);
                aria2Arguments = string.Format(defaultAria2Arguments, Environment.ProcessId);
            }
        }

        /// <summary>
        /// 初始化运行 Aria2 下载进程和下载监控服务
        /// </summary>
        public static void Initialize()
        {
            Task.Run(() =>
            {
                try
                {
                    Shell32Library.ShellExecute(nint.Zero, "open", aria2FilePath, aria2Arguments, null, WindowShowStyle.SW_HIDE);
                    downloadSchedulerTimer = ThreadPoolTimer.CreatePeriodicTimer(OnTimerElapsed, TimeSpan.FromSeconds(1));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 关闭 Aria2 下载监控服务
        /// </summary>
        public static void Release()
        {
            Task.Run(() =>
            {
                try
                {
                    downloadSchedulerTimer?.Cancel();
                    Aria2SemaphoreSlim?.Dispose();
                    Aria2SemaphoreSlim = null;
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 判断Aria2 rpc 端口是否存在
        /// </summary>
        public static async Task<bool> IsAria2ExistedAsync()
        {
            try
            {
                JsonObject versionObject = new()
                {
                    ["jsonrpc"] = JsonValue.CreateStringValue("2.0"),
                    ["id"] = JsonValue.CreateStringValue(string.Empty),
                    ["method"] = JsonValue.CreateStringValue("aria2.getVersion")
                };

                string versionString = versionObject.Stringify();
                byte[] contentBytes = Encoding.UTF8.GetBytes(versionString);
                HttpStringContent httpContent = new(versionString);
                httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";
                HttpClient httpClient = new();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Get Aria2 state failed", e);
                return false;
            }
        }

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static void CreateDownload(string url, string saveFilePath)
        {
            Task.Run(async () =>
            {
                try
                {
                    // 判断下载进程是否存在
                    if (await IsAria2ExistedAsync())
                    {
                        JsonObject jsonObject = new()
                        {
                            ["jsonrpc"] = JsonValue.CreateStringValue("2.0"),
                            ["id"] = JsonValue.CreateStringValue(string.Empty),
                            ["method"] = JsonValue.CreateStringValue("aria2.addUri"),
                            ["params"] = new JsonArray()
                            {
                                new JsonArray() { JsonValue.CreateStringValue(url) },
                                new JsonObject()
                                {
                                    ["dir"] = JsonValue.CreateStringValue(Path.GetDirectoryName(saveFilePath)),
                                    ["out"] = JsonValue.CreateStringValue(Path.GetFileName(saveFilePath))
                                }
                            }
                        };

                        string createDownloadString = jsonObject.Stringify();
                        byte[] contentBytes = Encoding.UTF8.GetBytes(createDownloadString);
                        HttpStringContent httpContent = new(createDownloadString);
                        httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                        httpContent.Headers.ContentType.CharSet = "utf-8";
                        HttpClient httpClient = new();
                        HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), httpContent);

                        // 请求成功
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            JsonObject resultObject = JsonObject.Parse(responseContent);
                            string gid = resultObject.GetNamedString("result");

                            Aria2SemaphoreSlim?.Wait();

                            try
                            {
                                Aria2DownloadDict.TryAdd(gid, saveFilePath);
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                Aria2SemaphoreSlim?.Release();
                            }

                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = gid,
                                DownloadProgressState = DownloadProgressState.Queued,
                                FileName = Path.GetFileName(saveFilePath),
                                FilePath = saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = 0,
                                TotalSize = 0,
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Create download failed.", e);
                }
            });
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        public static void ContinueDownload(string downloadID)
        {
            Task.Run(async () =>
            {
                try
                {
                    // 判断下载进程是否存在
                    if (await IsAria2ExistedAsync())
                    {
                        JsonObject jsonObject = new()
                        {
                            ["jsonrpc"] = JsonValue.CreateStringValue("2.0"),
                            ["id"] = JsonValue.CreateStringValue(string.Empty),
                            ["method"] = JsonValue.CreateStringValue("aria2.unpause"),
                            ["params"] = new JsonArray()
                            {
                                JsonValue.CreateStringValue(downloadID),
                            }
                        };

                        string pauseDownloadString = jsonObject.Stringify();
                        byte[] contentBytes = Encoding.UTF8.GetBytes(pauseDownloadString);
                        HttpStringContent httpContent = new(pauseDownloadString);
                        httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                        httpContent.Headers.ContentType.CharSet = "utf-8";
                        HttpClient httpClient = new();
                        HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), httpContent);

                        // 请求成功
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            JsonObject resultObject = JsonObject.Parse(responseContent);
                            string gid = resultObject.GetNamedString("result");

                            Aria2SemaphoreSlim?.Wait();

                            try
                            {
                                if (Aria2DownloadDict.TryGetValue(gid, out string saveFilePath))
                                {
                                    DownloadProgress?.Invoke(new DownloadProgress()
                                    {
                                        DownloadID = gid,
                                        DownloadProgressState = DownloadProgressState.Queued,
                                        FileName = Path.GetFileName(saveFilePath),
                                        FilePath = saveFilePath,
                                        DownloadSpeed = 0,
                                        CompletedSize = 0,
                                        TotalSize = 0,
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                Aria2SemaphoreSlim?.Release();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Continue download failed.", e);
                }
            });
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public static void PauseDownload(string downloadID)
        {
            Task.Run(async () =>
            {
                try
                {
                    // 判断下载进程是否存在
                    if (await IsAria2ExistedAsync())
                    {
                        JsonObject jsonObject = new()
                        {
                            ["jsonrpc"] = JsonValue.CreateStringValue("2.0"),
                            ["id"] = JsonValue.CreateStringValue(string.Empty),
                            ["method"] = JsonValue.CreateStringValue("aria2.forcePause"),
                            ["params"] = new JsonArray()
                            {
                                JsonValue.CreateStringValue(downloadID),
                            }
                        };

                        string pauseDownloadString = jsonObject.Stringify();
                        byte[] contentBytes = Encoding.UTF8.GetBytes(pauseDownloadString);
                        HttpStringContent httpContent = new(pauseDownloadString);
                        httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                        httpContent.Headers.ContentType.CharSet = "utf-8";
                        HttpClient httpClient = new();
                        HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), httpContent);

                        // 请求成功
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            JsonObject resultObject = JsonObject.Parse(responseContent);
                            string gid = resultObject.GetNamedString("result");

                            if (Aria2DownloadDict.TryGetValue(gid, out string saveFilePath))
                            {
                                DownloadProgress?.Invoke(new DownloadProgress()
                                {
                                    DownloadID = gid,
                                    DownloadProgressState = DownloadProgressState.Paused,
                                    FileName = Path.GetFileName(saveFilePath),
                                    FilePath = saveFilePath,
                                    DownloadSpeed = 0,
                                    CompletedSize = 0,
                                    TotalSize = 0,
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Pause download failed.", e);
                }
            });
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        public static void DeleteDownload(string downloadID)
        {
            Task.Run(async () =>
            {
                try
                {
                    // 判断下载进程是否存在
                    if (await IsAria2ExistedAsync())
                    {
                        JsonObject jsonObject = new()
                        {
                            ["jsonrpc"] = JsonValue.CreateStringValue("2.0"),
                            ["id"] = JsonValue.CreateStringValue(string.Empty),
                            ["method"] = JsonValue.CreateStringValue("aria2.forceRemove"),
                            ["params"] = new JsonArray()
                            {
                                JsonValue.CreateStringValue(downloadID),
                            }
                        };

                        string deleteDownloadString = jsonObject.Stringify();
                        byte[] contentBytes = Encoding.UTF8.GetBytes(deleteDownloadString);
                        HttpStringContent httpContent = new(deleteDownloadString);
                        httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                        httpContent.Headers.ContentType.CharSet = "utf-8";
                        HttpClient httpClient = new();
                        HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), httpContent);

                        // 请求成功
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            JsonObject resultObject = JsonObject.Parse(responseContent);
                            string gid = resultObject.GetNamedString("result");

                            Aria2SemaphoreSlim?.Wait();

                            try
                            {
                                if (Aria2DownloadDict.TryGetValue(gid, out string saveFilePath))
                                {
                                    Aria2DownloadDict.Remove(gid);
                                    DownloadProgress?.Invoke(new DownloadProgress()
                                    {
                                        DownloadID = gid,
                                        DownloadProgressState = DownloadProgressState.Deleted,
                                        FileName = Path.GetFileName(saveFilePath),
                                        FilePath = saveFilePath,
                                        DownloadSpeed = 0,
                                        CompletedSize = 0,
                                        TotalSize = 0,
                                    });
                                }

                                await RemoveResultAsync();
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                Aria2SemaphoreSlim?.Release();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Delete download failed.", e);
                }
            });
        }

        /// <summary>
        /// 汇报下载任务状态信息
        /// </summary>
        private static async Task<(bool, DownloadProgressState, double, double, double)> TellStatusAsync(string downloadID)
        {
            bool isTellStatusSuccessfully = false;
            DownloadProgressState downloadProgressState = DownloadProgressState.Failed;
            double completedSize = 0;
            double totalSize = 0;
            double downloadSpeed = 0;

            try
            {
                if (await IsAria2ExistedAsync())
                {
                    JsonObject jsonObject = new()
                    {
                        ["jsonrpc"] = JsonValue.CreateStringValue("2.0"),
                        ["id"] = JsonValue.CreateStringValue(string.Empty),
                        ["method"] = JsonValue.CreateStringValue("aria2.tellStatus"),
                        ["params"] = new JsonArray()
                        {
                            JsonValue.CreateStringValue(downloadID),
                            new JsonArray()
                            {
                                JsonValue.CreateStringValue("gid"),
                                JsonValue.CreateStringValue("status"),
                                JsonValue.CreateStringValue("totalLength"),
                                JsonValue.CreateStringValue("completedLength"),
                                JsonValue.CreateStringValue("downloadSpeed")
                            }
                        }
                    };

                    string tellStatusString = jsonObject.Stringify();
                    byte[] contentBytes = Encoding.UTF8.GetBytes(tellStatusString);
                    HttpStringContent httpContent = new(tellStatusString);
                    httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                    httpContent.Headers.ContentType.CharSet = "utf-8";
                    HttpClient httpClient = new();
                    HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), httpContent);

                    // 请求成功
                    if (response.IsSuccessStatusCode)
                    {
                        isTellStatusSuccessfully = true;

                        string responseContent = await response.Content.ReadAsStringAsync();
                        JsonObject resultObject = JsonObject.Parse(responseContent);
                        JsonObject downloadResultObject = resultObject.GetNamedObject("result");
                        string status = downloadResultObject.GetNamedString("status");
                        completedSize = Convert.ToDouble(downloadResultObject.GetNamedString("completedLength"));
                        totalSize = Convert.ToDouble(downloadResultObject.GetNamedString("totalLength"));
                        downloadSpeed = Convert.ToDouble(downloadResultObject.GetNamedString("downloadSpeed"));

                        if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadProgressState = DownloadProgressState.Downloading;
                        }
                        else if (string.Equals(status, "waiting", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadProgressState = DownloadProgressState.Queued;
                        }
                        else if (string.Equals(status, "paused", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadProgressState = DownloadProgressState.Paused;
                        }
                        else if (string.Equals(status, "error", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadProgressState = DownloadProgressState.Failed;
                        }
                        else if (string.Equals(status, "complete", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadProgressState = DownloadProgressState.Finished;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Get download state failed", e);
            }

            return ValueTuple.Create(isTellStatusSuccessfully, downloadProgressState, completedSize, totalSize, downloadSpeed);
        }

        /// <summary>
        /// 获取下载进度状态
        /// </summary>
        private static async void OnTimerElapsed(ThreadPoolTimer threadPoolTimer)
        {
            Aria2SemaphoreSlim?.Wait();

            try
            {
                List<string> finishedDownloadKeyList = [];

                foreach (KeyValuePair<string, string> aria2DownloadItem in Aria2DownloadDict)
                {
                    (bool isTellStausSuccessfully, DownloadProgressState downloadProgressState, double completedSize, double totalSize, double downloadSpeed) = await TellStatusAsync(aria2DownloadItem.Key);

                    if (isTellStausSuccessfully)
                    {
                        DownloadProgress?.Invoke(new DownloadProgress()
                        {
                            DownloadID = aria2DownloadItem.Key,
                            DownloadProgressState = downloadProgressState,
                            FileName = Path.GetFileName(aria2DownloadItem.Value),
                            FilePath = aria2DownloadItem.Value,
                            DownloadSpeed = downloadSpeed,
                            CompletedSize = completedSize,
                            TotalSize = totalSize,
                        });

                        // 任务下载失败或完成时从 Aria2 进程中删除队列任务
                        if (downloadProgressState is DownloadProgressState.Failed || downloadProgressState is DownloadProgressState.Finished)
                        {
                            finishedDownloadKeyList.Add(aria2DownloadItem.Key);
                        }
                    }
                }

                foreach (string finishedDownloadKey in finishedDownloadKeyList)
                {
                    Aria2DownloadDict.Remove(finishedDownloadKey);
                }

                await RemoveResultAsync();
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                Aria2SemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 移除下载完成结果
        /// </summary>
        private static async Task RemoveResultAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    // 判断下载进程是否存在
                    if (await IsAria2ExistedAsync())
                    {
                        JsonObject jsonObject = new()
                        {
                            ["jsonrpc"] = JsonValue.CreateStringValue("2.0"),
                            ["id"] = JsonValue.CreateStringValue(string.Empty),
                            ["method"] = JsonValue.CreateStringValue("aria2.purgeDownloadResult"),
                        };

                        string removeResultString = jsonObject.Stringify();
                        byte[] contentBytes = Encoding.UTF8.GetBytes(removeResultString);
                        HttpStringContent httpContent = new(removeResultString);
                        httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                        httpContent.Headers.ContentType.CharSet = "utf-8";
                        HttpClient httpClient = new();
                        await httpClient.PostAsync(new Uri(rpcServerLink), httpContent);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Delete download failed.", e);
                }
            });
        }
    }
}
