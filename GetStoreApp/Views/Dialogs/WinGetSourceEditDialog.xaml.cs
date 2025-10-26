using GetStoreApp.Extensions.DataType.Enums;
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

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// WinGet 数据源编辑对话框
    /// </summary>
    public sealed partial class WinGetSourceEditDialog : ContentDialog, INotifyPropertyChanged
    {
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

        private WinGetSourceEditKind WinGetSourceEditKind { get; }

        public AddPackageCatalogStatus? AddPackageCatalogStatusResult { get; private set; } = null;

        private bool _isSaving;

        public bool IsSaving
        {
            get { return _isSaving; }

            set
            {
                if (!string.Equals(_isSaving, value))
                {
                    _isSaving = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSaving)));
                }
            }
        }

        private string _editTitle;

        public string EditTitle
        {
            get { return _editTitle; }

            set
            {
                if (!string.Equals(_editTitle, value))
                {
                    _editTitle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EditTitle)));
                }
            }
        }

        private string _sourceName = string.Empty;

        public string SourceName
        {
            get { return _sourceName; }

            set
            {
                if (!string.Equals(_sourceName, value))
                {
                    _sourceName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceName)));
                }
            }
        }

        private string _sourceUri = string.Empty;

        public string SourceUri
        {
            get { return _sourceUri; }

            set
            {
                if (!string.Equals(_sourceUri, value))
                {
                    _sourceUri = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceUri)));
                }
            }
        }

        private string _customHeader = string.Empty;

        public string CustomHeader
        {
            get { return _customHeader; }

            set
            {
                if (!string.Equals(_customHeader, value))
                {
                    _customHeader = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomHeader)));
                }
            }
        }

        private string _sourceType = string.Empty;

        public string SourceType
        {
            get { return _sourceType; }

            set
            {
                if (!string.Equals(_sourceType, value))
                {
                    _sourceType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceType)));
                }
            }
        }

        private bool _explicit = false;

        public bool Explicit
        {
            get { return _explicit; }

            set
            {
                if (!Equals(_explicit, value))
                {
                    _explicit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Explicit)));
                }
            }
        }

        private KeyValuePair<PackageCatalogTrustLevel, string> _selectedCatalogTrustLevel;

        public KeyValuePair<PackageCatalogTrustLevel, string> SelectedCatalogTrustLevel
        {
            get { return _selectedCatalogTrustLevel; }

            set
            {
                if (!Equals(_selectedCatalogTrustLevel, value))
                {
                    _selectedCatalogTrustLevel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCatalogTrustLevel)));
                }
            }
        }

        private List<KeyValuePair<PackageCatalogTrustLevel, string>> CatalogTrustLevelList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetSourceEditDialog(WinGetSourceEditKind winGetSourceEditKind, WinGetSourceModel winGetSource)
        {
            InitializeComponent();
            WinGetSourceEditKind = winGetSourceEditKind;
            CatalogTrustLevelList.Add(KeyValuePair.Create(PackageCatalogTrustLevel.None, CatalogTrustLevelNoneString));
            CatalogTrustLevelList.Add(KeyValuePair.Create(PackageCatalogTrustLevel.Trusted, CatalogTrustLevelTrustedString));
            SelectedCatalogTrustLevel = CatalogTrustLevelList[0];
            EditTitle = winGetSourceEditKind is WinGetSourceEditKind.Add ? WinGetDataSourceAddString : WinGetDataSourceEditString;

            if (winGetSourceEditKind is WinGetSourceEditKind.Edit && winGetSource is not null)
            {
                SourceName = winGetSource.Name;
                SourceUri = winGetSource.Arguments;
                CustomHeader = string.Empty;
                SourceType = winGetSource.Type;
                Explicit = winGetSource.PackageCatalogInformation.Explicit;
                SelectedCatalogTrustLevel = CatalogTrustLevelList.Find(item => Equals(item.Key, winGetSource.PackageCatalogInformation.TrustLevel));
            }
        }

        /// <summary>
        /// 文本输入框内容发生更改时触发的事件
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                switch (Convert.ToString(textBox.Tag))
                {
                    case nameof(SourceName):
                        {
                            SourceName = textBox.Text;
                            break;
                        }
                    case nameof(SourceUri):
                        {
                            SourceUri = textBox.Text;
                            break;
                        }
                    case nameof(CustomHeader):
                        {
                            CustomHeader = textBox.Text;
                            break;
                        }
                    case nameof(SourceType):
                        {
                            SourceType = textBox.Text;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 是否为显性开关发生更改时触发的事件
        /// </summary>
        private void OnExplicitToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                Explicit = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 数据源信任等级发生更改时触发的事件
        /// </summary>
        private void OnCatalogTrustLevelClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                SelectedCatalogTrustLevel = CatalogTrustLevelList[Convert.ToInt32(tag)];
            }
        }

        /// <summary>
        /// 保存添加或修改的 WinGet 数据源
        /// </summary>
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
                    AddPackageCatalogResult addPackageCatalogResult = await Task.Run(async () =>
                    {
                        AddPackageCatalogOptions addPackageCatalogOptions = new()
                        {
                            Name = SourceName,
                            SourceUri = SourceUri,
                            Explicit = Explicit,
                            TrustLevel = SelectedCatalogTrustLevel.Key,
                            CustomHeader = string.IsNullOrEmpty(CustomHeader) ? string.Empty : CustomHeader,
                            Type = string.IsNullOrEmpty(SourceType) ? string.Empty : SourceType
                        };

                        PackageManager packageManager = new();
                        return await packageManager.AddPackageCatalogAsync(addPackageCatalogOptions);
                    });

                    AddPackageCatalogStatusResult = addPackageCatalogResult.Status;
                    IsSaving = false;

                    switch (addPackageCatalogResult.Status)
                    {
                        case AddPackageCatalogStatus.Ok:
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, true, WinGetDataSourceAddSuccessString));
                                break;
                            }
                        case AddPackageCatalogStatus.GroupPolicyError:
                            {
                                args.Cancel = true;
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddGroupPolicyErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                break;
                            }
                        case AddPackageCatalogStatus.CatalogError:
                            {
                                args.Cancel = true;
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddCatalogErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                break;
                            }
                        case AddPackageCatalogStatus.InternalError:
                            {
                                args.Cancel = true;
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddInternalErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                break;
                            }
                        case AddPackageCatalogStatus.InvalidOptions:
                            {
                                args.Cancel = true;
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddInvalidOptionsString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                break;
                            }
                        case AddPackageCatalogStatus.AccessDenied:
                            {
                                args.Cancel = true;
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddAccessDeniedString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                break;
                            }
                        case AddPackageCatalogStatus.AuthenticationError:
                            {
                                args.Cancel = true;
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceAddFailedString, WinGetDataSourceAddAuthenticationErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                break;
                            }
                    }
                }
                else if (WinGetSourceEditKind is WinGetSourceEditKind.Edit)
                {
                    RemovePackageCatalogResult removePackageCatalogResult = await Task.Run(async () =>
                    {
                        RemovePackageCatalogOptions removePackageCatalogOptions = new()
                        {
                            Name = SourceName,
                            PreserveData = true,
                        };

                        PackageManager packageManager = new();
                        return await packageManager.RemovePackageCatalogAsync(removePackageCatalogOptions);
                    });

                    if (removePackageCatalogResult.Status is RemovePackageCatalogStatus.Ok)
                    {
                        AddPackageCatalogResult addPackageCatalogResult = await Task.Run(async () =>
                        {
                            AddPackageCatalogOptions addPackageCatalogOptions = new()
                            {
                                Name = SourceName,
                                SourceUri = SourceUri,
                                Explicit = Explicit,
                                TrustLevel = SelectedCatalogTrustLevel.Key,
                                CustomHeader = string.IsNullOrEmpty(CustomHeader) ? string.Empty : CustomHeader,
                                Type = string.IsNullOrEmpty(SourceType) ? string.Empty : SourceType
                            };

                            PackageManager packageManager = new();
                            return await packageManager.AddPackageCatalogAsync(addPackageCatalogOptions);
                        });

                        AddPackageCatalogStatusResult = addPackageCatalogResult.Status;
                        IsSaving = false;

                        switch (addPackageCatalogResult.Status)
                        {
                            case AddPackageCatalogStatus.Ok:
                                {
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, true, WinGetDataSourceEditSuccessString));
                                    break;
                                }
                            case AddPackageCatalogStatus.GroupPolicyError:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditGroupPolicyErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                            case AddPackageCatalogStatus.CatalogError:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditCatalogErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                            case AddPackageCatalogStatus.InternalError:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditInternalErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                            case AddPackageCatalogStatus.InvalidOptions:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditInvalidOptionsString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                            case AddPackageCatalogStatus.AccessDenied:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditAccessDeniedString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                            case AddPackageCatalogStatus.AuthenticationError:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditAuthenticationErrorString, addPackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(addPackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                        }
                    }
                    else
                    {
                        IsSaving = false;

                        switch (removePackageCatalogResult.Status)
                        {
                            case RemovePackageCatalogStatus.Ok:
                                {
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, true, WinGetDataSourceEditSuccessString));
                                    break;
                                }
                            case RemovePackageCatalogStatus.GroupPolicyError:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditGroupPolicyErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                            case RemovePackageCatalogStatus.CatalogError:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditCatalogErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                            case RemovePackageCatalogStatus.InternalError:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditInternalErrorString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                            case RemovePackageCatalogStatus.InvalidOptions:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditInvalidOptionsString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                            case RemovePackageCatalogStatus.AccessDenied:
                                {
                                    args.Cancel = true;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceEditFailedString, WinGetDataSourceEditAccessDeniedString, removePackageCatalogResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(removePackageCatalogResult.ExtendedErrorCode.HResult, 16).ToUpper() : NotAvailableString)));
                                    break;
                                }
                        }
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
    }
}
