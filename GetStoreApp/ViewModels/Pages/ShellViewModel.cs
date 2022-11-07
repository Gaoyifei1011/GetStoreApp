using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Views.Pages;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace GetStoreApp.ViewModels.Pages
{
    public class ShellViewModel : ObservableRecipient
    {
        public INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        public IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        public IBackdropService BackdropService { get; } = ContainerHelper.GetInstance<IBackdropService>();

        public INavigationViewService NavigationViewService { get; } = ContainerHelper.GetInstance<INavigationViewService>();

        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private UISettings uiSettings = new UISettings();

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

        private SolidColorBrush _appBackgroundBrush;

        public SolidColorBrush AppBackgroundBrush
        {
            get { return _appBackgroundBrush; }

            set { SetProperty(ref _appBackgroundBrush, value); }
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
            uiSettings.ColorValuesChanged += OnColorValuesChanged;

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

            SetBackgruondBrush(ThemeService.AppTheme.InternalName, BackdropService.AppBackdrop.InternalName);

            // 设置主题发生变化时修改主页面导航控件的主题
            WeakReferenceMessenger.Default.Register<ShellViewModel, ThemeChangedMessage>(this, (shellViewModel, themeChangedMessage) =>
            {
                SetBackgruondBrush(ThemeService.AppTheme.InternalName, BackdropService.AppBackdrop.InternalName);
            });

            // 设置主题发生变化时修改主页面导航控件的主题
            WeakReferenceMessenger.Default.Register<ShellViewModel, BackdropChangedMessage>(this, (shellViewModel, backdropChangedMessage) =>
            {
                SetBackgruondBrush(ThemeService.AppTheme.InternalName, BackdropService.AppBackdrop.InternalName);
            });
        });

        // 页面被卸载时，注销所有事件
        public IRelayCommand UnloadedCommand => new RelayCommand(() =>
        {
            NavigationService.Navigated -= OnNavigated;
            uiSettings.ColorValuesChanged -= OnColorValuesChanged;

            WeakReferenceMessenger.Default.UnregisterAll(this);
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

        /// <summary>
        /// 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改应用背景色
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    SetBackgruondBrush(Convert.ToString(RegistryHelper.GetRegistryAppTheme()), BackdropService.AppBackdrop.InternalName);
                }
            });
        }

        /// <summary>
        /// 根据应用设置存储的主题值和背景色值设置应用背景色
        /// </summary>
        public void SetBackgruondBrush(string theme, string backdrop)
        {
            if (backdrop == BackdropService.BackdropList[0].InternalName)
            {
                if (theme == ThemeService.ThemeList[1].InternalName)
                {
                    AppBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                }
                else if (theme == ThemeService.ThemeList[2].InternalName)
                {
                    AppBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                }
                else
                {
                    if (RegistryHelper.GetRegistryAppTheme() == ElementTheme.Light)
                    {
                        AppBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                    }
                    else if (RegistryHelper.GetRegistryAppTheme() == ElementTheme.Dark)
                    {
                        AppBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                    }
                }
            }
            else
            {
                AppBackgroundBrush = new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}
