using GetStoreApp.Extensions.SystemTray;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Window;
using GetStoreApp.Views.Window;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace GetStoreApp
{
    public partial class WinUIApp : Application
    {
        public MainWindow MainWindow { get; private set; }

        public TrayMenuWindow TrayMenuWindow { get; private set; }

        public WindowTrayIcon TrayIcon { get; private set; }

        public JumpList TaskbarJumpList { get; private set; }

        public WinUIAppViewModel ViewModel { get; } = new WinUIAppViewModel();

        public WinUIApp()
        {
            InitializeComponent();
            UnhandledException += ViewModel.OnUnhandledException;
        }

        /// <summary>
        /// 应用启动后执行其他操作
        /// </summary>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            ResourceDictionaryHelper.InitializeResourceDictionary();

            InitializeMainWindow();
            InitializeTrayMenuWindow();
            InitializeTrayIcon();
            Program.IsAppLaunched = true;
            await ViewModel.ActivateAsync();

            await InitializeJumpListAsync();
            ViewModel.Startup();
        }

        /// <summary>
        /// 初始化应用的主窗口
        /// </summary>
        private void InitializeMainWindow()
        {
            MainWindow = new MainWindow();
            MainWindow.InitializeWindow();
        }

        /// <summary>
        /// 初始化应用的托盘右键菜单窗口
        /// </summary>
        private void InitializeTrayMenuWindow()
        {
            TrayMenuWindow = new TrayMenuWindow();
            TrayMenuWindow.InitializeWindow();
        }

        /// <summary>
        /// 初始化应用的托盘图标
        /// </summary>
        private void InitializeTrayIcon()
        {
            TrayIcon = new WindowTrayIcon(ResourceService.GetLocalized("Resources/AppDisplayName"));
            TrayIcon.DoubleClick += ViewModel.DoubleClick;
            TrayIcon.RightClick += ViewModel.RightClick;
        }

        /// <summary>
        /// 初始化任务栏的跳转列表
        /// </summary>
        private async Task InitializeJumpListAsync()
        {
            if (JumpList.IsSupported())
            {
                TaskbarJumpList = await JumpList.LoadCurrentAsync();
                TaskbarJumpList.SystemGroupKind = AppJumpList.GroupKind;
                ViewModel.RemoveUnusedItems();
                ViewModel.UpdateJumpListGroupName();
                await TaskbarJumpList.SaveAsync();
            }
        }

        /// <summary>
        /// 重启应用
        /// </summary>
        public void Restart()
        {
            Program.ApplicationRoot.MainWindow.AppWindow.Hide();
            unsafe
            {
                Kernel32Library.GetStartupInfo(out STARTUPINFO WinGetProcessStartupInfo);
                WinGetProcessStartupInfo.lpReserved = null;
                WinGetProcessStartupInfo.lpDesktop = null;
                WinGetProcessStartupInfo.lpTitle = null;
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

                bool createResult = Kernel32Library.CreateProcess(null, "GetStoreApp.exe Restart", IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.None, IntPtr.Zero, null, ref WinGetProcessStartupInfo, out PROCESS_INFORMATION WinGetProcessInformation);

                if (createResult)
                {
                    if (WinGetProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hProcess);
                    if (WinGetProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hThread);
                }
            }
            ViewModel.Dispose();
        }
    }
}
