using GetStoreApp.Helpers.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;
using System.Threading.Tasks;

namespace GetStoreApp.Views.CustomControls.Notifications
{
    /// <summary>
    /// 应用内通知控件
    /// </summary>
    public class InAppNotification : ContentControl
    {
        private Popup popup = new Popup();

        private Grid gridRoot;

        private Storyboard popupIn;

        private Storyboard popupOut;

        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(int), typeof(InAppNotification), new PropertyMetadata(2000));

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(InAppNotification), new PropertyMetadata(0.0));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(InAppNotification), new PropertyMetadata(0.0));

        public InAppNotification()
        {
            DefaultStyleKey = typeof(InAppNotification);
            Style = ResourceDictionaryHelper.InAppNotificationResourceDict["InAppNotificationStyle"] as Style;
            XamlRoot = Program.ApplicationRoot.MainWindow.Content.XamlRoot;
            popup.RequestedTheme = Program.ApplicationRoot.MainWindow.ViewModel.WindowTheme;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            gridRoot = GetTemplateChild("GridRoot") as Grid;
            gridRoot.Loaded += NotificationLoaded;
            gridRoot.Unloaded += NotificationUnLoaded;

            popupIn = GetTemplateChild("PopupIn") as Storyboard;
            popupIn.Completed += PopupInCompleted;

            popupOut = GetTemplateChild("PopupOut") as Storyboard;
            popupOut.Completed += PopupOutCompleted;
        }

        /// <summary>
        /// 控件加载完成后显示动画，动态设置控件位置
        /// </summary>
        public void NotificationLoaded(object sender, RoutedEventArgs args)
        {
            popupIn.Begin();
            XamlRoot.Changed += NotificationPlaceChanged;
            Program.ApplicationRoot.MainWindow.ViewModel.PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// 控件卸载时移除相应的事件
        /// </summary>
        private void NotificationUnLoaded(object sender, RoutedEventArgs args)
        {
            XamlRoot.Changed -= NotificationPlaceChanged;
            Program.ApplicationRoot.MainWindow.ViewModel.PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            popup.RequestedTheme = Program.ApplicationRoot.MainWindow.ViewModel.WindowTheme;
        }

        /// <summary>
        /// 窗口大小调整时修改应用内通知的相对位置
        /// </summary>
        private void NotificationPlaceChanged(XamlRoot sender, XamlRootChangedEventArgs args)
        {
            SetPopUpPlacement();
        }

        /// <summary>
        /// 应用内通知加载动画演示完成时发生
        /// </summary>
        private async void PopupInCompleted(object sender, object args)
        {
            await Task.Delay(Duration);
            popupOut.Begin();
        }

        /// <summary>
        /// 应用内通知加载动画卸载完成时发生，关闭控件
        /// </summary>
        public void PopupOutCompleted(object sender, object args)
        {
            gridRoot.Loaded -= NotificationLoaded;
            gridRoot.Unloaded -= NotificationUnLoaded;
            popupIn.Completed -= PopupInCompleted;
            popupOut.Completed -= PopupOutCompleted;
            popup.IsOpen = false;
        }

        /// <summary>
        /// 显示应用内通知
        /// </summary>
        public void Show()
        {
            SetPopUpPlacement();
            popup.XamlRoot = XamlRoot;
            popup.Child = this;
            popup.IsOpen = true;
        }

        /// <summary>
        /// 设置应用内通知的显示位置
        /// </summary>
        private void SetPopUpPlacement()
        {
            Width = XamlRoot.Size.Width;
            Height = XamlRoot.Size.Height;

            popup.VerticalOffset = VerticalOffset;
            popup.HorizontalOffset = HorizontalOffset;
        }
    }
}
