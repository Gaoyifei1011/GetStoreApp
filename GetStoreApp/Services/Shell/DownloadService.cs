using GetStoreApp.Helpers.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 下载服务
    /// </summary>
    public static class DownloadService
    {
        private static string Aria2FilePath { get; } = Path.Combine(AppContext.BaseDirectory, @"Aria2\Aria2c.exe");

        private static bool IsFileDownloading;

        private static int Aria2ProcessId;

        /// <summary>
        /// 下载相应的文件
        /// </summary>
        public static async Task QueryDownloadIndexAsync()
        {
            while (true)
            {
                Console.WriteLine(ResourceService.GetLocalized("Console/DownloadFile"));

                try
                {
                    List<string> IndexList = Console.ReadLine().Split(',').ToList();
                    bool CheckResult = true;
                    foreach (string indexItem in IndexList)
                    {
                        int index = Convert.ToInt32(indexItem);
                        if (index > ParseService.ResultDataList.Count || index < 1)
                        {
                            CheckResult = false;
                            break;
                        }
                    }

                    if (CheckResult)
                    {
                        for (int index = 0; index < IndexList.Count; index++)
                        {
                            string IndexItem = IndexList[index];
                            if (ConsoleLaunchService.IsAppRunning)
                            {
                                Console.WriteLine(ResourceService.GetLocalized("Console/DownloadingInformation"), index + 1, IndexList.Count);
                                await DownloadFileAsync(ParseService.ResultDataList[Convert.ToInt32(IndexItem) - 1].FileLink);
                            }
                        }
                        Console.WriteLine(ResourceService.GetLocalized("Console/DownloadCompleted"));
                        string InputString = Console.ReadLine();
                        if (InputString is "Y" || InputString is "y")
                        {
                            continue;
                        }
                        else
                        {
                            Console.WriteLine(ResourceService.GetLocalized("Console/OpenFolder"));
                            await OpenDownloadFolderAsync();
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine(ResourceService.GetLocalized("Console/SerialNumberOutRange"));
                        string InputString = Console.ReadLine();
                        if (InputString is "Y" || InputString is "y")
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(ResourceService.GetLocalized("Console/SerialNumberError"));
                    string InputString = Console.ReadLine();
                    if (InputString is "Y" || InputString is "y")
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        private static async Task DownloadFileAsync(string fileLink)
        {
            ProcessStartInfo Aria2Info = new ProcessStartInfo()
            {
                FileName = Aria2FilePath,

                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,

                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = string.Format("--file-allocation=none -d \"{0}\" {1}", DownloadOptionsService.DownloadFolder.Path, fileLink)
            };

            StreamReader streamReader;

            Process Aria2Process = new Process();
            Aria2Process.StartInfo = Aria2Info;
            Aria2Process.Start();
            IsFileDownloading = true;
            Aria2ProcessId = Aria2Process.Id;
            streamReader = Aria2Process.StandardOutput;
            while (!streamReader.EndOfStream)
            {
                char[] bs = new char[16];
                streamReader.Read(bs, 0, 16);
                foreach (char o in bs)
                {
                    Console.Write(o);
                }
            }
            await Aria2Process.WaitForExitAsync();
            Aria2Process.Close();
        }

        /// <summary>
        /// 打开下载完成后保存的目录
        /// </summary>
        private static async Task OpenDownloadFolderAsync()
        {
            await Launcher.LaunchFolderAsync(DownloadOptionsService.DownloadFolder);
        }

        /// <summary>
        /// 程序即将退出，停止下载文件
        /// </summary>
        public static void StopDownloadFile()
        {
            if (IsFileDownloading)
            {
                Aria2ProcessHelper.KillProcessAndChildren(Aria2ProcessId);
            }
        }
    }
}
