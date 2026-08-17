using GetStoreAppPinner.Helpers.Root;
using GetStoreAppPinner.Services.Root;
using GetStoreAppPinner.Services.Settings;
using System;
using System.Threading;
using Windows.System;
using Windows.UI.Xaml;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreAppPinner
{
    /// <summary>
    /// 网页浏览器
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [MTAThread]
        internal static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();

            if (!RuntimeHelper.IsMSIX)
            {
                Launcher.LaunchUriAsync(new("getstoreapp:")).Wait();
                return;
            }

            InitializeResources();

            Application.Start((param) =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                new App();
            });
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeResources()
        {
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);

            ThemeService.InitializeTheme();
        }
    }
}
