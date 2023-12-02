using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Controls.Settings;
using GetStoreAppWebView.Services.Root;
using System;
using System.Diagnostics;

namespace GetStoreAppWebView
{
    /// <summary>
    /// 网页浏览器
    /// </summary>
    public static class Program
    {
        public static WinUIApp WinUIApp { get; private set; }

        public static WPFApp WPFApp { get; private set; }

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

            InitializeResources();

            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[1])
            {
                Environment.SetEnvironmentVariable("WEBVIEW2_USE_VISUAL_HOSTING_FOR_OWNED_WINDOWS", "1");
            }

            WinUIApp = new WinUIApp();
            WPFApp = new WPFApp();
            WPFApp.Run();
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeResources()
        {
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            WebKernelService.InitializeWebKernel();
            LogService.Initialize();
        }
    }
}
