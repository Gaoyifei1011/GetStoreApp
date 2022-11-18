using GetStoreAppConsole.Extensions.DataType.Enums;
using GetStoreAppConsole.Services;
using System;
using System.Threading.Tasks;

namespace GetStoreAppConsole
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await ActivationService.ActivateAsync();
            ConsoleService.InitializeDescription();

            await Windows.System.Launcher.LaunchUriAsync(new Uri("getstoreapp://"));

            // 退出应用程序
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }
    }
}
