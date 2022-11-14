using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Download;
using GetStoreApp.Contracts.Services.Controls.Settings.Advanced;
using GetStoreApp.Contracts.Services.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.DataType.Events;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Messages;
using GetStoreApp.Models.Window;
using GetStoreApp.UI.Dialogs.ContentDialogs.Common;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Window
{
    public class MainWindowViewModel : ObservableRecipient
    {
        private IAria2Service Aria2Service { get; } = ContainerHelper.GetInstance<IAria2Service>();

        private IAppExitService AppExitService { get; } = ContainerHelper.GetInstance<IAppExitService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = ContainerHelper.GetInstance<IDownloadSchedulerService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

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

        // 窗口加载完成时，初始化导航视图控件属性
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
        public IRelayCommand BackRequestedCommand => new RelayCommand(() =>
        {
            NavigationService.NavigationFrom();
        });

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
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        public async void WindowClosing(object sender, WindowClosingEventArgs args)
        {
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
        /// 关闭应用并释放所有资源
        /// </summary>
        private async Task CloseApp()
        {
            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            WeakReferenceMessenger.Default.Send(new WindowClosedMessage(true));
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }
    }
}
