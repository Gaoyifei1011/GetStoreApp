using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 下载中页面
    /// </summary>
    public sealed partial class DownloadingPage : Page, INotifyPropertyChanged
    {
        private readonly string DownloadingCountInfoString = ResourceService.GetLocalized("Downloading/DownloadingCountInfo");
        private bool isInitialized;

        private bool _isSelectMode;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                if (!Equals(_isSelectMode, value))
                {
                    _isSelectMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectMode)));
                }
            }
        }

        private ObservableCollection<DownloadingModel> DownloadingCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadingPage()
        {
            InitializeComponent();
        }

        #region 重载父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;

                DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerService.DownloadSchedulerList)
                    {
                        DownloadingCollection.Add(new DownloadingModel()
                        {
                            IsSelected = false,
                            IsSelectMode = false,
                            IsNotOperated = true,
                            DownloadID = downloadSchedulerItem.DownloadID,
                            FileName = downloadSchedulerItem.FileName,
                            FilePath = downloadSchedulerItem.FilePath,
                            DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                            CompletedSize = downloadSchedulerItem.CompletedSize,
                            TotalSize = downloadSchedulerItem.TotalSize,
                            DownloadSpeed = downloadSchedulerItem.DownloadSpeed
                        });
                    }

                    if (!DownloadSchedulerService.IsDownloadingPageInitialized)
                    {
                        DownloadSchedulerService.IsDownloadingPageInitialized = true;

                        foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerService.DownloadFailedList)
                        {
                            DownloadingCollection.Add(new DownloadingModel()
                            {
                                IsSelected = false,
                                IsSelectMode = false,
                                IsNotOperated = true,
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed
                            });
                        }

                        DownloadSchedulerService.DownloadFailedList.Clear();
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
                }

                await Task.Run(() =>
                {
                    GlobalNotificationService.ApplicationExit += OnApplicationExit;
                    DownloadSchedulerService.DownloadProgress += OnDownloadProgress;
                });
            }
        }

        #endregion 重载父类事件

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 继续下载当前任务
        /// </summary>
        private void OnContinueExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadingModel downloading && !string.IsNullOrEmpty(downloading.DownloadID))
            {
                downloading.IsNotOperated = false;
                DownloadSchedulerService.ContinueDownload(downloading.DownloadID);
            }
        }

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadingModel downloading && !string.IsNullOrEmpty(downloading.DownloadID))
            {
                if (downloading.DownloadProgressState is DownloadProgressState.Queued || downloading.DownloadProgressState is DownloadProgressState.Downloading || downloading.DownloadProgressState is DownloadProgressState.Paused)
                {
                    DownloadSchedulerService.DeleteDownload(downloading.DownloadID);
                }
                else
                {
                    DownloadingCollection.Remove(downloading);
                }
            }
        }

        /// <summary>
        /// 暂停下载当前任务
        /// </summary>
        private void OnPauseExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadingModel downloading && !string.IsNullOrEmpty(downloading.DownloadID))
            {
                downloading.IsNotOperated = false;
                DownloadSchedulerService.PauseDownload(downloading.DownloadID);
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：下载中页面——挂载的事件

        /// <summary>
        /// 继续下载全部任务
        /// </summary>
        private void OnContinueAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                if (downloadingItem.DownloadProgressState is DownloadProgressState.Paused)
                {
                    downloadingItem.IsNotOperated = false;
                    DownloadSchedulerService.ContinueDownload(downloadingItem.DownloadID);
                }
            }
        }

        /// <summary>
        /// 暂停下载全部任务
        /// </summary>
        private void OnPauseAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                if (downloadingItem.DownloadProgressState is DownloadProgressState.Queued || downloadingItem.DownloadProgressState is DownloadProgressState.Downloading)
                {
                    downloadingItem.IsNotOperated = false;
                    DownloadSchedulerService.PauseDownload(downloadingItem.DownloadID);
                }
            }
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        private void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.IsSelectMode = true;
                downloadingItem.IsSelected = false;
            }

            IsSelectMode = true;
        }

        /// <summary>
        /// 全部选择
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.IsSelected = true;
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.IsSelected = false;
            }
        }

        /// <summary>
        /// 打开默认保存的文件夹
        /// </summary>
        private void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(DownloadOptionsService.OpenFolderAsync);
        }

        /// <summary>
        /// 了解应用具体的使用说明
        /// </summary>
        private async void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is DownloadPage downloadPage)
            {
                await Task.Delay(300);
                downloadPage.ShowUseInstruction();
            }
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        private async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<DownloadingModel> selectedDownloadingList = [];

            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                if (downloadingItem.IsSelected)
                {
                    selectedDownloadingList.Add(downloadingItem);
                }
            }

            // 没有选中任何内容时显示空提示对话框
            if (selectedDownloadingList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.SelectEmpty));
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await MainWindow.Current.ShowDialogAsync(new DeletePromptDialog(DeleteKind.Download));

            if (result is ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                for (int index = DownloadingCollection.Count - 1; index >= 0; index--)
                {
                    DownloadingModel downloadingItem = DownloadingCollection[index];
                    downloadingItem.IsSelectMode = false;

                    if (downloadingItem.IsSelected)
                    {
                        downloadingItem.IsSelected = false;
                        downloadingItem.IsNotOperated = false;

                        if (downloadingItem.DownloadProgressState is DownloadProgressState.Queued || downloadingItem.DownloadProgressState is DownloadProgressState.Downloading || downloadingItem.DownloadProgressState is DownloadProgressState.Paused)
                        {
                            DownloadSchedulerService.DeleteDownload(downloadingItem.DownloadID);
                        }
                        else
                        {
                            DownloadingCollection.RemoveAt(index);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 取消进入选择模式
        /// </summary>
        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            IsSelectMode = false;

            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.IsSelectMode = false;
            }
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        private void OnItemClick(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is DownloadingModel downloading)
            {
                downloading.IsSelected = !downloading.IsSelected;
            }
        }

        #endregion 第二部分：下载中页面——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                DownloadSchedulerService.DownloadProgress -= OnDownloadProgress;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Unregister Download scheduler service event failed", e);
            }
        }

        /// <summary>
        /// 下载状态发生改变时触发的事件
        /// </summary>
        private void OnDownloadProgress(DownloadSchedulerModel downloadScheduler)
        {
            // 处于等待中（新添加下载任务或者已经恢复下载）
            if (downloadScheduler.DownloadProgressState is DownloadProgressState.Queued)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    // 下载任务已经存在，更新下载状态
                    foreach (DownloadingModel downloadingItem in DownloadingCollection)
                    {
                        if (Equals(downloadingItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadingItem.IsNotOperated = true;
                            downloadingItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            return;
                        }
                    }

                    // 不存在则添加任务
                    DownloadingModel downloading = new()
                    {
                        IsSelected = false,
                        IsSelectMode = false,
                        IsNotOperated = true,
                        DownloadID = downloadScheduler.DownloadID,
                        FileName = downloadScheduler.FileName,
                        FilePath = downloadScheduler.FilePath,
                        DownloadProgressState = downloadScheduler.DownloadProgressState,
                        CompletedSize = downloadScheduler.CompletedSize,
                        TotalSize = downloadScheduler.TotalSize,
                        DownloadSpeed = downloadScheduler.DownloadSpeed
                    };

                    DownloadingCollection.Add(downloading);
                });
            }
            // 下载任务正在下载中
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Downloading)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (DownloadingModel downloadingItem in DownloadingCollection)
                    {
                        if (Equals(downloadingItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadingItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            downloadingItem.DownloadSpeed = downloadScheduler.DownloadSpeed;
                            downloadingItem.CompletedSize = downloadScheduler.CompletedSize;
                            downloadingItem.TotalSize = downloadScheduler.TotalSize;
                            return;
                        }
                    }
                });
            }
            // 下载任务已暂停或已失败
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Paused || downloadScheduler.DownloadProgressState is DownloadProgressState.Failed)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (DownloadingModel downloadingItem in DownloadingCollection)
                    {
                        if (Equals(downloadingItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadingItem.IsNotOperated = true;
                            downloadingItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            return;
                        }
                    }
                });
            }
            // 下载任务已完成
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Finished)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (DownloadingModel downloadingItem in DownloadingCollection)
                    {
                        if (Equals(downloadingItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadingItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            downloadingItem.DownloadSpeed = downloadScheduler.DownloadSpeed;
                            downloadingItem.CompletedSize = downloadScheduler.CompletedSize;
                            downloadingItem.TotalSize = downloadScheduler.TotalSize;
                            downloadingItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            DownloadingCollection.Remove(downloadingItem);
                            return;
                        }
                    }
                });
            }
            // 下载任务已删除
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Deleted)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (DownloadingModel downloadingItem in DownloadingCollection)
                    {
                        if (Equals(downloadingItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            DownloadingCollection.Remove(downloadingItem);
                            return;
                        }
                    }
                });
            }
        }

        #endregion 第三部分：自定义事件
    }
}
