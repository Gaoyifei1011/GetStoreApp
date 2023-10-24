using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Controls.Settings;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.Views.Forms;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace GetStoreAppWebView
{
    /// <summary>
    /// 获取商店应用 网页浏览器
    /// </summary>
    public static class Program
    {
        public static App ApplicationRoot { get; private set; }

        public static MainForm MainWindow { get; private set; }

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (!RuntimeHelper.IsMSIX)
            {
                Process.Start("explorer.exe", "explorer.exe shell:AppsFolder\\Gaoyifei1011.GetStoreApp_pystbwmrmew8c!GetStoreApp");
                return;
            }

            InitializeProgramResources();

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ApplicationExit += OnApplicationExit;
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            ApplicationRoot = new App();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow = new MainForm();
            Application.Run(MainWindow);
        }

        /// <summary>
        /// 在应用程序即将关闭时发生
        /// </summary>
        private static void OnApplicationExit(object sender, EventArgs args)
        {
            ApplicationRoot.Dispose();
        }

        /// <summary>
        /// 处理 Windows 窗体 UI 线程异常
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            MessageBox.Show(
                ResourceService.GetLocalized("WebView/ErrorTitle") + Environment.NewLine +
                ResourceService.GetLocalized("WebView/Content1") + Environment.NewLine +
                ResourceService.GetLocalized("WebView/Content2"),
                ResourceService.GetLocalized("WebView/AppDisplayName"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
                );
        }

        /// <summary>
        /// 处理 Windows 窗体非 UI 线程异常
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            MessageBox.Show(
                ResourceService.GetLocalized("WebView/ErrorTitle") + Environment.NewLine +
                ResourceService.GetLocalized("WebView/Content1") + Environment.NewLine +
                ResourceService.GetLocalized("WebView/Content2"),
                ResourceService.GetLocalized("WebView/AppDisplayName"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
                );
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeProgramResources()
        {
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
        }
    }
}
