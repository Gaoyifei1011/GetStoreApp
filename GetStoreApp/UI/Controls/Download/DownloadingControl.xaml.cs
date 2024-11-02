using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.UI.Controls.Download
{
    /// <summary>
    /// 下载页面：正在下载中控件
    /// </summary>
    public sealed partial class DownloadingControl : Grid, INotifyPropertyChanged
    {
        private bool isInitialized;

        private string DownloadingCountInfo { get; } = ResourceService.GetLocalized("Download/DownloadingCountInfo");

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

        public DownloadingControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 继续下载当前任务
        /// </summary>
        private void OnContinueExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Guid downloadID = (Guid)args.Parameter;

            if (downloadID != Guid.Empty)
            {
                foreach (DownloadingModel downloadingItem in DownloadingCollection)
                {
                    if (downloadingItem.DownloadID.Equals(downloadID))
                    {
                        downloadingItem.IsNotOperated = false;
                        break;
                    }
                }

                DownloadSchedulerService.ContinueDownload(downloadID);
            }
        }

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Guid downloadID = (Guid)args.Parameter;

            if (downloadID != Guid.Empty)
            {
                foreach (DownloadingModel downloadingItem in DownloadingCollection)
                {
                    if (downloadingItem.DownloadID.Equals(downloadID))
                    {
                        downloadingItem.IsNotOperated = false;
                        break;
                    }
                }

                DownloadSchedulerService.DeleteDownload(downloadID);
            }
        }

        /// <summary>
        /// 暂停下载当前任务
        /// </summary>
        private void OnPauseExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Guid downloadID = (Guid)args.Parameter;

            if (downloadID != Guid.Empty)
            {
                foreach (DownloadingModel downloadingItem in DownloadingCollection)
                {
                    if (downloadingItem.DownloadID.Equals(downloadID))
                    {
                        downloadingItem.IsNotOperated = false;
                        break;
                    }
                }

                DownloadSchedulerService.PauseDownload(downloadID);
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：正在下载中控件——挂载的事件

        /// <summary>
        /// 正在下载控件初始化完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                isInitialized = true;

                List<DownloadSchedulerModel> downloadingSchedulerList = await Task.Run(() =>
                {
                    GlobalNotificationService.ApplicationExit += OnApplicationExit;
                    DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();

                    try
                    {
                        return DownloadSchedulerService.GetDownloadSchedulerList();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Initialize DownloadingCollection failed", e);
                    }
                    finally
                    {
                        DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
                    }

                    return [];
                });

                foreach (DownloadSchedulerModel downloadSchedulerItem in downloadingSchedulerList)
                {
                    DownloadingCollection.Add(new DownloadingModel()
                    {
                        DownloadID = downloadSchedulerItem.DownloadID,
                        DownloadStatus = downloadSchedulerItem.DownloadStatus,
                        FileName = downloadSchedulerItem.FileName,
                        FilePath = downloadSchedulerItem.FilePath,
                        FileLink = downloadSchedulerItem.FileLink,
                        FinishedSize = downloadSchedulerItem.FinishedSize,
                        IsNotOperated = true,
                        CurrentSpeed = 0,
                        TotalSize = downloadSchedulerItem.TotalSize,
                        IsSelected = false,
                        IsSelectMode = false
                    });
                }

                await Task.Run(() =>
                {
                    DownloadSchedulerService.DownloadCreated += OnDownloadCreated;
                    DownloadSchedulerService.DownloadContinued += OnDownloadContinued;
                    DownloadSchedulerService.DownloadPaused += OnDownloadPaused;
                    DownloadSchedulerService.DownloadDeleted += OnDownloadDeleted;
                    DownloadSchedulerService.DownloadProgressing += OnDownloadProgressing;
                    DownloadSchedulerService.DownloadCompleted += OnDownloadCompleted;
                });
            }
        }

        /// <summary>
        /// 打开默认保存的文件夹
        /// </summary>
        private async void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        }

        /// <summary>
        /// 继续下载全部任务
        /// </summary>
        private void OnContinueAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                if (downloadingItem.DownloadStatus is DownloadStatus.Pause)
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
                if (downloadingItem.DownloadStatus is DownloadStatus.Downloading)
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
        /// 删除选中的任务
        /// </summary>
        private async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<DownloadingModel> selectedDownloadingList = [];

            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                if (downloadingItem.IsSelected is true)
                {
                    selectedDownloadingList.Add(downloadingItem);
                }
            }

            // 没有选中任何内容时显示空提示对话框
            if (selectedDownloadingList.Count is 0)
            {
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await ContentDialogHelper.ShowAsync(new DeletePromptDialog(DeleteKind.Download), this);

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

                        if (downloadingItem.DownloadStatus is DownloadStatus.Downloading || downloadingItem.DownloadStatus is DownloadStatus.Pause)
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
        private void OnItemInvoked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is DownloadingModel downloadingItem)
            {
                int clickedIndex = DownloadingCollection.IndexOf(downloadingItem);

                if (clickedIndex >= 0 && clickedIndex < DownloadingCollection.Count)
                {
                    DownloadingCollection[clickedIndex].IsSelected = !DownloadingCollection[clickedIndex].IsSelected;
                }
            }
        }

        #endregion 第二部分：正在下载中控件——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit(object sender, EventArgs args)
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                DownloadSchedulerService.DownloadCreated -= OnDownloadCreated;
                DownloadSchedulerService.DownloadContinued -= OnDownloadContinued;
                DownloadSchedulerService.DownloadPaused -= OnDownloadPaused;
                DownloadSchedulerService.DownloadDeleted -= OnDownloadDeleted;
                DownloadSchedulerService.DownloadProgressing -= OnDownloadProgressing;
                DownloadSchedulerService.DownloadCompleted -= OnDownloadCompleted;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Unregister Download scheduler service event failed", e);
            }
        }

        /// <summary>
        /// 下载任务已创建
        /// </summary>
        private void OnDownloadCreated(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                DownloadingCollection.Add(new DownloadingModel()
                {
                    DownloadID = downloadSchedulerItem.DownloadID,
                    DownloadStatus = downloadSchedulerItem.DownloadStatus,
                    FileName = downloadSchedulerItem.FileName,
                    FilePath = downloadSchedulerItem.FilePath,
                    FileLink = downloadSchedulerItem.FileLink,
                    FinishedSize = 0,
                    IsNotOperated = true,
                    CurrentSpeed = 0,
                    TotalSize = downloadSchedulerItem.TotalSize,
                    IsSelected = false,
                    IsSelectMode = false
                });
            });
        }

        /// <summary>
        /// 下载任务已继续下载
        /// </summary>
        private void OnDownloadContinued(Guid downloadID)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (DownloadingModel downloadingItem in DownloadingCollection)
                {
                    if (downloadingItem.DownloadID.Equals(downloadID))
                    {
                        downloadingItem.IsNotOperated = true;
                        downloadingItem.DownloadStatus = DownloadStatus.Downloading;
                    }
                }
            });
        }

        /// <summary>
        /// 下载任务已暂停下载
        /// </summary>

        private void OnDownloadPaused(Guid downloadID)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (DownloadingModel downloadingItem in DownloadingCollection)
                {
                    if (downloadingItem.DownloadID.Equals(downloadID))
                    {
                        downloadingItem.IsNotOperated = true;
                        downloadingItem.DownloadStatus = DownloadStatus.Pause;
                    }
                }
            });
        }

        /// <summary>
        /// 下载任务已删除
        /// </summary>
        private void OnDownloadDeleted(Guid downloadID)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (DownloadingModel downloadingItem in DownloadingCollection)
                {
                    if (downloadingItem.DownloadID.Equals(downloadID))
                    {
                        DownloadingCollection.Remove(downloadingItem);
                        break;
                    }
                }
            });
        }

        /// <summary>
        /// 下载任务下载进度发生变化
        /// </summary>

        private void OnDownloadProgressing(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (DownloadingModel downloadingItem in DownloadingCollection)
                {
                    if (downloadingItem.DownloadID.Equals(downloadID))
                    {
                        downloadingItem.IsNotOperated = true;
                        downloadingItem.DownloadStatus = downloadSchedulerItem.DownloadStatus;
                        downloadingItem.CurrentSpeed = downloadSchedulerItem.CurrentSpeed;
                        downloadingItem.FinishedSize = downloadSchedulerItem.FinishedSize;
                        downloadingItem.TotalSize = downloadSchedulerItem.TotalSize;
                    }
                }
            });
        }

        /// <summary>
        /// 下载任务已下载完成
        /// </summary>

        private void OnDownloadCompleted(Guid downloadID, DownloadSchedulerModel downloadSchedulerItem)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (DownloadingModel downloadingItem in DownloadingCollection)
                {
                    if (downloadingItem.DownloadID.Equals(downloadID))
                    {
                        downloadingItem.IsNotOperated = true;
                        downloadingItem.DownloadStatus = downloadSchedulerItem.DownloadStatus;
                        downloadingItem.CurrentSpeed = downloadSchedulerItem.CurrentSpeed;
                        downloadingItem.FinishedSize = downloadSchedulerItem.FinishedSize;
                        downloadingItem.TotalSize = downloadSchedulerItem.TotalSize;
                        DownloadingCollection.Remove(downloadingItem);
                        break;
                    }
                }
            });
        }

        #endregion 第三部分：自定义事件
    }
}
