using GetStoreApp.Helpers.Controls.Download;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.Web.Http;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// Aria2下载服务
    /// </summary>
    public static class Aria2Service
    {
        private static string aria2FilePath = Path.Combine(InfoHelper.AppInstalledLocation, "Mile.Aria2.exe");
        private static string aria2Arguments;
        private static string defaultAria2Arguments = "-c --fileStream-allocation=none --max-concurrent-downloads=3 --max-connection-per-server=5 --min-split-size=10M --split=5 --enable-rpc=true --rpc-allow-origin-all=true --rpc-listen-all=true --rpc-listen-port=6300 --stop-with-process={0} -D";
        private static string rpcServerLink = "http://127.0.0.1:6300/jsonrpc";

        public static string Aria2ConfPath { get; } = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Aria2.conf");

        /// <summary>
        /// 初始化Aria2配置文件
        /// </summary>
        public static async Task InitializeAria2ConfAsync()
        {
            User32Library.GetWindowThreadProcessId((IntPtr)MainWindow.Current.AppWindow.Id.Value, out uint processId);
            try
            {
                // 原配置文件存在且新的配置文件不存在，拷贝到指定目录
                if (!File.Exists(Aria2ConfPath))
                {
                    byte[] mileAria2 = await ResourceService.GetEmbeddedDataAsync("Files/EmbedAssets/Mile.Aria2.conf");
                    FileStream file = new FileStream(Aria2ConfPath, FileMode.Create);
                    file.Write(mileAria2, 0, mileAria2.Length);
                    file.Flush();
                    file.Close();
                }

                // 使用自定义的配置文件目录
                aria2Arguments = string.Format("--conf-path=\"{0}\" --stop-with-process={1} -D", Aria2ConfPath, processId);
            }
            //  发生异常时，使用默认的参数
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Aria2 config fileStream save failed.", e);
                aria2Arguments = string.Format(defaultAria2Arguments, processId);
            }
        }

        /// <summary>
        /// 初始化运行Aria2下载进程
        /// </summary>
        public static void StartAria2Process()
        {
            Aria2ProcessHelper.RunAria2Process(aria2FilePath, aria2Arguments);
        }

        /// <summary>
        /// 恢复配置文件默认值
        /// </summary>
        public static async Task RestoreDefaultAsync()
        {
            try
            {
                // 原配置文件存在时，覆盖已经修改的配置文件
                byte[] mileAria2 = await ResourceService.GetEmbeddedDataAsync("Files/EmbedAssets/Mile.Aria2.conf");
                FileStream fileStream = new FileStream(Aria2ConfPath, FileMode.Create);
                fileStream.Write(mileAria2, 0, mileAria2.Length);
                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();

                // 使用自定义的配置文件目录
                aria2Arguments = string.Format("--conf-path=\"{0}\" -D", Aria2ConfPath);
            }
            //  发生异常时，使用默认的参数
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Aria2 config fileStream save failed.", e);
                aria2Arguments = defaultAria2Arguments;
            }
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public static async Task<(bool, string)> AddUriAsync(string fileName, string downloadLink, string folderPath)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    return (false, string.Empty);
                }

                // 创建AddUri Json字符串对象
                JsonObject addUriObject = new JsonObject();

                // Aria2 请求的消息Id
                addUriObject["id"] = JsonValue.CreateStringValue(string.Empty);
                // 固定值
                addUriObject["jsonrpc"] = JsonValue.CreateStringValue("2.0");
                // Aria2 Json RPC对应的方法名
                addUriObject["method"] = JsonValue.CreateStringValue("aria2.addUri");

                // 创建子参数Json字符串对象。
                JsonObject subParamObject = new JsonObject();
                subParamObject["dir"] = JsonValue.CreateStringValue(folderPath);
                subParamObject["out"] = JsonValue.CreateStringValue(fileName);

                // 创建参数数组：
                // 第一个参数为数组，是下载文件的Url
                // 第二个参数是Json字符串对象，成员为下载参数
                JsonArray paramsArray = new JsonArray()
                {
                    new JsonArray(){ JsonValue.CreateStringValue(downloadLink) },
                    subParamObject
                };

                addUriObject["params"] = paramsArray;

                // 将下载信息转换为Json字符串
                string addUriString = addUriObject.Stringify();

                // 使用Aria2 Json RPC接口添加下载任务指令
                byte[] contentBytes = Encoding.UTF8.GetBytes(addUriString);

                HttpStringContent httpContent = new HttpStringContent(addUriString);
                httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();

                    JsonObject ResultObject = JsonObject.Parse(ResponseContent);
                    return (true, ResultObject.GetNamedString("result"));
                }
                // 请求失败
                else
                {
                    return (false, string.Empty);
                }
            }

            // 捕捉因访问超时引发的异常
            catch (OperationCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "Add download task canceled.", e);
                return (false, string.Empty);
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Add download task failed.", e);
                return (false, string.Empty);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// 暂停下载选定的任务
        /// </summary>
        public static async Task<(bool, string)> PauseAsync(string GID)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    return (false, string.Empty);
                }

                // 创建Pause Json字符串对象
                JsonObject pauseObject = new JsonObject();

                // Aria2 请求的消息Id
                pauseObject["id"] = JsonValue.CreateStringValue(string.Empty);
                // 固定值
                pauseObject["jsonrpc"] = JsonValue.CreateStringValue("2.0");
                // Aria2 Json RPC对应的方法名
                pauseObject["method"] = JsonValue.CreateStringValue("aria2.forceRemove");

                // 创建参数数组
                JsonArray paramsArray = new JsonArray()
                {
                    JsonValue.CreateStringValue(GID)
                };

                pauseObject["params"] = paramsArray;

                // 将暂停信息转换为Json字符串
                string pauseString = pauseObject.Stringify();

                // 使用Aria2 Json RPC接口添加暂停下载任务指令
                byte[] contentBytes = Encoding.UTF8.GetBytes(pauseString);

                HttpStringContent httpContent = new HttpStringContent(pauseString);
                httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();

                    JsonObject ResultObject = JsonObject.Parse(ResponseContent);
                    return (true, ResultObject.GetNamedString("result"));
                }
                // 请求失败
                else
                {
                    return (false, string.Empty);
                }
            }

            // 捕捉因访问超时引发的异常
            catch (OperationCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "Pause download task canceled.", e);
                return (false, string.Empty);
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Pause download task failed.", e);
                return (false, string.Empty);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// 取消下载选定的任务
        /// </summary>
        public static async Task<(bool, string)> DeleteAsync(string GID)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    return (false, string.Empty);
                }

                // 创建Delete Json字符串对象
                JsonObject deleteObject = new JsonObject();

                // Aria2 请求的消息Id
                deleteObject["id"] = JsonValue.CreateStringValue(string.Empty);
                // 固定值
                deleteObject["jsonrpc"] = JsonValue.CreateStringValue("2.0");
                // Aria2 Json RPC对应的方法名
                deleteObject["method"] = JsonValue.CreateStringValue("aria2.forceRemove");

                // 创建参数数组
                JsonArray paramsArray = new JsonArray()
                {
                    JsonValue.CreateStringValue(GID)
                };

                deleteObject["params"] = paramsArray;

                // 将删除信息转换为Json字符串
                string deleteString = deleteObject.Stringify();

                // 使用Aria2 Json RPC接口添加暂停下载任务指令
                byte[] contentBytes = Encoding.UTF8.GetBytes(deleteString);

                HttpStringContent httpContent = new HttpStringContent(deleteString);
                httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();

                    JsonObject ResultObject = JsonObject.Parse(ResponseContent);
                    return (true, ResultObject.GetNamedString("result"));
                }
                // 请求失败
                else
                {
                    return (false, string.Empty);
                }
            }

            // 捕捉因访问超时引发的异常
            catch (OperationCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "Delete download task canceled.", e);
                return (false, string.Empty);
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Delete download task failed.", e);
                return (false, string.Empty);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// 汇报下载任务状态信息
        /// </summary>
        public static async Task<(bool, string, double, double, double)> TellStatusAsync(string GID)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    return (false, string.Empty, default(double), default(double), default(double));
                }

                // 创建TellStatus Json字符串对象
                JsonObject tellStatusObject = new JsonObject();

                // Aria2 请求的消息Id
                tellStatusObject["id"] = JsonValue.CreateStringValue(string.Empty);
                // 固定值
                tellStatusObject["jsonrpc"] = JsonValue.CreateStringValue("2.0");
                // Aria2 Json RPC对应的方法名
                tellStatusObject["method"] = JsonValue.CreateStringValue("aria2.tellStatus");

                // 创建子参数数组
                JsonArray subParamArray = new JsonArray()
                {
                    JsonValue.CreateStringValue("gid"),
                    JsonValue.CreateStringValue("status"),
                    JsonValue.CreateStringValue("totalLength"),
                    JsonValue.CreateStringValue("completedLength"),
                    JsonValue.CreateStringValue("downloadSpeed")
                };

                // 创建参数数组
                // 第一个参数为数组，是要获取信息状态的Gid
                // 第二个参数为数组，是获取的内容名称
                JsonArray paramsArray = new JsonArray()
                {
                    JsonValue.CreateStringValue(GID),
                    subParamArray
                };

                tellStatusObject["params"] = paramsArray;

                // 将获取状态信息转换为Json字符串
                string tellStatusString = tellStatusObject.Stringify();

                // 使用Aria2 Json RPC接口添加获取状态指令
                byte[] contentBytes = Encoding.UTF8.GetBytes(tellStatusString);

                HttpStringContent httpContent = new HttpStringContent(tellStatusString);
                httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(rpcServerLink), httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    JsonObject resultObject = JsonObject.Parse(responseContent);
                    JsonObject downloadResultObject = resultObject.GetNamedObject("result");

                    return (true,
                        downloadResultObject.GetNamedString("status"),
                        Convert.ToDouble(downloadResultObject.GetNamedString("completedLength")),
                        Convert.ToDouble(downloadResultObject.GetNamedString("totalLength")),
                        Convert.ToDouble(downloadResultObject.GetNamedString("downloadSpeed"))
                        );
                }
                // 请求失败
                else
                {
                    return (false, string.Empty, default(double), default(double), default(double));
                }
            }

            // 捕捉因访问超时引发的异常
            catch (OperationCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "Get download status canceled.", e);
                return (false, string.Empty, default(double), default(double), default(double));
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Get download status failed.", e);
                return (false, string.Empty, default(double), default(double), default(double));
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
    }
}
