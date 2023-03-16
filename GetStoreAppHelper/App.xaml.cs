using GetStoreAppHelper.Extensions.DataType.Enums;
using GetStoreAppHelper.Extensions.SystemTray;
using GetStoreAppHelper.Helpers;
using GetStoreAppHelper.Services;
using GetStoreAppHelper.UI.Controls;
using GetStoreAppHelper.WindowsAPI.PInvoke.WinUI;
using System;
using System.Diagnostics;
using System.Timers;
using Windows.UI.Xaml;

namespace GetStoreAppHelper
{
    public sealed partial class App : Application
    {
        private Timer AppTimer { get; set; } = new Timer(1000);

        public WindowsTrayIcon NotifyIcon { get; private set; }

        public MileWindow MainWindow { get; set; }

        public App()
        {
            MileXamlLibrary.MileXamlGlobalInitialize();
            InitializeComponent();

            NotifyIcon = new WindowsTrayIcon();
            NotifyIcon.SetState(true);

            AppTimer.Elapsed += AppTimerElapsed;
            AppTimer.AutoReset = true;
            AppTimer.Start();
        }

        private void AppTimerElapsed(object sender, ElapsedEventArgs args)
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
        /// 关闭应用并释放所有资源
        /// </summary>
        public void CloseApp()
        {
            NotifyIcon?.Dispose();
            MileXamlLibrary.MileXamlGlobalUninitialize();
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }
    }
}
