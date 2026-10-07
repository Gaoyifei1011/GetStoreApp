using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.WinGet;
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
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 数据源页面
    /// </summary>
    internal sealed partial class SettingsWinGetSourcePage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string CustomDataSourceString = ResourceService.GetLocalized("SettingsWinGetSource/CustomDataSource");
        private readonly string DistrustedString = ResourceService.GetLocalized("SettingsWinGetSource/Distrusted");
        private readonly string InternalDataSourceString = ResourceService.GetLocalized("SettingsWinGetSource/InternalDataSource");
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

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private SettingsWinGetSourceResultKind _settingsWinGetSourceResultKind;

        private SettingsWinGetSourceResultKind SettingsWinGetSourceResultKind
        {
            get { return _settingsWinGetSourceResultKind; }

            set
            {
                if (!Equals(_settingsWinGetSourceResultKind, value))
                {
                    _settingsWinGetSourceResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(SettingsWinGetSourceResultKind)));
                }
            }
        }

        private WinGetSourceModel _selectedItem;

        public WinGetSourceModel SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedItem)));
                }
            }
        }

        private ObservableCollection<WinGetSourceGroupModel> WinGetSourceGroupCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal SettingsWinGetSourcePage()
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
                await InitializeWinGetSourceDataAsync();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

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
                    await InitializeWinGetSourceDataAsync();
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
                foreach (WinGetSourceGroupModel winGetSourceGroupItem in WinGetSourceGroupCollection)
                {
                    foreach (WinGetSourceModel winGetSourceItem in winGetSourceGroupItem.WinGetSourceCollection)
                    {
                        if (string.Equals(winGetSourceItem.Name, winGetSource.Name))
                        {
                            winGetSourceItem.IsOperating = true;
                            break;
                        }
                    }
                }

                RemovePackageCatalogResult removePackageCatalogResult = await RemovePackageCatalogAsync(winGetSource.Name, false);
                if (removePackageCatalogResult is not null && removePackageCatalogResult.Status is RemovePackageCatalogStatus.Ok)
                {
                    await RemoveWinGetDataSourceNameAsync(winGetSource.Name, winGetSource.IsInternal);

                    foreach (WinGetSourceGroupModel winGetSourceGroupItem in WinGetSourceGroupCollection)
                    {
                        foreach (WinGetSourceModel winGetSourceCustomItem in winGetSourceGroupItem.WinGetSourceCollection)
                        {
                            if (string.Equals(winGetSourceCustomItem.Name, winGetSource.Name) && Equals(winGetSourceCustomItem.IsInternal, winGetSource.IsInternal))
                            {
                                winGetSourceGroupItem.WinGetSourceCollection.Remove(winGetSourceCustomItem);
                                break;
                            }
                        }

                        // 组为空，删除空组
                        if (winGetSourceGroupItem.WinGetSourceCollection.Count is 0)
                        {
                            WinGetSourceGroupCollection.Remove(winGetSourceGroupItem);
                            break;
                        }
                    }
                }
                await ShowRemoveDataSourceResultNotificationAsync(removePackageCatalogResult);
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
                foreach (WinGetSourceGroupModel winGetSourceGroupItem in WinGetSourceGroupCollection)
                {
                    foreach (WinGetSourceModel winGetSourceItem in winGetSourceGroupItem.WinGetSourceCollection)
                    {
                        if (string.Equals(winGetSourceItem.Name, winGetSource.Name))
                        {
                            winGetSourceItem.IsOperating = true;
                            break;
                        }
                    }
                }

                RemovePackageCatalogResult removePackageCatalogResult = await RemovePackageCatalogAsync(winGetSource.Name, false);
                if (removePackageCatalogResult is not null && removePackageCatalogResult.Status is RemovePackageCatalogStatus.Ok)
                {
                    await RemoveWinGetDataSourceNameAsync(winGetSource.Name, winGetSource.IsInternal);

                    foreach (WinGetSourceGroupModel winGetSourceGroupItem in WinGetSourceGroupCollection)
                    {
                        foreach (WinGetSourceModel winGetSourceCustomItem in winGetSourceGroupItem.WinGetSourceCollection)
                        {
                            if (string.Equals(winGetSourceCustomItem.Name, winGetSource.Name) && Equals(winGetSourceCustomItem.IsInternal, winGetSource.IsInternal))
                            {
                                winGetSourceGroupItem.WinGetSourceCollection.Remove(winGetSourceCustomItem);
                                break;
                            }
                        }

                        // 组为空，删除空组
                        if (winGetSourceGroupItem.WinGetSourceCollection.Count is 0)
                        {
                            WinGetSourceGroupCollection.Remove(winGetSourceGroupItem);
                            break;
                        }
                    }
                }
                await ShowRemoveDataSourceResultNotificationAsync(removePackageCatalogResult);
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.NotElevated));
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

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
                    await InitializeWinGetSourceDataAsync();
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
            await InitializeWinGetSourceDataAsync();
        }

        [DynamicWindowsRuntimeCast(typeof(ListView))]
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ListView listView && !Equals(SelectedItem, listView.SelectedItem))
            {
                SelectedItem = listView.SelectedItem is WinGetSourceModel winGetSource ? winGetSource : null;

                if (SelectedItem is not null)
                {
                    UpdateWinGetSourceData(SelectedItem.Name, SelectedItem.IsInternal, true);
                }
                else
                {
                    if (args.RemovedItems.Count > 0 && args.RemovedItems[0] is WinGetSourceModel lastSelectedItem)
                    {
                        UpdateWinGetSourceData(lastSelectedItem.Name, lastSelectedItem.IsInternal, true);
                    }
                }
            }
        }

        /// <summary>
        /// 设置说明
        /// </summary>
        private async void OnSettingsInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                await settingsPage.ShowSettingsInstructionAsync(true);
            }
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化 WinGet 数据源信息
        /// </summary>
        private async Task InitializeWinGetSourceDataAsync()
        {
            SettingsWinGetSourceResultKind = SettingsWinGetSourceResultKind.Loading;
            WinGetSourceGroupCollection.Clear();

            if (await GetWinGetSourceInternalCollectionAsync() is ReadOnlyCollection<WinGetSourceModel> winGetSourceInternalCollection && winGetSourceInternalCollection.Count > 0)
            {
                WinGetSourceGroupModel winGetSourceGroup = new()
                {
                    GroupName = InternalDataSourceString,
                    WinGetSourceCollection = []
                };

                foreach (WinGetSourceModel winGetSourceItem in winGetSourceInternalCollection)
                {
                    winGetSourceGroup.WinGetSourceCollection.Add(winGetSourceItem);
                }
                WinGetSourceGroupCollection.Add(winGetSourceGroup);
            }

            if (await GetWinGetSourceCustomCollectionAsync() is ReadOnlyCollection<WinGetSourceModel> winGetSourceCustomCollection && winGetSourceCustomCollection.Count > 0)
            {
                WinGetSourceGroupModel winGetSourceGroup = new()
                {
                    GroupName = CustomDataSourceString,
                    WinGetSourceCollection = []
                };

                foreach (WinGetSourceModel winGetSourceItem in winGetSourceCustomCollection)
                {
                    winGetSourceGroup.WinGetSourceCollection.Add(winGetSourceItem);
                }
                WinGetSourceGroupCollection.Add(winGetSourceGroup);
            }

            KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
            foreach (WinGetSourceGroupModel winGetSourceGroupItem in WinGetSourceGroupCollection)
            {
                foreach (WinGetSourceModel winGetSourceItem in winGetSourceGroupItem.WinGetSourceCollection)
                {
                    if (Equals(winGetDataSourceName, KeyValuePair.Create(winGetSourceItem.Name, winGetSourceItem.IsInternal)))
                    {
                        SelectedItem = winGetSourceItem;
                        break;
                    }
                }
            }
            SettingsWinGetSourceResultKind = WinGetSourceGroupCollection.Count is 0 ? SettingsWinGetSourceResultKind.Empty : SettingsWinGetSourceResultKind.Successfully;
        }

        /// <summary>
        /// 获取 WinGet 内部数据源
        /// </summary>
        private async Task<ReadOnlyCollection<WinGetSourceModel>> GetWinGetSourceInternalCollectionAsync()
        {
            return await Task.Run(() =>
            {
                PackageManager packageManager = WinGetFactoryHelper.CreatePackageManager();
                List<WinGetSourceModel> winGetSourceInternalList = [];

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

                return winGetSourceInternalList.AsReadOnly();
            });
        }

        /// <summary>
        /// 获取 WinGet 自定义数据源
        /// </summary>
        private async Task<ReadOnlyCollection<WinGetSourceModel>> GetWinGetSourceCustomCollectionAsync()
        {
            return await Task.Run(() =>
            {
                PackageManager packageManager = WinGetFactoryHelper.CreatePackageManager();
                List<WinGetSourceModel> winGetSourceCustomList = [];

                if (packageManager.GetPackageCatalogs() is IReadOnlyList<PackageCatalogReference> packageCatalogReferenceList && packageCatalogReferenceList.Count > 0)
                {
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

                        winGetSourceCustomList.Add(winGetSource);
                    }
                }

                return winGetSourceCustomList.AsReadOnly();
            });
        }

        /// <summary>
        /// 更新 WinGet 数据源数据
        /// </summary>
        private void UpdateWinGetSourceData(string name, bool isInternal, bool isSelected)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = KeyValuePair.Create(name, isInternal);

                if (isSelected)
                {
                    WinGetConfigService.SetWinGetDataSourceName(winGetDataSourceName);
                }
                else
                {
                    WinGetConfigService.RemoveWinGetDataSourceName(winGetDataSourceName);
                }
            });
        }

        /// <summary>
        /// 移除 WinGet 数据源
        /// </summary>
        private async Task<RemovePackageCatalogResult> RemovePackageCatalogAsync(string name, bool preserveData)
        {
            if (string.IsNullOrEmpty(name))
            {
                return default;
            }

            return await Task.Run(async () =>
            {
                PackageManager packageManager = WinGetFactoryHelper.CreatePackageManager();
                RemovePackageCatalogOptions removePackageCatalogOptions = WinGetFactoryHelper.CreateRemovePackageCatalogOptions();
                removePackageCatalogOptions.Name = name;
                removePackageCatalogOptions.PreserveData = preserveData;
                return await packageManager.RemovePackageCatalogAsync(removePackageCatalogOptions);
            });
        }

        /// <summary>
        /// 移除 WinGet 数据源
        /// </summary>
        private async Task RemoveWinGetDataSourceNameAsync(string name, bool isInternal)
        {
            await Task.Run(() =>
            {
                WinGetConfigService.RemoveWinGetDataSourceName(KeyValuePair.Create(name, isInternal));
            });
        }

        /// <summary>
        /// 显示移除 WinGet 数据源结果通知
        /// </summary>
        private async Task ShowRemoveDataSourceResultNotificationAsync(RemovePackageCatalogResult removePackageCatalogResult)
        {
            if (removePackageCatalogResult is null)
            {
                return;
            }

            switch (removePackageCatalogResult.Status)
            {
                case RemovePackageCatalogStatus.Ok:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, true, WinGetDataSourceRemoveSuccessString));
                        break;
                    }
                case RemovePackageCatalogStatus.GroupPolicyError:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveGroupPolicyErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", removePackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                        break;
                    }
                case RemovePackageCatalogStatus.CatalogError:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveCatalogErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", removePackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                        break;
                    }
                case RemovePackageCatalogStatus.InternalError:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveInternalErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", removePackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                        break;
                    }
                case RemovePackageCatalogStatus.InvalidOptions:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveInvalidOptionsString, removePackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", removePackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                        break;
                    }
                case RemovePackageCatalogStatus.AccessDenied:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailedString, WinGetDataSourceRemoveAccessDeniedString, removePackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", removePackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                        break;
                    }
            }
        }

        private Visibility GetCountVisibility(ObservableCollection<WinGetSourceGroupModel> winGetSourceGroupCollection, bool isReverse)
        {
            int count = GetWinGetSourceListCount(winGetSourceGroupCollection);
            return isReverse ? count > 0 ? Visibility.Collapsed : Visibility.Visible : count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private string GetLocalizedWinGetDataSourceCountInfo(ObservableCollection<WinGetSourceGroupModel> winGetSourceGroupCollection)
        {
            int count = GetWinGetSourceListCount(winGetSourceGroupCollection);
            return string.Format(WinGetDataSourceCountInfoString, count);
        }

        private int GetWinGetSourceListCount(ObservableCollection<WinGetSourceGroupModel> winGetSourceGroupCollection)
        {
            int count = 0;
            foreach (WinGetSourceGroupModel winGetSourceGroupItem in winGetSourceGroupCollection)
            {
                count += winGetSourceGroupItem.WinGetSourceCollection.Count;
            }
            return count;
        }

        /// <summary>
        /// 获取加载 WinGet 数据源配置是否成功
        /// </summary>
        private Visibility GetSettingsWinGetSourceSuccessfullyVisibility(SettingsWinGetSourceResultKind settingsWinGetSourceResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? settingsWinGetSourceResultKind is SettingsWinGetSourceResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : settingsWinGetSourceResultKind is SettingsWinGetSourceResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查加载 WinGet 数据源配置是否成功
        /// </summary>
        private Visibility CheckSettingsWinGetSourceResultKindVisibility(SettingsWinGetSourceResultKind settingsWinGetSourceResultKind, SettingsWinGetSourceResultKind comparedSettingsWinGetSourceResultKind)
        {
            return Equals(settingsWinGetSourceResultKind, comparedSettingsWinGetSourceResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>
        private bool GetIsLoading(SettingsWinGetSourceResultKind settingsWinGetSourceResultKind)
        {
            return settingsWinGetSourceResultKind is not SettingsWinGetSourceResultKind.Loading;
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}
