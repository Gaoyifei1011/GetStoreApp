using GetStoreApp.Services.Root;
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
                NavigateTo(typeof(QueryLinksPage), args.Parameter, null);
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
        /// 点击选择器栏发生的事件
        /// </summary>
        private void OnSelectorBarTapped(object sender, TappedRoutedEventArgs args)
        {
            if (sender is SelectorBarItem selectorBarItem && selectorBarItem.Tag is string tag)
            {
                int index = Convert.ToInt32(tag);
                int currentIndex = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

                if (index is 0 && !Equals(GetCurrentPageType(), typeof(QueryLinksPage)))
                {
                    NavigateTo(typeof(QueryLinksPage), null, index > currentIndex);
                }
                else if (index is 1 && !Equals(GetCurrentPageType(), typeof(SearchStorePage)))
                {
                    NavigateTo(typeof(SearchStorePage), null, index > currentIndex);
                }
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

        #endregion 第二部分：应用商店页面——挂载的事件

        #region 第三部分：窗口导航方法

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
                LogService.WriteLog(LoggingLevel.Error, string.Format(ResourceService.GetLocalized("Window/NavigationFailed"), navigationPageType.FullName), e);
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
            if (!Equals(GetCurrentPageType(), typeof(QueryLinksPage)))
            {
                NavigateTo(typeof(QueryLinksPage), dataList, false);
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

        #endregion 第三部分：窗口导航方法
    }
}
