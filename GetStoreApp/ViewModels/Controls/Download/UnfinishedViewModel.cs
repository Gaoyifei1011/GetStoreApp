using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Download
{
    public class UnfinishedViewModel : ObservableRecipient
    {
        // 临界区资源访问互斥锁
        private readonly object UnfinishedDataListLock = new object();

        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        public ObservableCollection<DownloadModel> UnfinishedDataList { get; } = new ObservableCollection<DownloadModel>();

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set { SetProperty(ref _isSelectMode, value); }
        }

        // 打开默认保存的文件夹
        public IAsyncRelayCommand OpenFolderCommand => new AsyncRelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        });

        // 继续下载全部任务
        public IAsyncRelayCommand ContinueAllCommand => new AsyncRelayCommand(async () =>
        {
            List<DownloadModel> PauseList = UnfinishedDataList.Where(item => item.DownloadFlag == 2).ToList();

            foreach (DownloadModel downloadItem in PauseList)
            {
                bool ContinueResult = await DownloadSchedulerService.ContinueTaskAsync(downloadItem);

                if (ContinueResult)
                {
                    lock (UnfinishedDataListLock)
                    {
                        UnfinishedDataList.Remove(downloadItem);
                    }
                }
                else
                {
                    continue;
                }
            }
        });

        // 进入多选模式
        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            lock (UnfinishedDataListLock)
            {
                foreach (DownloadModel downloadItem in UnfinishedDataList)
                {
                    downloadItem.IsSelected = false;
                }
            }

            IsSelectMode = true;
            await Task.CompletedTask;
        });

        // 全部选择
        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            lock (UnfinishedDataListLock)
            {
                foreach (DownloadModel downloadItem in UnfinishedDataList)
                {
                    downloadItem.IsSelected = true;
                }
            }

            await Task.CompletedTask;
        });

        // 全部不选
        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(async () =>
        {
            lock (UnfinishedDataListLock)
            {
                foreach (DownloadModel downloadItem in UnfinishedDataList)
                {
                    downloadItem.IsSelected = false;
                }
            }

            await Task.CompletedTask;
        });

        // 删除选中的任务
        public IAsyncRelayCommand DeleteSelectedCommand => new AsyncRelayCommand(async () =>
        {
            List<DownloadModel> SelectedUnfinishedDataList = UnfinishedDataList.Where(item => item.IsSelected == true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedUnfinishedDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            IsSelectMode = false;

            foreach (DownloadModel downloadItem in SelectedUnfinishedDataList)
            {
                // 删除文件
                try
                {
                    string tempFilePath = downloadItem.FilePath;
                    string tempFileAria2Path = string.Format("{0}.{1}", downloadItem.FilePath, "Aria2");

                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }

                    if (File.Exists(tempFileAria2Path))
                    {
                        File.Delete(tempFileAria2Path);
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                bool DeleteResult = await DownloadDBService.DeleteAsync(downloadItem);

                if (DeleteResult)
                {
                    lock (UnfinishedDataListLock)
                    {
                        UnfinishedDataList.Remove(downloadItem);
                    }
                }
                else
                {
                    continue;
                }
            }
        });

        // 退出多选模式
        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        // 继续下载当前任务
        public IAsyncRelayCommand ContinueCommand => new AsyncRelayCommand<DownloadModel>(async (param) =>
        {
            if (param.DownloadFlag == 2)
            {
                bool ContinueResult = await DownloadSchedulerService.ContinueTaskAsync(param);

                if (ContinueResult)
                {
                    lock (UnfinishedDataList)
                    {
                        UnfinishedDataList.Remove(param);
                    }
                }
            }
        });

        // 删除当前任务
        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<DownloadModel>(async (param) =>
        {
            bool DeleteResult = await DownloadDBService.DeleteAsync(param);

            if (DeleteResult)
            {
                lock (UnfinishedDataListLock)
                {
                    UnfinishedDataList.Remove(param);
                }
            }
        });

        public UnfinishedViewModel()
        {
            Messenger.Register<UnfinishedViewModel, PivotSelectionMessage>(this, async (unfinishedViewModel, pivotSelectionMessage) =>
            {
                // 切换到已完成页面时，更新当前页面的数据
                if (pivotSelectionMessage.Value == 1)
                {
                    await GetDownloadDataListAsync();
                }

                // 从下载页面离开时，关闭所有事件。并注销所有消息服务
                else if (pivotSelectionMessage.Value == -1)
                {
                    Messenger.UnregisterAll(this);
                }

                await Task.CompletedTask;
            });
        }

        /// <summary>
        /// 从数据库中加载未下载完成和下载失败的数据
        /// </summary>
        private async Task GetDownloadDataListAsync()
        {
            List<DownloadModel> FailureDownloadRawList = await DownloadDBService.QueryAsync(0);

            List<DownloadModel> PauseDownloadRawList = await DownloadDBService.QueryAsync(2);

            lock (UnfinishedDataListLock)
            {
                UnfinishedDataList.Clear();
            }

            lock (UnfinishedDataListLock)
            {
                foreach (DownloadModel downloadItem in PauseDownloadRawList)
                {
                    UnfinishedDataList.Add(downloadItem);
                }

                foreach (DownloadModel downloadItem in FailureDownloadRawList)
                {
                    UnfinishedDataList.Add(downloadItem);
                }
            }
        }
    }
}
