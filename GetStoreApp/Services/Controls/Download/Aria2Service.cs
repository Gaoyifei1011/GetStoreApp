using Aria2NET;
using GetStoreApp.Extensions.DataType.Exceptions;
using GetStoreApp.Helpers.Controls.Download;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// Aria2下载服务
    /// </summary>
    public static class Aria2Service
    {
        private static string Aria2FilePath => Path.Combine(AppContext.BaseDirectory, @"Aria2\Aria2c.exe");

        private static string DefaultAria2ConfPath => Path.Combine(AppContext.BaseDirectory, @"Aria2\Aria2c.conf");

        public static string Aria2ConfPath => Path.Combine(ApplicationData.Current.LocalFolder.Path, "Aria2.conf");

        private static string Aria2Arguments { get; set; }

        private static string RPCServerLink => "http://127.0.0.1:6300/jsonrpc";

        private static Aria2NetClient Aria2Client { get; set; }

        /// <summary>
        /// 初始化Aria2配置文件
        /// </summary>
        public static async Task InitializeAria2ConfAsync()
        {
            try
            {
                // 原配置文件存在且新的配置文件不存在，拷贝到指定目录
                if (File.Exists(DefaultAria2ConfPath) && !File.Exists(Aria2ConfPath))
                {
                    new FileInfo(DefaultAria2ConfPath).CopyTo(Aria2ConfPath);
                }

                // 使用自定义的配置文件目录
                Aria2Arguments = string.Format("--conf-path=\"{0}\" -D", Aria2ConfPath);
            }

            //  发生异常时，使用默认的配置文件目录
            catch (Exception)
            {
                if (File.Exists(DefaultAria2ConfPath))
                {
                    Aria2Arguments = string.Format("--conf-path=\"{0}\" -D", DefaultAria2ConfPath);
                }
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// 初始化运行Aria2下载进程
        /// </summary>
        public static async Task StartAria2Async()
        {
            await Aria2ProcessHelper.RunAria2Async(Aria2FilePath, Aria2Arguments);
        }

        /// <summary>
        /// 关闭Aria2进程
        /// </summary>
        public static async Task CloseAria2Async()
        {
            Aria2ProcessHelper.KillProcessAndChildren(Aria2ProcessHelper.GetProcessID());
            await Task.CompletedTask;
        }

        /// <summary>
        /// 恢复配置文件默认值
        /// </summary>
        public static async Task RestoreDefaultAsync()
        {
            try
            {
                // 原配置文件存在时，覆盖已经修改的配置文件
                if (File.Exists(DefaultAria2ConfPath))
                {
                    new FileInfo(DefaultAria2ConfPath).CopyTo(Aria2ConfPath, true);
                }

                // 使用自定义的配置文件目录
                Aria2Arguments = string.Format("--conf-path=\"{0}\" -D", Aria2ConfPath);
            }

            //  发生异常时，使用默认的配置文件目录
            catch (Exception)
            {
                if (File.Exists(DefaultAria2ConfPath))
                {
                    Aria2Arguments = string.Format("--conf-path=\"{0}\" -D", DefaultAria2ConfPath);
                }
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public static async Task<(bool, string)> AddUriAsync(string downloadLink, string folderPath)
        {
            Aria2Client ??= new Aria2NetClient(RPCServerLink, null, null, 0);

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
        public static async Task<(bool, string)> PauseAsync(string GID)
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
        public static async Task<(bool, string)> DeleteAsync(string GID)
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
        public static async Task<(bool, string, long, long, long)> TellStatusAsync(string GID)
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
        public static async Task<(bool, long)> GetFileSizeAsync(string GID)
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
