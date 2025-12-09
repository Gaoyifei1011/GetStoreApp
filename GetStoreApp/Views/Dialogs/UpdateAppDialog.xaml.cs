using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class UpdateAppDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string CancelString = ResourceService.GetLocalized("Dialog/Cancel");
        private readonly string CloseString = ResourceService.GetLocalized("Dialog/Close");
        private readonly string CloseAppString = ResourceService.GetLocalized("Dialog/CloseApp");
        private readonly string UpdateString = ResourceService.GetLocalized("Dialog/Update");
        private readonly string UpdateDownloadingString = ResourceService.GetLocalized("Dialog/UpdateDownloading");
        private IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> storePackageUpdateProgress = null;

        private UpdateAppResultKind _updateAppResultKind = UpdateAppResultKind.Initialize;

        public UpdateAppResultKind UpdateAppResultKind
        {
            get { return _updateAppResultKind; }

            set
            {
                if (!Equals(_updateAppResultKind, value))
                {
                    _updateAppResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateAppResultKind)));
                }
            }
        }

        private string _primaryText;

        public string PrimaryText
        {
            get { return _primaryText; }

            set
            {
                if (!string.Equals(_primaryText, value))
                {
                    _primaryText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrimaryText)));
                }
            }
        }

        private string _closeText;

        public string CloseText
        {
            get { return _closeText; }

            set
            {
                if (!string.Equals(_closeText, value))
                {
                    _closeText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CloseText)));
                }
            }
        }

        private string _updateDownloadString;

        public string UpdateDownloadString
        {
            get { return _updateDownloadString; }

            set
            {
                if (!string.Equals(_updateDownloadString, value))
                {
                    _updateDownloadString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpdateDownloadString)));
                }
            }
        }

        private bool _isCancelingUpdate;

        public bool IsCancelingUpdate
        {
            get { return _isCancelingUpdate; }

            set
            {
                if (!Equals(_isCancelingUpdate, value))
                {
                    _isCancelingUpdate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCancelingUpdate)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public UpdateAppDialog()
        {
            InitializeComponent();
            PrimaryText = UpdateString;
            CloseText = CloseString;
        }

        /// <summary>
        /// 对话框关闭后触发的事件
        /// </summary>
        private void OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            if (storePackageUpdateProgress is not null && (UpdateAppResultKind is UpdateAppResultKind.Pending || UpdateAppResultKind is UpdateAppResultKind.Downloading || UpdateAppResultKind is UpdateAppResultKind.Deploying))
            {
                try
                {
                    storePackageUpdateProgress.Cancel();
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
                        StoreContext storeContext = StoreContext.GetDefault();
                        IReadOnlyList<StorePackageUpdate> storePackageUpdateList = await storeContext.GetAppAndOptionalStorePackageUpdatesAsync();
                        bool updateFailed = false;
                        storePackageUpdateProgress = storeContext.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdateList);
                        storePackageUpdateProgress.Progress += (sender, progress) =>
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
                                updateFailed = true;
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
                                updateFailed = true;
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
                                updateFailed = true;
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
                                updateFailed = true;
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
                                updateFailed = true;
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    if (UpdateAppResultKind is not UpdateAppResultKind.Canceling)
                                    {
                                        UpdateAppResultKind = UpdateAppResultKind.Failed;
                                        CloseText = CloseString;
                                    }
                                });
                            }
                        };
                        StorePackageUpdateResult storePackageUpdateResult = await storePackageUpdateProgress;
                        CloseText = CloseString;
                        if (storePackageUpdateResult.OverallState is StorePackageUpdateState.Completed)
                        {
                            if (updateFailed)
                            {
                                UpdateAppResultKind = UpdateAppResultKind.Failed;
                            }
                            else
                            {
                                UpdateAppResultKind = UpdateAppResultKind.Successfully;
                                PrimaryText = CloseAppString;
                            }
                        }
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
                        storePackageUpdateProgress.Cancel();
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
        /// 检查应用是否正在更新中
        /// </summary>
        private bool GetIsNotUpdating(UpdateAppResultKind updateAppResultKind)
        {
            return !(updateAppResultKind is UpdateAppResultKind.Pending || UpdateAppResultKind is UpdateAppResultKind.Downloading || UpdateAppResultKind is UpdateAppResultKind.Canceling || UpdateAppResultKind is UpdateAppResultKind.Deploying);
        }

        /// <summary>
        /// 检查应用是否正在更新中
        /// </summary>
        private Visibility GetUpdateProgressState(UpdateAppResultKind updateAppResultKind)
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
    }
}
