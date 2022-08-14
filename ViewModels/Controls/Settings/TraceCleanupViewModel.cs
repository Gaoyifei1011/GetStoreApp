using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.History;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class TraceCleanupViewModel : ObservableRecipient
    {
        private IHistoryDataService HistoryDataService { get; } = IOCHelper.GetService<IHistoryDataService>();

        private IDownloadDataService DownloadDataService { get; } = IOCHelper.GetService<IDownloadDataService>();

        public IAsyncRelayCommand TraceCleanupCommand { get; }

        public TraceCleanupViewModel()
        {
            TraceCleanupCommand = new AsyncRelayCommand(TraceCleanupAsync);
        }

        private async Task TraceCleanupAsync()
        {
            // 添加删除下载记录（包括文件）对话框
            ContentDialogResult result = await ShowTraceCleanupPromptDialogAsync();

            // 清理记录
            if (result == ContentDialogResult.Primary || result == ContentDialogResult.Secondary)
            {
                // 先清理文件，后清理记录
                if (result == ContentDialogResult.Primary)
                {
                    Tuple<List<DownloadModel>, bool> DownloadData = await DownloadDataService.QueryAllDownloadDataAsync();
                    List<DownloadModel> DownloadDataList = DownloadData.Item1;
                    DeleteFiles(ref DownloadDataList);
                }

                // 清理历史记录
                bool HistoryClearResult = await HistoryDataService.ClearHistoryDataAsync();

                // 清理下载记录
                bool DownloadClearResult = await DownloadDataService.ClearDownloadDataAsync();

                Messenger.Send(new HistoryMessage(true));
            }
        }

        /// <summary>
        /// 清理所有痕迹时显示是否要清理本地文件的对话框
        /// </summary>
        private async Task<ContentDialogResult> ShowTraceCleanupPromptDialogAsync()
        {
            TraceCleanupPromptDialog dialog = new TraceCleanupPromptDialog() { XamlRoot = App.MainWindow.Content.XamlRoot };
            return await dialog.ShowAsync();
        }

        /// <summary>
        /// 删除所有文件
        /// </summary>
        private void DeleteFiles(ref List<DownloadModel> downloadDataList)
        {
            try
            {
                // 文件存在时尝试删除文件
                foreach (var item in downloadDataList)
                {
                    if (File.Exists(item.FilePath)) File.Delete(item.FilePath);
                }
            }

            // 文件可能正在使用，不能被删除
            catch (IOException)
            {
            }

            // 删除文件没有权限
            catch (UnauthorizedAccessException)
            {
            }

            // 未知异常
            catch (Exception)
            {
            }
        }
    }
}
