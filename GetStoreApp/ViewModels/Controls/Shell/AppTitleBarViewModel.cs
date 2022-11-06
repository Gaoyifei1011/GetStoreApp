using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using System;
using Windows.Graphics;
using Windows.UI;
using Windows.UI.ViewManagement;
using WinRT.Interop;

namespace GetStoreApp.ViewModels.Controls.Shell
{
    public class AppTitleBarViewModel : ObservableRecipient
    {
        private IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private UISettings uiSettings = new UISettings();

        // 初始化自定义标题栏
        public IRelayCommand LoadedCommand => new RelayCommand<Grid>((appTitleBar) =>
        {
            SetTitleBarColor(ThemeService.AppTheme.InternalName);

            SetDragRectangles(Convert.ToInt32(appTitleBar.Margin.Left), appTitleBar.ActualWidth, appTitleBar.ActualHeight);

            uiSettings.ColorValuesChanged += OnColorValuesChanged;

            // 设置主题发生变化时修改标题栏按钮的主题
            WeakReferenceMessenger.Default.Register<AppTitleBarViewModel, ThemeChangedMessage>(this, (appTitleBarViewModel, themeChangedMessage) =>
            {
                SetTitleBarColor(themeChangedMessage.Value.InternalName);
            });
        });

        // 控件被卸载时，关闭所有事件，关闭消息服务
        public IRelayCommand UnLoadedCommand => new RelayCommand(() =>
        {
            uiSettings.ColorValuesChanged -= OnColorValuesChanged;

            WeakReferenceMessenger.Default.UnregisterAll(this);
        });

        // 控件大小发生变化时，修改拖动区域
        public IRelayCommand SizeChangedCommand => new RelayCommand<Grid>((appTitleBar) =>
        {
            SetDragRectangles(Convert.ToInt32(appTitleBar.Margin.Left), appTitleBar.ActualWidth, appTitleBar.ActualHeight);
        });

        /// <summary>
        /// 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改标题栏按钮主题
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    SetTitleBarButtonColor(App.AppWindow.TitleBar, GetRegistryTheme());
                }
            });
        }

        /// <summary>
        /// 根据应用设置存储的主题值设置标题栏按钮的颜色
        /// </summary>
        private void SetTitleBarColor(string theme)
        {
            if (theme == ThemeService.ThemeList[0].InternalName)
            {
                SetTitleBarButtonColor(App.AppWindow.TitleBar, GetRegistryTheme());
            }
            else if (theme == ThemeService.ThemeList[1].InternalName)
            {
                SetTitleBarButtonColor(App.AppWindow.TitleBar, ElementTheme.Light);
            }
            else if (theme == ThemeService.ThemeList[2].InternalName)
            {
                SetTitleBarButtonColor(App.AppWindow.TitleBar, ElementTheme.Dark);
            }
        }

        /// <summary>
        /// 设置标题栏按钮的颜色
        /// </summary>
        private void SetTitleBarButtonColor(AppWindowTitleBar bar, ElementTheme theme)
        {
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
            var windowHandle = WindowNative.GetWindowHandle(App.MainWindow);
            var currentDpi = PInvoke.User32.GetDpiForWindow(windowHandle);
            return Convert.ToInt32(pixel * (currentDpi / 96.0));
        }

        /// <summary>
        /// 获取系统注册表存储的应用主题值
        /// </summary>
        private ElementTheme GetRegistryTheme()
        {
            RegistryKey PersonalizeKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            int value = Convert.ToInt32(PersonalizeKey.GetValue("AppsUseLightTheme", null));

            return value == 0 ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}
