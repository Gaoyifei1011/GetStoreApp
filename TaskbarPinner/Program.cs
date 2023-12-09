using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Threading;
using TaskbarPinner.Helpers.Root;
using TaskbarPinner.Services.Controls.Settings;
using TaskbarPinner.Services.Root;
using TaskbarPinner.WindowsAPI.PInvoke.Kernel32;

namespace TaskbarPinner
{
    /// <summary>
    /// 任务栏固定辅助程序
    /// </summary>
    public class Program
    {
        public static App App { get; private set; }

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [MTAThread]
        public static void Main(string[] args)
        {
            if (!RuntimeHelper.IsMSIX)
            {
                Process.Start("explorer.exe", "explorer.exe shell:AppsFolder\\Gaoyifei1011.GetStoreApp_pystbwmrmew8c!GetStoreApp");
                return;
            }
            Kernel32Library.LoadPackagedLibrary("WinUICoreAppHook.dll");

            InitializeResources();

            Application.Start((param) =>
            {
                DispatcherQueueSynchronizationContext context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                App = new App();
            });
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeResources()
        {
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            LogService.Initialize();
        }
    }
}
