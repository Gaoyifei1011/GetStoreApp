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

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 主页面
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private List<ControlItemModel> HomeList { get; } =
        [
            new ControlItemModel()
            {
                Title = ResourceService.GetLocalized("Home/Store"),
                Description = ResourceService.GetLocalized("Home/StoreDescription"),
                ImagePath = "ms-appx:///Assets/Icon/Control/Store.png",
                Tag = "Store"
            },
            new ControlItemModel()
            {
                Title = ResourceService.GetLocalized("Home/AppUpdate"),
                Description = ResourceService.GetLocalized("Home/AppUpdateDescription"),
                ImagePath = "ms-appx:///Assets/Icon/Control/AppUpdate.png",
                Tag = "AppUpdate"
            },
            new ControlItemModel()
            {
                Title = ResourceService.GetLocalized("Home/WinGet"),
                Description = ResourceService.GetLocalized("Home/WinGetDescription"),
                ImagePath = "ms-appx:///Assets/Icon/Control/WinGet.png",
                Tag = "WinGet"
            },
            new ControlItemModel()
            {
                Title = ResourceService.GetLocalized("Home/AppManager"),
                Description = ResourceService.GetLocalized("Home/AppManagerDescription"),
                ImagePath = "ms-appx:///Assets/Icon/Control/AppManager.png",
                Tag = "AppManager"
            },
            new ControlItemModel()
            {
                Title = ResourceService.GetLocalized("Home/Download"),
                Description = ResourceService.GetLocalized("Home/DownloadDescription"),
                ImagePath = "ms-appx:///Assets/Icon/Control/Download.png",
                Tag = "Download"
            },
            new ControlItemModel()
            {
                Title = ResourceService.GetLocalized("Home/Web"),
                Description = ResourceService.GetLocalized("Home/WebDescription"),
                ImagePath = "ms-appx:///Assets/Icon/Control/Web.png",
                Tag = "Web"
            },
        ];

        public HomePage()
        {
            InitializeComponent();
        }

        #region 第一部分：主页面——挂载的事件

        /// <summary>
        /// 点击条目时进入条目对应的页面
        /// </summary>
        private void OnControlItemClickExcecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is ControlItemModel controlItem)
            {
                if (MainWindow.Current.PageList[MainWindow.Current.PageList.FindIndex(item => string.Equals(item.Key, controlItem.Tag))].Key is "Web")
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await Launcher.LaunchUriAsync(new Uri("getstoreappwebbrowser:"));
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                    });
                }
                else
                {
                    MainWindow.Current.NavigateTo(MainWindow.Current.PageList.Find(item => string.Equals(item.Key, Convert.ToString(controlItem.Tag))).Value);
                }
            }
        }

        #endregion 第一部分：主页面——挂载的事件
    }
}
