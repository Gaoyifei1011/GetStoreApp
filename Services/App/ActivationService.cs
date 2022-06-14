using GetStoreApp.Activation;
using GetStoreApp.Contracts.Services;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.Services.App
{
    public class ActivationService : IActivationService
    {
        private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private readonly INavigationService _navigationService;
        private UIElement _shell = null;

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static ElementTheme _theme;

        public static ElementTheme Theme
        {
            get { return _theme; }

            set
            {
                _theme = value;

                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Theme)));
            }
        }

        public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers, INavigationService navigationService)
        {
            _defaultHandler = defaultHandler;
            _activationHandlers = activationHandlers;
            _navigationService = navigationService;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (GetStoreApp.App.MainWindow.Content == null)
            {
                _shell = GetStoreApp.App.GetService<ShellPage>();
                GetStoreApp.App.MainWindow.Content = _shell ?? new Frame();
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);

            // Ensure the current window is active
            GetStoreApp.App.MainWindow.Activate();

            // Tasks after activation
            await StartupAsync();
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = _activationHandlers
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (_defaultHandler.CanHandle(activationArgs))
            {
                await _defaultHandler.HandleAsync(activationArgs);
            }
        }

        private async Task StartupAsync()
        {
            Theme = await ThemeSelectorService.InitializeAsync();
            await ThemeSelectorService.SetRequestedThemeAsync();

            await DataBaseService.InitializeDataBaseAsync();
        }
    }
}
