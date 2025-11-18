using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
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

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// WinGet 应用版本信息对话框
    /// </summary>
    public sealed partial class WinGetAppsVersionDialog : ContentDialog
    {
        private readonly string AppVersionInformationString = ResourceService.GetLocalized("Dialog/AppVersionInformation");
        private readonly string WinGetAppsDownloadOptionString = ResourceService.GetLocalized("Dialog/WinGetAppsDownloadOption");
        private readonly string WinGetAppsInstallOptionString = ResourceService.GetLocalized("Dialog/WinGetAppsInstallOption");
        private readonly string WinGetAppsRepairOptionString = ResourceService.GetLocalized("Dialog/WinGetAppsRepairOption");
        private readonly string WinGetAppsUpgradeOptionString = ResourceService.GetLocalized("Dialog/WinGetAppsUpgradeOption");

        private WinGetPage WinGetPage { get; }

        private object WinGetApps { get; }

        public List<Type> PageList { get; } = [typeof(WinGetAppsVersionInfoPage), typeof(WinGetAppsVersionOptionsPage)];

        private ObservableCollection<ContentLinkInfo> BreadCollection { get; } = [];

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
        private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            WinGetAppsVersionFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                NavigateTo(PageList[0], new List<object>() { WinGetPage, this, WinGetApps });
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
        private void OnItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            if (args.Item.As<ContentLinkInfo>() is ContentLinkInfo contentLinkInfo && BreadCollection.Count is 2 && string.Equals(contentLinkInfo.SecondaryText, BreadCollection[0].SecondaryText))
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
                    DisplayText = AppVersionInformationString,
                    SecondaryText = "AppVersionInformation"
                });
            }
            else if (BreadCollection.Count is 1 && Equals(GetCurrentPageType(), PageList[1]))
            {
                if (args.Parameter is List<object> argsList && argsList.Count is 3 && argsList[2] is PackageOperationModel packageOperation)
                {
                    switch (packageOperation.PackageOperationKind)
                    {
                        case PackageOperationKind.Download:
                            {
                                BreadCollection.Add(new ContentLinkInfo()
                                {
                                    DisplayText = WinGetAppsDownloadOptionString,
                                    SecondaryText = "DownloadOption"
                                });
                                break;
                            }
                        case PackageOperationKind.Install:
                            {
                                BreadCollection.Add(new ContentLinkInfo()
                                {
                                    DisplayText = WinGetAppsInstallOptionString,
                                    SecondaryText = "InstallOption"
                                });
                                break;
                            }
                        case PackageOperationKind.Repair:
                            {
                                BreadCollection.Add(new ContentLinkInfo()
                                {
                                    DisplayText = WinGetAppsRepairOptionString,
                                    SecondaryText = "RepairOption"
                                });
                                break;
                            }
                        case PackageOperationKind.Upgrade:
                            {
                                BreadCollection.Add(new ContentLinkInfo()
                                {
                                    DisplayText = WinGetAppsUpgradeOptionString,
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
            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetAppsVersionDialog), nameof(OnNavigated), 1, args.Exception);
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
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetAppsVersionDialog), nameof(NavigateTo), 1, e);
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
        private Type GetCurrentPageType()
        {
            return WinGetAppsVersionFrame.CurrentSourcePageType;
        }
    }
}
