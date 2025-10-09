using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;
using Windows.UI.Text;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private readonly string SettingsString = ResourceService.GetLocalized("Settings/Settings");
        private readonly string PackageVolumeConfigurationString = ResourceService.GetLocalized("Settings/PackageVolumeConfiguration");
        private readonly string WinGetSourceConfigurationString = ResourceService.GetLocalized("Settings/WinGetSourceConfiguration");

        public List<Type> PageList { get; } = [typeof(SettingsItemPage), typeof(SettingsWinGetSourcePage), typeof(SettingsPackageVolumePage)];

        public ObservableCollection<ContentLinkInfo> BreadCollection { get; } = [];

        public SettingsPage()
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
            SettingsFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            if (args.Parameter is AppNaviagtionArgs.WinGetDataSource)
            {
                // 导航到 WinGet 数据源配置页面
                if (!Equals(GetCurrentPageType(), PageList[1]))
                {
                    NavigateTo(PageList[1]);
                }
            }
            else if (args.Parameter is AppNaviagtionArgs.PackageVolume)
            {
                // 导航到应用包存储卷配置页面
                if (!Equals(GetCurrentPageType(), PageList[2]))
                {
                    NavigateTo(PageList[2]);
                }
            }
            else if (args.Parameter is AppNaviagtionArgs.Download)
            {
                if (!Equals(GetCurrentPageType(), PageList[0]))
                {
                    NavigateTo(PageList[0], args.Parameter);
                }
                else if (GetFrameContent() is SettingsItemPage itemPage && !Equals(itemPage.GetCurrentPageType(), itemPage.PageList[3]))
                {
                    if (itemPage.IsLoaded)
                    {
                        int currentIndex = itemPage.PageList.FindIndex(item => Equals(item, itemPage.GetCurrentPageType()));
                        itemPage.NavigateTo(itemPage.PageList[3], null, 3 > currentIndex);
                    }
                    else
                    {
                        itemPage.SetNavigateContent(true, itemPage.PageList[3]);
                    }
                }
            }
            else if (args.Parameter is AppNaviagtionArgs.AppInstall)
            {
                if (!Equals(GetCurrentPageType(), PageList[0]))
                {
                    NavigateTo(PageList[0], args.Parameter);
                }
                else if (GetFrameContent() is SettingsItemPage itemPage && !Equals(itemPage.GetCurrentPageType(), itemPage.PageList[4]))
                {
                    if (itemPage.IsLoaded)
                    {
                        int currentIndex = itemPage.PageList.FindIndex(item => Equals(item, itemPage.GetCurrentPageType()));
                        itemPage.NavigateTo(itemPage.PageList[4], null, 4 > currentIndex);
                    }
                    else
                    {
                        itemPage.SetNavigateContent(true, itemPage.PageList[4]);
                    }
                }
            }
            else
            {
                // 第一次导航
                if (GetCurrentPageType() is null)
                {
                    NavigateTo(PageList[0]);
                }
            }

            if (GetFrameContent() is SettingsItemPage settingsItemPage)
            {
                settingsItemPage.ResetFrameTransition();
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：设置页面——挂载的事件

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        private void OnItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is ContentLinkInfo contentLinkInfo && BreadCollection.Count is 2 && string.Equals(contentLinkInfo.SecondaryText, BreadCollection[0].SecondaryText))
            {
                NavigateTo(PageList[0], null, false);
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            if (BreadCollection.Count is 0)
            {
                if (Equals(GetCurrentPageType(), PageList[0]))
                {
                    BreadCollection.Add(new ContentLinkInfo()
                    {
                        DisplayText = SettingsString,
                        SecondaryText = "Settings"
                    });
                }
                else if (Equals(GetCurrentPageType(), PageList[1]))
                {
                    BreadCollection.Add(new ContentLinkInfo()
                    {
                        DisplayText = SettingsString,
                        SecondaryText = "Settings"
                    });
                    BreadCollection.Add(new ContentLinkInfo()
                    {
                        DisplayText = WinGetSourceConfigurationString,
                        SecondaryText = "WinGetSourceConfiguration"
                    });
                }
                else if (Equals(GetCurrentPageType(), PageList[2]))
                {
                    BreadCollection.Add(new ContentLinkInfo()
                    {
                        DisplayText = SettingsString,
                        SecondaryText = "Settings"
                    });
                    BreadCollection.Add(new ContentLinkInfo()
                    {
                        DisplayText = PackageVolumeConfigurationString,
                        SecondaryText = "PackageVolumeConfiguration"
                    });
                }
            }

            if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), PageList[1]))
            {
                BreadCollection.Add(new ContentLinkInfo()
                {
                    DisplayText = WinGetSourceConfigurationString,
                    SecondaryText = "WinGetSourceConfiguration"
                });
            }
            else if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), PageList[2]))
            {
                BreadCollection.Add(new ContentLinkInfo()
                {
                    DisplayText = PackageVolumeConfigurationString,
                    SecondaryText = "PackageVolumeConfiguration"
                });
            }
            else if (BreadCollection.Count is 2 && Equals(GetCurrentPageType(), PageList[0]))
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
            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPage), nameof(OnNavigationFailed), 1, args.Exception);
        }

        /// <summary>
        /// 关闭使用说明浮出栏
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (SettingsSplitView.IsPaneOpen)
            {
                SettingsSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        private void OnAppSettingsClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            SettingsSplitView.IsPaneOpen = false;
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 了解加密包
        /// </summary>
        private void OnLearnEncryptedPackageClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            SettingsSplitView.IsPaneOpen = false;
        }

        /// <summary>
        /// 了解包块映射文件
        /// </summary>
        private void OnLearnBlockMapClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            SettingsSplitView.IsPaneOpen = false;
        }

        /// <summary>
        /// 了解 WinGet 配置选项
        /// </summary>
        private void OnLearnWinGetConfigClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            SettingsSplitView.IsPaneOpen = false;
        }

        /// <summary>
        /// 了解传递优化
        /// </summary>
        private void OnLearnDeliveryOptimizationClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            SettingsSplitView.IsPaneOpen = false;
        }

        /// <summary>
        /// 了解后台智能传输服务
        /// </summary>
        private void OnLearnBitsClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            SettingsSplitView.IsPaneOpen = false;
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        private void OnTroubleShootClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            SettingsSplitView.IsPaneOpen = false;
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

        #endregion 第二部分：设置页面——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    SettingsFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                SettingsFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return SettingsFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 获取当前导航控件内容对应的页面
        /// </summary>
        public object GetFrameContent()
        {
            return SettingsFrame.Content;
        }

        /// <summary>
        /// 显示设置说明
        /// </summary>
        public void ShowSettingsInstruction()
        {
            if (!SettingsSplitView.IsPaneOpen)
            {
                SettingsSplitView.IsPaneOpen = true;
            }
        }
    }
}
