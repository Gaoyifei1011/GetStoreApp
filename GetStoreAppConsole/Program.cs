using GetStoreAppConsole.Helpers;

namespace GetStoreAppConsole
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 初始化控制台程序的本地化资源内容
            ConsoleHelper.InitializeLocalization();

            // 初始化控制台程序的信息显示
            ConsoleHelper.InitializeDescription();

            await Windows.System.Launcher.LaunchUriAsync(new Uri("getstoreapp://"));

            // 退出应用程序
            Environment.Exit(0);
        }
    }
}
