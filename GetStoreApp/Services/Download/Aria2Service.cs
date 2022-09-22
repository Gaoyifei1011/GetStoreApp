using Aria2NET;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Extensions.AppException;
using GetStoreApp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Download
{
    /// <summary>
    /// Aria2下载服务
    /// </summary>
    public class Aria2Service : IAria2Service
    {
        private string Aria2FileName => Path.Combine(AppContext.BaseDirectory, string.Format(@"Aria2\{0}\GetStoreAppAria2.exe", InfoHelper.GetAppProcessorArchitecture()));

        private string Aria2Arguments => string.Format("--conf-path=\"{0}\" -D", Path.Combine(AppContext.BaseDirectory, "Aria2\\Config\\aria2.conf"));

        private string RPCServerLink => "http://127.0.0.1:6300/jsonrpc";

        private Aria2NetClient Aria2Client { get; set; }

        /// <summary>
        /// 初始化运行Aria2下载进程
        /// </summary>
        public async Task InitializeAria2Async()
        {
            await Aria2ProcessHelper.RunAria2Async(Aria2FileName, Aria2Arguments);
        }

        /// <summary>
        /// 关闭Aria2进程
        /// </summary>
        public async Task CloseAria2Async()
        {
            Aria2ProcessHelper.KillProcessAndChildren(Aria2ProcessHelper.GetProcessID());
            await Task.CompletedTask;
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public async Task<(bool, string)> AddUriAsync(string downloadLink, string folderPath)
        {
            if (Aria2Client is null)
            {
                Aria2Client = new Aria2NetClient(RPCServerLink, null, null, 0);
            }

            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    throw new ProcessNotExistException();
                }

                string AddResult = await Aria2Client.AddUriAsync(
                    new List<string> { downloadLink },
                    new Dictionary<string, object> { { "dir", folderPath } },
                    0);

                return (true, AddResult);
            }
            catch (ProcessNotExistException)
            {
                return (false, string.Empty);
            }
            catch (Exception)
            {
                return (false, string.Empty);
            }
        }

        /// <summary>
        /// 暂停下载选定的任务
        /// </summary>
        public async Task<(bool, string)> PauseAsync(string GID)
        {
            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    throw new ProcessNotExistException();
                }

                string PauseResult = await Aria2Client.ForceRemoveAsync(GID);
                return (true, PauseResult);
            }
            catch (ProcessNotExistException)
            {
                return (false, string.Empty);
            }
            catch (Exception)
            {
                return (false, string.Empty);
            }
        }

        /// <summary>
        /// 取消下载选定的任务
        /// </summary>
        public async Task<(bool, string)> DeleteAsync(string GID)
        {
            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    throw new ProcessNotExistException();
                }

                string DeleteResult = await Aria2Client.ForceRemoveAsync(GID);
                return (true, DeleteResult);
            }
            catch (ProcessNotExistException)
            {
                return (false, string.Empty);
            }
            catch (Exception)
            {
                return (false, string.Empty);
            }
        }

        /// <summary>
        /// 汇报下载任务状态信息
        /// </summary>
        public async Task<(bool, string, long, long, long)> TellStatusAsync(string GID)
        {
            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    throw new ProcessNotExistException();
                }

                DownloadStatusResult TellStatusResult = await Aria2Client.TellStatusAsync(GID);

                return (true,
                 TellStatusResult.Status, TellStatusResult.CompletedLength, TellStatusResult.TotalLength, TellStatusResult.DownloadSpeed);
            }
            catch (ProcessNotExistException)
            {
                return (false, string.Empty, default(long), default(long), default(long));
            }
            catch (Exception)
            {
                return (false, string.Empty, default(long), default(long), default(long));
            }
        }

        /// <summary>
        /// 获取下载文件大小信息
        /// </summary>
        public async Task<(bool, long)> GetFileSizeAsync(string GID)
        {
            try
            {
                // 判断下载进程是否存在
                if (!Aria2ProcessHelper.IsAria2ProcessExisted())
                {
                    throw new ProcessNotExistException();
                }

                DownloadStatusResult GetFileSizeResult = await Aria2Client.TellStatusAsync(GID);

                return (true, GetFileSizeResult.TotalLength);
            }
            catch (ProcessNotExistException)
            {
                return (false, default(long));
            }
            catch (Exception)
            {
                return (false, default(long));
            }
        }
    }
}
