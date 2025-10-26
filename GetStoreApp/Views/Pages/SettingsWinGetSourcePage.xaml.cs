using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.NotificationTips;
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
        private readonly string DistrustedString = ResourceService.GetLocalized("SettingsWinGetSource/Distrusted");
        private readonly string MicrosoftEntraIdString = ResourceService.GetLocalized("SettingsWinGetSource/MicrosoftEntraId");
        private readonly string MicrosoftEntraIdForAzureBlobStorageString = ResourceService.GetLocalized("SettingsWinGetSource/MicrosoftEntraIdForAzureBlobStorage");
        private readonly string NoString = ResourceService.GetLocalized("SettingsWinGetSource/No");
        private readonly string NoneString = ResourceService.GetLocalized("SettingsWinGetSource/None");
        private readonly string NotAvailableString = ResourceService.GetLocalized("SettingsWinGetSource/NotAvailable");
        private readonly string PredefinedString = ResourceService.GetLocalized("SettingsWinGetSource/Predefined");
        private readonly string TrustedString = ResourceService.GetLocalized("SettingsWinGetSource/Trusted");
        private readonly string UserString = ResourceService.GetLocalized("SettingsWinGetSource/User");
        private readonly string WinGetDataSourceCountInfoString = ResourceService.GetLocalized("SettingsWinGetSource/WinGetDataSourceCountInfo");
        private readonly string WinGetDataSourceRemoveAccessDeniedString = ResourceService.GetLocalized("SettingsWinGetSource/WinGetDataSourceRemoveAccessDenied");
        private readonly string WinGetDataSourceRemoveCatalogErrorString = ResourceService.GetLocalized("SettingsWinGetSource/WinGetDataSourceRemoveCatalogError");
        private readonly string WinGetDataSourceRemoveFailedString = ResourceService.GetLocalized("SettingsWinGetSource/WinGetDataSourceRemoveFailed");
        private readonly string WinGetDataSourceRemoveGroupPolicyErrorString = ResourceService.GetLocalized("SettingsWinGetSource/WinGetDataSourceRemoveGroupPolicyError");
        private readonly string WinGetDataSourceRemoveInternalErrorString = ResourceService.GetLocalized("SettingsWinGetSource/WinGetDataSourceRemoveInternalError");
        private readonly string WinGetDataSourceRemoveInvalidOptionsString = ResourceService.GetLocalized("SettingsWinGetSource/WinGetDataSourceRemoveInvalidOptions");
        private readonly string WinGetDataSourceRemoveSuccessString = ResourceService.GetLocalized("SettingsWinGetSource/WinGetDataSourceRemoveSuccess");
        private readonly string YesString = ResourceService.GetLocalized("SettingsWinGetSource/Yes");
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
        private void OnRadioButtonClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is WinGetSourceModel winGetSource)
            {
                foreach (WinGetSourceModel wingetSourceInternalItem in WinGetSourceInternalCollection)
                {
                    if (!Equals(wingetSourceInternalItem, winGetSource))
                    {
                        wingetSourceInternalItem.IsSelected = false;
                    }
                }

                foreach (WinGetSourceModel wingetSourcecCustomItem in WinGetSourceCustomCollection)
                {
                    if (!Equals(wingetSourcecCustomItem, winGetSource))
                    {
                        wingetSourcecCustomItem.IsSelected = false;
                    }
                }

                Task.Run(() =>
                {
                    KeyValuePair<string, bool> winGetDataSourceName = KeyValuePair.Create(winGetSource.Name, winGetSource.IsInternal);

                    if (winGetSource.IsSelected)
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
            if (RuntimeHelper.IsElevated && args.Parameter is WinGetSourceModel winGetSource)
            {
                WinGetSourceEditDialog winGetSourceEditDialog = new(WinGetSourceEditKind.Edit, winGetSource);
                ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(winGetSourceEditDialog);

                if (contentDialogResult is ContentDialogResult.Primary && winGetSourceEditDialog.AddPackageCatalogStatusResult.HasValue && winGetSourceEditDialog.AddPackageCatalogStatusResult is AddPackageCatalogStatus.Ok)
                {
                    IsLoadedCompleted = false;
                    await InitializeWinGetSourceDataAsync();
                    IsLoadedCompleted = true;
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NotElevated));
            }
        }

        /// <summary>
        /// 移除数据源
        /// </summary>
        private async void OnRemoveDataSourceExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is WinGetSourceModel winGetSource)
            {
                foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                {
                    if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                    {
                        wingetSourceCustomItem.IsOperating = true;
                        break;
                    }
                }

                RemovePackageCatalogResult removePackageCatalogResult = await Task.Run(async () =>
                {
                    PackageManager packageManager = new();
                    RemovePackageCatalogOptions removePackageCatalogOptions = new()
                    {
                        Name = winGetSource.Name,
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
                                WinGetConfigService.RemoveWinGetDataSourceName(KeyValuePair.Create(winGetSource.Name, winGetSource.IsInternal));
                            });

                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    WinGetSourceCustomCollection.Remove(wingetSourceCustomItem);
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, true, WinGetDataSourceRemoveSuccessString));
                            break;
                        }
                    case RemovePackageCatalogStatus.GroupPolicyError:
                        {
                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    wingetSourceCustomItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveGroupPolicyErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                            break;
                        }
                    case RemovePackageCatalogStatus.CatalogError:
                        {
                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    wingetSourceCustomItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveCatalogErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InternalError:
                        {
                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    wingetSourceCustomItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveInternalErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InvalidOptions:
                        {
                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    wingetSourceCustomItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveInvalidOptionsString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                            break;
                        }
                    case RemovePackageCatalogStatus.AccessDenied:
                        {
                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    wingetSourceCustomItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveAccessDeniedString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                            break;
                        }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NotElevated));
            }
        }

        /// <summary>
        /// 移除数据源
        /// </summary>
        private async void OnRemoveDataSourcePreserveDataExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is WinGetSourceModel winGetSource)
            {
                foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                {
                    if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                    {
                        wingetSourceCustomItem.IsOperating = true;
                        break;
                    }
                }

                RemovePackageCatalogResult removePackageCatalogResult = await Task.Run(async () =>
                {
                    PackageManager packageManager = new();
                    RemovePackageCatalogOptions removePackageCatalogOptions = new()
                    {
                        Name = winGetSource.Name,
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
                                WinGetConfigService.RemoveWinGetDataSourceName(KeyValuePair.Create(winGetSource.Name, winGetSource.IsInternal));
                            });

                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    WinGetSourceCustomCollection.Remove(wingetSourceCustomItem);
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, true, WinGetDataSourceRemoveSuccessString));
                            break;
                        }
                    case RemovePackageCatalogStatus.GroupPolicyError:
                        {
                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    wingetSourceCustomItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveGroupPolicyErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                            break;
                        }
                    case RemovePackageCatalogStatus.CatalogError:
                        {
                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    wingetSourceCustomItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveGroupPolicyErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InternalError:
                        {
                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    wingetSourceCustomItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveGroupPolicyErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InvalidOptions:
                        {
                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    wingetSourceCustomItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveGroupPolicyErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                            break;
                        }
                    case RemovePackageCatalogStatus.AccessDenied:
                        {
                            foreach (WinGetSourceModel wingetSourceCustomItem in WinGetSourceCustomCollection)
                            {
                                if (string.Equals(wingetSourceCustomItem.Name, winGetSource.Name))
                                {
                                    wingetSourceCustomItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveGroupPolicyErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                            break;
                        }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NotElevated));
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
                ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(winGetSourceEditDialog);

                if (contentDialogResult is ContentDialogResult.Primary && winGetSourceEditDialog.AddPackageCatalogStatusResult.HasValue && winGetSourceEditDialog.AddPackageCatalogStatusResult is AddPackageCatalogStatus.Ok)
                {
                    IsLoadedCompleted = false;
                    await InitializeWinGetSourceDataAsync();
                    IsLoadedCompleted = true;
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NotElevated));
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

        /// <summary>
        /// 设置说明
        /// </summary>
        private void OnSettingsInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                settingsPage.ShowSettingsInstruction();
            }
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

                    WinGetSourceModel winGetSource = new()
                    {
                        IsOperating = false,
                        IsSelected = Equals(winGetDataSourceName, KeyValuePair.Create(packageCatalogInformation.Name, true)),
                        PackageCatalogInformation = packageCatalogInformation,
                        Name = packageCatalogInformation.Name,
                        Arguments = string.IsNullOrEmpty(packageCatalogInformation.Arguments) ? NoneString : packageCatalogReference.Info.Argument,
                        Explicit = packageCatalogInformation.Explicit ? YesString : NoString,
                        TrustLevel = packageCatalogInformation.TrustLevel is PackageCatalogTrustLevel.Trusted ? TrustedString : DistrustedString,
                        SourceId = packageCatalogInformation.Id,
                        LastUpdateTime = packageCatalogInformation.LastUpdateTime.ToString("yyyy/MM/dd HH:mm"),
                        Origin = packageCatalogInformation.Origin is PackageCatalogOrigin.Predefined ? PredefinedString : UserString,
                        Type = packageCatalogInformation.Type,
                        AcceptSourceAgreements = packageCatalogInformation.AcceptSourceAgreements ? YesString : NoString,
                        AuthenticationType = packageCatalogInformation.AuthenticationType switch
                        {
                            AuthenticationType.None => NoneString,
                            AuthenticationType.Unknown => NotAvailableString,
                            AuthenticationType.MicrosoftEntraId => MicrosoftEntraIdString,
                            AuthenticationType.MicrosoftEntraIdForAzureBlobStorage => MicrosoftEntraIdForAzureBlobStorageString,
                            _ => NotAvailableString
                        },
                        AdditionalPackageCatalogArguments = string.IsNullOrEmpty(packageCatalogInformation.AdditionalPackageCatalogArguments) ? NoneString : packageCatalogInformation.AdditionalPackageCatalogArguments,
                        AuthenticationAccount = string.IsNullOrEmpty(packageCatalogInformation.AuthenticationAccount) ? NoneString : packageCatalogInformation.AuthenticationAccount,
                        PackageCatalogBackgroundUpdateInterval = Convert.ToString(packageCatalogInformation.PackageCatalogBackgroundUpdateInterval),
                        IsInternal = true,
                        PredefinedPackageCatalog = predefinedPackageCatalog
                    };

                    winGetSourceInternalList.Add(winGetSource);
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

                    WinGetSourceModel winGetSource = new()
                    {
                        IsOperating = false,
                        IsSelected = Equals(winGetDataSourceName, KeyValuePair.Create(packageCatalogInformation.Name, false)),
                        PackageCatalogInformation = packageCatalogInformation,
                        Name = packageCatalogInformation.Name,
                        Arguments = string.IsNullOrEmpty(packageCatalogInformation.Arguments) ? NoneString : packageCatalogReference.Info.Argument,
                        Explicit = packageCatalogInformation.Explicit ? YesString : NoString,
                        TrustLevel = packageCatalogInformation.TrustLevel is PackageCatalogTrustLevel.Trusted ? TrustedString : DistrustedString,
                        SourceId = packageCatalogInformation.Id,
                        LastUpdateTime = packageCatalogInformation.LastUpdateTime.ToString("yyyy/MM/dd HH:mm"),
                        Origin = packageCatalogInformation.Origin is PackageCatalogOrigin.Predefined ? PredefinedString : UserString,
                        Type = packageCatalogInformation.Type,
                        AcceptSourceAgreements = packageCatalogInformation.AcceptSourceAgreements ? YesString : NoString,
                        AuthenticationType = packageCatalogInformation.AuthenticationType switch
                        {
                            AuthenticationType.None => NoneString,
                            AuthenticationType.Unknown => NotAvailableString,
                            AuthenticationType.MicrosoftEntraId => MicrosoftEntraIdString,
                            AuthenticationType.MicrosoftEntraIdForAzureBlobStorage => MicrosoftEntraIdForAzureBlobStorageString,
                            _ => NotAvailableString
                        },
                        AdditionalPackageCatalogArguments = string.IsNullOrEmpty(packageCatalogInformation.AdditionalPackageCatalogArguments) ? NoneString : packageCatalogInformation.AdditionalPackageCatalogArguments,
                        AuthenticationAccount = string.IsNullOrEmpty(packageCatalogInformation.AuthenticationAccount) ? NoneString : packageCatalogInformation.AuthenticationAccount,
                        PackageCatalogBackgroundUpdateInterval = Convert.ToString(packageCatalogInformation.PackageCatalogBackgroundUpdateInterval),
                        IsInternal = false
                    };

                    wingetSourceCustomList.Add(winGetSource);
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

        private string GetLocalizedWinGetDataSourceCountInfo(int winGetSourceInternalCollectionCount, int winGetSourceCustomCollectionCount)
        {
            return string.Format(WinGetDataSourceCountInfoString, winGetSourceInternalCollectionCount + winGetSourceCustomCollectionCount);
        }
    }
}
