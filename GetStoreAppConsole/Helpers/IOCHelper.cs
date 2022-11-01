using GetStoreAppConsole.Contracts;
using GetStoreAppConsole.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GetStoreAppConsole.Helpers
{
    /// <summary>
    /// 控制翻转/依赖注入
    /// </summary>
    public static class IOCHelper
    {
        public static IHost Host { get; set; }

        public static T GetService<T>() where T : class
        {
            if (Host.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} 需要在IOCHelper.cs中的ConfigureServices中注册。");
            }

            return service;
        }

        public static void InitializeService()
        {
            Host = Microsoft.Extensions.Hosting.Host.
            CreateDefaultBuilder().
            UseContentRoot(AppContext.BaseDirectory).
            ConfigureServices((context, services) =>
            {
                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IConfigStoreageService,ConfigStorageService>();
                services.AddSingleton<IConsoleService, ConsoleService>();
                services.AddSingleton<ILanguageService, LanguageService>();
                services.AddSingleton<IResourceService, ResourceService>();
            }).Build();
        }
    }
}
