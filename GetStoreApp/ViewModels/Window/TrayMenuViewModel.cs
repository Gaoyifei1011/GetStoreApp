using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.ComponentModel;
using Windows.System;
using Windows.UI.ViewManagement;

namespace GetStoreApp.ViewModels.Controls
{
    /// <summary>
    /// 任务栏右键菜单浮出窗口视图模型
    /// </summary>
    public sealed class TrayMenuViewModel : ViewModelBase
    {
        private UISettings AppUISettings = new UISettings();

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
            AppUISettings.ColorValuesChanged += OnColorValuesChanged;
            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// 窗口被卸载时，注销所有事件
        /// </summary>
        public void OnUnloaded(object sender, RoutedEventArgs args)
        {
            AppUISettings.ColorValuesChanged -= OnColorValuesChanged;
            PropertyChanged -= OnPropertyChanged;
        }

        /// <summary>
        /// 设置主题发生变化时修改标题栏按钮的主题
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            if (ThemeService.NotifyIconMenuTheme.InternalName == ThemeService.NotifyIconMenuThemeList[0].InternalName)
            {
                Program.ApplicationRoot.TrayMenuWindow.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, SetWindowBackground);
            }
        }

        /// <summary>
        /// 可通知的属性发生更改时的事件处理
        /// </summary>
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(WindowTheme) || args.PropertyName == nameof(SystemBackdrop))
            {
                Program.ApplicationRoot.TrayMenuWindow.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, SetWindowBackground);
            }
        }

        /// <summary>
        /// 设置应用的背景主题色
        /// </summary>
        public void SetSystemBackdrop(string backdropName)
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
        }

        /// <summary>
        /// 应用背景色设置跟随系统发生变化时，修改控件的背景色值
        /// </summary>
        private void SetWindowBackground()
        {
            if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[0].InternalName)
            {
                if (WindowTheme == ElementTheme.Default)
                {
                    if (Application.Current.RequestedTheme == ApplicationTheme.Light)
                    {
                        WindowBackground = ResourceDictionaryHelper.WindowChromeDict["MenuFlyoutWindowLightBrush"] as AcrylicBrush;
                    }
                    else
                    {
                        WindowBackground = ResourceDictionaryHelper.WindowChromeDict["MenuFlyoutWindowDarkBrush"] as AcrylicBrush;
                    }
                }
                else if (WindowTheme == ElementTheme.Light)
                {
                    WindowBackground = ResourceDictionaryHelper.WindowChromeDict["MenuFlyoutWindowLightBrush"] as AcrylicBrush;
                }
                else if (WindowTheme == ElementTheme.Dark)
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
