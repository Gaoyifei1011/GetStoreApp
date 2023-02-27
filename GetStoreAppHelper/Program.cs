using GetStoreAppHelper.Services;
using System;
using System.Threading.Tasks;

// 获取商店应用辅助程序，创建任务栏菜单图标，使用Mile Xaml(Xaml Islands)实现任务栏右键菜单外观现代化，为获取商店应用程序提供模态对话框支持。（尚未进行）
namespace GetStoreAppHelper
{
    public static class Program
    {
        public static SingleInstanceManager SingleInstanceApp { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            InitializeProgramResourcesAsync().Wait();

            SingleInstanceApp = new SingleInstanceManager();
            SingleInstanceApp.Start(args);
            SingleInstanceApp.Dispose();
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
