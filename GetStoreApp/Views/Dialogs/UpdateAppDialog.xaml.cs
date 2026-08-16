using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Services.Store;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 更新应用对话框
    /// </summary>
    internal sealed partial class UpdateAppDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string CancelString = ResourceService.GetLocalized("Dialog/Cancel");
        private readonly string CloseString = ResourceService.GetLocalized("Dialog/Close");
        private readonly string CloseAppString = ResourceService.GetLocalized("Dialog/CloseApp");
        private readonly string UpdateString = ResourceService.GetLocalized("Dialog/Update");
        private readonly string UpdateDownloadingString = ResourceService.GetLocalized("Dialog/UpdateDownloading");
        private IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> storePackageUpdateProgress = null;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private UpdateAppResultKind _updateAppResultKind;

        private UpdateAppResultKind UpdateAppResultKind
        {
            get { return _updateAppResultKind; }

            set
            {
                if (!Equals(_updateAppResultKind, value))
                {
                    _updateAppResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(UpdateAppResultKind)));
                }
            }
        }

        private string _primaryText;

        private string PrimaryText
        {
            get { return _primaryText; }

            set
            {
                if (!string.Equals(_primaryText, value))
                {
                    _primaryText = value;
                    PropertyChanged?.Invoke(this, new(nameof(PrimaryText)));
                }
            }
        }

        private string _closeText;

        private string CloseText
        {
            get { return _closeText; }

            set
            {
                if (!string.Equals(_closeText, value))
                {
                    _closeText = value;
                    PropertyChanged?.Invoke(this, new(nameof(CloseText)));
                }
            }
        }

        private string _updateDownloadString;

        private string UpdateDownloadString
        {
            get { return _updateDownloadString; }

            set
            {
                if (!string.Equals(_updateDownloadString, value))
                {
                    _updateDownloadString = value;
                    PropertyChanged?.Invoke(this, new(nameof(UpdateDownloadString)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal UpdateAppDialog()
        {
            InitializeComponent();
            UpdateAppResultKind = UpdateAppResultKind.Initialize;
            PrimaryText = UpdateString;
            CloseText = CloseString;
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 对话框关闭后触发的事件
        /// </summary>
        private void OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            if (storePackageUpdateProgress is not null && (UpdateAppResultKind is UpdateAppResultKind.Pending || UpdateAppResultKind is UpdateAppResultKind.Downloading || UpdateAppResultKind is UpdateAppResultKind.Deploying))
            {
                try
                {
                    CancelUpdate();
                    UpdateAppResultKind = UpdateAppResultKind.Canceling;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(UpdateAppDialog), nameof(OnCancelOrCloseClicked), 1, e);
                }
            }
        }

        /// <summary>
        /// 更新应用
        /// </summary>
        private async void OnUpdateClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                args.Cancel = true;
                if (UpdateAppResultKind is UpdateAppResultKind.Successfully)
                {
                    (Application.Current as MainApp).Dispose();
                }
                else
                {
                    UpdateAppResultKind = UpdateAppResultKind.Pending;
                    UpdateDownloadString = string.Format(UpdateDownloadingString, VolumeSizeHelper.ConvertVolumeSizeToString(0), VolumeSizeHelper.ConvertVolumeSizeToString(0));
                    CloseText = CancelString;
                    if (storePackageUpdateProgress is null)
                    {
                        UpdateAppResultKind = await UpdateStorePackageAppAsync();
                        CloseText = CloseString;
                        PrimaryText = UpdateAppResultKind is UpdateAppResultKind.Successfully ? CloseAppString : UpdateString;
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                UpdateAppResultKind = UpdateAppResultKind.Canceled;
                storePackageUpdateProgress = null;
                CloseText = CloseString;
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(UpdateAppDialog), nameof(OnUpdateClicked), 1, e);
            }
            catch (Exception e)
            {
                UpdateAppResultKind = UpdateAppResultKind.Failed;
                storePackageUpdateProgress = null;
                CloseText = CloseString;
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(UpdateAppDialog), nameof(OnUpdateClicked), 2, e);
            }
        }

        /// <summary>
        /// 取消更新或关闭更新窗口
        /// </summary>
        private void OnCancelOrCloseClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (UpdateAppResultKind is UpdateAppResultKind.Pending || UpdateAppResultKind is UpdateAppResultKind.Downloading || UpdateAppResultKind is UpdateAppResultKind.Deploying)
            {
                args.Cancel = true;

                if (storePackageUpdateProgress is not null)
                {
                    try
                    {
                        CancelUpdate();
                        UpdateAppResultKind = UpdateAppResultKind.Canceling;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(UpdateAppDialog), nameof(OnCancelOrCloseClicked), 1, e);
                    }
                }
                else
                {
                    UpdateAppResultKind = UpdateAppResultKind.Canceled;
                }
            }
        }

        /// <summary>
        /// 应用更新进度发生变化时触发的事件
        /// </summary>
        private void OnStorePackageUpdateProgress(IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> sender, StorePackageUpdateStatus progress)
        {
            if (progress.PackageUpdateState is StorePackageUpdateState.Pending)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                    {
                        UpdateAppResultKind = UpdateAppResultKind.Pending;
                        CloseText = CancelString;
                    }
                });
            }
            else if (progress.PackageUpdateState is StorePackageUpdateState.Downloading)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    string downloadedSize = VolumeSizeHelper.ConvertVolumeSizeToString(progress.PackageDownloadSizeInBytes);
                    string totalSize = VolumeSizeHelper.ConvertVolumeSizeToString(progress.PackageBytesDownloaded);
                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                    {
                        UpdateAppResultKind = UpdateAppResultKind.Downloading;
                        UpdateDownloadString = string.Format(UpdateDownloadingString, downloadedSize, totalSize);
                        CloseText = CancelString;
                    }
                });
            }
            else if (progress.PackageUpdateState is StorePackageUpdateState.Deploying)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                    {
                        UpdateAppResultKind = UpdateAppResultKind.Deploying;
                        CloseText = CancelString;
                    }
                });
            }
            else if (progress.PackageUpdateState is StorePackageUpdateState.Canceled)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                    {
                        UpdateAppResultKind = UpdateAppResultKind.Canceled;
                        CloseText = CloseString;
                    }
                });
            }
            else if (progress.PackageUpdateState is StorePackageUpdateState.OtherError)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                    {
                        UpdateAppResultKind = UpdateAppResultKind.Failed;
                        CloseText = CloseString;
                    }
                });
            }
            else if (progress.PackageUpdateState is StorePackageUpdateState.ErrorLowBattery)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                    {
                        UpdateAppResultKind = UpdateAppResultKind.Failed;
                        CloseText = CloseString;
                    }
                });
            }
            else if (progress.PackageUpdateState is StorePackageUpdateState.ErrorWiFiRecommended)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                    {
                        UpdateAppResultKind = UpdateAppResultKind.Failed;
                        CloseText = CloseString;
                    }
                });
            }
            else if (progress.PackageUpdateState is StorePackageUpdateState.ErrorWiFiRequired)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                    {
                        UpdateAppResultKind = UpdateAppResultKind.Failed;
                        CloseText = CloseString;
                    }
                });
            }
        }

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 取消更新
        /// </summary>
        private void CancelUpdate()
        {
            storePackageUpdateProgress?.Cancel();
        }

        /// <summary>
        /// 更新商店应用
        /// </summary>
        private async Task<UpdateAppResultKind> UpdateStorePackageAppAsync()
        {
            try
            {
                StoreContext storeContext = StoreContext.GetDefault();
                IReadOnlyList<StorePackageUpdate> storePackageUpdateList = await storeContext.GetAppAndOptionalStorePackageUpdatesAsync();
                storePackageUpdateProgress = storeContext.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdateList);
                storePackageUpdateProgress.Progress += OnStorePackageUpdateProgress;
                StorePackageUpdateResult storePackageUpdateResult = await storePackageUpdateProgress;
                storePackageUpdateProgress?.Progress -= OnStorePackageUpdateProgress;
                if (storePackageUpdateResult.OverallState is StorePackageUpdateState.Completed)
                {
                    bool isUpdateFailed = false;
                    foreach (StorePackageUpdateStatus storePackageUpdateStatus in storePackageUpdateResult.StorePackageUpdateStatuses)
                    {
                        if (storePackageUpdateStatus.PackageUpdateState is StorePackageUpdateState.Canceled ||
                            storePackageUpdateStatus.PackageUpdateState is StorePackageUpdateState.OtherError ||
                            storePackageUpdateStatus.PackageUpdateState is StorePackageUpdateState.ErrorLowBattery ||
                            storePackageUpdateStatus.PackageUpdateState is StorePackageUpdateState.ErrorWiFiRecommended ||
                            storePackageUpdateStatus.PackageUpdateState is StorePackageUpdateState.ErrorWiFiRequired)
                        {
                            isUpdateFailed = true;
                        }
                    }

                    return isUpdateFailed ? UpdateAppResultKind.Failed : UpdateAppResultKind.Successfully;
                }
                else
                {
                    return UpdateAppResultKind.Failed;
                }
            }
            catch (OperationCanceledException e)
            {
                storePackageUpdateProgress = null;
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(UpdateAppDialog), nameof(UpdateStorePackageAppAsync), 1, e);
                return UpdateAppResultKind.Canceled;
            }
            catch (Exception e)
            {
                storePackageUpdateProgress = null;
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(UpdateAppDialog), nameof(UpdateStorePackageAppAsync), 2, e);
                return UpdateAppResultKind.Failed;
            }
        }

        /// <summary>
        /// 检查应用是否正在更新中
        /// </summary>
        private bool GetIsNotUpdating(UpdateAppResultKind updateAppResultKind)
        {
            return !(updateAppResultKind is UpdateAppResultKind.Pending || UpdateAppResultKind is UpdateAppResultKind.Downloading || UpdateAppResultKind is UpdateAppResultKind.Canceling || UpdateAppResultKind is UpdateAppResultKind.Deploying);
        }

        /// <summary>
        /// 检查应用是否正在更新中
        /// </summary>
        private Visibility GetUpdatingAppVisibility(UpdateAppResultKind updateAppResultKind)
        {
            return (updateAppResultKind is UpdateAppResultKind.Pending || UpdateAppResultKind is UpdateAppResultKind.Downloading || UpdateAppResultKind is UpdateAppResultKind.Canceling || UpdateAppResultKind is UpdateAppResultKind.Deploying) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查应用是否正在取消更新中
        /// </summary>
        private bool GetIsNotCanceling(UpdateAppResultKind updateAppResultKind)
        {
            return updateAppResultKind is not UpdateAppResultKind.Canceling;
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}
