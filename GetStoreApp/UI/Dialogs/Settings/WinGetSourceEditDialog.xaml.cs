using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.UI.Dialogs.Settings
{
    /// <summary>
    /// WinGet 数据源编辑对话框
    /// </summary>
    public sealed partial class WinGetSourceEditDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string WinGetDataSourceSaveFailed = ResourceService.GetLocalized("Dialog/WinGetDataSourceSaveFailed");
        private readonly string WinGetDataSourceAddGroupPolicyError = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddGroupPolicyError");
        private readonly string WinGetDataSourceAddCatalogError = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddCatalogError");
        private readonly string WinGetDataSourceAddInternalError = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddInternalError");
        private readonly string WinGetDataSourceAddInvalidOptions = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddInvalidOptions");
        private readonly string WinGetDataSourceAddAccessDenied = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddAccessDenied");
        private readonly string WinGetDataSourceAddAuthenticationError = ResourceService.GetLocalized("Dialog/WinGetDataSourceAddAuthenticationError");
        private readonly string Unknown = ResourceService.GetLocalized("Dialog/Unknown");
        private readonly PackageManager packageManager = new();

        private WinGetSourceEditKind WinGetSourceEditKind { get; }

        private bool _hasSaved;

        public bool HasSaved
        {
            get { return _hasSaved; }

            set
            {
                if (!Equals(_hasSaved, value))
                {
                    _hasSaved = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasSaved)));
                }
            }
        }

        private bool _isSaving;

        public bool IsSaving
        {
            get { return _isSaving; }

            set
            {
                if (!Equals(_isSaving, value))
                {
                    _isSaving = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSaving)));
                }
            }
        }

        private bool _isSaved;

        public bool IsSaved
        {
            get { return _isSaved; }

            set
            {
                if (!Equals(_isSaved, value))
                {
                    _isSaved = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSaved)));
                }
            }
        }

        private string _sourceName = string.Empty;

        public string SourceName
        {
            get { return _sourceName; }

            set
            {
                if (!Equals(_sourceName, value))
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
                if (!Equals(_sourceUri, value))
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
                if (!Equals(_customHeader, value))
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
                if (!Equals(_sourceType, value))
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

        private string _saveFailedContent;

        public string SaveFailedContent
        {
            get { return _saveFailedContent; }

            set
            {
                if (!Equals(_saveFailedContent, value))
                {
                    _saveFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveFailedContent)));
                }
            }
        }

        private List<KeyValuePair<PackageCatalogTrustLevel, string>> CatalogTrustLevelList { get; } =
        [
            KeyValuePair.Create(PackageCatalogTrustLevel.None, ResourceService.GetLocalized("Dialog/CatalogTrustLevelNone")),
            KeyValuePair.Create(PackageCatalogTrustLevel.Trusted, ResourceService.GetLocalized("Dialog/CatalogTrustLevelTrusted"))
        ];

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetSourceEditDialog(WinGetSourceEditKind winGetSourceEditKind, WinGetSourceModel winGetSourceItem)
        {
            InitializeComponent();
            WinGetSourceEditKind = winGetSourceEditKind;
            SelectedCatalogTrustLevel = CatalogTrustLevelList[0];
            Title = winGetSourceEditKind is WinGetSourceEditKind.Add ? ResourceService.GetLocalized("Dialog/WinGetDataSourceAdd") : ResourceService.GetLocalized("Dialog/WinGetDataSourceEdit");

            if (winGetSourceEditKind is WinGetSourceEditKind.Edit && winGetSourceItem is not null)
            {
                SourceName = winGetSourceItem.Name;
                SourceUri = winGetSourceItem.Arguments;
                CustomHeader = string.Empty;
                SourceType = winGetSourceItem.Type;
                Explicit = winGetSourceItem.PackageCatalogInformation.Explicit;
                SelectedCatalogTrustLevel = CatalogTrustLevelList.Find(item => item.Key.Equals(winGetSourceItem.PackageCatalogInformation.TrustLevel));
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
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                SelectedCatalogTrustLevel = CatalogTrustLevelList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
            }
        }

        /// <summary>
        /// 保存添加或修改的 WinGet 数据源
        /// </summary>
        private async void OnSaveClicked(object sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            if (string.IsNullOrEmpty(SourceName))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.SourceNameEmpty));
                return;
            }

            if (string.IsNullOrEmpty(SourceUri))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.SourceUriEmpty));
                return;
            }

            HasSaved = true;
            IsSaving = true;
            IsSaved = false;

            if (WinGetSourceEditKind is WinGetSourceEditKind.Add)
            {
                AddPackageCatalogOptions addPackageCatalogOptions = new()
                {
                    Name = string.IsNullOrEmpty(SourceName) ? string.Empty : SourceName,
                    SourceUri = string.IsNullOrEmpty(SourceUri) ? string.Empty : SourceUri,
                    Explicit = Explicit,
                    TrustLevel = SelectedCatalogTrustLevel.Key,
                    CustomHeader = string.IsNullOrEmpty(CustomHeader) ? string.Empty : CustomHeader,
                    Type = string.IsNullOrEmpty(SourceType) ? string.Empty : SourceType
                };

                AddPackageCatalogResult addPackageCatalogResult = await packageManager.AddPackageCatalogAsync(addPackageCatalogOptions);
                await Task.Delay(500);
                IsSaving = false;
                IsSaved = true;

                switch (addPackageCatalogResult.Status)
                {
                    case AddPackageCatalogStatus.Ok:
                        {
                            Hide();
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(WinGetSourceEditKind is WinGetSourceEditKind.Add ? OperationKind.WinGetSourceAdd : OperationKind.WinGetSourceEdit));
                            break;
                        }
                    case AddPackageCatalogStatus.GroupPolicyError:
                        {
                            SaveFailedContent = string.Format(WinGetDataSourceSaveFailed, WinGetDataSourceAddGroupPolicyError, addPackageCatalogResult.ExtendedErrorCode is not null ? addPackageCatalogResult.ExtendedErrorCode.HResult : Unknown);
                            break;
                        }
                    case AddPackageCatalogStatus.CatalogError:
                        {
                            SaveFailedContent = string.Format(WinGetDataSourceSaveFailed, WinGetDataSourceAddCatalogError, addPackageCatalogResult.ExtendedErrorCode is not null ? addPackageCatalogResult.ExtendedErrorCode.HResult : Unknown);
                            break;
                        }
                    case AddPackageCatalogStatus.InternalError:
                        {
                            SaveFailedContent = string.Format(WinGetDataSourceSaveFailed, WinGetDataSourceAddInternalError, addPackageCatalogResult.ExtendedErrorCode is not null ? addPackageCatalogResult.ExtendedErrorCode.HResult : Unknown);
                            break;
                        }
                    case AddPackageCatalogStatus.InvalidOptions:
                        {
                            SaveFailedContent = string.Format(WinGetDataSourceSaveFailed, WinGetDataSourceAddInvalidOptions, addPackageCatalogResult.ExtendedErrorCode is not null ? addPackageCatalogResult.ExtendedErrorCode.HResult : Unknown);
                            break;
                        }
                    case AddPackageCatalogStatus.AccessDenied:
                        {
                            SaveFailedContent = string.Format(WinGetDataSourceSaveFailed, WinGetDataSourceAddAccessDenied, addPackageCatalogResult.ExtendedErrorCode is not null ? addPackageCatalogResult.ExtendedErrorCode.HResult : Unknown);
                            break;
                        }
                    case AddPackageCatalogStatus.AuthenticationError:
                        {
                            SaveFailedContent = string.Format(WinGetDataSourceSaveFailed, WinGetDataSourceAddAuthenticationError, addPackageCatalogResult.ExtendedErrorCode is not null ? addPackageCatalogResult.ExtendedErrorCode.HResult : Unknown);
                            break;
                        }
                }
            }
            else if (WinGetSourceEditKind is WinGetSourceEditKind.Edit)
            {
            }
            else
            {
                Hide();
            }
        }
    }
}
