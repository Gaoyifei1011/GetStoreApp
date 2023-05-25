using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 历史记录复制成功后应用内通知视图
    /// </summary>
    public sealed partial class HistoryCopyNotification : InAppNotification
    {
        private int Count = 0;

        private bool IsMultiSelected = false;

        public HistoryCopyNotification([Optional, DefaultParameterValue(false)] bool copyState, [Optional, DefaultParameterValue(false)] bool isMultiSelected, [Optional, DefaultParameterValue(0)] int count)
        {
            InitializeComponent();
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            ViewModel.Initialize(copyState, isMultiSelected);
        }

        public void CopySelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            if (IsMultiSelected)
            {
                CopySelectedSuccess.Text = string.Format(ResourceService.GetLocalized("Notification/HistorySelectedCopySuccessfully"), Count);
            }
        }

        public void CopySelectedFailedLoaded(object sender, RoutedEventArgs args)
        {
            if (IsMultiSelected)
            {
                CopySelectedFailed.Text = string.Format(ResourceService.GetLocalized("Notification/HistorySelectedCopyFailed"), Count);
            }
        }

        public bool ControlLoad(bool copyState, bool isMultiSelected, string controlName)
        {
            if (controlName is "CopySuccess" && copyState && !isMultiSelected)
            {
                return true;
            }
            else if (controlName is "CopyFailed" && !copyState && !isMultiSelected)
            {
                return true;
            }
            else if (controlName is "CopySelectedSuccess" && copyState && isMultiSelected)
            {
                return true;
            }
            else if (controlName is "CopySelectedFailed" && !copyState && isMultiSelected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
