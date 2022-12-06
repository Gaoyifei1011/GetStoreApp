using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class ResultContentCopyNotification : UserControl
    {
        private Popup Popup { get; set; } = new Popup();

        private int Duration = 2000;

        private int Count = 0;

        private bool IsMultiSelected = false;

        public ResultContentCopyNotification(bool copyState, bool isMultiSelected = false, int count = 0, double? duration = null)
        {
            IsMultiSelected = isMultiSelected;
            Count = count;

            InitializeComponent();
            ViewModel.Initialize(copyState, isMultiSelected);

            SetPopUpPlacement();

            Popup.Child = this;
            Popup.XamlRoot = App.MainWindow.Content.XamlRoot;

            if (duration.HasValue)
            {
                Duration = Convert.ToInt32(duration * 1000);
            }
        }

        public void CopySelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            if (IsMultiSelected)
            {
                CopySelectedSuccess.Text = string.Format(ResourceService.GetLocalized("/Notification/ResultContentSelectedCopySuccessfully"), Count);
            }
        }

        public void CopySelectedFailedLoaded(object sender, RoutedEventArgs args)
        {
            if (IsMultiSelected)
            {
                CopySelectedFailed.Text = string.Format(ResourceService.GetLocalized("/Notification/ResultContentSelectedCopyFailed"), Count);
            }
        }

        public bool ControlLoad(bool copyState, bool isMultiSelected, int visibilityFlag)
        {
            if (visibilityFlag == 1 && (copyState && !isMultiSelected))
            {
                return true;
            }
            else if (visibilityFlag == 2 && (!copyState && !isMultiSelected))
            {
                return true;
            }
            else if (visibilityFlag == 3 && (copyState && isMultiSelected))
            {
                return true;
            }
            else if (visibilityFlag == 4 && (!copyState && isMultiSelected))
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
            App.MainWindow.SizeChanged += NotificationPlaceChanged;
        }

        /// <summary>
        /// 控件卸载时移除相应的事件
        /// </summary>
        private void NotificationUnLoaded(object sender, RoutedEventArgs args)
        {
            App.MainWindow.SizeChanged -= NotificationPlaceChanged;
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
            Width = App.MainWindow.Bounds.Width;
            Height = App.MainWindow.Bounds.Height;

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
