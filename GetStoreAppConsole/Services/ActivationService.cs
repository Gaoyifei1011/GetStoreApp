using System.Threading.Tasks;

namespace GetStoreAppConsole.Services
{
    public static class ActivationService
    {
        /// <summary>
        /// 初始化控制台设置
        /// </summary>
        public static async Task ActivateAsync()
        {
            await LanguageService.InitializeLanguageAsync();
            await ResourceService.InitializeResourceAsync(LanguageService.DefaultConsoleLanguage, LanguageService.ConsoleLanguage);
        }
    }
}
