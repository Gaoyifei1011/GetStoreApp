using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Download
{
    /// <summary>
    /// Aria2下载服务
    /// </summary>
    public class Aria2Service : IAria2Service
    {
        private string Aria2Path => Path.Combine(AppContext.BaseDirectory, "Aria2\\aria2c.exe");

        private string Aria2ConfPath => Path.Combine(AppContext.BaseDirectory, "Aria2\\Config\\aria2.conf");

        private string RPCServerLink => "http://127.0.0.1:6300/jsonrpc";

        // 获取汇报下载任务状态信息内容的具体选项列表
        private List<string> TellStatusInfoList = new List<string>()
        {
            "gid",
            "status",
            "totalLength",
            "completedLength",
            "downloadSpeed"
        };

        // 添加下载任务内容
        private Dictionary<string, object> AddTaskContent = new Dictionary<string, object>()
        {
            { "id", string.Empty },
            { "jsonrpc", "2.0" },
            { "method", "aria2.addUri" }
        };

        // 继续下载任务内容
        private Dictionary<string, object> ContinueAllContent = new Dictionary<string, object>()
        {
            { "id", string.Empty },
            { "jsonrpc", "2.0" },
            { "method", "aria2.unpause" }
        };

        // 暂停下载任务内容
        private Dictionary<string, object> PauseAllContent = new Dictionary<string, object>()
        {
            { "id", string.Empty },
            { "jsonrpc", "2.0" },
            { "method", "aria2.pause" }
        };

        // 删除选定的下载项目内容
        private Dictionary<string, object> DeleteSeletedContent = new Dictionary<string, object>()
        {
            { "id", string.Empty },
            { "jsonrpc", "2.0" },
            { "method", "aria2.remove" }
        };

        // 继续下载任务内容
        private Dictionary<string, object> ContinueDownloadContent = new Dictionary<string, object>()
        {
            { "id", string.Empty },
            { "jsonrpc", "2.0" },
            { "method", "aria2.unpause" }
        };

        // 暂停下载任务内容
        private Dictionary<string, object> PauseDownloadContent = new Dictionary<string, object>()
        {
            { "id", string.Empty },
            { "jsonrpc", "2.0" },
            { "method", "aria2.pause" }
        };

        // 删除下载任务内容
        private Dictionary<string, object> DeleteTaskContent = new Dictionary<string, object>()
        {
            { "id", string.Empty },
            { "jsonrpc", "2.0" },
            { "method", "aria2.remove" }
        };

        // 汇报下载任务状态内容
        private Dictionary<string, object> TellStatusContent = new Dictionary<string, object>()
        {
            { "id", string.Empty },
            { "jsonrpc", "2.0" },
            { "method", "aria2.tellStatus" }
        };

        /// <summary>
        /// 初始化运行Aria2下载进程
        /// </summary>
        public async Task InitializeAria2Async()
        {
            string Aria2ExecuteCmd = string.Format("{0} --conf-path=\"{1}\" -D", Aria2Path, Aria2ConfPath);

            await Aria2ProcessHelper.RunCmdAsync();
            await Aria2ProcessHelper.ExecuteCmdAsync(Aria2ExecuteCmd);
        }

        /// <summary>
        /// 关闭Aria2进程
        /// </summary>
        public async Task CloseAria2Async()
        {
            int ProcessID = Aria2ProcessHelper.GetProcessID();
            Aria2ProcessHelper.KillProcessAndChildren(ProcessID);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public async Task<string> AddUriAsync(string downloadLink, string folderPath, string fileName)
        {
            // 下载配置内容
            Dictionary<string, string> DownloadConfiguration = new Dictionary<string, string>()
            {
                { "dir", folderPath },
                { "out", fileName }
            };

            // 成功添加任务返回的信息
            Dictionary<string, string> ResultContent;
            // 添加下载链接列表
            List<string> UrlList = new List<string>() { downloadLink };
            // 添加下载参数列表
            List<object> ParamsList = new List<object>();

            // 添加下载信息
            ParamsList.Add(UrlList);
            ParamsList.Add(DownloadConfiguration);
            AddTaskContent["params"] = ParamsList;

            // 将下载信息转换为字符串
            string AddTaskString = JsonConvert.SerializeObject(AddTaskContent);

            // 使用Aria2 RPC接口添加下载任务指令
            try
            {
                byte[] ContentBytes = Encoding.UTF8.GetBytes(AddTaskString);

                HttpContent httpContent = new StringContent(AddTaskString);
                httpContent.Headers.ContentLength = ContentBytes.Length;
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(RPCServerLink),
                    Timeout = new TimeSpan(0, 0, 30)
                };

                HttpResponseMessage response = await httpClient.PostAsync(RPCServerLink, httpContent);

                // 返回成功添加任务的GID信息
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();
                    ResultContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(ResponseContent);
                    return ResultContent["result"];
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 继续下载全部任务
        /// </summary>
        public async Task<List<string>> ContinueAllAsync(List<string> GIDList)
        {
            // 成功添加任务返回的信息
            Dictionary<string, string> ResultContent;
            // 继续下载参数列表
            List<string> ParamsList = new List<string>();
            // 成功添加任务信息的GID列表
            List<string> ResultGIDList = new List<string>();

            // 使用Aria2 RPC接口添加继续下载任务指令
            try
            {
                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(RPCServerLink),
                    Timeout = new TimeSpan(0, 0, 30)
                };

                foreach (string GID in GIDList)
                {
                    ParamsList.Clear();
                    ParamsList.Add(GID);
                    ContinueAllContent["params"] = ParamsList;

                    // 将继续下载任务信息转换为字符串
                    string ContinueAllString = JsonConvert.SerializeObject(ContinueAllContent);

                    byte[] ContentBytes = Encoding.UTF8.GetBytes(ContinueAllString);

                    HttpContent httpContent = new StringContent(ContinueAllString);
                    httpContent.Headers.ContentLength = ContentBytes.Length;
                    httpContent.Headers.ContentType.CharSet = "utf-8";

                    HttpResponseMessage response = await httpClient.PostAsync(RPCServerLink, httpContent);

                    // 返回成功添加任务的GID信息
                    if (response.IsSuccessStatusCode)
                    {
                        string ResponseContent = await response.Content.ReadAsStringAsync();
                        ResultContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(ResponseContent);
                        ResultGIDList.Add(ResultContent["result"]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                return ResultGIDList;
            }
            catch (Exception)
            {
                return ResultGIDList;
            }
        }

        /// <summary>
        /// 暂停下载全部任务
        /// </summary>
        public async Task<List<string>> PauseAllAsync(List<string> GIDList)
        {
            // 成功添加任务返回的信息
            Dictionary<string, string> ResultContent;
            // 暂停下载参数列表
            List<string> ParamsList = new List<string>();
            // 成功添加任务信息的GID列表
            List<string> ResultGIDList = new List<string>();

            // 使用Aria2 RPC接口添加暂停下载任务指令
            try
            {
                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(RPCServerLink),
                    Timeout = new TimeSpan(0, 0, 30)
                };

                foreach (string GID in GIDList)
                {
                    ParamsList.Clear();
                    ParamsList.Add(GID);
                    PauseAllContent["params"] = ParamsList;

                    // 将暂停下载任务信息转换为字符串
                    string ContinueAllString = JsonConvert.SerializeObject(PauseAllContent);

                    byte[] ContentBytes = Encoding.UTF8.GetBytes(ContinueAllString);

                    HttpContent httpContent = new StringContent(ContinueAllString);
                    httpContent.Headers.ContentLength = ContentBytes.Length;
                    httpContent.Headers.ContentType.CharSet = "utf-8";

                    HttpResponseMessage response = await httpClient.PostAsync(RPCServerLink, httpContent);

                    // 返回成功添加任务的GID信息
                    if (response.IsSuccessStatusCode)
                    {
                        string ResponseContent = await response.Content.ReadAsStringAsync();
                        ResultContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(ResponseContent);
                        ResultGIDList.Add(ResultContent["result"]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                return ResultGIDList;
            }
            catch (Exception)
            {
                return ResultGIDList;
            }
        }

        /// <summary>
        /// 多选模式下删除选择的任务
        /// </summary>
        public async Task<List<string>> DeleteSelectedAsync(List<string> GIDList)
        {
            // 成功添加任务返回的信息
            Dictionary<string, string> ResultContent;
            // 删除选定的下载项目参数列表
            List<string> ParamsList = new List<string>();
            // 成功添加任务信息的GID列表
            List<string> ResultGIDList = new List<string>();

            // 使用Aria2 RPC接口添加删除选定的下载项目任务指令
            try
            {
                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(RPCServerLink),
                    Timeout = new TimeSpan(0, 0, 30)
                };

                foreach (string GID in GIDList)
                {
                    ParamsList.Clear();
                    ParamsList.Add(GID);
                    DeleteSeletedContent["params"] = ParamsList;

                    // 将删除选定的下载项目任务信息转换为字符串
                    string DeleteSelectedString = JsonConvert.SerializeObject(DeleteSeletedContent);

                    byte[] ContentBytes = Encoding.UTF8.GetBytes(DeleteSelectedString);

                    HttpContent httpContent = new StringContent(DeleteSelectedString);
                    httpContent.Headers.ContentLength = ContentBytes.Length;
                    httpContent.Headers.ContentType.CharSet = "utf-8";

                    HttpResponseMessage response = await httpClient.PostAsync(RPCServerLink, httpContent);

                    // 返回成功添加任务的GID信息
                    if (response.IsSuccessStatusCode)
                    {
                        string ResponseContent = await response.Content.ReadAsStringAsync();
                        ResultContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(ResponseContent);
                        ResultGIDList.Add(ResultContent["result"]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                return ResultGIDList;
            }
            catch (Exception)
            {
                return ResultGIDList;
            }
        }

        /// <summary>
        /// 继续下载选定的任务
        /// </summary>
        public async Task<string> ContinueAsync(string GID)
        {
            // 删除下载任务参数列表
            List<string> ParamsList = new List<string>() { GID };

            ContinueDownloadContent["params"] = ParamsList;

            // 将继续下载任务信息转换为字符串
            string ContinueDownloadString = JsonConvert.SerializeObject(ContinueDownloadContent);

            // 使用Aria2 RPC接口添加继续下载任务指令
            try
            {
                byte[] ContentBytes = Encoding.UTF8.GetBytes(ContinueDownloadString);

                HttpContent httpContent = new StringContent(ContinueDownloadString);
                httpContent.Headers.ContentLength = ContentBytes.Length;
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(RPCServerLink),
                    Timeout = new TimeSpan(0, 0, 30)
                };

                HttpResponseMessage response = await httpClient.PostAsync(RPCServerLink, httpContent);

                // 返回成功添加任务的GID信息
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(ResponseContent)["result"];
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 暂停下载选定的任务
        /// </summary>
        public async Task<string> PauseAsync(string GID)
        {
            // 删除下载任务参数列表
            List<string> ParamsList = new List<string>() { GID };

            PauseDownloadContent["params"] = ParamsList;

            // 将暂停下载任务信息转换为字符串
            string PauseDownloadString = JsonConvert.SerializeObject(PauseDownloadContent);

            // 使用Aria2 RPC接口添加暂停下载任务指令
            try
            {
                byte[] ContentBytes = Encoding.UTF8.GetBytes(PauseDownloadString);

                HttpContent httpContent = new StringContent(PauseDownloadString);
                httpContent.Headers.ContentLength = ContentBytes.Length;
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(RPCServerLink),
                    Timeout = new TimeSpan(0, 0, 30)
                };

                HttpResponseMessage response = await httpClient.PostAsync(RPCServerLink, httpContent);

                // 返回成功添加任务的GID信息
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(ResponseContent)["result"];
                }
                else throw new Exception();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 取消下载选定的任务
        /// </summary>
        public async Task<string> DeleteAsync(string GID)
        {
            List<string> ParamsList = new List<string>() { GID };
            // 删除下载任务参数列表
            DeleteTaskContent["params"] = ParamsList;

            // 将删除任务信息转换为字符串
            string DeleteTaskString = JsonConvert.SerializeObject(DeleteTaskContent);

            // 使用Aria2 RPC接口添加删除任务指令
            try
            {
                byte[] ContentBytes = Encoding.UTF8.GetBytes(DeleteTaskString);

                HttpContent httpContent = new StringContent(DeleteTaskString);
                httpContent.Headers.ContentLength = ContentBytes.Length;
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(RPCServerLink),
                    Timeout = new TimeSpan(0, 0, 30)
                };

                HttpResponseMessage response = await httpClient.PostAsync(RPCServerLink, httpContent);

                // 返回成功删除任务的GID信息
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(ResponseContent)["result"];
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 汇报下载任务状态信息
        /// </summary>
        public async Task<Tuple<string, string, int, int, int>> TellStatusAsync(string GID)
        {
            // 汇报下载任务状态参数列表
            List<object> ParamsList = new List<object>() { GID };
            // 成功添加任务返回的信息
            Dictionary<string, string> ResultContent;

            ParamsList.Add(TellStatusInfoList);
            TellStatusContent["params"] = ParamsList;

            // 汇报下载任务状态信息转换为字符串
            string TellStatusString = JsonConvert.SerializeObject(TellStatusContent);

            // 使用Aria2 RPC接口添加汇报下载任务状态任务指令
            try
            {
                byte[] ContentBytes = Encoding.UTF8.GetBytes(TellStatusString);

                HttpContent httpContent = new StringContent(TellStatusString);
                httpContent.Headers.ContentLength = ContentBytes.Length;
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(RPCServerLink),
                    Timeout = new TimeSpan(0, 0, 30)
                };

                HttpResponseMessage response = await httpClient.PostAsync(RPCServerLink, httpContent);

                // 返回成功添加任务的GID信息
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();
                    string Result = JsonConvert.DeserializeObject<Dictionary<string, string>>(ResponseContent)["result"];
                    ResultContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(Result);

                    return Tuple.Create(
                        ResultContent["gid"],
                        ResultContent["status"],
                        Convert.ToInt32(ResultContent["completedLength"]),
                        Convert.ToInt32(ResultContent["totalLength"]),
                        Convert.ToInt32(ResultContent["downloadSpeed"])
                        );
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
