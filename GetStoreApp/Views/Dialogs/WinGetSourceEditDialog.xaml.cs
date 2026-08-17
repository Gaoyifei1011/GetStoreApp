using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.WinGet;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using WinRT;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// WinGet 数据源编辑对话框
    /// </summary>
    internal sealed partial class WinGetSourceEditDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string CatalogTrustLevelNoneString = ResourceService.GetLocalized("Dialog/CatalogTrustLevelNone");
        private readonly string CatalogTrustLevelTrustedString = ResourceService.GetLocalized("Dialog/CatalogTrustLevelTrusted");
        private readonly string NotAvailableString = ResourceService.GetLocalized("Dialog/NotAvailable");
        private readonly string WinGetDataSourceAddAccessDeniedString = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddAccessDenied");
        private readonly string WinGetDataSourceAddAuthenticationErrorString = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddAuthenticationError");
        private readonly string WinGetDataSourceAddCatalogErrorString = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddCatalogError");
        private readonly string WinGetDataSourceAddFailedString = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddFailed");
        private readonly string WinGetDataSourceAddGroupPolicyErrorString = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddGroupPolicyError");
        private readonly string WinGetDataSourceAddInternalErrorString = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddInternalError");
        private readonly string WinGetDataSourceAddInvalidOptionsString = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddInvalidOptions");
        private readonly string WinGetDataSourceAddString = ResourceService.GetLocalized("Dialog/WinGetDataSourceAdd");
        private readonly string WinGetDataSourceAddSuccessString = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddSuccess");
        private readonly string WinGetDataSourceEditAccessDeniedString = ResourceService.GetLocalized("Dialog/WinGetDataSourceEditAccessDenied");
        private readonly string WinGetDataSourceEditAuthenticationErrorString = ResourceService.GetLocalized("Dialog/WinGetDataSourceEditAuthenticationError");
        private readonly string WinGetDataSourceEditCatalogErrorString = ResourceService.GetLocalized("Dialog/WinGetDataSourceEditCatalogError");
        private readonly string WinGetDataSourceEditFailedString = ResourceService.GetLocalized("Dialog/WinGetDataSourceEditFailed");
        private readonly string WinGetDataSourceEditGroupPolicyErrorString = ResourceService.GetLocalized("Dialog/WinGetDataSourceEditGroupPolicyError");
        private readonly string WinGetDataSourceEditInternalErrorString = ResourceService.GetLocalized("Dialog/WinGetDataSourceEditInternalError");
        private readonly string WinGetDataSourceEditInvalidOptionsString = ResourceService.GetLocalized("Dialog/WinGetDataSourceEditInvalidOptions");
        private readonly string WinGetDataSourceEditString = ResourceService.GetLocalized("Dialog/WinGetDataSourceEdit");
        private readonly string WinGetDataSourceEditSuccessString = ResourceService.GetLocalized("Dialog/WinGetDataSourceEditSuccess");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private WinGetSourceEditKind WinGetSourceEditKind { get; }

        internal AddPackageCatalogStatus? AddPackageCatalogStatusResult { get; private set; } = null;

        private bool _isSaving;

        private bool IsSaving
        {
            get { return _isSaving; }

            set
            {
                if (!Equals(_isSaving, value))
                {
                    _isSaving = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSaving)));
                }
            }
        }

        private string _editTitle;

        private string EditTitle
        {
            get { return _editTitle; }

            set
            {
                if (!string.Equals(_editTitle, value))
                {
                    _editTitle = value;
                    PropertyChanged?.Invoke(this, new(nameof(EditTitle)));
                }
            }
        }

        private string _sourceName;

        private string SourceName
        {
            get { return _sourceName; }

            set
            {
                if (!string.Equals(_sourceName, value))
                {
                    _sourceName = value;
                    PropertyChanged?.Invoke(this, new(nameof(SourceName)));
                }
            }
        }

        private string _sourceUri;

        private string SourceUri
        {
            get { return _sourceUri; }

            set
            {
                if (!string.Equals(_sourceUri, value))
                {
                    _sourceUri = value;
                    PropertyChanged?.Invoke(this, new(nameof(SourceUri)));
                }
            }
        }

        private string _customHeader;

        private string CustomHeader
        {
            get { return _customHeader; }

            set
            {
                if (!string.Equals(_customHeader, value))
                {
                    _customHeader = value;
                    PropertyChanged?.Invoke(this, new(nameof(CustomHeader)));
                }
            }
        }

        private string _sourceType;

        private string SourceType
        {
            get { return _sourceType; }

            set
            {
                if (!string.Equals(_sourceType, value))
                {
                    _sourceType = value;
                    PropertyChanged?.Invoke(this, new(nameof(SourceType)));
                }
            }
        }

        private bool _explicit;

        private bool Explicit
        {
            get { return _explicit; }

            set
            {
                if (!Equals(_explicit, value))
                {
                    _explicit = value;
                    PropertyChanged?.Invoke(this, new(nameof(Explicit)));
                }
            }
        }

        private ComboBoxItemModel _selectedCatalogTrustLevel;

        private ComboBoxItemModel SelectedCatalogTrustLevel
        {
            get { return _selectedCatalogTrustLevel; }

            set
            {
                if (!Equals(_selectedCatalogTrustLevel, value))
                {
                    _selectedCatalogTrustLevel = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedCatalogTrustLevel)));
                }
            }
        }

        private List<ComboBoxItemModel> CatalogTrustLevelList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal WinGetSourceEditDialog(WinGetSourceEditKind winGetSourceEditKind, WinGetSourceModel winGetSource)
        {
            InitializeComponent();
            WinGetSourceEditKind = winGetSourceEditKind;
            InitializeData();
            SelectedCatalogTrustLevel = CatalogTrustLevelList[0];
            EditTitle = winGetSourceEditKind is WinGetSourceEditKind.Add ? WinGetDataSourceAddString : WinGetDataSourceEditString;

            if (winGetSourceEditKind is WinGetSourceEditKind.Edit && winGetSource is not null)
            {
                SourceName = winGetSource.Name;
                SourceUri = winGetSource.Arguments;
                CustomHeader = string.Empty;
                SourceType = winGetSource.Type;
                Explicit = winGetSource.PackageCatalogInformation.Explicit;
                SelectedCatalogTrustLevel = CatalogTrustLevelList.Find(item => Equals(item.SelectedValue, winGetSource.PackageCatalogInformation.TrustLevel));
            }
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 数据源名称文本输入框内容发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(TextBox))]
        private void OnSourceNameTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                SourceName = textBox.Text;
            }
        }

        /// <summary>
        /// 数据源链接文本输入框内容发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(TextBox))]
        private void OnSourceUriTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                SourceUri = textBox.Text;
            }
        }

        /// <summary>
        /// 自定义标头文本输入框内容发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(TextBox))]
        private void OnCustomHeaderTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                CustomHeader = textBox.Text;
            }
        }

        /// <summary>
        /// 数据源类型文本输入框内容发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(TextBox))]
        private void OnSourceTypeTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                SourceType = textBox.Text;
            }
        }

        /// <summary>
        /// 是否为显性开关发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnExplicitToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(Explicit, toggleSwitch.IsOn))
            {
                Explicit = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 数据源信任等级发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnCatalogTrustLevelSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(SelectedCatalogTrustLevel, comboBox.SelectedItem))
            {
                SelectedCatalogTrustLevel = comboBox.SelectedItem is ComboBoxItemModel catalogTrustLevel ? catalogTrustLevel : null;
            }
        }

        /// <summary>
        /// 保存添加或修改的 WinGet 数据源
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageCatalogTrustLevel))]
        private async void OnSaveClicked(object sender, ContentDialogButtonClickEventArgs args)
        {
            ContentDialogButtonClickDeferral contentDialogButtonClickDeferral = args.GetDeferral();

            try
            {
                if (string.IsNullOrEmpty(SourceName))
                {
                    args.Cancel = true;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SourceNameEmpty));
                    return;
                }

                if (string.IsNullOrEmpty(SourceUri))
                {
                    args.Cancel = true;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SourceUriEmpty));
                    return;
                }

                IsSaving = true;

                if (WinGetSourceEditKind is WinGetSourceEditKind.Add)
                {
                    PackageManager packageManager = WinGetFactoryHelper.CreatePackageManager();
                    AddPackageCatalogResult addPackageCatalogResult = await AddPackageCatalogAsync(SourceName, SourceUri, Explicit, (PackageCatalogTrustLevel)SelectedCatalogTrustLevel.SelectedValue, CustomHeader, SourceType);
                    IsSaving = false;

                    if (addPackageCatalogResult is not null)
                    {
                        AddPackageCatalogStatusResult = addPackageCatalogResult.Status;
                        args.Cancel = addPackageCatalogResult.Status is not AddPackageCatalogStatus.Ok;
                        await ShowAddPackageCatalogResultNotificationAsync(addPackageCatalogResult);
                    }
                }
                else if (WinGetSourceEditKind is WinGetSourceEditKind.Edit)
                {
                    RemovePackageCatalogResult removePackageCatalogResult = await RemovePackageCatalogAsync(SourceName, true);

                    if (removePackageCatalogResult.Status is RemovePackageCatalogStatus.Ok)
                    {
                        AddPackageCatalogResult addPackageCatalogResult = await AddPackageCatalogAsync(SourceName, SourceUri, Explicit, (PackageCatalogTrustLevel)SelectedCatalogTrustLevel.SelectedValue, CustomHeader, SourceType);
                        IsSaving = false;

                        if (addPackageCatalogResult is not null)
                        {
                            AddPackageCatalogStatusResult = addPackageCatalogResult.Status;
                            args.Cancel = addPackageCatalogResult.Status is not AddPackageCatalogStatus.Ok;
                            await ShowAddPackageCatalogResultNotificationAsync(addPackageCatalogResult);
                        }
                    }
                    else
                    {
                        IsSaving = false;
                        args.Cancel = removePackageCatalogResult.Status is not RemovePackageCatalogStatus.Ok;
                        await ShowRemovePackageCatalogNotificationAsync(removePackageCatalogResult);
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                contentDialogButtonClickDeferral.Complete();
            }
        }

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            CatalogTrustLevelList.Add(new() { SelectedValue = PackageCatalogTrustLevel.None, DisplayMember = CatalogTrustLevelNoneString });
            CatalogTrustLevelList.Add(new() { SelectedValue = PackageCatalogTrustLevel.Trusted, DisplayMember = CatalogTrustLevelTrustedString });
        }

        /// <summary>
        /// 添加 WinGet 数据源
        /// </summary>
        private async Task<AddPackageCatalogResult> AddPackageCatalogAsync(string sourceName, string sourceUri, bool explict, PackageCatalogTrustLevel packageCatalogTrustLevel, string customHeader, string sourceType)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    PackageManager packageManager = WinGetFactoryHelper.CreatePackageManager();
                    AddPackageCatalogOptions addPackageCatalogOptions = WinGetFactoryHelper.CreateAddPackageCatalogOptions();
                    addPackageCatalogOptions.Name = string.IsNullOrEmpty(sourceName) ? string.Empty : sourceName;
                    addPackageCatalogOptions.SourceUri = string.IsNullOrEmpty(sourceUri) ? string.Empty : sourceUri;
                    addPackageCatalogOptions.Explicit = explict;
                    addPackageCatalogOptions.TrustLevel = packageCatalogTrustLevel;
                    addPackageCatalogOptions.CustomHeader = string.IsNullOrEmpty(customHeader) ? string.Empty : customHeader;
                    addPackageCatalogOptions.Type = string.IsNullOrEmpty(sourceType) ? string.Empty : sourceType;
                    return await packageManager.AddPackageCatalogAsync(addPackageCatalogOptions);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetSourceEditDialog), nameof(AddPackageCatalogAsync), 1, e);
                    return default;
                }
            });
        }

        /// <summary>
        /// 移除 WinGet 数据源
        /// </summary>
        private async Task<RemovePackageCatalogResult> RemovePackageCatalogAsync(string sourceName, bool preserveData)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    PackageManager packageManager = WinGetFactoryHelper.CreatePackageManager();
                    RemovePackageCatalogOptions removePackageCatalogOptions = WinGetFactoryHelper.CreateRemovePackageCatalogOptions();
                    removePackageCatalogOptions.Name = string.IsNullOrEmpty(sourceName) ? string.Empty : sourceName;
                    removePackageCatalogOptions.PreserveData = preserveData;
                    return await packageManager.RemovePackageCatalogAsync(removePackageCatalogOptions);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetSourceEditDialog), nameof(RemovePackageCatalogAsync), 1, e);
                    return default;
                }
            });
        }

        /// <summary>
        /// 显示添加 WinGet 数据源通知
        /// </summary>
        private async Task ShowAddPackageCatalogResultNotificationAsync(AddPackageCatalogResult addPackageCatalogResult)
        {
            if (addPackageCatalogResult is not null)
            {
                switch (addPackageCatalogResult.Status)
                {
                    case AddPackageCatalogStatus.Ok:
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, true, WinGetDataSourceAddSuccessString));
                            break;
                        }
                    case AddPackageCatalogStatus.GroupPolicyError:
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddGroupPolicyErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", addPackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                            break;
                        }
                    case AddPackageCatalogStatus.CatalogError:
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddCatalogErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", addPackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                            break;
                        }
                    case AddPackageCatalogStatus.InternalError:
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddInternalErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", addPackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                            break;
                        }
                    case AddPackageCatalogStatus.InvalidOptions:
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddInvalidOptionsString, addPackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", addPackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                            break;
                        }
                    case AddPackageCatalogStatus.AccessDenied:
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddAccessDeniedString, addPackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", addPackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                            break;
                        }
                    case AddPackageCatalogStatus.AuthenticationError:
                        {
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddAuthenticationErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", addPackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 显示移除 WinGet 数据源通知
        /// </summary>
        private async Task ShowRemovePackageCatalogNotificationAsync(RemovePackageCatalogResult removePackageCatalogResult)
        {
            switch (removePackageCatalogResult.Status)
            {
                case RemovePackageCatalogStatus.Ok:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, true, WinGetDataSourceEditSuccessString));
                        break;
                    }
                case RemovePackageCatalogStatus.GroupPolicyError:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditGroupPolicyErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", removePackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                        break;
                    }
                case RemovePackageCatalogStatus.CatalogError:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditCatalogErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", removePackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                        break;
                    }
                case RemovePackageCatalogStatus.InternalError:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditInternalErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", removePackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                        break;
                    }
                case RemovePackageCatalogStatus.InvalidOptions:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditInvalidOptionsString, removePackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", removePackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                        break;
                    }
                case RemovePackageCatalogStatus.AccessDenied:
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditAccessDeniedString, removePackageCatalogResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", removePackageCatalogResult.ExtendedErrorCode.HResult) : NotAvailableString)));
                        break;
                    }
            }
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}
