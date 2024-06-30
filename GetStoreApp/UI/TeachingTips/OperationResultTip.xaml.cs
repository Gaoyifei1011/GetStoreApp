using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultTip : TeachingTip
    {
        public OperationResultTip(OperationKind operationKind)
        {
            InitializeComponent();

            if (operationKind is OperationKind.FileLost)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.GetLocalized("Notification/FileLost");
            }
            else if (operationKind is OperationKind.FolderPicker)
            {
                OperationResultSuccess.Visibility = Visibility.Collapsed;
                OperationResultFailed.Visibility = Visibility.Visible;
                OperationResultFailed.Text = ResourceService.GetLocalized("Notification/FolderPickerFailed");
            }
            else if (operationKind is OperationKind.LanguageChange)
            {
                OperationResultSuccess.Visibility = Visibility.Visible;
                OperationResultFailed.Visibility = Visibility.Collapsed;
                OperationResultSuccess.Text = ResourceService.GetLocalized("Notification/LanguageChange");
            }
        }

        public OperationResultTip(OperationKind operationKind, bool operationResult)
        {
            InitializeComponent();

            if (operationKind is OperationKind.CheckUpdate)
            {
                if (operationResult)
                {
                    OperationResultSuccess.Text = ResourceService.GetLocalized("Notification/NewestVersion");
                    OperationResultSuccess.Visibility = Visibility.Visible;
                    OperationResultFailed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.GetLocalized("Notification/NotNewestVersion");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
            }
            else if (operationKind is OperationKind.DownloadCreate)
            {
                if (operationResult)
                {
                    OperationResultSuccess.Text = ResourceService.GetLocalized("Notification/DownloadCreateSuccessfully");
                    OperationResultSuccess.Visibility = Visibility.Visible;
                    OperationResultFailed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.GetLocalized("Notification/DownloadCreateFailed");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
            }
            else if (operationKind is OperationKind.LogClean)
            {
                if (operationResult)
                {
                    OperationResultSuccess.Text = ResourceService.GetLocalized("Notification/LogCleanSuccessfully");
                    OperationResultSuccess.Visibility = Visibility.Visible;
                    OperationResultFailed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.GetLocalized("Notification/LogCleanFailed");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
            }
            else if (operationKind is OperationKind.TerminateProcess)
            {
                if (operationResult)
                {
                    OperationResultSuccess.Text = ResourceService.GetLocalized("Notification/TerminateSuccess");
                    OperationResultSuccess.Visibility = Visibility.Visible;
                    OperationResultFailed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.GetLocalized("Notification/TerminateFailed");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
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
                    OperationResultFailed.Text = string.Format(ResourceService.GetLocalized("Notification/ShareSelectedFailed"), count);
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
                else
                {
                    OperationResultFailed.Text = ResourceService.GetLocalized("Notification/ShareFailed");
                    OperationResultSuccess.Visibility = Visibility.Collapsed;
                    OperationResultFailed.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
