using GetStoreAppConsole.Contracts;
using GetStoreAppConsole.Helpers;
using System;
using System.Threading.Tasks;

namespace GetStoreAppConsole
{
    public class Program
    {
        private static IActivationService ActivationService { get; set; }

        private static IConsoleService ConsoleService { get; set; }

        public static async Task Main(string[] args)
        {
            IOCHelper.InitializeService();

            ActivationService = IOCHelper.GetService<IActivationService>();
            ConsoleService = IOCHelper.GetService<IConsoleService>();

            await ActivationService.ActivateAsync();
            ConsoleService.InitializeDescription();

            await Windows.System.Launcher.LaunchUriAsync(new Uri("getstoreapp://"));

            // 退出应用程序
            Environment.Exit(0);
        }
    }
}
