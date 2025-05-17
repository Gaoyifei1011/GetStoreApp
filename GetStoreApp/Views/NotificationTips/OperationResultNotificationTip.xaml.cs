using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultTip : TeachingTip, INotifyPropertyChanged
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
                if (!Equals(_operationContent, value))
                {
                    _operationContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OperationContent)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public OperationResultTip(OperationKind operationKind)
        {
            InitializeComponent();

            if (operationKind is OperationKind.FileLost)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("Notification/FileLost");
            }
            else if (operationKind is OperationKind.FolderPicker)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("Notification/FolderPickerFailed");
            }
            else if (operationKind is OperationKind.InstallingNotify)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("Notification/InstallingNotify");
            }
            else if (operationKind is OperationKind.LanguageChange)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.GetLocalized("Notification/LanguageChange");
            }
            else if (operationKind is OperationKind.NotElevated)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("Notification/NotElevated");
            }
            else if (operationKind is OperationKind.SelectEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("Notification/SelectEmpty");
            }
            else if (operationKind is OperationKind.SourceNameEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("Notification/SourceNameEmpty");
            }
            else if (operationKind is OperationKind.SourceUriEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("Notification/SourceUriEmpty");
            }
        }

        public OperationResultTip(OperationKind operationKind, bool operationResult)
        {
            InitializeComponent();

            if (operationKind is OperationKind.CheckUpdate)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("Notification/NewestVersion");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("Notification/NotNewestVersion");
                }
            }
            else if (operationKind is OperationKind.DownloadCreate)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("Notification/DownloadCreateSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("Notification/DownloadCreateFailed");
                }
            }
            else if (operationKind is OperationKind.LogClean)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("Notification/LogCleanSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("Notification/LogCleanFailed");
                }
            }
            else if (operationKind is OperationKind.TerminateProcess)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("Notification/TerminateSuccess");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("Notification/TerminateFailed");
                }
            }
            else if (operationKind is OperationKind.Desktop)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("Notification/DesktopShortcutSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("Notification/DesktopShortFailed");
                }
            }
            else if (operationKind is OperationKind.StartScreen)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("Notification/StartScreenSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("Notification/StartScreenFailed");
                }
            }
            else if (operationKind is OperationKind.Taskbar)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("Notification/TaskbarSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("Notification/TaskbarFailed");
                }
            }
        }

        public OperationResultTip(OperationKind operationKind, bool isMultiSelected, int count)
        {
            InitializeComponent();

            if (operationKind is OperationKind.ShareFailed)
            {
                if (isMultiSelected)
                {
                    IsSuccessOperation = false;
                    OperationContent = string.Format(ResourceService.GetLocalized("Notification/ShareSelectedFailed"), count);
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("Notification/ShareFailed");
                }
            }
        }

        public OperationResultTip(OperationKind operationKind, bool operationResult, string reason)
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
