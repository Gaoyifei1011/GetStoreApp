using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.UI.StartScreen;

namespace GetStoreApp
{
    /// <summary>
    /// 获取商店应用程序
    /// </summary>
    public partial class WinUIApp : Application, IDisposable
    {
        private bool isDisposed;

        public Window Window { get; private set; }

        public WinUIApp()
        {
            InitializeComponent();
            DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 启动应用程序时调用，初始化应用主窗口
        /// </summary>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            Window = new MainWindow();
            MainWindow.Current.Show(true);
            InitializeJumpList();
            ConfigApp();
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;

            // 系统背景色弹出的异常，不进行处理
            if (args.Exception.HResult is -2147024809 && args.Exception.StackTrace.Contains("SystemBackdropConfiguration"))
            {
                LogService.WriteLog(LoggingLevel.Warning, "System backdrop config warning.", args.Exception);
                return;
            }
            // 处理其他异常
            else
            {
                LogService.WriteLog(LoggingLevel.Error, "Unknown unhandled exception.", args.Exception);

                // 退出应用
                Dispose();
            }
        }

        /// <summary>
        /// 初始化任务栏的跳转列表
        /// </summary>
        private void InitializeJumpList()
        {
            if (JumpList.IsSupported())
            {
                Task.Run(async () =>
                {
                    JumpList TaskbarJumpList = await JumpList.LoadCurrentAsync();
                    TaskbarJumpList.Items.Clear();
                    TaskbarJumpList.SystemGroupKind = JumpListSystemGroupKind.None;

                    JumpListItem storeItem = JumpListItem.CreateWithArguments("JumpList Store", ResourceService.GetLocalized("Window/Store"));
                    storeItem.Logo = new Uri("ms-appx:///Assets/ControlIcon/Store.png");
                    TaskbarJumpList.Items.Add(storeItem);

                    JumpListItem appUpdateItem = JumpListItem.CreateWithArguments("JumpList AppUpdate", ResourceService.GetLocalized("Window/AppUpdate"));
                    appUpdateItem.Logo = new Uri("ms-appx:///Assets/ControlIcon/AppUpdate.png");
                    TaskbarJumpList.Items.Add(appUpdateItem);

                    TaskbarJumpList.Items.Add(JumpListItem.CreateSeparator());

                    JumpListItem wingetItem = JumpListItem.CreateWithArguments("JumpList WinGet", ResourceService.GetLocalized("Window/WinGet"));
                    wingetItem.Logo = new Uri("ms-appx:///Assets/ControlIcon/WinGet.png");
                    TaskbarJumpList.Items.Add(wingetItem);

                    JumpListItem uwpAppItem = JumpListItem.CreateWithArguments("JumpList UWPApp", ResourceService.GetLocalized("Window/UWPApp"));
                    uwpAppItem.Logo = new Uri("ms-appx:///Assets/ControlIcon/UWPApp.png");
                    TaskbarJumpList.Items.Add(uwpAppItem);

                    TaskbarJumpList.Items.Add(JumpListItem.CreateSeparator());

                    JumpListItem downloadItem = JumpListItem.CreateWithArguments("JumpList Download", ResourceService.GetLocalized("Window/Download"));
                    downloadItem.Logo = new Uri("ms-appx:///Assets/ControlIcon/Download.png");
                    TaskbarJumpList.Items.Add(downloadItem);

                    JumpListItem webItem = JumpListItem.CreateWithArguments("JumpList Web", ResourceService.GetLocalized("Window/Web"));
                    webItem.Logo = new Uri("ms-appx:///Assets/ControlIcon/Web.png");
                    TaskbarJumpList.Items.Add(webItem);

                    await TaskbarJumpList.SaveAsync();
                });
            }
        }

        /// <summary>
        /// 窗口激活后配置其他设置
        /// </summary>
        private void ConfigApp()
        {
            // 设置应用主题
            ThemeService.SetWindowTheme();

            // 设置应用背景色
            BackdropService.SetAppBackdrop();

            // 设置应用置顶状态
            TopMostService.SetAppTopMost();

            // 初始化 WinGet 程序包安装信息
            WinGetService.InitializeService();

            // 初始化下载监控服务
            DownloadSchedulerService.InitializeDownloadScheduler();

            // 初始化Aria2配置文件信息
            Aria2Service.InitializeAria2Conf();

            // 启动Aria2下载服务
            Aria2Service.StartAria2Process();
        }

        /// <summary>
        /// 重启应用
        /// </summary>
        public void Restart()
        {
            MainWindow.Current.AppWindow.Hide();
            ProcessStarter.StartProcess(Path.Combine(InfoHelper.AppInstalledLocation, "GetStoreApp.exe"), "Restart", out _);
            Dispose();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WinUIApp()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                if (RuntimeHelper.IsElevated && MainWindow.Current.AppWindow.Id.Value is not 0)
                {
                    CHANGEFILTERSTRUCT changeFilterStatus = new CHANGEFILTERSTRUCT();
                    changeFilterStatus.cbSize = Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT));
                    User32Library.ChangeWindowMessageFilterEx((IntPtr)MainWindow.Current.AppWindow.Id.Value, WindowMessage.WM_COPYDATA, ChangeFilterAction.MSGFLT_RESET, in changeFilterStatus);
                }

                MainWindow.Current.SaveWindowInformation();
                DownloadSchedulerService.CloseDownloadScheduler();

                isDisposed = true;
            }

            Environment.Exit(0);
        }
    }
}
