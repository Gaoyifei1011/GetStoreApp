using GetStoreApp.Helpers.Converters;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly Guid CLSID_OpenControlPanel = new("06622D85-6856-4460-8DE1-A81921B41C4B");
        private IOpenControlPanel openControlPanel;

        public readonly Lock installStateLock = new();

        internal Dictionary<string, CancellationTokenSource> InstallingStateDict { get; } = [];

        public ObservableCollection<InstallingAppsModel> InstallingAppsCollection { get; } = [];

        public unsafe WinGetPage()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                int createResult = Ole32Library.CoCreateInstance(CLSID_OpenControlPanel, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER | CLSCTX.CLSCTX_INPROC_HANDLER | CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_REMOTE_SERVER, typeof(IOpenControlPanel).GUID, out IntPtr ppv);

                if (createResult is 0)
                {
                    openControlPanel = ComInterfaceMarshaller<IOpenControlPanel>.ConvertToManaged((void*)ppv);
                }
            });
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
                        if (InstallingStateDict.TryGetValue(appId, out CancellationTokenSource tokenSource))
                        {
                            if (!tokenSource.IsCancellationRequested)
                            {
                                tokenSource.Cancel();
                                tokenSource.Dispose();
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
        /// 分割控件选中项发生改变时引发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectorBarSelectionChangedEventArgs args)
        {
            if (sender is SelectorBar selectorBar && selectorBar.SelectedItem is not null)
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
            if (ValueCheckConverterHelper.IsWinGetExisted(WinGetService.IsOfficialVersionExisted, WinGetService.IsDevVersionExisted, false))
            {
                SearchApps.InitializeWingetInstance(this);
                UpgradableApps.InitializeWingetInstance(this);
            }

            if (WinGetSelectorBar is not null)
            {
                WinGetSelectorBar.SelectedItem = WinGetSelectorBar.Items[0];
            }
        }

        /// <summary>
        /// 点击关闭按钮关闭任务管理
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            TaskManagerFlyout.Hide();
        }

        /// <summary>
        /// 打开控制面板的程序与功能
        /// </summary>
        private void OnControlPanelClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                openControlPanel?.Open("Microsoft.ProgramsAndFeatures", null, IntPtr.Zero);
            });
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
    }
}
