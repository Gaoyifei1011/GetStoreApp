using GetStoreAppHelper.Services;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

// 获取商店应用辅助程序，创建任务栏菜单图标，使用Mile Xaml(Xaml Islands)实现任务栏右键菜单外观现代化，为获取商店应用程序提供模态对话框支持。（尚未进行）
namespace GetStoreAppHelper
{
    public static class Program
    {
        private static Mutex AppMutex = null;

        public static App ApplicationRoot { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            bool isExists = Mutex.TryOpenExisting(Assembly.GetExecutingAssembly().GetName().Name, out AppMutex);
            if (isExists && AppMutex is not null) { return; }
            else
            {
                AppMutex = new Mutex(true, Assembly.GetExecutingAssembly().GetName().Name);
            }

            InitializeProgramResourcesAsync().Wait();

            ApplicationRoot = new App();
            ApplicationRoot.Run();
            ApplicationRoot.TrayIcon.Dispose();
            ApplicationRoot.Dispose();
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeProgramResourcesAsync()
        {
            await LanguageService.InitializeLanguageAsync();
            await ResourceService.InitializeResourceAsync(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
        }
    }
}
