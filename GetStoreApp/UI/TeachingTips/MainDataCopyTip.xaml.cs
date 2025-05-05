using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 数据复制应用内通知
    /// </summary>
    public sealed partial class MainDataCopyTip : TeachingTip
    {
        public MainDataCopyTip(DataCopyKind dataCopyKind, bool isSuccessfully = false, bool isMultiSelected = false, int count = 0)
        {
            InitializeComponent();
            InitializeContent(dataCopyKind, isSuccessfully, isMultiSelected, count);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(DataCopyKind dataCopyKind, bool isSuccessfully, bool isMultiSelected, int count)
        {
            if (isSuccessfully)
            {
                CopySuccess.Visibility = Visibility.Visible;
                CopyFailed.Visibility = Visibility.Collapsed;

                switch (dataCopyKind)
                {
                    case DataCopyKind.AppInformation:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/AppInformationCopy");
                            break;
                        }
                    case DataCopyKind.AppUserModelId:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/AppUserModelIdCopy");
                            break;
                        }
                    case DataCopyKind.DependencyName:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/DependencyNameCopy");
                            break;
                        }
                    case DataCopyKind.DependencyInformation:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/DependencyInformationCopy");
                            break;
                        }
                    case DataCopyKind.Exception:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/ExceptionCopy");
                            break;
                        }
                    case DataCopyKind.FileInformation:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/FileInformationCopy");
                            break;
                        }
                    case DataCopyKind.History:
                        {
                            CopySuccess.Text = isMultiSelected ? string.Format(ResourceService.GetLocalized("Notification/HistorySelectedCopy"), count) : ResourceService.GetLocalized("Notification/HistoryCopy");
                            break;
                        }
                    case DataCopyKind.PackageInformation:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/PackageInformationCopy");
                            break;
                        }
                    case DataCopyKind.ResultInformation:
                        {
                            CopySuccess.Text = isMultiSelected ? string.Format(ResourceService.GetLocalized("Notification/ResultContentSelectedCopy"), count) : ResourceService.GetLocalized("Notification/ResultContentCopy");
                            break;
                        }
                    case DataCopyKind.ResultID:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/ResultIDCopy");
                            break;
                        }
                    case DataCopyKind.ResultLink:
                        {
                            CopySuccess.Text = isMultiSelected ? string.Format(ResourceService.GetLocalized("Notification/ResultLinkSelectedCopy"), count) : ResourceService.GetLocalized("Notification/ResultLinkCopy");
                            break;
                        }
                    case DataCopyKind.WinGetAppInformation:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/WinGetAppInformation");
                            break;
                        }
                    case DataCopyKind.WinGetSearchDownload:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/SearchDownloadCopy");
                            break;
                        }
                    case DataCopyKind.WinGetSearchInstall:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/SearchInstallCopy");
                            break;
                        }
                    case DataCopyKind.WinGetUninstall:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/UninstallCopy");
                            break;
                        }
                    case DataCopyKind.WinGetUpgradeInstall:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/UpgradeInstallCopy");
                            break;
                        }
                }
            }
            else
            {
                CopySuccess.Visibility = Visibility.Collapsed;
                CopyFailed.Visibility = Visibility.Visible;
            }
        }
    }
}
