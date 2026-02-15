using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
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
                NavigateTo(PageList[0], args.Parameter);
            }
            else
            {
                if (args.Parameter is List<string> dataList)
                {
                    InitializeQueryLinksContent(dataList);
                }

                if (StoreFrame.Content is SearchStorePage searchStorePage)
                {
                    if (Equals(SearchAppsModeService.SearchAppsMode, SearchAppsModeService.SearchAppsModeList[0]))
                    {
                        searchStorePage.UseSearchType = false;
                    }
                    else if (Equals(SearchAppsModeService.SearchAppsMode, SearchAppsModeService.SearchAppsModeList[1]))
                    {
                        searchStorePage.UseSearchType = true;
                    }
                    else
                    {
                        searchStorePage.UseSearchType = false;
                    }
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：应用商店页面——挂载的事件

        /// <summary>
        /// 点击选择器栏选中项发生变化时发生的事件
        /// </summary>
        private void OnSelectorBarSelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectedItem = sender.SelectedItem;
            int index = sender.Items.IndexOf(SelectedItem);
            Type currentPage = GetCurrentPageType();
            int currentIndex = PageList.FindIndex(item => Equals(item, currentPage));

            if (index is 0)
            {
                if (currentPage is null)
                {
                    NavigateTo(PageList[0]);
                }
                else if (!Equals(currentPage, PageList[0]))
                {
                    NavigateTo(PageList[0], null, index > currentIndex);
                }
            }
            else if (index is 1 && !Equals(GetCurrentPageType(), PageList[1]))
            {
                NavigateTo(PageList[1], null, index > currentIndex);
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
                SelectedItem = StoreSelectorBar.Items[index];
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
                SelectedItem = StoreSelectorBar.Items[index];
            }

            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(StorePage), nameof(OnNavigationFailed), 1, args.Exception);
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
        /// 打开下载设置
        /// </summary>
        private async void OnDownloadSettingsClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            StoreSplitView.IsPaneOpen = false;
            await Task.Delay(300);
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.Download);
        }

        #endregion 第二部分：应用商店页面——挂载的事件

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
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(StorePage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return StoreFrame.CurrentSourcePageType;
        }

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
