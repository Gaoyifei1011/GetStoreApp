using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.History;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs
{
    public class TraceCleanupPromptViewModel : ObservableRecipient
    {
        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private IHistoryDBService HistoryDBService { get; } = IOCHelper.GetService<IHistoryDBService>();

        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        // 立即清理按钮的可用状态
        private bool _isButtonEnabled = true;

        public bool IsButtonEnabled
        {
            get { return _isButtonEnabled; }

            set { SetProperty(ref _isButtonEnabled, value); }
        }

        // 是否要清理历史记录
        private bool _isHistoryClean = false;

        public bool IsHistoryClean
        {
            get { return _isHistoryClean; }

            set { SetProperty(ref _isHistoryClean, value); }
        }

        // 是否要清理下载记录
        private bool _isDownloadClean = false;

        public bool IsDownloadClean
        {
            get { return _isDownloadClean; }

            set { SetProperty(ref _isDownloadClean, value); }
        }

        // 是否要清理本地创建的文件
        private bool _isLocalFileClean = false;

        public bool IsLocalFileClean
        {
            get { return _isLocalFileClean; }

            set { SetProperty(ref _isLocalFileClean, value); }
        }

        // 清理状态显示标志
        private bool _clearState = false;

        public bool ClearState
        {
            get { return _clearState; }

            set { SetProperty(ref _clearState, value); }
        }

        // 清理状态圆环显示标志
        private bool _clearStateRing = false;

        public bool ClearStateRing
        {
            get { return _clearStateRing; }

            set { SetProperty(ref _clearStateRing, value); }
        }

        // 清理状态文字信息
        private string _clearStateText = "";

        public string ClearStateText
        {
            get { return _clearStateText; }

            set { SetProperty(ref _clearStateText, value); }
        }

        // 清理失误显示标志
        private bool _clearErrorVisable = false;

        public bool ClearErrorVisable
        {
            get { return _clearErrorVisable; }

            set { SetProperty(ref _clearErrorVisable, value); }
        }

        // 历史记录清理失败显示标志
        private bool _historyCleanErrorVisable = false;

        public bool HistoryCleanErrorVisable
        {
            get { return _historyCleanErrorVisable; }

            set { SetProperty(ref _historyCleanErrorVisable, value); }
        }

        // 下载记录清理失败显示标志
        private bool _downloadCleanErrorVisable = false;

        public bool DownloadCleanErrorVisable
        {
            get { return _downloadCleanErrorVisable; }

            set { SetProperty(ref _downloadCleanErrorVisable, value); }
        }

        // 本地创建的文件清理失败显示标志
        private bool _localFileCleanErrorVisable = false;

        public bool LocalFileCleanErrorVisable
        {
            get { return _localFileCleanErrorVisable; }

            set { SetProperty(ref _localFileCleanErrorVisable, value); }
        }

        public IAsyncRelayCommand TraceCleanupSureCommand => new AsyncRelayCommand(TraceCleanupAsync);

        public IAsyncRelayCommand TraceCleanupCancelCommand => new AsyncRelayCommand<ContentDialog>(async (param) =>
        {
            param.Hide();
            await Task.CompletedTask;
        });

        /// <summary>
        /// 痕迹清理
        /// </summary>
        private async Task TraceCleanupAsync()
        {
            // 显示正在清理中的状态信息
            IsButtonEnabled = false;
            ClearState = true;
            ClearStateRing = true;
            ClearStateText = ResourceService.GetLocalized("/Dialog/CleaningNow");
            LocalFileCleanErrorVisable = false;
            HistoryCleanErrorVisable = false;
            DownloadCleanErrorVisable = false;

            // 清理所有痕迹
            Tuple<bool, bool, bool> CleanResult = await CleanAsync();
            await Task.Delay(1000);

            // 清理完成，显示清理完成的结果
            ClearStateRing = false;
            IsButtonEnabled = true;

            // 成功清理
            if (CleanResult.Item1 && CleanResult.Item2 && CleanResult.Item3)
            {
                ClearStateText = ResourceService.GetLocalized("/Dialog/CleanSuccessfully");

                // 发送消息，更新UI界面
                WeakReferenceMessenger.Default.Send(new HistoryMessage(true));
            }

            // 清理失败，显示清理异常错误信息
            else
            {
                ClearStateText = ResourceService.GetLocalized("/Dialog/CleanFailed");

                LocalFileCleanErrorVisable = CleanResult.Item1 == false;

                if (CleanResult.Item2) WeakReferenceMessenger.Default.Send(new HistoryMessage(true));
                else HistoryCleanErrorVisable = true;

                DownloadCleanErrorVisable = CleanResult.Item3 == false;
            }
        }

        /// <summary>
        /// 清理记录
        /// </summary>
        private async Task<Tuple<bool, bool, bool>> CleanAsync()
        {
            bool LocalCleanResult = true;
            bool HistoryCleanResult = true;
            bool DownloadCleanResult = true;

            // 清理本地创建的文件
            if (IsLocalFileClean)
            {
                List<DownloadModel> LocalFileData = (await DownloadDBService.QueryAsync(4));
                LocalCleanResult = DeleteFiles(ref LocalFileData);
            }

            // 清理历史记录
            if (IsHistoryClean) HistoryCleanResult = await HistoryDBService.ClearAsync();

            // 清理下载记录
            if (IsDownloadClean) DownloadCleanResult = await DownloadDBService.ClearAsync();

            return Tuple.Create(LocalCleanResult, HistoryCleanResult, DownloadCleanResult);
        }

        /// <summary>
        /// 删除所有文件
        /// </summary>
        private bool DeleteFiles(ref List<DownloadModel> downloadDataList)
        {
            try
            {
                // 文件存在时尝试删除文件
                foreach (DownloadModel downloadItem in downloadDataList)
                {
                    if (File.Exists(downloadItem.FilePath))
                    {
                        File.Delete(downloadItem.FilePath);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
