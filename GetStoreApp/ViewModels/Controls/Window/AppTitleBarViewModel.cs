using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Graphics;

namespace GetStoreApp.ViewModels.Controls.Window
{
    /// <summary>
    /// 应用标题栏用户控件视图模型
    /// </summary>
    public sealed class AppTitleBarViewModel
    {
        /// <summary>
        /// 初始化自定义标题栏
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            Grid appTitleBar = sender as Grid;
            if (appTitleBar is not null)
            {
                SetTitleBarColor();

                SetDragRectangles(Convert.ToInt32(appTitleBar.Margin.Left), appTitleBar.ActualWidth, appTitleBar.ActualHeight);

                ((FrameworkElement)Program.ApplicationRoot.MainWindow.Content).ActualThemeChanged += OnActualThemeChanged;

                // 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改标题栏按钮主题
                Messenger.Default.Register<bool>(this, MessageToken.SystemSettingsChanged, (systemSettingsChangedMessage) =>
                {
                    if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                    {
                        SetTitleBarButtonColor(RegistryHelper.GetRegistryAppTheme());
                    }
                });
            }
        }

        /// <summary>
        /// 控件大小发生变化时，修改拖动区域
        /// </summary>
        public void OnSizeChanged(object sender, RoutedEventArgs args)
        {
            Grid appTitleBar = sender as Grid;
            if (appTitleBar is not null)
            {
                SetDragRectangles(Convert.ToInt32(appTitleBar.Margin.Left), appTitleBar.ActualWidth, appTitleBar.ActualHeight);
            }
        }

        /// <summary>
        /// 控件被卸载时，关闭所有事件，关闭消息服务
        /// </summary>
        public void OnUnloaded(object sender, RoutedEventArgs args)
        {
            ((FrameworkElement)Program.ApplicationRoot.MainWindow.Content).ActualThemeChanged -= OnActualThemeChanged;
            Messenger.Default.Unregister(this);
        }

        /// <summary>
        /// 设置主题发生变化时修改标题栏按钮的主题
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
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
            AppWindowTitleBar bar = Program.ApplicationRoot.AppWindow.TitleBar;

            switch (theme)
            {
                case ElementTheme.Light:
                    {
                        bar.ButtonBackgroundColor = Colors.Transparent;
                        bar.ButtonForegroundColor = Colors.Black;
                        bar.ButtonHoverBackgroundColor = ColorHelper.FromArgb(255, 233, 233, 233);
                        bar.ButtonHoverForegroundColor = Colors.Black;
                        bar.ButtonPressedBackgroundColor = ColorHelper.FromArgb(255, 237, 237, 237);
                        bar.ButtonPressedForegroundColor = Colors.Black;
                        bar.ButtonInactiveBackgroundColor = Colors.Transparent;
                        bar.ButtonInactiveForegroundColor = Colors.Gray;
                        break;
                    }
                case ElementTheme.Dark:
                    {
                        bar.ButtonBackgroundColor = Colors.Transparent;
                        bar.ButtonForegroundColor = Colors.White;
                        bar.ButtonHoverBackgroundColor = ColorHelper.FromArgb(255, 45, 45, 45);
                        bar.ButtonHoverForegroundColor = Colors.White;
                        bar.ButtonPressedBackgroundColor = ColorHelper.FromArgb(255, 40, 40, 40);
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
            Program.ApplicationRoot.AppWindow.TitleBar.SetDragRectangles(new RectInt32[] { new RectInt32(leftMargin, 0, GetActualPixel(actualWidth), GetActualPixel(actualHeight)) });
        }

        /// <summary>
        /// 在设置拖动区域时，需要考虑到系统缩放比例对像素的影响.
        /// </summary>
        private static int GetActualPixel(double pixel)
        {
            int currentDpi = User32Library.GetDpiForWindow(Program.ApplicationRoot.MainWindow.GetMainWindowHandle());
            return Convert.ToInt32(pixel * (currentDpi / 96.0));
        }
    }
}
