using GetStoreApp.Activation;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.History;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Contracts.Services.Web;
using GetStoreApp.Services.App;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.History;
using GetStoreApp.Services.Settings;
using GetStoreApp.Services.Shell;
using GetStoreApp.Services.Web;
using GetStoreApp.UI.Controls.About;
using GetStoreApp.UI.Controls.Home;
using GetStoreApp.UI.Controls.Settings;
using GetStoreApp.ViewModels.Controls.About;
using GetStoreApp.ViewModels.Controls.Home;
using GetStoreApp.ViewModels.Controls.Settings;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace GetStoreApp
{
    public partial class App : Application
    {
        private static IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Services
                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IConfigService, ConfigService>();
                services.AddSingleton<IDataBaseService, DataBaseService>();
                services.AddSingleton<IResourceService, ResourceService>();

                services.AddTransient<IDownloadDataService, DownloadDataService>();

                services.AddTransient<IHistoryDataService, HistoryDataService>();

                services.AddSingleton<IBackdropService, BackdropService>();
                services.AddSingleton<IHistoryItemValueService, HistoryItemValueService>();
                services.AddSingleton<ILanguageService, LanguageService>();
                services.AddSingleton<ILinkFilterService, LinkFilterService>();
                services.AddSingleton<IRegionService, RegionService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<IUseInstructionService, UseInstructionService>();

                services.AddSingleton<INavigationService, NavigationService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();
                services.AddSingleton<IPageService, PageService>();

                services.AddTransient<IWebViewService, WebViewService>();

                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Pages and ViewModels
                services.AddTransient<AboutPage>();
                services.AddTransient<AboutViewModel>();
                services.AddTransient<DownloadPage>();
                services.AddTransient<DownloadViewModel>();
                services.AddTransient<HistoryPage>();
                services.AddTransient<HistoryViewModel>();
                services.AddTransient<HomePage>();
                services.AddTransient<HomeViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();
                services.AddTransient<WebPage>();
                services.AddTransient<WebViewModel>();

                // Controls and ViewModels
                services.AddTransient<HeaderControl>();
                services.AddTransient<HeaderViewModel>();
                services.AddTransient<InstructionsControl>();
                services.AddTransient<InstructionsViewModel>();
                services.AddTransient<PrecautionControl>();
                services.AddTransient<PrecautionViewModel>();
                services.AddTransient<ReferenceControl>();
                services.AddTransient<ReferenceViewModel>();

                services.AddTransient<HistoryItemControl>();
                services.AddTransient<HistoryItemViewModel>();
                services.AddTransient<RequestControl>();
                services.AddTransient<RequestViewModel>();
                services.AddTransient<ResultControl>();
                services.AddTransient<ResultViewModel>();
                services.AddTransient<StatusBarControl>();
                services.AddTransient<StatusBarViewModel>();
                services.AddTransient<TitleControl>();
                services.AddTransient<TitleViewModel>();

                services.AddTransient<BackdropControl>();
                services.AddTransient<BackdropViewModel>();
                services.AddTransient<ClearRecordControl>();
                services.AddTransient<ClearRecordViewModel>();
                services.AddTransient<HistoryItemValueControl>();
                services.AddTransient<HistoryItemValueViewModel>();
                services.AddTransient<LauguageControl>();
                services.AddTransient<LanguageViewModel>();
                services.AddTransient<LinkFilterControl>();
                services.AddTransient<LinkFilterViewModel>();
                services.AddTransient<RegionControl>();
                services.AddTransient<RegionViewModel>();
                services.AddTransient<ThemeControl>();
                services.AddTransient<ThemeViewModel>();
                services.AddTransient<UseInstructionControl>();
                services.AddTransient<UseInstructionViewModel>();
            })
            .Build();

        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        public static Window MainWindow { get; set; } = new Window();

        public App()
        {
            InitializeComponent();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            IActivationService activationService = GetService<IActivationService>();
            IResourceService resourceService = GetService<IResourceService>();

            await activationService.ActivateAsync(args);
            MainWindow.Title = resourceService.GetLocalized("AppDisplayName");
        }
    }
}
