using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Models.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;

namespace GetStoreApp.ViewModels.Window
{
    /// <summary>
    /// 应用主窗口数据模型
    /// </summary>
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private UISettings AppUISettings { get; } = new UISettings();

        private SolidColorBrush _appBackground;

        public SolidColorBrush AppBackground
        {
            get { return _appBackground; }

            set
            {
                _appBackground = value;
                OnPropertyChanged();
            }
        }

        private Thickness _appTitleBarMargin;

        public Thickness AppTitleBarMargin
        {
            get { return _appTitleBarMargin; }

            set
            {
                _appTitleBarMargin = value;
                OnPropertyChanged();
            }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                _isBackEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isPaneToggleButtonVisible;

        public bool IsPaneToggleButtonVisible
        {
            get { return _isPaneToggleButtonVisible; }

            set
            {
                _isPaneToggleButtonVisible = value;
                OnPropertyChanged();
            }
        }

        private NavigationViewItem _selectedItem;

        public NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, Type> PageDict { get; } = new Dictionary<string, Type>()
        {
            {"Home",typeof(HomePage) },
            {"History",typeof(HistoryPage) },
            {"Download",typeof(DownloadPage) },
            {"Web",typeof(WebPage) },
            {"About",typeof(AboutPage) },
            {"Settings",typeof(SettingsPage) }
        };

        // 设置窗口处于非激活状态时的背景色
        public IRelayCommand ActivatedCommand => new RelayCommand<WindowActivatedEventArgs>((args) =>
        {
            BackdropHelper.SetBackdropState(AlwaysShowBackdropService.AlwaysShowBackdropValue, args);
        });

        // 关闭窗口之后关闭其他服务
        public IRelayCommand ClosedCommand => new RelayCommand<WindowEventArgs>(async (args) =>
        {
            args.Handled = true;

            if (AppExitService.AppExit.InternalName == AppExitService.AppExitList[0].InternalName)
            {
                WindowHelper.HideAppWindow();
            }
            else
            {
                // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
                if (DownloadSchedulerService.DownloadingList.Count > 0 || DownloadSchedulerService.WaitingList.Count > 0)
                {
                    WindowHelper.ShowAppWindow();

                    // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                    ContentDialogResult result = await new ClosingWindowDialog().ShowAsync();

                    if (result is ContentDialogResult.Primary)
                    {
                        await Program.ApplicationRoot.ViewModel.CloseAppAsync();
                    }
                    else if (result is ContentDialogResult.Secondary)
                    {
                        if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                        {
                            NavigationService.NavigateTo(typeof(DownloadPage));
                        }
                    }
                }
                else
                {
                    await Program.ApplicationRoot.ViewModel.CloseAppAsync();
                }
            }
        });

        // 当菜单中的项收到交互（如单击或点击）时发生
        public IRelayCommand NavigationItemCommand => new RelayCommand<object>((invokedItemTag) =>
        {
            if (invokedItemTag is not null)
            {
                NavigationModel navigationViewItem = NavigationService.NavigationItemList.Find(item => item.NavigationTag == Convert.ToString(invokedItemTag));
                if (SelectedItem != navigationViewItem.NavigationItem)
                {
                    NavigationService.NavigateTo(navigationViewItem.NavigationPage);
                }
            }
        });

        // 打开应用“关于”页面
        public IRelayCommand AboutAppCommand => new RelayCommand(() =>
        {
            // 窗口置前端
            WindowHelper.ShowAppWindow();

            if (NavigationService.GetCurrentPageType() != typeof(AboutPage))
            {
                NavigationService.NavigateTo(typeof(AboutPage));
            }
        });

        // 显示 / 隐藏窗口
        public IRelayCommand ShowOrHideWindowCommand => new RelayCommand(() =>
        {
            // 隐藏窗口
            if (WindowHelper.IsWindowVisible)
            {
                WindowHelper.HideAppWindow();
            }
            // 显示窗口
            else
            {
                WindowHelper.ShowAppWindow();
            }
        });

        // 打开设置
        public IRelayCommand SettingsCommand => new RelayCommand(() =>
        {
            // 窗口置前端
            WindowHelper.ShowAppWindow();

            if (NavigationService.GetCurrentPageType() != typeof(SettingsPage))
            {
                NavigationService.NavigateTo(typeof(SettingsPage));
            }
        });

        // 退出应用
        public IRelayCommand ExitCommand => new RelayCommand(Program.ApplicationRoot.MainWindow.Close);

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        public void OnNavigationViewBackRequested(object sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.NavigationFrom();
        }

        // 导航视图显示的样式发生改变时发生
        public void OnDisplayModeChanged(object sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            if (args.DisplayMode == NavigationViewDisplayMode.Minimal)
            {
                IsPaneToggleButtonVisible = true;
                AppTitleBarMargin = new Thickness(96, 0, 0, 0);
            }
            else
            {
                IsPaneToggleButtonVisible = false;
                AppTitleBarMargin = new Thickness(48, 0, 0, 0);
            }
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航视图控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        public void OnNavigationViewLoaded(object sender, RoutedEventArgs args)
        {
            SetAppBackground();

            ((FrameworkElement)Program.ApplicationRoot.MainWindow.Content).ActualThemeChanged += OnActualThemeChanged;
            AppUISettings.ColorValuesChanged += OnColorValuesChanged;

            // 导航控件加载完成后初始化内容

            if (sender is not NavigationView navigationView || sender is null)
            {
                return;
            }

            //初始化导航视图控件属性和应用的背景色
            if (navigationView.DisplayMode == NavigationViewDisplayMode.Minimal)
            {
                IsPaneToggleButtonVisible = true;
                AppTitleBarMargin = new Thickness(96, 0, 0, 0);
            }
            else
            {
                IsPaneToggleButtonVisible = false;
                AppTitleBarMargin = new Thickness(48, 0, 0, 0);
            }

            foreach (object item in navigationView.MenuItems)
            {
                NavigationViewItem navigationViewItem = item as NavigationViewItem;
                if (navigationViewItem is not null)
                {
                    string Tag = Convert.ToString(navigationViewItem.Tag);

                    NavigationService.NavigationItemList.Add(new NavigationModel()
                    {
                        NavigationTag = Tag,
                        NavigationItem = navigationViewItem,
                        NavigationPage = PageDict[Tag],
                    });
                }
            }

            foreach (object item in navigationView.FooterMenuItems)
            {
                NavigationViewItem navigationViewItem = item as NavigationViewItem;
                if (navigationViewItem is not null)
                {
                    string Tag = Convert.ToString(navigationViewItem.Tag);

                    NavigationService.NavigationItemList.Add(new NavigationModel()
                    {
                        NavigationTag = Tag,
                        NavigationItem = navigationViewItem,
                        NavigationPage = PageDict[Tag],
                    });
                }
            }

            SelectedItem = NavigationService.NavigationItemList[0].NavigationItem;
            NavigationService.NavigateTo(typeof(HomePage));
            IsBackEnabled = NavigationService.CanGoBack();

            // 应用背景色设置跟随系统发生变化时，当系统背景色设置发生变化时修改应用背景色
            Messenger.Default.Register<BackdropModel>(this, MessageToken.BackdropChanged, (backdropMessage) =>
            {
                SetAppBackground();
            });
        }

        /// <summary>
        /// 页面被卸载时，注销所有事件
        /// </summary>
        public void OnNavigationViewUnLoaded(object sender, RoutedEventArgs args)
        {
            AppUISettings.ColorValuesChanged -= OnColorValuesChanged;
            Messenger.Default.Unregister(this);
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        public void OnFrameNavigated(object sender, NavigationEventArgs args)
        {
            Type CurrentPageType = NavigationService.GetCurrentPageType();
            SelectedItem = NavigationService.NavigationItemList.Find(item => item.NavigationPage == CurrentPageType).NavigationItem;
            IsBackEnabled = NavigationService.CanGoBack();
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        public void OnFrameNavgationFailed(object sender, NavigationFailedEventArgs args)
        {
            throw new ApplicationException(string.Format(ResourceService.GetLocalized("/Window/NavigationFailed"), args.SourcePageType.FullName));
        }

        /// <summary>
        /// 设置主题发生变化时修改标题栏按钮的主题
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetAppBackground();
        }

        /// <summary>
        /// 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, SetAppBackground);
        }

        /// <summary>
        /// 设置应用背景色
        /// </summary>
        private void SetAppBackground()
        {
            if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[0].InternalName)
            {
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                    {
                        AppBackground = new SolidColorBrush(ColorHelper.FromArgb(255, 240, 243, 249));
                    }
                    else
                    {
                        AppBackground = new SolidColorBrush(ColorHelper.FromArgb(255, 20, 20, 20));
                    }
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    AppBackground = new SolidColorBrush(ColorHelper.FromArgb(255, 240, 243, 249));
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
                {
                    AppBackground = new SolidColorBrush(ColorHelper.FromArgb(255, 20, 20, 20));
                }
            }
            else
            {
                AppBackground = new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}
