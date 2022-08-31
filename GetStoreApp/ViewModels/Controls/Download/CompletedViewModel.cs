using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.ViewModels.Controls.Download
{
    public class CompletedViewModel : ObservableRecipient
    {
        // 临界区资源访问互斥锁
        private readonly object CompletedDataListLock = new object();

        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        public ObservableCollection<DownloadModel> CompletedDataList { get; } = new ObservableCollection<DownloadModel>();

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

        // 进入多选模式
        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            lock (CompletedDataListLock)
            {
                foreach (DownloadModel downloadItem in CompletedDataList)
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
            lock (CompletedDataListLock)
            {
                foreach (DownloadModel downloadItem in CompletedDataList)
                {
                    downloadItem.IsSelected = true;
                }
            }

            await Task.CompletedTask;
        });

        // 全部不选
        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(async () =>
        {
            lock (CompletedDataListLock)
            {
                foreach (DownloadModel downloadItem in CompletedDataList)
                {
                    downloadItem.IsSelected = false;
                }
            }

            await Task.CompletedTask;
        });

        // 删除选中的任务
        public IAsyncRelayCommand DeleteRecordCommand => new AsyncRelayCommand(async () =>
        {
            List<DownloadModel> SelectedCompletedDataList = CompletedDataList.Where(item => item.IsSelected == true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedCompletedDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog().ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                bool DeleteSelectedResult = await DownloadDBService.DeleteSelectedAsync(SelectedCompletedDataList);

                lock (CompletedDataListLock)
                {
                    foreach (DownloadModel downloadItem in SelectedCompletedDataList)
                    {
                        CompletedDataList.Remove(downloadItem);
                    }
                }
            }
        });

        public IAsyncRelayCommand DeleteRecordWithFileCommand => new AsyncRelayCommand(async () =>
        {
            List<DownloadModel> SelectedCompletedDataList = CompletedDataList.Where(item => item.IsSelected == true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedCompletedDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog().ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                foreach (DownloadModel downloadItem in SelectedCompletedDataList)
                {
                    // 删除文件
                    try
                    {
                        if (File.Exists(downloadItem.FilePath))
                        {
                            File.Delete(downloadItem.FilePath);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    // 删除记录
                    bool DeleteResult = await DownloadDBService.DeleteAsync(downloadItem);

                    if (DeleteResult)
                    {
                        lock (CompletedDataListLock)
                        {
                            CompletedDataList.Remove(downloadItem);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        });

        // 退出多选模式
        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        // 打开当前项目保存的文件夹
        public IAsyncRelayCommand OpenItemFolderCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            if (param is not null)
            {
                await DownloadOptionsService.OpenFolderAsync(await StorageFolder.GetFolderFromPathAsync(param));
            }
        });

        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<DownloadModel>(async (param) =>
        {
            if (param is not null)
            {
                bool DeleteResult = await DownloadDBService.DeleteAsync(param);

                if (DeleteResult)
                {
                    lock (CompletedDataListLock)
                    {
                        CompletedDataList.Remove(param);
                    }
                }
            }
        });

        public CompletedViewModel()
        {
            WeakReferenceMessenger.Default.Register<CompletedViewModel, PivotSelectionMessage>(this, async (completedViewModel, pivotSelectionMessage) =>
            {
                // 切换到已完成页面时，更新当前页面的数据
                if (pivotSelectionMessage.Value == 2)
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
        /// 从数据库中加载已下载完成的数据
        /// </summary>
        private async Task GetDownloadDataListAsync()
        {
            List<DownloadModel> DownloadRawList = await DownloadDBService.QueryAsync(4);

            lock (CompletedDataListLock)
            {
                CompletedDataList.Clear();
            }

            lock (CompletedDataListLock)
            {
                foreach (DownloadModel downloadRawData in DownloadRawList)
                {
                    CompletedDataList.Add(downloadRawData);
                }
            }
        }
    }
}
