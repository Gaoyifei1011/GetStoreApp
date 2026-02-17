using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.NotificationTips
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultNotificationTip : TeachingTip, INotifyPropertyChanged
    {
        private bool _isSuccessOperation;

        public bool IsSuccessOperation
        {
            get { return _isSuccessOperation; }

            set
            {
                if (!Equals(_isSuccessOperation, value))
                {
                    _isSuccessOperation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccessOperation)));
                }
            }
        }

        private string _operationContent;

        public string OperationContent
        {
            get { return _operationContent; }

            set
            {
                if (!string.Equals(_operationContent, value))
                {
                    _operationContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OperationContent)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public OperationResultNotificationTip(OperationKind operationKind)
        {
            InitializeComponent();

            if (operationKind is OperationKind.FileLost)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/FileLost");
            }
            else if (operationKind is OperationKind.FolderPicker)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/FolderPickerFailed");
            }
            else if (operationKind is OperationKind.InstallingNotify)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/InstallingNotify");
            }
            else if (operationKind is OperationKind.LanguageChange)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.GetLocalized("NotificationTip/LanguageChange");
            }
            else if (operationKind is OperationKind.NotElevated)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/NotElevated");
            }
            else if (operationKind is OperationKind.SelectEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SelectEmpty");
            }
            else if (operationKind is OperationKind.SelectFolderEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SelectFolderEmpty");
            }
            else if (operationKind is OperationKind.SelectPackageVolumeEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SelectPackageVolumeEmpty");
            }
            else if (operationKind is OperationKind.SourceNameEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SourceNameEmpty");
            }
            else if (operationKind is OperationKind.SourceUriEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SourceUriEmpty");
            }
        }

        public OperationResultNotificationTip(OperationKind operationKind, bool operationResult)
        {
            InitializeComponent();

            if (operationKind is OperationKind.Desktop)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/DesktopShortcutSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/DesktopShortcutFailed");
                }
            }
            else if (operationKind is OperationKind.DownloadCreate)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/DownloadCreateSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/DownloadCreateFailed");
                }
            }
            else if (operationKind is OperationKind.LogClean)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/LogCleanSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/LogCleanFailed");
                }
            }
            else if (operationKind is OperationKind.StartScreen)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/StartScreenSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/StartScreenFailed");
                }
            }
            else if (operationKind is OperationKind.Taskbar)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/TaskbarSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/TaskbarFailed");
                }
            }
            else if (operationKind is OperationKind.TerminateProcess)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/TerminateSuccess");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/TerminateFailed");
                }
            }
        }

        public OperationResultNotificationTip(OperationKind operationKind, bool isMultiSelected, int count)
        {
            InitializeComponent();

            if (operationKind is OperationKind.ShareFailed)
            {
                if (isMultiSelected)
                {
                    IsSuccessOperation = false;
                    OperationContent = string.Format(ResourceService.GetLocalized("NotificationTip/ShareSelectedFailed"), count);
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/ShareFailed");
                }
            }
        }

        public OperationResultNotificationTip(OperationKind operationKind, int statusKind)
        {
            InitializeComponent();

            if (operationKind is OperationKind.CheckUpdate)
            {
                if (statusKind is 0)
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/NotNewestVersion");
                }
                else if (statusKind is 1)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/NewestVersion");
                }
                else if (statusKind is 2)
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/UpdateCheckFailed");
                }
            }
        }

        public OperationResultNotificationTip(OperationKind operationKind, bool operationResult, string reason)
        {
            InitializeComponent();

            if (operationKind is OperationKind.WinGetSource)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = reason;
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = reason;
                }
            }
        }
    }
}
