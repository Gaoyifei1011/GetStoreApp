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
    internal sealed partial class WinGetAppsVersionDialog : ContentDialog
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AppVersionInformationString = ResourceService.GetLocalized("Dialog/AppVersionInformation");
        private readonly string WinGetAppsDownloadOptionString = ResourceService.GetLocalized("Dialog/WinGetAppsDownloadOption");
        private readonly string WinGetAppsInstallOptionString = ResourceService.GetLocalized("Dialog/WinGetAppsInstallOption");
        private readonly string WinGetAppsRepairOptionString = ResourceService.GetLocalized("Dialog/WinGetAppsRepairOption");
        private readonly string WinGetAppsUpgradeOptionString = ResourceService.GetLocalized("Dialog/WinGetAppsUpgradeOption");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private WinGetPage WinGetPage { get; }

        private object WinGetApps { get; }

        internal List<Type> PageList { get; } = [typeof(WinGetAppsVersionInfoPage), typeof(WinGetAppsVersionOptionsPage)];

        private ObservableCollection<ContentLinkInfo> BreadCollection { get; } = [];

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal WinGetAppsVersionDialog(WinGetOperationKind winGetOptionKind, WinGetPage winGetPage, object winGetApps)
        {
            InitializeComponent();
            WinGetPage = winGetPage;
            WinGetApps = winGetApps;
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 打开内容对话框时触发的事件
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
                                BreadCollection.Add(new()
                                {
                                    DisplayText = WinGetAppsDownloadOptionString,
                                    SecondaryText = "DownloadOption"
                                });
                                break;
                            }
                        case PackageOperationKind.Install:
                            {
                                BreadCollection.Add(new()
                                {
                                    DisplayText = WinGetAppsInstallOptionString,
                                    SecondaryText = "InstallOption"
                                });
                                break;
                            }
                        case PackageOperationKind.Repair:
                            {
                                BreadCollection.Add(new()
                                {
                                    DisplayText = WinGetAppsRepairOptionString,
                                    SecondaryText = "RepairOption"
                                });
                                break;
                            }
                        case PackageOperationKind.Upgrade:
                            {
                                BreadCollection.Add(new()
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
        /// 导航失败后发生的事件
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

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 页面向前导航
        /// </summary>
        internal void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                WinGetAppsVersionFrame.ContentTransitions = slideDirection.HasValue ? slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection : SuppressNavigationTransitionCollection;

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

        #endregion 第五部分：数据操作与业务逻辑
    }
}
