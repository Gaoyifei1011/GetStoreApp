using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 主页面
    /// </summary>
    internal sealed partial class HomePage : Page
    {
        #region 第一部分：属性、集合与事件

        private readonly string AppManagerDescriptionString = ResourceService.GetLocalized("Home/AppManagerDescription");
        private readonly string AppManagerString = ResourceService.GetLocalized("Home/AppManager");
        private readonly string AppUpdateDescriptionString = ResourceService.GetLocalized("Home/AppUpdateDescription");
        private readonly string AppUpdateString = ResourceService.GetLocalized("Home/AppUpdate");
        private readonly string DownloadDescriptionString = ResourceService.GetLocalized("Home/DownloadDescription");
        private readonly string DownloadString = ResourceService.GetLocalized("Home/Download");
        private readonly string StoreDescriptionString = ResourceService.GetLocalized("Home/StoreDescription");
        private readonly string StoreString = ResourceService.GetLocalized("Home/Store");
        private readonly string WinGetDescriptionString = ResourceService.GetLocalized("Home/WinGetDescription");
        private readonly string WinGetString = ResourceService.GetLocalized("Home/WinGet");
        private readonly string WebDescriptionString = ResourceService.GetLocalized("Home/WebDescription");
        private readonly string WebString = ResourceService.GetLocalized("Home/Web");

        #endregion 第一部分：属性、集合与事件

        #region 第二部分：属性、集合与事件

        private List<ControlItemModel> HomeList { get; } = [];

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal HomePage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：命令调用处理

        /// <summary>
        /// 点击条目时进入条目对应的页面
        /// </summary>
        private void OnControlItemClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ControlItemModel controlItem)
            {
                if (controlItem.Tag is "Web")
                {
                    LaunchWebView();
                }
                else if (MainWindow.Current.GetSelectedItem(controlItem.NavigationPage, MainWindow.Current.NavigationViewItemMenuItemsCollection) is NavigationViewItemModel navigationViewItem)
                {
                    MainWindow.Current.NavigateTo(navigationViewItem.NavigationPage);
                }
            }
        }

        #endregion 第四部分：命令调用处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            HomeList.Add(new()
            {
                Title = StoreString,
                Description = StoreDescriptionString,
                ImagePath = "ms-appx:///Assets/Icon/Control/Store.png",
                Tag = "Store",
                NavigationPage = typeof(StorePage)
            });
            HomeList.Add(new()
            {
                Title = AppUpdateString,
                Description = AppUpdateDescriptionString,
                ImagePath = "ms-appx:///Assets/Icon/Control/AppUpdate.png",
                Tag = "AppUpdate",
                NavigationPage = typeof(AppUpdatePage)
            });
            HomeList.Add(new()
            {
                Title = WinGetString,
                Description = WinGetDescriptionString,
                ImagePath = "ms-appx:///Assets/Icon/Control/WinGet.png",
                Tag = "WinGet",
                NavigationPage = typeof(WinGetPage)
            });
            HomeList.Add(new()
            {
                Title = AppManagerString,
                Description = AppManagerDescriptionString,
                ImagePath = "ms-appx:///Assets/Icon/Control/AppManager.png",
                Tag = "AppManager",
                NavigationPage = typeof(AppManagerPage)
            });
            HomeList.Add(new()
            {
                Title = DownloadString,
                Description = DownloadDescriptionString,
                ImagePath = "ms-appx:///Assets/Icon/Control/Download.png",
                Tag = "Download",
                NavigationPage = typeof(DownloadPage)
            });
            HomeList.Add(new()
            {
                Title = WebString,
                Description = WebDescriptionString,
                ImagePath = "ms-appx:///Assets/Icon/Control/Web.png",
                Tag = "Web",
                NavigationPage = null
            });
        }

        /// <summary>
        /// 启动网页
        /// </summary>
        private void LaunchWebView()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("getstoreappwebview:"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}
