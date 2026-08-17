using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation.Diagnostics;
using Windows.UI.Text;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 应用管理页面
    /// </summary>
    internal sealed partial class AppManagerPage : Page
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AppListString = ResourceService.GetLocalized("AppManager/AppList");
        private readonly string AppInformationString = ResourceService.GetLocalized("AppManager/AppInformation");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        internal List<Type> PageList { get; } = [typeof(AppListPage), typeof(AppInformationPage)];

        internal ObservableCollection<ContentLinkInfo> BreadCollection { get; } = [];

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal AppManagerPage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            AppManagerFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航或者再次导航时不在应用列表页面
            if (GetCurrentPageType() is null || GetCurrentPageType() != PageList[0])
            {
                NavigateTo(PageList[0]);
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ContentLinkInfo))]
        private void OnItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is ContentLinkInfo contentLinkInfo && BreadCollection.Count is 2 && string.Equals(contentLinkInfo.SecondaryText, BreadCollection[0].SecondaryText))
            {
                NavigateTo(PageList[0], null, false);
            }
        }

        /// <summary>
        /// 导航完成后发生的事件
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            if (BreadCollection.Count is 0 && Equals(GetCurrentPageType(), PageList[0]))
            {
                BreadCollection.Add(new()
                {
                    DisplayText = AppListString,
                    SecondaryText = "AppList"
                });
            }
            else if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), PageList[1]))
            {
                BreadCollection.Add(new()
                {
                    DisplayText = AppInformationString,
                    SecondaryText = "AppInformation"
                });
            }
            else if (BreadCollection.Count is 2 && Equals(GetCurrentPageType(), PageList[0]))
            {
                BreadCollection.RemoveAt(1);
            }
        }

        /// <summary>
        /// 导航失败后发生的事件
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppManagerPage), nameof(OnNavigationFailed), 1, args.Exception);
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 页面向前导航
        /// </summary>
        internal void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                AppManagerFrame.ContentTransitions = slideDirection.HasValue ? slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection : SuppressNavigationTransitionCollection;

                // 导航到该项目对应的页面
                AppManagerFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppManagerPage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        internal Type GetCurrentPageType()
        {
            return AppManagerFrame.CurrentSourcePageType;
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
