using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.NotificationTips;
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
    internal sealed partial class DownloadingPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string DownloadingCountInfoString = ResourceService.GetLocalized("Downloading/DownloadingCountInfo");
        private bool isInitialized;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private ListViewSelectionMode _selectionMode;

        private ListViewSelectionMode SelectionMode
        {
            get { return _selectionMode; }

            set
            {
                if (!Equals(_selectionMode, value))
                {
                    _selectionMode = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectionMode)));
                }
            }
        }

        private ObservableCollection<DownloadingModel> DownloadingCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal DownloadingPage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                await InitializeDataAsync();
                await MountDownloadEventAsync();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 继续下载当前任务
        /// </summary>
        private void OnContinueExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadingModel downloading && !string.IsNullOrEmpty(downloading.DownloadID))
            {
                downloading.IsOperating = true;
                ContinueDownload(downloading);
            }
        }

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadingModel downloading && !string.IsNullOrEmpty(downloading.DownloadID))
            {
                DeleteDownload(downloading);
            }
        }

        /// <summary>
        /// 暂停下载当前任务
        /// </summary>
        private void OnPauseExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is DownloadingModel downloading && !string.IsNullOrEmpty(downloading.DownloadID))
            {
                downloading.IsOperating = true;
                PauseDownload(downloading);
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 继续下载全部任务
        /// </summary>
        private void OnContinueAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                if (downloadingItem.DownloadProgressState is DownloadProgressState.Paused)
                {
                    downloadingItem.IsOperating = true;
                    ContinueDownload(downloadingItem);
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
                    downloadingItem.IsOperating = true;
                    PauseDownload(downloadingItem);
                }
            }
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        private void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            SelectionMode = ListViewSelectionMode.Multiple;
            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.SelectionMode = ListViewSelectionMode.Multiple;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            DownloadingListView.SelectAll();
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            DownloadingListView.DeselectRange(new(0, (uint)DownloadingListView.Items.Count));
        }

        /// <summary>
        /// 全部反选
        /// </summary>
        private void OnSelectReverseClicked(object sender, RoutedEventArgs args)
        {
            List<object> selectedItemsList = [.. DownloadingListView.SelectedItems];

            foreach (object item in DownloadingListView.Items)
            {
                if (selectedItemsList.Contains(item))
                {
                    DownloadingListView.SelectedItems.Remove(item);
                }
                else
                {
                    DownloadingListView.SelectedItems.Add(item);
                }
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
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is DownloadPage downloadPage)
            {
                downloadPage.ShowUseInstruction();
            }
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        private async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<DownloadingModel> selectedDownloadingList = GetSelectedDownloadingList([.. DownloadingCollection]);

            // 没有选中任何内容时显示空提示对话框
            if (selectedDownloadingList is null || selectedDownloadingList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectEmpty));
                return;
            }

            SelectionMode = ListViewSelectionMode.None;

            foreach (DownloadingModel downloadingItem in selectedDownloadingList)
            {
                downloadingItem.IsOperating = true;
                DeleteDownload(downloadingItem);
            }
        }

        /// <summary>
        /// 取消进入选择模式
        /// </summary>
        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            SelectionMode = ListViewSelectionMode.None;

            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.SelectionMode = ListViewSelectionMode.None;
            }
        }

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            DismountDownloadEvent();
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
                        if (string.Equals(downloadingItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadingItem.IsOperating = false;
                            downloadingItem.DownloadProgressState = downloadScheduler.DownloadProgressState;
                            return;
                        }
                    }

                    // 不存在则添加任务
                    DownloadingCollection.Add(new()
                    {
                        SelectionMode = SelectionMode,
                        IsOperating = false,
                        DownloadID = downloadScheduler.DownloadID,
                        FileName = downloadScheduler.FileName,
                        FilePath = downloadScheduler.FilePath,
                        DownloadProgressState = downloadScheduler.DownloadProgressState,
                        CompletedSize = downloadScheduler.CompletedSize,
                        TotalSize = downloadScheduler.TotalSize,
                        DownloadSpeed = downloadScheduler.DownloadSpeed
                    });
                });
            }
            // 下载任务正在下载中
            else if (downloadScheduler.DownloadProgressState is DownloadProgressState.Downloading)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (DownloadingModel downloadingItem in DownloadingCollection)
                    {
                        if (string.Equals(downloadingItem.DownloadID, downloadScheduler.DownloadID))
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
                        if (string.Equals(downloadingItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            downloadingItem.IsOperating = false;
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
                        if (string.Equals(downloadingItem.DownloadID, downloadScheduler.DownloadID))
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
                        if (string.Equals(downloadingItem.DownloadID, downloadScheduler.DownloadID))
                        {
                            DownloadingCollection.Remove(downloadingItem);
                            return;
                        }
                    }
                });
            }
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private async Task InitializeDataAsync()
        {
            DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();

            try
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerService.DownloadSchedulerList)
                {
                    DownloadingCollection.Add(new()
                    {
                        SelectionMode = SelectionMode,
                        IsOperating = false,
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
                        DownloadingCollection.Add(new()
                        {
                            SelectionMode = SelectionMode,
                            IsOperating = false,
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
        }

        /// <summary>
        /// 挂载与下载相关的事件
        /// </summary>
        private async Task MountDownloadEventAsync()
        {
            await Task.Run(() =>
            {
                GlobalNotificationService.ApplicationExit += OnApplicationExit;
                DownloadSchedulerService.DownloadProgress += OnDownloadProgress;
            });
        }

        /// <summary>
        /// 卸载与下载相关的事件
        /// </summary>
        private void DismountDownloadEvent()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                DownloadSchedulerService.DownloadProgress -= OnDownloadProgress;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadingPage), nameof(DismountDownloadEvent), 1, e);
            }
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        private List<DownloadingModel> GetSelectedDownloadingList(List<DownloadingModel> downloadingList)
        {
            if (downloadingList is null || downloadingList.Count is 0)
            {
                return default;
            }

            List<DownloadingModel> selectedDownloadingList = [];

            foreach (object downloadingItemObj in downloadingList)
            {
                if (downloadingItemObj is DownloadingModel downloadingItem)
                {
                    selectedDownloadingList.Add(downloadingItem);
                }
            }

            return selectedDownloadingList;
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        private void ContinueDownload(DownloadingModel downloading)
        {
            if (downloading is null)
            {
                return;
            }

            DownloadSchedulerService.ContinueDownload(downloading.DownloadID);
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        private void DeleteDownload(DownloadingModel downloading)
        {
            if (downloading is null)
            {
                return;
            }

            if (downloading.DownloadProgressState is DownloadProgressState.Queued || downloading.DownloadProgressState is DownloadProgressState.Downloading || downloading.DownloadProgressState is DownloadProgressState.Paused)
            {
                DownloadSchedulerService.DeleteDownload(downloading.DownloadID);
            }
            else
            {
                DownloadingCollection.Remove(downloading);
            }
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        private void PauseDownload(DownloadingModel downloading)
        {
            if (downloading is null)
            {
                return;
            }

            DownloadSchedulerService.PauseDownload(downloading.DownloadID);
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}
