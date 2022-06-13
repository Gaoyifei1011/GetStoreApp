using GetStoreApp.Contracts.Services;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.System;

namespace GetStoreApp.Views
{
    // TODO: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; }

        public ShellPage(ShellViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            ViewModel.NavigationService.Frame = NavigationFrame;
            ViewModel.NavigationViewService.Initialize(NavigationViewControl);

            // TODO: Set the title bar icon by updating /Assets/WindowIcon.png.
            // A custom title bar is required for full window theme and Mica support.
            // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
            App.MainWindow.ExtendsContentIntoTitleBar = true;
            App.MainWindow.SetTitleBar(AppTitleBar);

            BackdropHelper.SetBackdrop();
            SetAppTitleBarIcon();
        }

        /// <summary>
        /// 修改标题栏的应用名称
        /// </summary>
        private void SetAppTitleBarIcon()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(myWndId);

            appWindow.SetIcon(Path.Combine(Package.Current.InstalledLocation.Path, "Assets/Logo/GetStoreApp.ico"));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TitleBarHelper.UpdateTitleBar(RequestedTheme);

            KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
            KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
        }

        private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            AppTitleBar.Margin = new Thickness()
            {
                Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
                Top = AppTitleBar.Margin.Top,
                Right = AppTitleBar.Margin.Right,
                Bottom = AppTitleBar.Margin.Bottom
            };
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var navigationService = App.GetService<INavigationService>();

            var result = navigationService.GoBack();

            args.Handled = result;
        }
    }
}
