using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.System;
using Windows.UI.ViewManagement;

namespace GetStoreApp.ViewModels.Controls
{
    /// <summary>
    /// 任务栏右键菜单浮出窗口视图模型
    /// </summary>
    public sealed class TrayMenuViewModel : ViewModelBase
    {
        private UISettings AppUISettings { get; } = new UISettings();

        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                _windowTheme = value;
                OnPropertyChanged();
            }
        }

        private Brush _WindowBackground;

        public Brush WindowBackground
        {
            get { return _WindowBackground; }

            set
            {
                _WindowBackground = value;
                OnPropertyChanged();
            }
        }

        private SystemBackdrop _systemBackdrop;

        public SystemBackdrop SystemBackdrop
        {
            get { return _systemBackdrop; }

            set
            {
                _systemBackdrop = value;
                OnPropertyChanged();
            }
        }

        // 项目主页
        public IRelayCommand ProjectDescriptionCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
            Program.ApplicationRoot.TrayMenuWindow.AppWindow.Hide();
        });

        // 打开应用“关于”页面
        public IRelayCommand AboutAppCommand => new RelayCommand(() =>
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                Program.ApplicationRoot.MainWindow.ViewModel.AboutAppCommand.Execute(null);
            });
            Program.ApplicationRoot.TrayMenuWindow.AppWindow.Hide();
        });

        // 显示 / 隐藏窗口
        public IRelayCommand ShowOrHideWindowCommand => new RelayCommand(() =>
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                Program.ApplicationRoot.MainWindow.ViewModel.ShowOrHideWindowCommand.Execute(null);
            });
            Program.ApplicationRoot.TrayMenuWindow.AppWindow.Hide();
        });

        // 打开设置
        public IRelayCommand SettingsCommand => new RelayCommand(() =>
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                Program.ApplicationRoot.MainWindow.ViewModel.SettingsCommand.Execute(null);
            });
            Program.ApplicationRoot.TrayMenuWindow.AppWindow.Hide();
        });

        // 退出应用
        public IRelayCommand ExitCommand => new RelayCommand(() =>
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                Program.ApplicationRoot.MainWindow.ViewModel.ExitCommand.Execute(null);
            });
        });

        // 窗口处于非激活状态时自动隐藏窗口
        public void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                Program.ApplicationRoot.TrayMenuWindow.AppWindow.Hide();
            }
        }

        /// <summary>
        /// 浮出窗口加载完成后初始化内容，初始化导航视图控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            SetWindowBackground();

            ((FrameworkElement)Program.ApplicationRoot.MainWindow.Content).ActualThemeChanged += OnActualThemeChanged;
            AppUISettings.ColorValuesChanged += OnColorValuesChanged;
        }

        /// <summary>
        /// 窗口被卸载时，注销所有事件
        /// </summary>
        public void OnUnloaded(object sender, RoutedEventArgs args)
        {
            AppUISettings.ColorValuesChanged -= OnColorValuesChanged;
            ((FrameworkElement)Program.ApplicationRoot.MainWindow.Content).ActualThemeChanged -= OnActualThemeChanged;
        }

        /// <summary>
        /// 设置主题发生变化时修改标题栏按钮的主题
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetWindowBackground();
        }

        /// <summary>
        /// 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, SetWindowBackground);
        }

        /// <summary>
        /// 设置应用的背景主题色和控件的背景色
        /// </summary>
        public void SetSystemBackdropAndBackground(string backdropName)
        {
            switch (backdropName)
            {
                case "Mica":
                    {
                        SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.Base };
                        break;
                    }
                case "MicaAlt":
                    {
                        SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.BaseAlt };
                        break;
                    }
                case "Acrylic":
                    {
                        SystemBackdrop = new DesktopAcrylicBackdrop();
                        break;
                    }
                default:
                    {
                        SystemBackdrop = null;
                        break;
                    }
            }

            SetWindowBackground();
        }

        /// <summary>
        /// 应用背景色设置跟随系统发生变化时，修改控件的背景值
        /// </summary>
        private void SetWindowBackground()
        {
            if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[0].InternalName)
            {
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                    {
                        WindowBackground = ResourceDictionaryHelper.WindowChromeDict["MenuFlyoutWindowLightBrush"] as AcrylicBrush;
                    }
                    else
                    {
                        WindowBackground = ResourceDictionaryHelper.WindowChromeDict["MenuFlyoutWindowDarkBrush"] as AcrylicBrush;
                    }
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    WindowBackground = ResourceDictionaryHelper.WindowChromeDict["MenuFlyoutWindowLightBrush"] as AcrylicBrush;
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
                {
                    WindowBackground = ResourceDictionaryHelper.WindowChromeDict["MenuFlyoutWindowDarkBrush"] as AcrylicBrush;
                }
            }
            else
            {
                WindowBackground = ResourceDictionaryHelper.WindowChromeDict["WindowSystemBackdropBrush"] as SolidColorBrush;
            }
        }
    }
}
