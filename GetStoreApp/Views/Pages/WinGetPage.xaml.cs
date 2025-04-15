using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 程序包页面
    /// </summary>
    public sealed partial class WinGetPage : Page
    {
        private bool isInitialized;
        public readonly Lock installStateLock = new();

        internal Dictionary<string, IAsyncInfo> InstallingStateDict { get; } = [];

        public ObservableCollection<InstallingAppsModel> InstallingAppsCollection { get; } = [];

        public WinGetPage()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 取消下载
        /// </summary>
        private void OnCancelInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appId && !string.IsNullOrEmpty(appId))
            {
                foreach (InstallingAppsModel installingAppsItem in InstallingAppsCollection)
                {
                    if (installingAppsItem.AppID.Equals(appId))
                    {
                        installingAppsItem.IsCanceling = true;
                    }
                }

                Task.Run(() =>
                {
                    installStateLock.Enter();

                    try
                    {
                        if (InstallingStateDict.TryGetValue(appId, out IAsyncInfo asyncInfo))
                        {
                            if (asyncInfo.Status is not AsyncStatus.Canceled)
                            {
                                asyncInfo.Cancel();
                                asyncInfo.Close();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Cancel winget download task failed", e);
                    }
                    finally
                    {
                        installStateLock.Exit();
                    }
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：WinGet 程序包页面——挂载的事件

        /// <summary>
        /// 初始化 WinGet 程序包页面
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                if (WinGetConfigService.IsWinGetInstalled)
                {
                    SearchApps.InitializeWingetInstance(this);
                    UpgradableApps.InitializeWingetInstance(this);
                }

                if (WinGetSelectorBar is not null)
                {
                    WinGetSelectorBar.SelectedItem = WinGetSelectorBar.Items[0];
                }
            }

            isInitialized = true;
        }

        /// <summary>
        /// 点击关闭按钮关闭任务管理
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            TaskManagerFlyout.Hide();
        }

        /// <summary>
        /// 了解更多有关 WinGet 程序包的描述信息
        /// </summary>
        private async void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri(@"https://learn.microsoft.com/windows/package-manager"));
        }

        /// <summary>
        /// 从微软商店中下载 WinGet 程序包管理器
        /// </summary>
        private async void OnDownloadFromMicrosoftStoreClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/ProductId=9NBLGGH4NNS1"));
        }

        /// <summary>
        /// 从Github中下载 WinGet 程序包管理器
        /// </summary>
        private async void OnDownloadFromGithubClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/winget-cli/releases"));
        }

        #endregion 第二部分：WinGet 程序包页面——挂载的事件

        private Visibility GetSelectedItem(SelectorBarItem selectorBarItem, int index)
        {
            return WinGetSelectorBar is not null ? WinGetSelectorBar.Items.IndexOf(selectorBarItem).Equals(index) ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
        }
    }
}
