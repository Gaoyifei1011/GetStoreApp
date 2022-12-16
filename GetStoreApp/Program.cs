using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using WinRT;

namespace GetStoreApp
{
    public class Program
    {
        // 换行符
        private static string LineBreaks = "\r\n";

        private static bool IsDesktopProgram = true;

        public static List<string> CommandLineArgs { get; set; }

        /// <summary>
        /// 程序入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            CommandLineArgs = args.ToList();
            IsDesktopProgram = CommandLineArgs.FindIndex(item => item.Equals("Console")) == -1;

            InitializeProgramResourcesAsync().Wait();

            // 以桌面应用程序方式正常启动
            if (IsDesktopProgram)
            {
                ComWrappersSupport.InitializeComWrappers();
                Application.Start((p) =>
                {
                    DispatcherQueueSynchronizationContext context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    new App(args);
                });
            }

            // 以控制台方式启动程序
            else
            {
                bool AttachResult = Kernel32Library.AttachConsole();
                if (!AttachResult)
                {
                    Kernel32Library.AllocConsole();
                }

                Console.Title = ResourceService.GetLocalized("/Console/Title");

                Console.WriteLine(string.Format(ResourceService.GetLocalized("/Console/HeaderDescription1"),
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision
                    ) + LineBreaks);
                Console.WriteLine(ResourceService.GetLocalized("/Console/HeaderDescription2"));
                Console.WriteLine(ResourceService.GetLocalized("/Console/HeaderDescription3") + LineBreaks);

                Console.WriteLine(ResourceService.GetLocalized("/Console/UnfinishedNotification1"));
                Console.WriteLine(ResourceService.GetLocalized("/Console/UnfinishedNotification2") + LineBreaks);

                Windows.System.Launcher.LaunchUriAsync(new Uri("getstoreapp://")).GetAwaiter().GetResult();

                Kernel32Library.FreeConsole();

                // 退出应用程序
                Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
            }
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        public static async Task InitializeProgramResourcesAsync()
        {
            // 初始化应用资源，应用使用的语言信息和启动参数
            await LanguageService.InitializeLanguageAsync();
            await ResourceService.InitializeResourceAsync(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
        }
    }
}
