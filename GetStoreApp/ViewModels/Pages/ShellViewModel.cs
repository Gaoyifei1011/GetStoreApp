using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.ViewModels.Pages
{
    public class ShellViewModel : ObservableRecipient
    {
        public INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        public INavigationViewService NavigationViewService { get; } = ContainerHelper.GetInstance<INavigationViewService>();

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set { SetProperty(ref _isBackEnabled, value); }
        }

        private bool _paneToggleButtonVisible;

        public bool PaneToggleButtonVisible
        {
            get { return _paneToggleButtonVisible; }

            set { SetProperty(ref _paneToggleButtonVisible, value); }
        }

        private NavigationViewPaneDisplayMode _navigationDisplayMode;

        public NavigationViewPaneDisplayMode NavigationDispalyMode
        {
            get { return _navigationDisplayMode; }

            set { SetProperty(ref _navigationDisplayMode, value); }
        }

        private NavigationViewItem _selectedItem;

        public NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set { SetProperty(ref _selectedItem, value); }
        }

        // 页面加载时，挂载所有事件，初始化导航视图控件属性和初次导航到的页面
        public IRelayCommand LoadedCommand => new RelayCommand(() =>
        {
            SelectedItem = NavigationViewService.GetSelectedItem(NavigationService.Frame.CurrentSourcePageType);
            NavigationService.Navigated += OnNavigated;

            if (App.MainWindow.Width >= 768)
            {
                PaneToggleButtonVisible = false;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.Left;
            }
            else
            {
                PaneToggleButtonVisible = true;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.LeftMinimal;
            }
        });

        // 页面被卸载时，注销所有事件
        public IRelayCommand UnloadedCommand => new RelayCommand(() =>
        {
            NavigationService.Navigated -= OnNavigated;
        });

        // 窗口大小发生改变时，设置页面导航视图样式
        public IRelayCommand SizeChangedCommand => new RelayCommand(() =>
        {
            if (App.MainWindow.Width >= 768)
            {
                PaneToggleButtonVisible = false;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.Left;
            }
            else
            {
                PaneToggleButtonVisible = true;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.LeftMinimal;
            }
        });

        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            IsBackEnabled = NavigationService.CanGoBack;

            if (args.SourcePageType == typeof(SettingsPage))
            {
                SelectedItem = NavigationViewService.SettingsItem;
                return;
            }

            var selectedItem = NavigationViewService.GetSelectedItem(args.SourcePageType);
            if (selectedItem is not null)
            {
                SelectedItem = selectedItem;
            }
        }
    }
}
