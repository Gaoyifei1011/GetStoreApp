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
    internal sealed partial class OperationResultNotificationTip : TeachingTip, INotifyPropertyChanged
    {
        #region 第一部分：属性、集合与事件

        internal bool IsInitialized { get; private set; }

        private bool IsSuccessOperation
        {
            get;

            set
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSuccessOperation)));
                }
            }
        }

        private string OperationContent
        {
            get;

            set
            {
                if (!string.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new(nameof(OperationContent)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第一部分：属性、集合与事件

        #region 第二部分：构造函数

        internal OperationResultNotificationTip(OperationKind operationKind)
        {
            InitializeComponent();
            InitializeOperationResultNotification(operationKind);
        }

        internal OperationResultNotificationTip(OperationKind operationKind, bool operationResult)
        {
            InitializeComponent();
            InitializeOperationResultNotification(operationKind, operationResult);
        }

        internal OperationResultNotificationTip(OperationKind operationKind, bool isMultiSelected, int count)
        {
            InitializeComponent();
            InitializeOperationResultNotification(operationKind, isMultiSelected, count);
        }

        internal OperationResultNotificationTip(OperationKind operationKind, int statusKind)
        {
            InitializeComponent();
            InitializeOperationResultNotification(operationKind, statusKind);
        }

        internal OperationResultNotificationTip(OperationKind operationKind, bool operationResult, string reason)
        {
            InitializeComponent();
            InitializeOperationResultNotification(operationKind, operationResult, reason);
        }

        #endregion 第二部分：构造函数

        #region 第三部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化操作完成后应用内通知
        /// </summary>
        private void InitializeOperationResultNotification(OperationKind operationKind)
        {
            if (operationKind is OperationKind.FileLost)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/FileLost");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.FolderPicker)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/FolderPickerFailed");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.InstallingNotify)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/InstallingNotify");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.LanguageChange)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.GetLocalized("NotificationTip/LanguageChange");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.NotElevated)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/NotElevated");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.SelectEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SelectEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.SelectFolderEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SelectFolderEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.SelectPackageVolumeEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SelectPackageVolumeEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.SourceNameEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SourceNameEmpty");
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.SourceUriEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SourceUriEmpty");
                IsInitialized = true;
            }
        }

        /// <summary>
        /// 初始化操作完成后应用内通知
        /// </summary>
        private void InitializeOperationResultNotification(OperationKind operationKind, bool operationResult)
        {
            if (operationKind is OperationKind.CopyToClipboard)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/CopyToClipboardSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/CopyToClipboardFailed");
                }
                IsInitialized = true;
            }
            else if (operationKind is OperationKind.Desktop)
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
                IsInitialized = true;
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
                IsInitialized = true;
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
                IsInitialized = true;
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
                IsInitialized = true;
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
                IsInitialized = true;
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
                IsInitialized = true;
            }
        }

        /// <summary>
        /// 初始化操作完成后应用内通知
        /// </summary>
        private void InitializeOperationResultNotification(OperationKind operationKind, bool isMultiSelected, int count)
        {
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
                IsInitialized = true;
            }
        }

        /// <summary>
        /// 初始化操作完成后应用内通知
        /// </summary>
        private void InitializeOperationResultNotification(OperationKind operationKind, int statusKind)
        {
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
                IsInitialized = true;
            }
        }

        /// <summary>
        /// 初始化操作完成后应用内通知
        /// </summary>
        private void InitializeOperationResultNotification(OperationKind operationKind, bool operationResult, string reason)
        {
            if (operationKind is OperationKind.WinGetSource && !string.IsNullOrEmpty(reason))
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
                IsInitialized = true;
            }
        }

        #endregion 第三部分：数据操作与业务逻辑
    }
}
