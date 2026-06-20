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
using Windows.Foundation.Collections;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.UserControls
{
    /// <summary>
    /// 搜索应用结果用户控件
    /// </summary>
    public sealed partial class SearchAppsResultUserControl : UserControl
    {
        private readonly string SearchAppsResultCountInfoString = ResourceService.GetLocalized("SearchAppsResult/SearchAppsResultCountInfo");
        private bool isInitialized;
        private StorePage storePage;

        private ObservableCollection<SearchAppsResultModel> SearchAppsResultCollection { get; } = [];

        public SearchAppsResultUserControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

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
                Task.Run(async () =>
                {
                    try
                    {
                        if (Equals(AppLinkOpenModeService.AppLinkOpenMode, AppLinkOpenModeService.AppLinkOpenModeList[0]))
                        {
                            await Launcher.LaunchUriAsync(new Uri("getstoreappwebview:"), new LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
                            {
                                {"AppLink", appLink },
                            });
                        }
                        else if (Equals(AppLinkOpenModeService.AppLinkOpenMode, AppLinkOpenModeService.AppLinkOpenModeList[1]))
                        {
                            await Launcher.LaunchUriAsync(new Uri(appLink));
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                });
            }
        }

        /// <summary>
        /// 查询指定应用及其依赖的下载链接
        /// </summary>
        private void OnQueryLinksExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            //if (args.Parameter is string appLink && MainWindow.Current.GetFrameContent() is StorePage storePage && !Equals(storePage.GetCurrentPageType(), storePage.PageList[0]))
            //{
            //    storePage.NavigateTo(storePage.PageList[0], new List<string> { "0", null, appLink }, false);
            //}
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：搜索应用结果用户控件——挂载的事件

        /// <summary>
        /// 返回主页
        /// </summary>
        private void OnBackToHomePageClicked(object sender, RoutedEventArgs args)
        {
            storePage.StoreControl = StoreControl.StoreSelector;
        }

        #endregion 第二部分：搜索应用结果用户控件——挂载的事件

        /// <summary>
        /// 初始化搜索应用结果用户控件
        /// </summary>
        public void InitializeSearchAppsResult(StorePage storePageData)
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
        public void UpdateSearchAppsResultData(List<SearchAppsResultModel> searchAppsResultList)
        {
            SearchAppsResultCollection.Clear();
            foreach (SearchAppsResultModel searchAppsResultItem in searchAppsResultList)
            {
                SearchAppsResultCollection.Add(searchAppsResultItem);
            }
        }
    }
}
