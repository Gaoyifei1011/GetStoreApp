using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Windows.System;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 程序包页面
    /// </summary>
    public sealed partial class WinGetPage : Page
    {
        public readonly object installingAppsObject = new();

        private bool isInitialized;

        public Dictionary<string, CancellationTokenSource> InstallingStateDict { get; } = [];

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
            string appId = args.Parameter as string;
            if (appId is not null)
            {
                lock (installingAppsObject)
                {
                    foreach (InstallingAppsModel installingAppsItem in InstallingAppsCollection)
                    {
                        if (installingAppsItem.AppID == appId)
                        {
                            installingAppsItem.IsCanceling = true;
                        }
                    }

                    if (InstallingStateDict.TryGetValue(appId, out CancellationTokenSource tokenSource))
                    {
                        if (!tokenSource.IsCancellationRequested)
                        {
                            tokenSource.Cancel();
                            tokenSource.Dispose();
                        }
                    }
                }
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：WinGet 程序包页面——挂载的事件

        /// <summary>
        /// 分割控件选中项发生改变时引发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBar selectorBar = sender as SelectorBar;

            if (selectorBar is not null && selectorBar.SelectedItem is not null)
            {
                int selectedIndex = selectorBar.Items.IndexOf(selectorBar.SelectedItem);

                if (selectedIndex is 0)
                {
                    SearchApps.Visibility = Visibility.Visible;
                    InstalledApps.Visibility = Visibility.Collapsed;
                    UpgradableApps.Visibility = Visibility.Collapsed;
                    ControlPanel.Visibility = Visibility.Collapsed;
                }
                else if (selectedIndex is 1)
                {
                    SearchApps.Visibility = Visibility.Collapsed;
                    InstalledApps.Visibility = Visibility.Visible;
                    UpgradableApps.Visibility = Visibility.Collapsed;
                    ControlPanel.Visibility = Visibility.Visible;
                }
                else if (selectedIndex is 2)
                {
                    SearchApps.Visibility = Visibility.Collapsed;
                    InstalledApps.Visibility = Visibility.Collapsed;
                    UpgradableApps.Visibility = Visibility.Visible;
                    ControlPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 初始化 WinGet 程序包页面
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (IsWinGetExisted(WinGetService.IsOfficialVersionExisted, WinGetService.IsDevVersionExisted, false))
            {
                if (!isInitialized)
                {
                    SearchApps.InitializeWingetInstance(this);
                    UpgradableApps.InitializeWingetInstance(this);
                    isInitialized = true;
                }
            }

            if (WinGetSelectorBar is not null)
            {
                WinGetSelectorBar.SelectedItem = WinGetSelectorBar.Items[0];
            }
        }

        /// <summary>
        /// 打开控制面板的程序与功能
        /// </summary>
        private void OnControlPanelClicked(object sender, RoutedEventArgs args)
        {
            ProcessHelper.StartProcess("control.exe", "appwiz.cpl", out _);
        }

        /// <summary>
        /// 了解更多有关 WinGet 程序包的描述信息
        /// </summary>
        private async void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri(@"https://learn.microsoft.com/windows/package-manager/"));
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

        /// <summary>
        /// 判断 WinGet 程序包是否存在
        /// </summary>
        private bool IsWinGetExisted(bool isOfficialVersionExisted, bool isDevVersionExisted, bool needReverseValue)
        {
            bool result = isOfficialVersionExisted || isDevVersionExisted;
            if (needReverseValue)
            {
                return !result;
            }
            else
            {
                return result;
            }
        }
    }
}
