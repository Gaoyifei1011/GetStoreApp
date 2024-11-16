using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
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
            SetAppIcon(MainWindow.Current.AppWindow);

            if (JumpList.IsSupported())
            {
                Task.Run(async () =>
                {
                    JumpList taskbarJumpList = await JumpList.LoadCurrentAsync();
                    taskbarJumpList.Items.Clear();
                    taskbarJumpList.SystemGroupKind = JumpListSystemGroupKind.None;

                    JumpListItem storeItem = JumpListItem.CreateWithArguments("JumpList Store", ResourceService.GetLocalized("Window/Store"));
                    storeItem.Logo = new Uri("ms-appx:///Assets/Icon/Control/Store.png");
                    taskbarJumpList.Items.Add(storeItem);

                    JumpListItem appUpdateItem = JumpListItem.CreateWithArguments("JumpList AppUpdate", ResourceService.GetLocalized("Window/AppUpdate"));
                    appUpdateItem.Logo = new Uri("ms-appx:///Assets/Icon/Control/AppUpdate.png");
                    taskbarJumpList.Items.Add(appUpdateItem);

                    taskbarJumpList.Items.Add(JumpListItem.CreateSeparator());

                    JumpListItem wingetItem = JumpListItem.CreateWithArguments("JumpList WinGet", ResourceService.GetLocalized("Window/WinGet"));
                    wingetItem.Logo = new Uri("ms-appx:///Assets/Icon/Control/WinGet.png");
                    taskbarJumpList.Items.Add(wingetItem);

                    JumpListItem appManagerItem = JumpListItem.CreateWithArguments("JumpList AppManager", ResourceService.GetLocalized("Window/AppManager"));
                    appManagerItem.Logo = new Uri("ms-appx:///Assets/Icon/Control/AppManager.png");
                    taskbarJumpList.Items.Add(appManagerItem);

                    taskbarJumpList.Items.Add(JumpListItem.CreateSeparator());

                    JumpListItem downloadItem = JumpListItem.CreateWithArguments("JumpList Download", ResourceService.GetLocalized("Window/Download"));
                    downloadItem.Logo = new Uri("ms-appx:///Assets/Icon/Control/Download.png");
                    taskbarJumpList.Items.Add(downloadItem);

                    JumpListItem webItem = JumpListItem.CreateWithArguments("JumpList Web", ResourceService.GetLocalized("Window/Web"));
                    webItem.Logo = new Uri("ms-appx:///Assets/Icon/Control/Web.png");
                    taskbarJumpList.Items.Add(webItem);

                    await taskbarJumpList.SaveAsync();
                });
            }
        }

        /// <summary>
        /// 处理桌面应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Error, "Unknown unhandled exception.", args.Exception);
        }

        /// <summary>
        /// 设置应用窗口图标
        /// </summary>
        private void SetAppIcon(AppWindow appWindow)
        {
            // 选中文件中的图标总数
            int iconTotalCount = User32Library.PrivateExtractIcons(Environment.ProcessPath, 0, 0, 0, null, null, 0, 0);

            // 用于接收获取到的图标指针
            IntPtr[] hIcons = new IntPtr[iconTotalCount];

            // 对应的图标id
            int[] ids = new int[iconTotalCount];

            // 成功获取到的图标个数
            int successCount = User32Library.PrivateExtractIcons(Environment.ProcessPath, 0, 256, 256, hIcons, ids, iconTotalCount, 0);

            // GetStoreApp.exe 应用程序只有一个图标
            if (successCount >= 1 && hIcons[0] != IntPtr.Zero)
            {
                appWindow.SetIcon(Win32Interop.GetIconIdFromIcon(hIcons[0]));
            }
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
                GlobalNotificationService.SendNotification();
                MainWindow.Current?.SaveWindowInformation();
                DownloadSchedulerService.CloseDownloadScheduler(true);
                LogService.CloseLog();
                isDisposed = true;
            }

            Exit();
        }
    }
}
