using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 数据复制应用内通知
    /// </summary>
    public sealed partial class DataCopyTip : TeachingTip
    {
        public DataCopyTip(DataCopyKind dataCopyKind, bool isMultiSelected = false, int count = 0)
        {
            InitializeComponent();
            InitializeContent(dataCopyKind, isMultiSelected, count);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(DataCopyKind dataCopyKind, bool isMultiSelected, int count)
        {
            switch (dataCopyKind)
            {
                case DataCopyKind.AppInformation:
                    {
                        Content = ResourceService.GetLocalized("Notification/AppInformationCopy");
                        break;
                    }
                case DataCopyKind.AppUserModelId:
                    {
                        Content = ResourceService.GetLocalized("Notification/AppUserModelIdCopy");
                        break;
                    }
                case DataCopyKind.DependencyName:
                    {
                        Content = ResourceService.GetLocalized("Notification/DependencyNameCopy");
                        break;
                    }
                case DataCopyKind.DependencyInformation:
                    {
                        Content = ResourceService.GetLocalized("Notification/DependencyInformationCopy");
                        break;
                    }
                case DataCopyKind.Exception:
                    {
                        Content = ResourceService.GetLocalized("Notification/ExceptionCopy");
                        break;
                    }
                case DataCopyKind.FileInformation:
                    {
                        Content = ResourceService.GetLocalized("Notification/FileInformationCopy");
                        break;
                    }
                case DataCopyKind.History:
                    {
                        if (isMultiSelected)
                        {
                            Content = string.Format(ResourceService.GetLocalized("Notification/HistorySelectedCopy"), count);
                        }
                        else
                        {
                            Content = ResourceService.GetLocalized("Notification/HistoryCopy");
                        }
                        break;
                    }
                case DataCopyKind.PackageInformation:
                    {
                        Content = ResourceService.GetLocalized("Notification/PackageInformationCopy");
                        break;
                    }
                case DataCopyKind.ResultInformation:
                    {
                        if (isMultiSelected)
                        {
                            Content = string.Format(ResourceService.GetLocalized("Notification/ResultContentSelectedCopy"), count);
                        }
                        else
                        {
                            Content = ResourceService.GetLocalized("Notification/ResultContentCopy");
                        }
                        break;
                    }
                case DataCopyKind.ResultID:
                    {
                        Content = ResourceService.GetLocalized("Notification/ResultIDCopy");
                        break;
                    }
                case DataCopyKind.ResultLink:
                    {
                        if (isMultiSelected)
                        {
                            Content = string.Format(ResourceService.GetLocalized("Notification/ResultLinkSelectedCopy"), count);
                        }
                        else
                        {
                            Content = ResourceService.GetLocalized("Notification/ResultLinkCopy");
                        }
                        break;
                    }
                case DataCopyKind.ShareFile:
                    {
                        if (isMultiSelected)
                        {
                            Content = string.Format(ResourceService.GetLocalized("Notification/ShareFileSelectedCopy"), count);
                        }
                        else
                        {
                            Content = ResourceService.GetLocalized("Notification/ShareFileCopy");
                        }
                        break;
                    }
                case DataCopyKind.WinGetSearchInstall:
                    {
                        Content = ResourceService.GetLocalized("Notification/SearchInstallCopy");
                        break;
                    }
                case DataCopyKind.WinGetUnInstall:
                    {
                        Content = ResourceService.GetLocalized("Notification/UnInstallCopy");
                        break;
                    }
                case DataCopyKind.WinGetUpgradeInstall:
                    {
                        Content = ResourceService.GetLocalized("Notification/UpgradeInstallCopy");
                        break;
                    }
            }
        }
    }
}
