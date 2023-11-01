using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 数据复制应用内通知
    /// </summary>
    public sealed partial class DataCopyNotification : InAppNotification
    {
        private bool IsMultiSelected;
        private int Count;
        private DataCopyKind DataCopyType;

        public DataCopyNotification(FrameworkElement element, DataCopyKind copyType, bool isMultiSelected = false, int count = 0) : base(element)
        {
            InitializeComponent();
            DataCopyType = copyType;
            IsMultiSelected = isMultiSelected;
            Count = count;

            InitializeContent();
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent()
        {
            switch (DataCopyType)
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
                        if (IsMultiSelected)
                        {
                            Content = string.Format(ResourceService.GetLocalized("Notification/HistorySelectedCopy"), Count);
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
                case DataCopyKind.ResultContent:
                    {
                        if (IsMultiSelected)
                        {
                            Content = string.Format(ResourceService.GetLocalized("Notification/ResultContentSelectedCopy"), Count);
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
                        if (IsMultiSelected)
                        {
                            Content = string.Format(ResourceService.GetLocalized("Notification/ResultLinkSelectedCopy"), Count);
                        }
                        else
                        {
                            Content = ResourceService.GetLocalized("Notification/ResultLinkCopy");
                        }
                        break;
                    }
                case DataCopyKind.ShareFile:
                    {
                        if (IsMultiSelected)
                        {
                            Content = string.Format(ResourceService.GetLocalized("Notification/ShareFileSelectedCopy"), Count);
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
