using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.ViewModels.Pages
{
    public class ShellViewModel : ObservableRecipient
    {
        private bool _isBackEnabled;
        private object _selected;

        public INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public INavigationViewService NavigationViewService { get; } = IOCHelper.GetService<INavigationViewService>();

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

        public IRelayCommand LoadedCommand => new RelayCommand(() =>
        {
            NavigationService.Navigated += OnNavigated;
        });

        // 页面被卸载时，注销所有事件
        public IRelayCommand UnloadedCommand => new RelayCommand(() =>
        {
            NavigationService.Navigated -= OnNavigated;
        });

        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            IsBackEnabled = NavigationService.CanGoBack;

            if (args.SourcePageType == typeof(SettingsPage))
            {
                Selected = NavigationViewService.SettingsItem;
                return;
            }

            var selectedItem = NavigationViewService.GetSelectedItem(args.SourcePageType);
            if (selectedItem is not null)
            {
                Selected = selectedItem;
            }
        }
    }
}
