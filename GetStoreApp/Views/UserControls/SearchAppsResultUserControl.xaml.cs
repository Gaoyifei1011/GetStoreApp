using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.UserControls
{
    /// <summary>
    /// 搜索应用结果用户控件
    /// </summary>
    internal sealed partial class SearchAppsResultUserControl : UserControl
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string SearchAppsResultCountInfoString = ResourceService.GetLocalized("SearchAppsResult/SearchAppsResultCountInfo");
        private bool isInitialized;
        private StorePage storePage;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private ObservableCollection<SearchAppsResultModel> SearchAppsResultCollection { get; } = [];

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal SearchAppsResultUserControl()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：命令调用处理

        /// <summary>
        /// 复制指定应用的链接
        /// </summary>
        private async void OnCopyLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string storeAppLink && !string.IsNullOrEmpty(storeAppLink))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(storeAppLink);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 打开指定项目的链接
        /// </summary>
        private void OnOpenLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appLink && !string.IsNullOrEmpty(appLink))
            {
                OpenLink(appLink);
            }
        }

        /// <summary>
        /// 查询指定应用及其依赖的下载链接
        /// </summary>
        private void OnQueryLinksExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appLink)
            {
                storePage.StoreControl = StoreControl.StoreSelector;
                storePage.StoreSelector.QueryLinksText = appLink;
            }
        }

        #endregion 第四部分：命令调用处理

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 返回主页
        /// </summary>
        private void OnBackToHomePageClicked(object sender, RoutedEventArgs args)
        {
            storePage.StoreControl = StoreControl.StoreSelector;
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化搜索应用结果用户控件
        /// </summary>
        internal void InitializeSearchAppsResult(StorePage storePageData)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                storePage = storePageData;
            }
        }

        /// <summary>
        /// 更新搜索应用结果
        /// </summary>
        internal void UpdateSearchAppsResultData(List<SearchAppsResultModel> searchAppsResultList)
        {
            SearchAppsResultCollection.Clear();
            foreach (SearchAppsResultModel searchAppsResultItem in searchAppsResultList)
            {
                SearchAppsResultCollection.Add(searchAppsResultItem);
            }
        }

        /// <summary>
        /// 打开链接
        /// </summary>
        private void OpenLink(string appLink)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (Equals(AppLinkOpenModeService.AppLinkOpenMode, AppLinkOpenModeService.AppLinkOpenModeList[0]))
                    {
                        await Launcher.LaunchUriAsync(new("getstoreappwebview:"), new() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new()
                            {
                                {"AppLink", appLink },
                            });
                    }
                    else if (Equals(AppLinkOpenModeService.AppLinkOpenMode, AppLinkOpenModeService.AppLinkOpenModeList[1]))
                    {
                        await Launcher.LaunchUriAsync(new(appLink));
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
