using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation.Diagnostics;
using Windows.UI.Text;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// WinGet 应用版本信息对话框
    /// </summary>
    public sealed partial class WinGetAppsVersionDialog : ContentDialog
    {
        private WinGetPage WinGetPage { get; }

        private object WinGetApps { get; }

        public List<Type> PageList { get; } = [typeof(WinGetAppsVersionInfoPage), typeof(WinGetAppsVersionOptionsPage)];

        public ObservableCollection<ContentLinkInfo> BreadCollection { get; } = [];

        public WinGetAppsVersionDialog(WinGetOperationKind winGetOptionKind, WinGetPage winGetPage, object winGetApps)
        {
            InitializeComponent();
            WinGetPage = winGetPage;
            WinGetApps = winGetApps;
        }

        #region 第一部分：WinGet 应用版本信息对话框——挂载的事件

        /// <summary>
        /// 打开对话框时触发的事件
        /// </summary>
        private void OnOpened(object sender, ContentDialogOpenedEventArgs args)
        {
            WinGetAppsVersionFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (WinGetConfigService.IsWinGetInstalled && GetCurrentPageType() is null)
            {
                NavigateTo(PageList[0], new List<object>() { WinGetPage, this, WinGetApps }, null);
            }
        }

        /// <summary>
        /// 点击返回到应用版本信息页面
        /// </summary>
        private void OnBackClicked(object sender, RoutedEventArgs args)
        {
            if (BreadCollection.Count is 2 && Equals(GetCurrentPageType(), PageList[1]))
            {
                NavigateTo(PageList[0], null, false);
            }
        }

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        private void OnItemClicked(object sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item is ContentLinkInfo contentLinkInfo && BreadCollection.Count is 2 && Equals(contentLinkInfo.SecondaryText, BreadCollection[0].SecondaryText))
            {
                NavigateTo(PageList[0], null, false);
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            if (BreadCollection.Count is 0 && Equals(GetCurrentPageType(), PageList[0]))
            {
                BreadCollection.Add(new ContentLinkInfo()
                {
                    DisplayText = ResourceService.GetLocalized("Dialog/AppVersionInformation"),
                    SecondaryText = "AppVersionInformation"
                });
            }
            if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), PageList[1]))
            {
                if (args.Parameter is List<object> argsList && argsList.Count is 3 && argsList[2] is PackageOperationModel packageOperation)
                {
                    switch (packageOperation.PackageOperationKind)
                    {
                        case PackageOperationKind.Download:
                            {
                                BreadCollection.Add(new ContentLinkInfo()
                                {
                                    DisplayText = ResourceService.GetLocalized("Dialog/WinGetAppsDownloadOption"),
                                    SecondaryText = "DownloadOption"
                                });
                                break;
                            }
                        case PackageOperationKind.Install:
                            {
                                BreadCollection.Add(new ContentLinkInfo()
                                {
                                    DisplayText = ResourceService.GetLocalized("Dialog/WinGetAppsInstallOption"),
                                    SecondaryText = "InstallOption"
                                });
                                break;
                            }
                        case PackageOperationKind.Repair:
                            {
                                BreadCollection.Add(new ContentLinkInfo()
                                {
                                    DisplayText = ResourceService.GetLocalized("Dialog/WinGetAppsRepairOption"),
                                    SecondaryText = "RepairOption"
                                });
                                break;
                            }
                        case PackageOperationKind.Upgrade:
                            {
                                BreadCollection.Add(new ContentLinkInfo()
                                {
                                    DisplayText = ResourceService.GetLocalized("Dialog/WinGetAppsUpgradeOption"),
                                    SecondaryText = "UpgradeOption"
                                });
                                break;
                            }
                    }
                }
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
            LogService.WriteLog(LoggingLevel.Warning, string.Format(ResourceService.GetLocalized("Dialog/NavigationFailed"), args.SourcePageType.FullName), args.Exception);
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Hide();
        }

        #endregion 第一部分：WinGet 应用版本信息对话框——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    WinGetAppsVersionFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                WinGetAppsVersionFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, string.Format(ResourceService.GetLocalized("Window/NavigationFailed"), navigationPageType.FullName), e);
            }
        }

        /// <summary>
        /// 获取能否返回到应用版本信息页面
        /// </summary>
        private bool GetCanBack(int count)
        {
            return count > 1;
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return WinGetAppsVersionFrame.CurrentSourcePageType;
        }
    }
}
