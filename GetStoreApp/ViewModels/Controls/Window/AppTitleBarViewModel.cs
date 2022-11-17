using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Controls.Settings.Appearance;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreAppWindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Graphics;
using Windows.UI;
using Windows.Win32;
using Windows.Win32.Foundation;
using WinRT.Interop;

namespace GetStoreApp.ViewModels.Controls.Window
{
    public class AppTitleBarViewModel : ObservableRecipient
    {
        private IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        // 初始化自定义标题栏
        public IRelayCommand LoadedCommand => new RelayCommand<Grid>((appTitleBar) =>
        {
            SetTitleBarColor();

            SetDragRectangles(Convert.ToInt32(appTitleBar.Margin.Left), appTitleBar.ActualWidth, appTitleBar.ActualHeight);

            ((FrameworkElement)App.MainWindow.Content).ActualThemeChanged += WindowThemeChanged;

            // 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改标题栏按钮主题
            WeakReferenceMessenger.Default.Register<AppTitleBarViewModel, SystemSettingsChnagedMessage>(this, (appTitleBarViewModel, systemSettingsChangedMessage) =>
            {
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    SetTitleBarButtonColor(RegistryHelper.GetRegistryAppTheme());
                }
            });
        });

        // 控件被卸载时，关闭所有事件，关闭消息服务
        public IRelayCommand UnLoadedCommand => new RelayCommand(() =>
        {
            ((FrameworkElement)App.MainWindow.Content).ActualThemeChanged -= WindowThemeChanged;
            WeakReferenceMessenger.Default.UnregisterAll(this);
        });

        // 控件大小发生变化时，修改拖动区域
        public IRelayCommand SizeChangedCommand => new RelayCommand<Grid>((appTitleBar) =>
        {
            SetDragRectangles(Convert.ToInt32(appTitleBar.Margin.Left), appTitleBar.ActualWidth, appTitleBar.ActualHeight);
        });

        /// <summary>
        /// 设置主题发生变化时修改标题栏按钮的主题
        /// </summary>
        private void WindowThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarColor();
        }

        /// <summary>
        /// 根据应用设置存储的主题值设置标题栏按钮的颜色
        /// </summary>
        private void SetTitleBarColor()
        {
            if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
            {
                SetTitleBarButtonColor(RegistryHelper.GetRegistryAppTheme());
            }
            else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
            {
                SetTitleBarButtonColor(ElementTheme.Light);
            }
            else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
            {
                SetTitleBarButtonColor(ElementTheme.Dark);
            }
        }

        /// <summary>
        /// 设置标题栏按钮的颜色
        /// </summary>
        private void SetTitleBarButtonColor(ElementTheme theme)
        {
            AppWindowTitleBar bar = App.AppWindow.TitleBar;

            switch (theme)
            {
                case ElementTheme.Light:
                    {
                        bar.ButtonBackgroundColor = Colors.Transparent;
                        bar.ButtonForegroundColor = Colors.Black;
                        bar.ButtonHoverBackgroundColor = Color.FromArgb(255, 233, 233, 233);
                        bar.ButtonHoverForegroundColor = Colors.Black;
                        bar.ButtonPressedBackgroundColor = Color.FromArgb(255, 237, 237, 237);
                        bar.ButtonPressedForegroundColor = Colors.Black;
                        bar.ButtonInactiveBackgroundColor = Colors.Transparent;
                        bar.ButtonInactiveForegroundColor = Colors.Gray;
                        break;
                    }
                case ElementTheme.Dark:
                    {
                        bar.ButtonBackgroundColor = Colors.Transparent;
                        bar.ButtonForegroundColor = Colors.White;
                        bar.ButtonHoverBackgroundColor = Color.FromArgb(255, 45, 45, 45);
                        bar.ButtonHoverForegroundColor = Colors.White;
                        bar.ButtonPressedBackgroundColor = Color.FromArgb(255, 40, 40, 40);
                        bar.ButtonPressedForegroundColor = Colors.White;
                        bar.ButtonInactiveBackgroundColor = Colors.Transparent;
                        bar.ButtonInactiveForegroundColor = Colors.Gray;
                        break;
                    }
            }
        }

        /// <summary>
        /// 设置标题栏的可拖动区域
        /// </summary>
        private void SetDragRectangles(int leftMargin, double actualWidth, double actualHeight)
        {
            App.AppWindow.TitleBar.SetDragRectangles(new RectInt32[] { new RectInt32(leftMargin, 0, GetActualPixel(actualWidth), GetActualPixel(actualHeight)) });
        }

        /// <summary>
        /// 在设置拖动区域时，需要考虑到系统缩放比例对像素的影响.
        /// </summary>
        private static int GetActualPixel(double pixel)
        {
            int currentDpi = User32Library.GetDpiForWindow(WindowNative.GetWindowHandle(App.MainWindow));
            return Convert.ToInt32(pixel * (currentDpi / 96.0));
        }
    }
}
