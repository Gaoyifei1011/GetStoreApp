using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.System;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 程序包页面
    /// </summary>
    public sealed partial class WinGetPage : Page, INotifyPropertyChanged
    {
        public readonly object InstallingAppsObject = new object();

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<InstallingAppsModel> InstallingAppsList = new ObservableCollection<InstallingAppsModel>();

        public Dictionary<string, CancellationTokenSource> InstallingStateDict = new Dictionary<string, CancellationTokenSource>();

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 判断 WinGet 程序包是否存在
        /// </summary>
        public bool IsWinGetExisted(bool isOfficialVersionExisted, bool isDevVersionExisted, bool needReverseValue)
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

        /// <summary>
        /// 取消下载
        /// </summary>
        public void OnCancelInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string appId = args.Parameter as string;
            if (appId is not null)
            {
                lock (InstallingAppsObject)
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
            }
        }

        /// <summary>
        /// 初始化 WinGet 程序包
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            SearchApps.WinGetInstance = this;
            UpgradableApps.WinGetInstance = this;
        }

        /// <summary>
        /// 打开控制面板的程序与功能
        /// </summary>
        public void OnControlPanelClicked(object sender, RoutedEventArgs args)
        {
            Kernel32Library.GetStartupInfo(out STARTUPINFO WinGetProcessStartupInfo);
            WinGetProcessStartupInfo.lpReserved = IntPtr.Zero;
            WinGetProcessStartupInfo.lpDesktop = IntPtr.Zero;
            WinGetProcessStartupInfo.lpTitle = IntPtr.Zero;
            WinGetProcessStartupInfo.dwX = 0;
            WinGetProcessStartupInfo.dwY = 0;
            WinGetProcessStartupInfo.dwXSize = 0;
            WinGetProcessStartupInfo.dwYSize = 0;
            WinGetProcessStartupInfo.dwXCountChars = 500;
            WinGetProcessStartupInfo.dwYCountChars = 500;
            WinGetProcessStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
            WinGetProcessStartupInfo.wShowWindow = WindowShowStyle.SW_SHOWNORMAL;
            WinGetProcessStartupInfo.cbReserved2 = 0;
            WinGetProcessStartupInfo.lpReserved2 = IntPtr.Zero;
            WinGetProcessStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

            bool createResult = Kernel32Library.CreateProcess(null, "control.exe appwiz.cpl", IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.None, IntPtr.Zero, null, ref WinGetProcessStartupInfo, out PROCESS_INFORMATION WinGetProcessInformation);

            if (createResult)
            {
                if (WinGetProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hProcess);
                if (WinGetProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hThread);
            }
        }

        /// <summary>
        /// 了解更多有关 WinGet 程序包的描述信息
        /// </summary>
        public async void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri(@"https://learn.microsoft.com/windows/package-manager/"));
        }

        /// <summary>
        /// 从微软商店中下载 WinGet 程序包管理器
        /// </summary>
        public async void OnDownloadFromMicrosoftStoreClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/ProductId=9NBLGGH4NNS1"));
        }

        /// <summary>
        /// 从Github中下载 WinGet 程序包管理器
        /// </summary>
        public async void OnDownloadFromGithubClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/winget-cli/releases"));
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
