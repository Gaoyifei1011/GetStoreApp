using GetStoreAppConsole.Contracts;
using GetStoreAppConsole.Helpers;
using System.Threading.Tasks;

namespace GetStoreAppConsole.Services
{
    public class ActivationService : IActivationService
    {
        private ILanguageService LanguageService { get; } = IOCHelper.GetService<ILanguageService>();

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        /// <summary>
        /// 初始化控制台设置
        /// </summary>
        public async Task ActivateAsync()
        {
            await LanguageService.InitializeLanguageAsync();
            await ResourceService.InitializeResourceAsync(LanguageService.DefaultConsoleLanguage, LanguageService.ConsoleLanguage);
        }
    }
}
