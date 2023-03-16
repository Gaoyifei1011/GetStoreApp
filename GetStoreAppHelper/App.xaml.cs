using GetStoreAppHelper.Extensions.DataType.Enums;
using GetStoreAppHelper.Extensions.SystemTray;
using GetStoreAppHelper.Helpers;
using GetStoreAppHelper.Helpers.Root;
using GetStoreAppHelper.Services;
using GetStoreAppHelper.UI.Controls;
using GetStoreAppHelper.WindowsAPI.PInvoke.User32;
using GetStoreAppHelper.WindowsAPI.PInvoke.WinUI;
using System;
using System.Diagnostics;
using System.Timers;
using Windows.Graphics;
using Windows.UI.Xaml;

namespace GetStoreAppHelper
{
    public sealed partial class App : Application
    {
        private Timer AppTimer { get; set; } = new Timer(1000);

        public WindowsTrayIcon TrayIcon { get; private set; }

        public MileWindow MainWindow { get; set; }

        public App()
        {
            MileXamlLibrary.MileXamlGlobalInitialize();
            InitializeComponent();

            AppTimer.Elapsed += AppTimerElapsed;
            AppTimer.AutoReset = true;
            AppTimer.Start();
        }

        private void AppTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Process[] GetStoreAppProcess = Process.GetProcessesByName("GetStoreApp");

            if (GetStoreAppProcess.Length == 0)
            {
                AppTimer.Stop();
                AppTimer.Elapsed -= AppTimerElapsed;
                AppTimer.Dispose();

                CloseApp();
            }
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        public void Run()
        {
            ResourceDictionaryHelper.InitializeResourceDictionaryAsync().Wait();

            InitializeTrayIcon();

            MainWindow = new MileWindow();
            MainWindow.Content = new TrayMenuControl();
            MainWindow.Title = ResourceService.GetLocalized("HelperResources/Title");
            MainWindow.Position.X = 0;
            MainWindow.Position.Y = 0;
            MainWindow.Size.X = 0;
            MainWindow.Size.Y = 0;
            MainWindow.InitializeWindow();
            MainWindow.Activate();
        }

        /// <summary>
        /// 初始化应用的托盘图标
        /// </summary>
        private void InitializeTrayIcon()
        {
            TrayIcon = new WindowsTrayIcon(
                string.Format(@"{0}\{1}", InfoHelper.GetAppInstalledLocation(), "GetStoreApp.exe"),
                ResourceService.GetLocalized("HelperResources/AppName")
            );

            TrayIcon.DoubleClick = () =>
            {
                if (MainWindow.Content is not null)
                {
                    (MainWindow.Content as TrayMenuControl).ViewModel.ShowOrHideWindowCommand.Execute(null);
                }
            };

            TrayIcon.RightClick = () =>
            {
                User32Library.GetCursorPos(out PointInt32 CurrentPoint);
                User32Library.SetForegroundWindow(MainWindow.Handle);

                if (MainWindow.Content is not null)
                {
                    (MainWindow.Content as TrayMenuControl).SetXamlRoot(MainWindow.Content.XamlRoot);
                    (MainWindow.Content as TrayMenuControl).ShowMenuFlyout(CurrentPoint);
                }
            };
        }

        /// <summary>
        /// 关闭应用并释放所有资源
        /// </summary>
        public void CloseApp()
        {
            TrayIcon?.Dispose();

            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }
    }
}
