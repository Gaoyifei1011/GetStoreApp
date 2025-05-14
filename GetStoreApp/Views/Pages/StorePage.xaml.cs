using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 微软商店页面
    /// </summary>
    public sealed partial class StorePage : Page, INotifyPropertyChanged
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

        public List<Type> PageList { get; } = [typeof(QueryLinksPage), typeof(SearchStorePage)];

        public event PropertyChangedEventHandler PropertyChanged;

        public StorePage()
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
            StoreFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                NavigateTo(PageList[0], args.Parameter, null);
            }
            else
            {
                if (args.Parameter is List<string> dataList)
                {
                    InitializeQueryLinksContent(dataList);
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：应用商店页面——挂载的事件

        /// <summary>
        /// 点击选择器栏发生的事件
        /// </summary>
        private void OnSelectorBarTapped(object sender, TappedRoutedEventArgs args)
        {
            if (sender is SelectorBarItem selectorBarItem && selectorBarItem.Tag is string tag)
            {
                int index = Convert.ToInt32(tag);
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
        /// 打开设置中的语言和区域
        /// </summary>
        private void OnLanguageAndRegionClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:regionformatting"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 了解应用具体的使用说明
        /// </summary>
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (!StoreSplitView.IsPaneOpen)
            {
                StoreSplitView.IsPaneOpen = true;
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < StoreSelectorBar.Items.Count)
            {
                SelectedItem = StoreSelectorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < StoreSelectorBar.Items.Count)
            {
                SelectedItem = StoreSelectorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }

            LogService.WriteLog(LoggingLevel.Warning, string.Format(ResourceService.GetLocalized("Store/NavigationFailed"), args.SourcePageType.FullName), args.Exception);
        }

        /// <summary>
        /// 关闭使用说明浮出栏
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (StoreSplitView.IsPaneOpen)
            {
                StoreSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// 桌面程序启动参数说明
        /// </summary>
        private async void OnDesktopLaunchClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            await MainWindow.Current.ShowDialogAsync(new DesktopStartupArgsDialog());
        }

        /// <summary>
        /// 控制台程序启动参数说明
        /// </summary>
        private async void OnConsoleLaunchClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            await MainWindow.Current.ShowDialogAsync(new ConsoleStartupArgsDialog());
        }

        /// <summary>
        /// 检查网络
        /// </summary>
        private void OnCheckNetWorkClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        private void OnTroubleShootClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 了解更多下载管理说明
        /// </summary>
        private async void OnLearnDownloadMoreClicked(object sender, RoutedEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            await Task.Delay(300);
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);

            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                settingsPage.ShowSettingsInstruction();
            }
        }

        /// <summary>
        /// 打开应用“下载设置”
        /// </summary>
        private async void OnOpenDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            await Task.Delay(300);
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }

        /// <summary>
        /// 打开下载设置
        /// </summary>
        private async void OnDownloadSettingsClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            await Task.Delay(300);
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }

        #endregion 第二部分：应用商店页面——挂载的事件

        #region 第三部分：应用商店页面——窗口导航方法

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    StoreFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                StoreFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, string.Format(ResourceService.GetLocalized("Store/NavigationFailed"), navigationPageType.FullName), e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return StoreFrame.CurrentSourcePageType;
        }

        #endregion 第三部分：应用商店页面——窗口导航方法

        /// <summary>
        /// 初始化查询链接内容
        /// </summary>

        public void InitializeQueryLinksContent(List<string> dataList)
        {
            if (!Equals(GetCurrentPageType(), PageList[0]))
            {
                NavigateTo(PageList[0], dataList, false);
            }
            else
            {
                if (StoreFrame.Content is QueryLinksPage queryLinksPage && dataList.Count is 3)
                {
                    queryLinksPage.SelectedType = Convert.ToInt32(dataList[0]) is -1 ? queryLinksPage.TypeList[0] : queryLinksPage.TypeList[Convert.ToInt32(dataList[0])];
                    queryLinksPage.SelectedChannel = Convert.ToInt32(dataList[1]) is -1 ? queryLinksPage.ChannelList[3] : queryLinksPage.ChannelList[Convert.ToInt32(dataList[1])];
                    queryLinksPage.LinkText = dataList[2] is "PlaceHolderText" ? string.Empty : dataList[2];
                }
            }
        }
    }
}
