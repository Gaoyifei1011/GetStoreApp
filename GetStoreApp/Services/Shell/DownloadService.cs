using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 下载服务
    /// </summary>
    public static class DownloadService
    {
        private static readonly AutoResetEvent autoResetEvent = new(false);

        /// <summary>
        /// 下载任务已创建
        /// </summary>
        public static void OnDownloadCreated(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            ConsoleHelper.SetTextColor(0x01);
            ConsoleHelper.WriteLine(string.Format(ResourceService.GetLocalized("Console/DownloadCreated"), downloadSchedulerItem.FileName));
            ConsoleHelper.ResetTextColor();
        }

        /// <summary>
        /// 下载任务下载进度发生变化
        /// </summary>

        public static void OnDownloadProgressing(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            ConsoleHelper.WriteLine(string.Format(ResourceService.GetLocalized("Console/DownloadProgressing"), [FileSizeHelper.ConvertFileSizeToString(downloadSchedulerItem.FinishedSize), FileSizeHelper.ConvertFileSizeToString(downloadSchedulerItem.TotalSize), SpeedHelper.ConvertSpeedToString(downloadSchedulerItem.CurrentSpeed), DownloadProgress(downloadSchedulerItem.FinishedSize, downloadSchedulerItem.TotalSize)]));
        }

        /// <summary>
        /// 下载任务已下载完成
        /// </summary>

        public static void OnDownloadCompleted(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            ConsoleHelper.SetTextColor(0x02);
            ConsoleHelper.WriteLine(string.Format(ResourceService.GetLocalized("Console/DownloadCompleted"), downloadSchedulerItem.FileName));
            ConsoleHelper.ResetTextColor();
            ConsoleHelper.Write(Environment.NewLine);
            autoResetEvent?.Set();
        }

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
                    List<string> indexList = [.. ConsoleHelper.ReadLine().Split(',')];

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
                        ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/DownloadCompletedAll"));
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
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
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
            DownloadSchedulerService.CreateDownload(fileLink, Path.Combine(DownloadOptionsService.DownloadFolder.Path, fileName));
            autoResetEvent.WaitOne();
        }

        /// <summary>
        /// 打开下载完成后保存的目录
        /// </summary>
        private static async Task OpenDownloadFolderAsync()
        {
            await Launcher.LaunchFolderAsync(DownloadOptionsService.DownloadFolder);
        }

        /// <summary>
        /// 计算当前文件的下载进度
        /// </summary>
        private static double DownloadProgress(double finishedSize, double totalSize)
        {
            return totalSize == default ? 0 : Math.Round(finishedSize / totalSize * 100, 2);
        }
    }
}
