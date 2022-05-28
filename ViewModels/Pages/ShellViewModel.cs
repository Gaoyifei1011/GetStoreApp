using CommunityToolkit.Mvvm.ComponentModel;

using GetStoreApp.Contracts.Services;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views;

using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.ViewModels.Pages
{
    public class ShellViewModel : ObservableRecipient
    {
        public static string Home { get; } = LanguageService.GetResources("/Shell/Home");

        public static string Download { get; } = LanguageService.GetResources("/Shell/Download");

        public static string History { get; } = LanguageService.GetResources("/Shell/History");

        public static string Web { get; } = LanguageService.GetResources("/Shell/Web");

        public static string About { get; } = LanguageService.GetResources("/Shell/About");

        private bool _isBackEnabled;
        private object _selected;

        public INavigationService NavigationService { get; }

        public INavigationViewService NavigationViewService { get; }

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { SetProperty(ref _isBackEnabled, value); }
        }

        public object Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
        {
            NavigationService = navigationService;
            NavigationService.Navigated += OnNavigated;
            NavigationViewService = navigationViewService;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = NavigationViewService.SettingsItem;
                return;
            }

            var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }
    }
}