using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 下载页面
    /// </summary>
    public sealed partial class DownloadPage : Page, INotifyPropertyChanged
    {
        private SelectorBarItem _selectedItem;

        public SelectorBarItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        public List<Type> PageList { get; } = [typeof(DownloadingPage), typeof(CompletedPage)];

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            DownloadFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                NavigateTo(PageList[0], args.Parameter, null);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：下载页面——挂载的事件

        /// <summary>
        /// 点击选择器栏发生的事件
        /// </summary>
        private void OnSelectorBarTapped(object sender, TappedRoutedEventArgs args)
        {
            if (sender is SelectorBarItem selectorBarItem && selectorBarItem.Tag is Type pageType)
            {
                int index = PageList.IndexOf(pageType);
                int currentIndex = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

                if (index is 0 && !Equals(GetCurrentPageType(), PageList[0]))
                {
                    NavigateTo(PageList[0], null, index > currentIndex);
                }
                else if (index is 1 && !Equals(GetCurrentPageType(), PageList[1]))
                {
                    NavigateTo(PageList[1], null, index > currentIndex);
                }
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < DownloadSelctorBar.Items.Count)
            {
                SelectedItem = DownloadSelctorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < DownloadSelctorBar.Items.Count)
            {
                SelectedItem = DownloadSelctorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }

            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadPage), nameof(OnNavigationFailed), 1, args.Exception);
        }

        /// <summary>
        /// 关闭使用说明浮出栏
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (DownloadSplitView.IsPaneOpen)
            {
                DownloadSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// 了解更多下载管理说明
        /// </summary>
        private async void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            DownloadSplitView.IsPaneOpen = false;
            await Task.Delay(300);
            MainWindow.Current.NavigateTo(typeof(SettingsPage));

            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                settingsPage.ShowSettingsInstruction();
            }
        }

        /// <summary>
        /// 打开网络和 Internet 设置
        /// </summary>
        private void OnNetworkInternetClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:network-status"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开应用“下载设置”
        /// </summary>
        private async void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            DownloadSplitView.IsPaneOpen = false;
            await Task.Delay(300);
            MainWindow.Current.NavigateTo(typeof(SettingsPage));
        }

        #endregion 第二部分：下载页面——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        private void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    DownloadFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                DownloadFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadPage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        private Type GetCurrentPageType()
        {
            return DownloadFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 显示下载使用说明
        /// </summary>
        public void ShowUseInstruction()
        {
            if (!DownloadSplitView.IsPaneOpen)
            {
                DownloadSplitView.IsPaneOpen = true;
            }
        }
    }
}
