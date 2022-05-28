﻿using GetStoreApp.Activation;
using GetStoreApp.Contracts.Services;
using GetStoreApp.Core.Contracts.Services;
using GetStoreApp.Core.Services;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using GetStoreApp.Services.App;
using GetStoreApp.Services.Settings;
using GetStoreApp.Services.Shell;
using GetStoreApp.Services.Web;
using GetStoreApp.UI.Controls.General;
using GetStoreApp.UI.Controls.Home;
using GetStoreApp.UI.Controls.Settings;
using GetStoreApp.ViewModels.Controls.General;
using GetStoreApp.ViewModels.Controls.Home;
using GetStoreApp.ViewModels.Controls.Settings;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

// To learn more about WinUI3, see: https://docs.microsoft.com/windows/apps/winui/winui3/.
namespace GetStoreApp
{
    public partial class App : Application
    {
        // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Other Activation Handlers

                // Services
                services.AddSingleton<ILocalSettingsService, LocalSettingsServicePackaged>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddTransient<IWebViewService, WebViewService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();

                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // Core Services
                services.AddSingleton<IFileService, FileService>();

                // Pages and ViewModels

                services.AddTransient<AboutViewModel>();
                services.AddTransient<AboutPage>();
                services.AddTransient<DownloadViewModel>();
                services.AddTransient<DownloadPage>();
                services.AddTransient<HistoryViewModel>();
                services.AddTransient<HistoryPage>();
                services.AddTransient<HomeViewModel>();
                services.AddTransient<HomePage>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();
                services.AddTransient<WebViewModel>();
                services.AddTransient<WebPage>();

                // Controls and ViewModels
                services.AddTransient<CardControl>();
                services.AddTransient<CardViewModel>();

                services.AddTransient<ResultControl>();
                services.AddTransient<ResultViewModel>();
                services.AddTransient<RequestControl>();
                services.AddTransient<RequestViewModel>();
                services.AddTransient<StatusBarControl>();
                services.AddTransient<StatusBarViewModel>();
                services.AddTransient<TitleControl>();
                services.AddTransient<TitleViewModel>();

                services.AddTransient<HistoryItemValueControl>();
                services.AddTransient<HistoryItemValueViewModel>();
                services.AddTransient<LauguageControl>();
                services.AddTransient<LanguageViewModel>();
                services.AddTransient<RegionControl>();
                services.AddTransient<RegionViewModel>();
                services.AddTransient<ThemeControl>();
                services.AddTransient<ThemeViewModel>();
                services.AddTransient<UseInstructionControl>();
                services.AddTransient<UseInstructionViewModel>();
                services.AddTransient<LinkFilterControl>();
                services.AddTransient<LinkFilterViewModel>();

                // Configuration
                services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
            })
            .Build();

        public static T GetService<T>()
            where T : class
            => _host.Services.GetService(typeof(T)) as T;

        public static Window MainWindow { get; set; } = new Window() { Title = "AppDisplayName".GetLocalized() };

        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // TODO: Log and handle exceptions as appropriate.
            // For more details, see https://docs.microsoft.com/windows/winui/api/microsoft.ui.xaml.unhandledexceptioneventargs.
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            var activationService = GetService<IActivationService>();
            await activationService.ActivateAsync(args);
        }
    }
}