using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 下载服务
    /// </summary>
    public static class DownloadService
    {
        private static bool isFileDownloading = false;

        public static STARTUPINFO downloadStartupInfo;
        public static PROCESS_INFORMATION downloadInformation;

        /// <summary>
        /// 下载相应的文件
        /// </summary>
        public static async Task QueryDownloadIndexAsync(List<QueryLinksModel> queryLinksList)
        {
            while (true)
            {
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/DownloadFile"));

                try
                {
                    List<string> indexList = new List<string>();
                    foreach (string index in ConsoleHelper.ReadLine().Split(','))
                    {
                        indexList.Add(index);
                    }

                    bool checkResult = true;
                    foreach (string indexItem in indexList)
                    {
                        int index = Convert.ToInt32(indexItem);
                        if (index > queryLinksList.Count || index < 1)
                        {
                            checkResult = false;
                            break;
                        }
                    }

                    if (checkResult)
                    {
                        for (int index = 0; index < indexList.Count; index++)
                        {
                            string indexItem = indexList[index];
                            if (ConsoleLaunchService.IsAppRunning)
                            {
                                ConsoleHelper.WriteLine(string.Format(ResourceService.GetLocalized("Console/DownloadingInformation"), index + 1, indexList.Count));
                                DownloadFile(queryLinksList[Convert.ToInt32(indexItem) - 1].FileName, queryLinksList[Convert.ToInt32(indexItem) - 1].FileLink);
                            }
                        }
                        ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/DownloadCompleted"));
                        string inputString = ConsoleHelper.ReadLine();
                        if (inputString is "Y" or "y")
                        {
                            continue;
                        }
                        else
                        {
                            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/OpenFolder"));
                            await OpenDownloadFolderAsync();
                            break;
                        }
                    }
                    else
                    {
                        ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/SerialNumberOutRange"));
                        string inputString = ConsoleHelper.ReadLine();
                        if (inputString is "Y" or "y")
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
                    ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/SerialNumberError"));
                    string inputString = ConsoleHelper.ReadLine();
                    if (inputString is "Y" or "y")
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
        private static void DownloadFile(string fileName, string fileLink)
        {
            // TODO ：需要更新下载服务
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
            if (isFileDownloading)
            {
                if (downloadInformation.dwProcessId is not 0)
                {
                    IntPtr hProcess = Kernel32Library.OpenProcess(EDESIREDACCESS.PROCESS_TERMINATE, false, downloadInformation.dwProcessId);
                    if (hProcess != IntPtr.Zero)
                    {
                        Kernel32Library.TerminateProcess(hProcess, 0);
                    }
                    Kernel32Library.CloseHandle(hProcess);
                }
            }
        }
    }
}
