using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 数据复制应用内通知
    /// </summary>
    public sealed partial class DataCopyTip : TeachingTip
    {
        public DataCopyTip(DataCopyKind dataCopyKind, bool isSuccessfully = false, bool isMultiSelected = false, int count = 0)
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
                            if (isMultiSelected)
                            {
                                CopySuccess.Text = string.Format(ResourceService.GetLocalized("Notification/HistorySelectedCopy"), count);
                            }
                            else
                            {
                                CopySuccess.Text = ResourceService.GetLocalized("Notification/HistoryCopy");
                            }
                            break;
                        }
                    case DataCopyKind.PackageInformation:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/PackageInformationCopy");
                            break;
                        }
                    case DataCopyKind.ResultInformation:
                        {
                            if (isMultiSelected)
                            {
                                CopySuccess.Text = string.Format(ResourceService.GetLocalized("Notification/ResultContentSelectedCopy"), count);
                            }
                            else
                            {
                                CopySuccess.Text = ResourceService.GetLocalized("Notification/ResultContentCopy");
                            }
                            break;
                        }
                    case DataCopyKind.ResultID:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/ResultIDCopy");
                            break;
                        }
                    case DataCopyKind.ResultLink:
                        {
                            if (isMultiSelected)
                            {
                                CopySuccess.Text = string.Format(ResourceService.GetLocalized("Notification/ResultLinkSelectedCopy"), count);
                            }
                            else
                            {
                                CopySuccess.Text = ResourceService.GetLocalized("Notification/ResultLinkCopy");
                            }
                            break;
                        }
                    case DataCopyKind.WinGetSearchInstall:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/SearchInstallCopy");
                            break;
                        }
                    case DataCopyKind.WinGetUnInstall:
                        {
                            CopySuccess.Text = ResourceService.GetLocalized("Notification/UnInstallCopy");
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
