using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Models.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.ViewManagement;

namespace GetStoreApp.ViewModels.Window
{
    /// <summary>
    /// 应用主窗口数据模型
    /// </summary>
    public sealed class MainWindowViewModel : ViewModelBase
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

        private SolidColorBrush _windowBackground;

        public SolidColorBrush WindowBackground
        {
            get { return _windowBackground; }

            set
            {
                _windowBackground = value;
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

        private Dictionary<string, Type> PageDict { get; } = new Dictionary<string, Type>()
        {
            {"Store",typeof(StorePage) },
            {"WinGet",typeof(WinGetPage) },
            {"History",typeof(HistoryPage) },
            {"Download",typeof(DownloadPage) },
            {"Web",typeof(WebPage) },
            {"About",typeof(AboutPage) },
            {"Settings",typeof(SettingsPage) }
        };

        public List<string> TagList { get; } = new List<string>()
        {
            "Store",
            "WinGet",
            "History",
            "Download",
            "Web",
            "About",
            "Settings"
        };

        /// <summary>
        /// 设置窗口处于非激活状态时的背景色
        /// </summary>
        public void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[1].InternalName || BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[2].InternalName)
            {
                MicaSystemBackdrop micaBackdrop = SystemBackdrop as MicaSystemBackdrop;

                if (micaBackdrop is not null && micaBackdrop.BackdropConfiguration is not null)
                {
                    if (AlwaysShowBackdropService.AlwaysShowBackdropValue)
                    {
                        micaBackdrop.BackdropConfiguration.IsInputActive = true;
                    }
                    else
                    {
                        micaBackdrop.BackdropConfiguration.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;
                    }
                }
            }
            else if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[3].InternalName)
            {
                DesktopAcrylicSystemBackdrop desktopAcrylicSystemBackdrop = SystemBackdrop as DesktopAcrylicSystemBackdrop;

                if (desktopAcrylicSystemBackdrop is not null && desktopAcrylicSystemBackdrop.BackdropConfiguration is not null)
                {
                    if (AlwaysShowBackdropService.AlwaysShowBackdropValue)
                    {
                        desktopAcrylicSystemBackdrop.BackdropConfiguration.IsInputActive = true;
                    }
                    else
                    {
                        desktopAcrylicSystemBackdrop.BackdropConfiguration.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;
                    }
                }
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        public async void OnClosed(object sender, WindowEventArgs args)
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
                        Program.ApplicationRoot.ViewModel.Dispose();
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
                    Program.ApplicationRoot.ViewModel.Dispose();
                }
            }
        }

        /// <summary>
        /// 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, SetWindowBackground);
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
        public void OnFrameNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            throw new ApplicationException(string.Format(ResourceService.GetLocalized("/Window/NavigationFailed"), args.SourcePageType.FullName));
        }

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        public void OnNavigationViewBackRequested(object sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.NavigationFrom();
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航视图控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        public void OnNavigationViewLoaded(object sender, RoutedEventArgs args)
        {
            PropertyChanged += OnPropertyChanged;
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
            NavigationService.NavigateTo(typeof(StorePage));
            IsBackEnabled = NavigationService.CanGoBack();
        }

        /// <summary>
        /// 窗口被卸载时，注销所有事件
        /// </summary>
        public void OnNavigationViewUnLoaded(object sender, RoutedEventArgs args)
        {
            AppUISettings.ColorValuesChanged -= OnColorValuesChanged;
            PropertyChanged -= OnPropertyChanged;
        }

        /// <summary>
        /// 可通知的属性发生更改时的事件处理
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(WindowTheme) || args.PropertyName == nameof(SystemBackdrop))
            {
                Program.ApplicationRoot.TrayMenuWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, SetWindowBackground);
            }
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        public void OnTapped(object sender, TappedRoutedEventArgs args)
        {
            NavigationViewItem navigationViewItem = sender as NavigationViewItem;
            if (navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationService.NavigationItemList.Find(item => item.NavigationTag == Convert.ToString(navigationViewItem.Tag));
                if (SelectedItem != navigationItem.NavigationItem)
                {
                    NavigationService.NavigateTo(navigationItem.NavigationPage);
                }
            }
        }

        /// <summary>
        /// 设置应用的背景主题色和控件的背景色
        /// </summary>
        public void SetSystemBackdrop(string backdropName)
        {
            switch (backdropName)
            {
                case "Mica":
                    {
                        SystemBackdrop = new MicaSystemBackdrop() { Kind = MicaKind.Base };
                        break;
                    }
                case "MicaAlt":
                    {
                        SystemBackdrop = new MicaSystemBackdrop() { Kind = MicaKind.BaseAlt };
                        break;
                    }
                case "Acrylic":
                    {
                        SystemBackdrop = new DesktopAcrylicSystemBackdrop();
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
        /// 应用背景色设置跟随系统发生变化时，修改控件的背景值
        /// </summary>
        public void SetWindowBackground()
        {
            if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[0].InternalName)
            {
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                    {
                        WindowBackground = ResourceDictionaryHelper.WindowChromeDict["WindowLightBrush"] as SolidColorBrush;
                    }
                    else
                    {
                        WindowBackground = ResourceDictionaryHelper.WindowChromeDict["WindowDarkBrush"] as SolidColorBrush;
                    }
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    WindowBackground = ResourceDictionaryHelper.WindowChromeDict["WindowLightBrush"] as SolidColorBrush;
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
                {
                    WindowBackground = ResourceDictionaryHelper.WindowChromeDict["WindowDarkBrush"] as SolidColorBrush;
                }
            }
            else
            {
                WindowBackground = ResourceDictionaryHelper.WindowChromeDict["WindowSystemBackdropBrush"] as SolidColorBrush;
            }
        }
    }
}
