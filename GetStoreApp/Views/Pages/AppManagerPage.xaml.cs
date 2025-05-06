using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using Windows.Foundation.Diagnostics;
using Windows.UI.Text;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 应用管理页面
    /// </summary>
    public sealed partial class AppManagerPage : Page
    {
        public ObservableCollection<ContentLinkInfo> BreadCollection { get; } = [];

        public AppManagerPage()
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
            AppManagerFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                NavigateTo(typeof(AppListPage), null, null);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：应用管理页面——挂载的事件

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        private void OnItemClicked(object sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is ContentLinkInfo breadItem && BreadCollection.Count is 2 && Equals(breadItem.SecondaryText, BreadCollection[0].SecondaryText))
            {
                NavigateTo(typeof(AppListPage), null, false);
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            if (BreadCollection.Count is 0 && Equals(GetCurrentPageType(), typeof(AppListPage)))
            {
                BreadCollection.Add(new ContentLinkInfo()
                {
                    DisplayText = ResourceService.GetLocalized("AppManager/AppList"),
                    SecondaryText = "AppList"
                });
            }
            if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), typeof(AppInformationPage)))
            {
                BreadCollection.Add(new ContentLinkInfo()
                {
                    DisplayText = ResourceService.GetLocalized("AppManager/AppInformation"),
                    SecondaryText = "AppInformation"
                });
            }
            else if (BreadCollection.Count is 2 && Equals(GetCurrentPageType(), typeof(AppListPage)))
            {
                BreadCollection.RemoveAt(1);
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Warning, string.Format(ResourceService.GetLocalized("AppManager/NavigationFailed"), args.SourcePageType.FullName), args.Exception);
        }

        #endregion 第二部分：应用管理页面——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    AppManagerFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                AppManagerFrame.Navigate(navigationPageType, parameter);
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
            return AppManagerFrame.CurrentSourcePageType;
        }
    }
}
