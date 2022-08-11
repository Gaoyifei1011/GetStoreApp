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
        private string Aria2Path { get; } = Path.Combine(AppContext.BaseDirectory, "Aria2\\aria2c.exe");

        private string Aria2ConfPath { get; } = Path.Combine(AppContext.BaseDirectory, "Aria2\\Config\\aria2.conf");

        private string RPCServerLink { get; } = "http://127.0.0.1:6300/jsonrpc";

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
        public async Task<string> AddTaskAsync(string downloadLink,string folderPath,string fileName)
        {
            // 下载配置内容
            Dictionary<string, string> DownloadConfiguration = new Dictionary<string, string>();
            // 下载任务内容
            Dictionary<string, object> AddTaskContent = new Dictionary<string, object>();
            // 成功添加任务信息
            Dictionary<string, string> ResultContent;
            // 下载链接列表
            List<string> UrlList = new List<string>();
            // 下载参数列表
            List<object> ParamsList = new List<object>();

            // 添加下载信息
            UrlList.Add(downloadLink);
            DownloadConfiguration.Add("dir",folderPath);
            DownloadConfiguration.Add("out", fileName);
            ParamsList.Add(UrlList);
            ParamsList.Add(DownloadConfiguration);

            AddTaskContent.Add("id", "");
            AddTaskContent.Add("jsonrpc", "2.0");
            AddTaskContent.Add("method", "aria2.addUri");
            AddTaskContent.Add("params", ParamsList);

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

                HttpResponseMessage response =  await httpClient.PostAsync(RPCServerLink, httpContent);

                // 返回成功添加任务的GID信息
                if (response.IsSuccessStatusCode)
                {
                    string ResponseContent = await response.Content.ReadAsStringAsync();

                    ResultContent = JsonConvert.DeserializeObject<Dictionary<string,string>>(ResponseContent);

                    return ResultContent["result"];
                }
                
                else throw new Exception();
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 继续下载全部任务
        /// </summary>
        public async Task<bool> ContinueAllAsync()
        {
            Dictionary<string, object> ContinueAllContent = new Dictionary<string, object>();
            List<string> ParamsList = new List<string>();

            ParamsList.Add("");

            ContinueAllContent.Add("id", "");
            ContinueAllContent.Add("jsonrpc", "2.0");
            ContinueAllContent.Add("method", "aria2.unpause");
            ContinueAllContent.Add("params", ParamsList);

            // 将暂停下载任务信息转换为字符串
            string ContinueAllString = JsonConvert.SerializeObject(ContinueAllContent);

            // 使用Aria2 RPC接口添加暂停下载任务指令
            try
            {
                byte[] ContentBytes = Encoding.UTF8.GetBytes(ContinueAllString);

                HttpContent httpContent = new StringContent(ContinueAllString);
                httpContent.Headers.ContentLength = ContentBytes.Length;
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(RPCServerLink),
                    Timeout = new TimeSpan(0, 0, 30)
                };

                HttpResponseMessage response = await httpClient.PostAsync(RPCServerLink, httpContent);

                // 返回成功添加任务的GID信息
                if (response.IsSuccessStatusCode) return true;

                else throw new Exception();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 暂停下载全部任务
        /// </summary>
        public async Task<bool> PauseAllAsync()
        {
            Dictionary<string, object> PauseAllContent = new Dictionary<string, object>();
            List<string> ParamsList = new List<string>();

            ParamsList.Add("");

            PauseAllContent.Add("id", "");
            PauseAllContent.Add("jsonrpc", "2.0");
            PauseAllContent.Add("method", "aria2.unpause");
            PauseAllContent.Add("params", ParamsList);

            // 将暂停下载任务信息转换为字符串
            string ContinueAllString = JsonConvert.SerializeObject(PauseAllContent);

            // 使用Aria2 RPC接口添加暂停下载任务指令
            try
            {
                byte[] ContentBytes = Encoding.UTF8.GetBytes(ContinueAllString);

                HttpContent httpContent = new StringContent(ContinueAllString);
                httpContent.Headers.ContentLength = ContentBytes.Length;
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri(RPCServerLink),
                    Timeout = new TimeSpan(0, 0, 30)
                };

                HttpResponseMessage response = await httpClient.PostAsync(RPCServerLink, httpContent);

                // 返回成功添加任务的GID信息
                if (response.IsSuccessStatusCode) return true;

                else throw new Exception();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 多选模式下删除选择的任务
        /// </summary>
        public async Task<bool> DeleteSelectedAsync(List<string> GIDList)
        {
            return await Task.FromResult(true);
        }

        /// <summary>
        /// 继续下载选定的任务
        /// </summary>
        public async Task<bool> ContinueAsync(string GID)
        {
            Dictionary<string, object> ContinueDownloadContent = new Dictionary<string, object>();
            List<string> ParamsList = new List<string>();

            ParamsList.Add(GID);

            ContinueDownloadContent.Add("id", "");
            ContinueDownloadContent.Add("jsonrpc", "2.0");
            ContinueDownloadContent.Add("method", "aria2.unpause");
            ContinueDownloadContent.Add("params", ParamsList);

            // 将暂停下载任务信息转换为字符串
            string ContinueDownloadString = JsonConvert.SerializeObject(ContinueDownloadContent);

            // 使用Aria2 RPC接口添加暂停下载任务指令
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
                if (response.IsSuccessStatusCode) return true;

                else throw new Exception();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 暂停下载选定的任务
        /// </summary>
        public async Task<bool> PauseAsync(string GID)
        {
            Dictionary<string, object> PauseDownloadContent = new Dictionary<string, object>();
            List<string> ParamsList = new List<string>();

            ParamsList.Add(GID);

            PauseDownloadContent.Add("id", "");
            PauseDownloadContent.Add("jsonrpc", "2.0");
            PauseDownloadContent.Add("method", "aria2.pause");
            PauseDownloadContent.Add("params", ParamsList);

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
                if (response.IsSuccessStatusCode) return true;

                else throw new Exception();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 取消下载选定的任务
        /// </summary>
        public async Task<bool> DeleteAsync(string GID)
        {
            Dictionary<string,object> DeleteTaskContent = new Dictionary<string,object>();
            List<string> ParamsList = new List<string>();

            ParamsList.Add(GID);

            DeleteTaskContent.Add("id", "");
            DeleteTaskContent.Add("jsonrpc", "2.0");
            DeleteTaskContent.Add("method", "aria2.remove");
            DeleteTaskContent.Add("params", ParamsList);

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

                // 返回成功添加任务的GID信息
                if (response.IsSuccessStatusCode) return true;

                else throw new Exception();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 下载任务状态
        /// </summary>
        public async Task TellStatusAsync()
        {
            await Task.CompletedTask;
        }
    }
}
