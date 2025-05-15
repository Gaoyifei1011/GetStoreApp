using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Settings;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 数据源页面
    /// </summary>
    public sealed partial class SettingsWinGetSourcePage : Page, INotifyPropertyChanged
    {
        private readonly string None = ResourceService.GetLocalized("Settings/None");
        private readonly string Yes = ResourceService.GetLocalized("Settings/Yes");
        private readonly string No = ResourceService.GetLocalized("Settings/No");
        private readonly string Trusted = ResourceService.GetLocalized("Settings/Trusted");
        private readonly string Distrusted = ResourceService.GetLocalized("Settings/Distrusted");
        private readonly string Predefined = ResourceService.GetLocalized("Settings/Predefined");
        private readonly string User = ResourceService.GetLocalized("Settings/User");
        private readonly string Unknown = ResourceService.GetLocalized("Settings/Unknown");
        private readonly string MicrosoftEntraId = ResourceService.GetLocalized("Settings/MicrosoftEntraId");
        private readonly string MicrosoftEntraIdForAzureBlobStorage = ResourceService.GetLocalized("Settings/MicrosoftEntraIdForAzureBlobStorage");
        private readonly string WinGetDataSourceRemoveFailed = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveFailed");
        private readonly string WinGetDataSourceRemoveGroupPolicyError = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveGroupPolicyError");
        private readonly string WinGetDataSourceRemoveCatalogError = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveCatalogError");
        private readonly string WinGetDataSourceRemoveInternalError = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveInternalError");
        private readonly string WinGetDataSourceRemoveInvalidOptions = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveInvalidOptions");
        private readonly string WinGetDataSourceRemoveAccessDenied = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveAccessDenied");
        private readonly string WinGetDataSourceRemoveSuccess = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveSuccess");
        private readonly string WinGetSourceCountInfo = ResourceService.GetLocalized("Settings/WinGetSourceCountInfo");
        private bool isInitialized;

        private bool _isLoadedCompleted;

        public bool IsLoadedCompleted
        {
            get { return _isLoadedCompleted; }

            private set
            {
                if (!Equals(_isLoadedCompleted, value))
                {
                    _isLoadedCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadedCompleted)));
                }
            }
        }

        private ObservableCollection<WinGetSourceModel> WinGetSourceInternalCollection { get; } = [];

        private ObservableCollection<WinGetSourceModel> WinGetSourceCustomCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsWinGetSourcePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                await InitializeWinGetSourceDataAsync();
                IsLoadedCompleted = true;
            }
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 搜索时是否使用本数据源
        /// </summary>
        private void OnCheckBoxClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is WinGetSourceModel winGetSourceItem)
            {
                foreach (WinGetSourceModel internalSourceItem in WinGetSourceInternalCollection)
                {
                    if (!internalSourceItem.Equals(winGetSourceItem))
                    {
                        internalSourceItem.IsSelected = false;
                    }
                }

                foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                {
                    if (!customSourceItem.Equals(winGetSourceItem))
                    {
                        customSourceItem.IsSelected = false;
                    }
                }

                Task.Run(() =>
                {
                    KeyValuePair<string, bool> winGetDataSourceName = KeyValuePair.Create(winGetSourceItem.Name, winGetSourceItem.IsInternal);

                    if (winGetSourceItem.IsSelected)
                    {
                        WinGetConfigService.SetWinGetDataSourceName(winGetDataSourceName);
                    }
                    else
                    {
                        WinGetConfigService.RemoveWinGetDataSourceName(winGetDataSourceName);
                    }
                });
            }
        }

        /// <summary>
        /// 编辑 WinGet 数据源
        /// </summary>
        private async void OnEditExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is WinGetSourceModel winGetSourceItem)
            {
                WinGetSourceEditDialog winGetSourceEditDialog = new(WinGetSourceEditKind.Edit, winGetSourceItem);
                await MainWindow.Current.ShowDialogAsync(winGetSourceEditDialog);

                if (winGetSourceEditDialog.AddPackageCatalogStatusResult.HasValue && winGetSourceEditDialog.AddPackageCatalogStatusResult is AddPackageCatalogStatus.Ok)
                {
                    IsLoadedCompleted = false;
                    await InitializeWinGetSourceDataAsync();
                    IsLoadedCompleted = true;
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NotElevated));
            }
        }

        /// <summary>
        /// 移除数据源
        /// </summary>
        private async void OnRemoveExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is WinGetSourceModel winGetSourceItem)
            {
                foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                {
                    if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                    {
                        customSourceItem.IsOperating = true;
                        break;
                    }
                }

                RemovePackageCatalogResult removePackageCatalogResult = await Task.Run(async () =>
                {
                    PackageManager packageManager = new();
                    RemovePackageCatalogOptions removePackageCatalogOptions = new()
                    {
                        Name = winGetSourceItem.Name,
                        PreserveData = false
                    };

                    return await packageManager.RemovePackageCatalogAsync(removePackageCatalogOptions);
                });

                switch (removePackageCatalogResult.Status)
                {
                    case RemovePackageCatalogStatus.Ok:
                        {
                            await Task.Run(() =>
                            {
                                WinGetConfigService.RemoveWinGetDataSourceName(KeyValuePair.Create(winGetSourceItem.Name, winGetSourceItem.IsInternal));
                            });

                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    WinGetSourceCustomCollection.Remove(customSourceItem);
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, true, WinGetDataSourceRemoveSuccess));
                            break;
                        }
                    case RemovePackageCatalogStatus.GroupPolicyError:
                        {
                            foreach (WinGetSourceModel item in WinGetSourceCustomCollection)
                            {
                                if (item.Name.Equals(winGetSourceItem.Name))
                                {
                                    item.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.CatalogError:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveCatalogError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InternalError:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveInternalError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InvalidOptions:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveInvalidOptions, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.AccessDenied:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveAccessDenied, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NotElevated));
            }
        }

        /// <summary>
        /// 移除数据源
        /// </summary>
        private async void OnRemovePreserveDataExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is WinGetSourceModel winGetSourceItem)
            {
                foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                {
                    if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                    {
                        customSourceItem.IsOperating = true;
                        break;
                    }
                }

                RemovePackageCatalogResult removePackageCatalogResult = await Task.Run(async () =>
                {
                    PackageManager packageManager = new();
                    RemovePackageCatalogOptions removePackageCatalogOptions = new()
                    {
                        Name = winGetSourceItem.Name,
                        PreserveData = true
                    };

                    return await packageManager.RemovePackageCatalogAsync(removePackageCatalogOptions);
                });

                switch (removePackageCatalogResult.Status)
                {
                    case RemovePackageCatalogStatus.Ok:
                        {
                            await Task.Run(() =>
                            {
                                WinGetConfigService.RemoveWinGetDataSourceName(KeyValuePair.Create(winGetSourceItem.Name, winGetSourceItem.IsInternal));
                            });

                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    WinGetSourceCustomCollection.Remove(customSourceItem);
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, true, WinGetDataSourceRemoveSuccess));
                            break;
                        }
                    case RemovePackageCatalogStatus.GroupPolicyError:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.CatalogError:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InternalError:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InvalidOptions:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.AccessDenied:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NotElevated));
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：设置页面——挂载的事件

        /// <summary>
        /// 添加数据源
        /// </summary>
        private async void OnAddNewSourceClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                WinGetSourceEditDialog winGetSourceEditDialog = new(WinGetSourceEditKind.Add, null);
                await MainWindow.Current.ShowDialogAsync(winGetSourceEditDialog);

                if (winGetSourceEditDialog.AddPackageCatalogStatusResult.HasValue && winGetSourceEditDialog.AddPackageCatalogStatusResult is AddPackageCatalogStatus.Ok)
                {
                    IsLoadedCompleted = false;
                    await InitializeWinGetSourceDataAsync();
                    IsLoadedCompleted = true;
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NotElevated));
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            IsLoadedCompleted = false;
            await InitializeWinGetSourceDataAsync();
            IsLoadedCompleted = true;
        }

        #endregion 第二部分：设置页面——挂载的事件

        /// <summary>
        /// 初始化 WinGet 数据源信息
        /// </summary>
        private async Task InitializeWinGetSourceDataAsync()
        {
            WinGetSourceInternalCollection.Clear();
            WinGetSourceCustomCollection.Clear();

            List<WinGetSourceModel> winGetSourceInternalList = await Task.Run(() =>
            {
                PackageManager packageManager = new();
                List<WinGetSourceModel> winGetSourceInternalList = [];
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                List<PackageCatalogReference> predefinedPackageCatalogReferenceList = [];
                foreach (PredefinedPackageCatalog predefinedPackageCatalog in Enum.GetValues<PredefinedPackageCatalog>())
                {
                    PackageCatalogReference packageCatalogReference = packageManager.GetPredefinedPackageCatalog(predefinedPackageCatalog);

                    PackageCatalogInformation packageCatalogInformation = new()
                    {
                        Name = packageCatalogReference.Info.Name,
                        Arguments = packageCatalogReference.Info.Argument,
                        Explicit = packageCatalogReference.Info.Explicit,
                        TrustLevel = packageCatalogReference.Info.TrustLevel,
                        Id = packageCatalogReference.Info.Id,
                        LastUpdateTime = packageCatalogReference.Info.LastUpdateTime,
                        Origin = packageCatalogReference.Info.Origin,
                        Type = packageCatalogReference.Info.Type,
                        AcceptSourceAgreements = packageCatalogReference.AcceptSourceAgreements,
                        AdditionalPackageCatalogArguments = packageCatalogReference.AdditionalPackageCatalogArguments,
                        AuthenticationType = packageCatalogReference.AuthenticationInfo.AuthenticationType,
                        AuthenticationAccount = packageCatalogReference.AuthenticationArguments is not null && !string.IsNullOrEmpty(packageCatalogReference.AuthenticationArguments.AuthenticationAccount) ? packageCatalogReference.AuthenticationArguments.AuthenticationAccount : string.Empty,
                        PackageCatalogBackgroundUpdateInterval = packageCatalogReference.PackageCatalogBackgroundUpdateInterval,
                    };

                    WinGetSourceModel winGetSourceItem = new()
                    {
                        IsOperating = false,
                        IsSelected = winGetDataSourceName.Equals(KeyValuePair.Create(packageCatalogInformation.Name, true)),
                        PackageCatalogInformation = packageCatalogInformation,
                        Name = packageCatalogInformation.Name,
                        Arguments = string.IsNullOrEmpty(packageCatalogInformation.Arguments) ? None : packageCatalogReference.Info.Argument,
                        Explicit = packageCatalogInformation.Explicit ? Yes : No,
                        TrustLevel = packageCatalogInformation.TrustLevel is PackageCatalogTrustLevel.Trusted ? Trusted : Distrusted,
                        Id = packageCatalogInformation.Id,
                        LastUpdateTime = packageCatalogInformation.LastUpdateTime.ToString("yyyy/MM/dd HH:mm"),
                        Origin = packageCatalogInformation.Origin is PackageCatalogOrigin.Predefined ? Predefined : User,
                        Type = packageCatalogInformation.Type,
                        AcceptSourceAgreements = packageCatalogInformation.AcceptSourceAgreements ? Yes : No,
                        AuthenticationType = packageCatalogInformation.AuthenticationType switch
                        {
                            AuthenticationType.None => None,
                            AuthenticationType.Unknown => Unknown,
                            AuthenticationType.MicrosoftEntraId => MicrosoftEntraId,
                            AuthenticationType.MicrosoftEntraIdForAzureBlobStorage => MicrosoftEntraIdForAzureBlobStorage,
                            _ => Unknown
                        },
                        AdditionalPackageCatalogArguments = string.IsNullOrEmpty(packageCatalogInformation.AdditionalPackageCatalogArguments) ? None : packageCatalogInformation.AdditionalPackageCatalogArguments,
                        AuthenticationAccount = string.IsNullOrEmpty(packageCatalogInformation.AuthenticationAccount) ? None : packageCatalogInformation.AuthenticationAccount,
                        PackageCatalogBackgroundUpdateInterval = packageCatalogInformation.PackageCatalogBackgroundUpdateInterval.ToString(),
                        IsInternal = true,
                        PredefinedPackageCatalog = predefinedPackageCatalog
                    };

                    winGetSourceInternalList.Add(winGetSourceItem);
                }

                return winGetSourceInternalList;
            });

            List<WinGetSourceModel> wingetSourceCustomList = await Task.Run(() =>
            {
                PackageManager packageManager = new();
                List<WinGetSourceModel> wingetSourceCustomList = [];
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                IReadOnlyList<PackageCatalogReference> packageCatalogReferenceList = packageManager.GetPackageCatalogs();

                for (int index = 0; index < packageCatalogReferenceList.Count; index++)
                {
                    PackageCatalogReference packageCatalogReference = packageCatalogReferenceList[index];

                    PackageCatalogInformation packageCatalogInformation = new()
                    {
                        Name = packageCatalogReference.Info.Name,
                        Arguments = packageCatalogReference.Info.Argument,
                        Explicit = packageCatalogReference.Info.Explicit,
                        TrustLevel = packageCatalogReference.Info.TrustLevel,
                        Id = packageCatalogReference.Info.Id,
                        LastUpdateTime = packageCatalogReference.Info.LastUpdateTime,
                        Origin = packageCatalogReference.Info.Origin,
                        Type = packageCatalogReference.Info.Type,
                        AcceptSourceAgreements = packageCatalogReference.AcceptSourceAgreements,
                        AdditionalPackageCatalogArguments = packageCatalogReference.AdditionalPackageCatalogArguments,
                        AuthenticationType = packageCatalogReference.AuthenticationInfo.AuthenticationType,
                        AuthenticationAccount = packageCatalogReference.AuthenticationArguments is not null && !string.IsNullOrEmpty(packageCatalogReference.AuthenticationArguments.AuthenticationAccount) ? packageCatalogReference.AuthenticationArguments.AuthenticationAccount : string.Empty,
                        PackageCatalogBackgroundUpdateInterval = packageCatalogReference.PackageCatalogBackgroundUpdateInterval,
                    };

                    WinGetSourceModel winGetSourceItem = new()
                    {
                        IsOperating = false,
                        IsSelected = winGetDataSourceName.Equals(KeyValuePair.Create(packageCatalogInformation.Name, false)),
                        PackageCatalogInformation = packageCatalogInformation,
                        Name = packageCatalogInformation.Name,
                        Arguments = string.IsNullOrEmpty(packageCatalogInformation.Arguments) ? None : packageCatalogReference.Info.Argument,
                        Explicit = packageCatalogInformation.Explicit ? Yes : No,
                        TrustLevel = packageCatalogInformation.TrustLevel is PackageCatalogTrustLevel.Trusted ? Trusted : Distrusted,
                        Id = packageCatalogInformation.Id,
                        LastUpdateTime = packageCatalogInformation.LastUpdateTime.ToString("yyyy/MM/dd HH:mm"),
                        Origin = packageCatalogInformation.Origin is PackageCatalogOrigin.Predefined ? Predefined : User,
                        Type = packageCatalogInformation.Type,
                        AcceptSourceAgreements = packageCatalogInformation.AcceptSourceAgreements ? Yes : No,
                        AuthenticationType = packageCatalogInformation.AuthenticationType switch
                        {
                            AuthenticationType.None => None,
                            AuthenticationType.Unknown => Unknown,
                            AuthenticationType.MicrosoftEntraId => MicrosoftEntraId,
                            AuthenticationType.MicrosoftEntraIdForAzureBlobStorage => MicrosoftEntraIdForAzureBlobStorage,
                            _ => Unknown
                        },
                        AdditionalPackageCatalogArguments = string.IsNullOrEmpty(packageCatalogInformation.AdditionalPackageCatalogArguments) ? None : packageCatalogInformation.AdditionalPackageCatalogArguments,
                        AuthenticationAccount = string.IsNullOrEmpty(packageCatalogInformation.AuthenticationAccount) ? None : packageCatalogInformation.AuthenticationAccount,
                        PackageCatalogBackgroundUpdateInterval = packageCatalogInformation.PackageCatalogBackgroundUpdateInterval.ToString(),
                        IsInternal = false
                    };

                    wingetSourceCustomList.Add(winGetSourceItem);
                }

                return wingetSourceCustomList;
            });

            foreach (WinGetSourceModel wingetSourceItem in winGetSourceInternalList)
            {
                WinGetSourceInternalCollection.Add(wingetSourceItem);
            }

            foreach (WinGetSourceModel wingetSourceItem in wingetSourceCustomList)
            {
                WinGetSourceCustomCollection.Add(wingetSourceItem);
            }
        }

        private Visibility GetCountVisibility(int winGetSourceInternalCollectionCount, int winGetSourceCustomCollectionCount, bool isReverse)
        {
            return isReverse ? (winGetSourceInternalCollectionCount + winGetSourceCustomCollectionCount) > 0 ? Visibility.Collapsed : Visibility.Visible : (winGetSourceInternalCollectionCount + winGetSourceCustomCollectionCount) > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private string GetLocalizedWinGetSourceCountInfo(int winGetSourceInternalCollectionCount, int winGetSourceCustomCollectionCount)
        {
            return string.Format(WinGetSourceCountInfo, winGetSourceInternalCollectionCount + winGetSourceCustomCollectionCount);
        }
    }
}
