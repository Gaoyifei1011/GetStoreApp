using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Controls.Download;
using GetStoreApp.Contracts.Controls.Settings.Advanced;
using GetStoreApp.Contracts.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Messages;
using GetStoreApp.Models.Window;
using GetStoreApp.UI.Dialogs.ContentDialogs.Common;
using GetStoreApp.Views.Pages;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;

namespace GetStoreApp.ViewModels.Window
{
    public class MainWindowViewModel : ObservableRecipient
    {
        private IAria2Service Aria2Service { get; } = ContainerHelper.GetInstance<IAria2Service>();

        private IAppExitService AppExitService { get; } = ContainerHelper.GetInstance<IAppExitService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = ContainerHelper.GetInstance<IDownloadSchedulerService>();

        private IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        private IBackdropService BackdropService { get; } = ContainerHelper.GetInstance<IBackdropService>();

        private IAlwaysShowBackdropService AlwaysShowBackdropService { get; } = ContainerHelper.GetInstance<IAlwaysShowBackdropService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        private SolidColorBrush _appBackground;

        public SolidColorBrush AppBackground
        {
            get { return _appBackground; }

            set { SetProperty(ref _appBackground, value); }
        }

        private Thickness _appTitleBarMargin;

        public Thickness AppTitleBarMargin
        {
            get { return _appTitleBarMargin; }

            set { SetProperty(ref _appTitleBarMargin, value); }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set { SetProperty(ref _isBackEnabled, value); }
        }

        private bool _isPaneToggleButtonVisible;

        public bool IsPaneToggleButtonVisible
        {
            get { return _isPaneToggleButtonVisible; }

            set { SetProperty(ref _isPaneToggleButtonVisible, value); }
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

        private Dictionary<string, Type> PageDict { get; } = new Dictionary<string, Type>()
        {
            {"Home",typeof(HomePage) },
            {"History",typeof(HistoryPage) },
            {"Download",typeof(DownloadPage) },
            {"Web",typeof(WebPage) },
            {"About",typeof(AboutPage) },
            {"Settings",typeof(SettingsPage) }
        };

        // 窗口加载完成时，初始化导航视图控件属性和应用的背景色
        public IRelayCommand LoadedCommand => new RelayCommand(() =>
        {
            if (App.MainWindow.Width >= 768)
            {
                IsPaneToggleButtonVisible = false;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.Left;
                AppTitleBarMargin = new Thickness(48, 0, 0, 0);
            }
            else
            {
                IsPaneToggleButtonVisible = true;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.LeftMinimal;
                AppTitleBarMargin = new Thickness(96, 0, 0, 0);
            }

            SetAppBackground();

            ((FrameworkElement)App.MainWindow.Content).ActualThemeChanged += WindowThemeChanged;

            // 应用背景色设置跟随系统发生变化时，当系统背景色设置发生变化时修改应用背景色
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, BackdropChangedMessage>(this, (appTitleBarViewModel, backdropChangedMessage) =>
            {
                SetAppBackground();
            });

            // 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, SystemSettingsChnagedMessage>(this, (appTitleBarViewModel, systemSettingsChangedMessage) =>
            {
                SetAppBackground();
            });
        });

        // 页面被卸载时，注销所有事件
        public IRelayCommand UnloadedCommand => new RelayCommand(() =>
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        });

        // 窗口大小发生改变时，设置页面导航视图样式
        public IRelayCommand SizeChangedCommand => new RelayCommand(() =>
        {
            if (App.MainWindow.Width >= 768)
            {
                IsPaneToggleButtonVisible = false;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.Left;
                AppTitleBarMargin = new Thickness(48, 0, 0, 0);
            }
            else
            {
                IsPaneToggleButtonVisible = true;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.LeftMinimal;
                AppTitleBarMargin = new Thickness(96, 0, 0, 0);
            }
        });

        // 当后退按钮收到交互（如单击或点击）时发生。
        public IRelayCommand BackRequestedCommand => new RelayCommand(NavigationService.NavigationFrom);

        // 当菜单中的项收到交互（如单击或点击）时发生。
        public IRelayCommand ItemInvokedCommand => new RelayCommand<NavigationViewItemInvokedEventArgs>((args) =>
        {
            NavigationModel navigationViewItem = NavigationService.NavigationItemList.Find(item => item.NavigationTag == Convert.ToString(args.InvokedItemContainer.Tag));
            if (SelectedItem != navigationViewItem.NavigationItem)
            {
                NavigationService.NavigateTo(navigationViewItem.NavigationPage);
            }
        });

        // 导航完成后发生
        public IRelayCommand NavigatedCommand => new RelayCommand(() =>
        {
            Type CurrentPageType = NavigationService.GetCurrentPageType();
            SelectedItem = NavigationService.NavigationItemList.Find(item => item.NavigationPage == CurrentPageType).NavigationItem;
            IsBackEnabled = NavigationService.CanGoBack();
        });

        // 导航失败时发生
        public IRelayCommand NavigationFailedCommand => new RelayCommand<NavigationFailedEventArgs>((args) =>
        {
            throw new Exception("页面" + args.SourcePageType.FullName + "加载失败");
        });

        /// <summary>
        /// 设置窗口处于非激活状态时的背景色
        /// </summary>
        public void WindowActivated(object sender, WindowActivatedEventArgs args)
        {
            BackdropHelper.SetBackdropState(AlwaysShowBackdropService.AlwaysShowBackdropValue, args);
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        public async void WindowClosed(object sender, WindowEventArgs args)
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
                    // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                    if (!App.IsDialogOpening)
                    {
                        App.IsDialogOpening = true;

                        ContentDialogResult result = await new ClosingWindowDialog().ShowAsync();

                        if (result == ContentDialogResult.Primary)
                        {
                            await CloseApp();
                        }
                        else if (result == ContentDialogResult.Secondary)
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadPage));
                            }
                        }

                        App.IsDialogOpening = false;
                    }
                }
                else
                {
                    await CloseApp();
                }
            }
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容
        /// </summary>
        public void NavigationViewLoaded(object sender, RoutedEventArgs args)
        {
            NavigationView navigationView = sender as NavigationView;

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
        }

        /// <summary>
        /// 设置主题发生变化时修改标题栏按钮的主题
        /// </summary>
        private void WindowThemeChanged(FrameworkElement sender, object args)
        {
            SetAppBackground();
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
                    if (RegistryHelper.GetRegistryAppTheme() == ElementTheme.Light)
                    {
                        AppBackground = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                    }
                    else
                    {
                        AppBackground = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                    }
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    AppBackground = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
                {
                    AppBackground = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                }
            }
            else
            {
                AppBackground = new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// 关闭应用并释放所有资源
        /// </summary>
        private async Task CloseApp()
        {
            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            WeakReferenceMessenger.Default.Send(new WindowClosedMessage(true));
            BackdropHelper.ReleaseBackdrop();
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }
    }
}
