using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls
{
    /// <summary>
    /// 任务栏右键菜单浮出窗口视图模型
    /// </summary>
    public sealed class TrayMenuViewModel : ViewModelBase
    {
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

        /// <summary>
        /// 打开应用的项目主页
        /// </summary>
        public async void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        }

        /// <summary>
        /// 打开应用“关于”页面
        /// </summary>
        public void OnAboutAppClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                // 窗口置前端
                Program.ApplicationRoot.MainWindow.Show();

                if (NavigationService.GetCurrentPageType() != typeof(AboutPage))
                {
                    NavigationService.NavigateTo(typeof(AboutPage));
                }
            });
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        public void OnExitClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(Program.ApplicationRoot.MainWindow.Close);
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        public void OnSettingsClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                // 窗口置前端
                Program.ApplicationRoot.MainWindow.Show();

                if (NavigationService.GetCurrentPageType() != typeof(SettingsPage))
                {
                    NavigationService.NavigateTo(typeof(SettingsPage));
                }
            });
        }

        /// <summary>
        /// 显示 / 隐藏窗口
        /// </summary>
        public void OnShowOrHideWindowClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                // 隐藏窗口
                if (Program.ApplicationRoot.MainWindow.Visible)
                {
                    Program.ApplicationRoot.MainWindow.AppWindow.Hide();
                }
                // 显示窗口
                else
                {
                    Program.ApplicationRoot.MainWindow.Show();
                }
            });
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
    }
}
