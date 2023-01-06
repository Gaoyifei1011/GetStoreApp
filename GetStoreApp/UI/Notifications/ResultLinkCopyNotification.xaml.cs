using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class ResultLinkCopyNotification : UserControl
    {
        public ElementTheme NotificationTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        private Popup Popup { get; set; } = new Popup();

        private int Duration = 2000;

        private int Count = 0;

        private bool IsMultiSelected = false;

        public ResultLinkCopyNotification(bool copyState, bool isMultiSelected = false, int count = 0, double? duration = null)
        {
            IsMultiSelected = isMultiSelected;
            Count = count;

            InitializeComponent();
            ViewModel.Initialize(copyState, isMultiSelected);

            SetPopUpPlacement();

            Popup.Child = this;
            Popup.XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();

            if (duration.HasValue)
            {
                Duration = Convert.ToInt32(duration * 1000);
            }
        }

        public void CopySelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            if (IsMultiSelected)
            {
                CopySelectedSuccess.Text = string.Format(ResourceService.GetLocalized("Notification/ResultLinkSelectedCopySuccessfully"), Count);
            }
        }

        public void CopySelectedFailedLoaded(object sender, RoutedEventArgs args)
        {
            if (IsMultiSelected)
            {
                CopySelectedFailed.Text = string.Format(ResourceService.GetLocalized("Notification/ResultLinkSelectedCopyFailed"), Count);
            }
        }

        public bool ControlLoad(bool copyState, bool isMultiSelected, int visibilityFlag)
        {
            if (visibilityFlag is 1 && copyState && !isMultiSelected)
            {
                return true;
            }
            else if (visibilityFlag is 2 && !copyState && !isMultiSelected)
            {
                return true;
            }
            else if (visibilityFlag is 3 && copyState && isMultiSelected)
            {
                return true;
            }
            else if (visibilityFlag is 4 && !copyState && isMultiSelected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 控件加载完成后显示动画，动态设置控件位置
        /// </summary>
        private void NotificationLoaded(object sender, RoutedEventArgs args)
        {
            PopupIn.Begin();
            Program.ApplicationRoot.MainWindow.SizeChanged += NotificationPlaceChanged;
        }

        /// <summary>
        /// 控件卸载时移除相应的事件
        /// </summary>
        private void NotificationUnLoaded(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.SizeChanged -= NotificationPlaceChanged;
        }

        /// <summary>
        /// 窗口大小调整时修改应用内通知的相对位置
        /// </summary>
        private void NotificationPlaceChanged(object sender, WindowSizeChangedEventArgs args)
        {
            SetPopUpPlacement();
        }

        /// <summary>
        /// 应用内通知加载动画演示完成时发生
        /// </summary>
        private async void PopupInCompleted(object sender, object e)
        {
            await Task.Delay(Duration);
            PopupOut.Begin();
        }

        /// <summary>
        /// 应用内通知加载动画卸载完成时发生，关闭控件
        /// </summary>
        public void PopupOutCompleted(object sender, object e)
        {
            Popup.IsOpen = false;
        }

        /// <summary>
        /// 设置PopUp的显示位置
        /// </summary>
        private void SetPopUpPlacement()
        {
            Width = Program.ApplicationRoot.MainWindow.Bounds.Width;
            Height = Program.ApplicationRoot.MainWindow.Bounds.Height;

            Popup.VerticalOffset = 75;
        }

        /// <summary>
        /// 显示弹窗
        /// </summary>
        public void Show()
        {
            Popup.IsOpen = true;
        }
    }
}
