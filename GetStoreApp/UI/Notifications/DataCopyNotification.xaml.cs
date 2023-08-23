using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class DataCopyNotification : InAppNotification
    {
        private DataCopyType DataCopyType { get; }

        private bool IsMultiSelected { get; }

        private int Count { get; }

        public DataCopyNotification(FrameworkElement element, DataCopyType copyType, bool isMultiSelected = false, int count = 0) : base(element)
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
                case DataCopyType.AppInformation:
                    {
                        NotificationContent.Text = ResourceService.GetLocalized("Notification/AppInformationCopy");
                        break;
                    }
                case DataCopyType.AppUserModelId:
                    {
                        NotificationContent.Text = ResourceService.GetLocalized("Notification/AppUserModelIdCopy");
                        break;
                    }
                case DataCopyType.Exception:
                    {
                        NotificationContent.Text = ResourceService.GetLocalized("Notification/ExceptionCopy");
                        break;
                    }
                case DataCopyType.FileInformation:
                    {
                        NotificationContent.Text = ResourceService.GetLocalized("Notification/FileInformationCopy");
                        break;
                    }
                case DataCopyType.History:
                    {
                        if (IsMultiSelected)
                        {
                            NotificationContent.Text = string.Format(ResourceService.GetLocalized("Notification/HistorySelectedCopy"), Count);
                        }
                        else
                        {
                            NotificationContent.Text = ResourceService.GetLocalized("Notification/HistoryCopy");
                        }
                        break;
                    }
                case DataCopyType.PackageInformation:
                    {
                        NotificationContent.Text = ResourceService.GetLocalized("Notification/PackageInformationCopy");
                        break;
                    }
                case DataCopyType.ResultContent:
                    {
                        if (IsMultiSelected)
                        {
                            NotificationContent.Text = string.Format(ResourceService.GetLocalized("Notification/ResultContentSelectedCopy"), Count);
                        }
                        else
                        {
                            NotificationContent.Text = ResourceService.GetLocalized("Notification/ResultContentCopy");
                        }
                        break;
                    }
                case DataCopyType.ResultID:
                    {
                        NotificationContent.Text = ResourceService.GetLocalized("Notification/ResultIDCopy");
                        break;
                    }
                case DataCopyType.ResultLink:
                    {
                        if (IsMultiSelected)
                        {
                            NotificationContent.Text = string.Format(ResourceService.GetLocalized("Notification/ResultLinkSelectedCopy"), Count);
                        }
                        else
                        {
                            NotificationContent.Text = ResourceService.GetLocalized("Notification/ResultLinkCopy");
                        }
                        break;
                    }
                case DataCopyType.WinGetSearchInstall:
                    {
                        NotificationContent.Text = ResourceService.GetLocalized("Notification/SearchInstallCopy");
                        break;
                    }
                case DataCopyType.WinGetUnInstall:
                    {
                        NotificationContent.Text = ResourceService.GetLocalized("Notification/UnInstallCopy");
                        break;
                    }
                case DataCopyType.WinGetUpgradeInstall:
                    {
                        NotificationContent.Text = ResourceService.GetLocalized("Notification/UpgradeInstallCopy");
                        break;
                    }
            }
        }
    }
}
