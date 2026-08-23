using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store.Preview;
using Windows.Foundation.Diagnostics;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Shell;
using Windows.UI.StartScreen;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置项页面
    /// </summary>
    internal sealed partial class SettingsItemPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private bool needNavigate;
        private Type navigateType;
        private object navigateParameter;
        private bool? slideDirection;
        private bool canScrollHorizontally;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private bool _isPreviousEnabled;

        private bool IsPreviousEnabled
        {
            get { return _isPreviousEnabled; }

            set
            {
                if (!Equals(_isPreviousEnabled, value))
                {
                    _isPreviousEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsPreviousEnabled)));
                }
            }
        }

        private bool _isNextEnabled;

        private bool IsNextEnabled
        {
            get { return _isNextEnabled; }

            set
            {
                if (!Equals(_isNextEnabled, value))
                {
                    _isNextEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsNextEnabled)));
                }
            }
        }

        private SelectorBarItem _selectedItem;

        private SelectorBarItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedItem)));
                }
            }
        }

        internal List<Type> PageList { get; } = [typeof(SettingsGeneralPage), typeof(SettingsStoreAndUpdatePage), typeof(SettingsWinGetPage), typeof(SettingsDownloadPage), typeof(SettingsAppInstallPage), typeof(SettingsAdvancedPage), typeof(SettingsAboutPage)];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal SettingsItemPage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            SettingsItemFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            if (args.Parameter is AppNaviagtionArgs.Download)
            {
                if (!Equals(GetCurrentPageType(), PageList[3]))
                {
                    NavigateTo(PageList[3]);
                }
            }
            else if (args.Parameter is AppNaviagtionArgs.AppInstall)
            {
                if (!Equals(GetCurrentPageType(), PageList[4]))
                {
                    NavigateTo(PageList[4]);
                }
            }
            else
            {
                // 第一次导航
                if (GetCurrentPageType() is null)
                {
                    NavigateTo(PageList[0]);
                }
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 设置项页面加载完成后触发的事件
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            canScrollHorizontally = SettingsScrollViewer.ExtentWidth > SettingsScrollViewer.ViewportWidth;
            IsPreviousEnabled = false;
            IsNextEnabled = false;

            if (needNavigate)
            {
                NavigateTo(navigateType, navigateParameter, slideDirection);
                needNavigate = false;
                navigateType = null;
                navigateParameter = null;
                slideDirection = null;
            }
        }

        /// <summary>
        /// 鼠标进入后触发的事件
        /// </summary>
        private void OnSelectorBarPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            if (canScrollHorizontally)
            {
                if (SettingsScrollViewer.HorizontalOffset <= 0)
                {
                    IsPreviousEnabled = false;
                    IsNextEnabled = true;
                }
                else if (SettingsScrollViewer.HorizontalOffset >= SettingsScrollViewer.ScrollableWidth)
                {
                    IsPreviousEnabled = true;
                    IsNextEnabled = false;
                }
                else
                {
                    IsPreviousEnabled = true;
                    IsNextEnabled = true;
                }
            }
        }

        /// <summary>
        /// 鼠标退出后触发的事件
        /// </summary>
        private void OnSelectorBarPointerExited(object sender, PointerRoutedEventArgs args)
        {
            IsPreviousEnabled = false;
            IsNextEnabled = false;
        }

        /// <summary>
        /// 大小发生变化后触发的事件
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            canScrollHorizontally = SettingsScrollViewer.ExtentWidth > SettingsScrollViewer.ViewportWidth;
            IsPreviousEnabled = false;
            IsNextEnabled = false;
        }

        /// <summary>
        /// 当滚动和缩放等操作导致视图更改时发生的事件
        /// </summary>
        private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs args)
        {
            if (canScrollHorizontally)
            {
                if (SettingsScrollViewer.HorizontalOffset <= 0)
                {
                    IsPreviousEnabled = false;
                    IsNextEnabled = true;
                }
                else if (SettingsScrollViewer.HorizontalOffset >= SettingsScrollViewer.ScrollableWidth)
                {
                    IsPreviousEnabled = true;
                    IsNextEnabled = false;
                }
                else
                {
                    IsPreviousEnabled = true;
                    IsNextEnabled = true;
                }
            }
        }

        /// <summary>
        /// 向前移动
        /// </summary>
        private void OnPreviousClick(object sender, RoutedEventArgs args)
        {
            SettingsScrollViewer.ChangeView(SettingsScrollViewer.HorizontalOffset < 150 ? 0 : SettingsScrollViewer.HorizontalOffset - 150, null, null);
        }

        /// <summary>
        /// 向后移动
        /// </summary>
        private void OnNextClick(object sender, RoutedEventArgs args)
        {
            SettingsScrollViewer.ChangeView(SettingsScrollViewer.HorizontalOffset >= SettingsScrollViewer.ScrollableWidth - 150 ? SettingsScrollViewer.ScrollableWidth : SettingsScrollViewer.HorizontalOffset + 150, null, null);
        }

        /// <summary>
        /// 点击选择器栏选中项发生变化时发生的事件
        /// </summary>
        private void OnSelectorBarSelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            if (!Equals(SelectedItem, sender.SelectedItem))
            {
                SelectedItem = sender.SelectedItem;
            }

            if (SelectedItem is null)
            {
                return;
            }

            int index = sender.Items.IndexOf(SelectedItem);
            Type currentPage = GetCurrentPageType();
            int currentIndex = PageList.FindIndex(item => Equals(item, currentPage));

            if (index is 0)
            {
                if (currentPage is null)
                {
                    NavigateTo(PageList[0]);
                }
                else if (!Equals(currentPage, PageList[0]))
                {
                    NavigateTo(PageList[0], null, index > currentIndex);
                }
            }
            else if (index is 1 && !Equals(GetCurrentPageType(), PageList[1]))
            {
                NavigateTo(PageList[1], null, index > currentIndex);
            }
            else if (index is 2 && !Equals(GetCurrentPageType(), PageList[2]))
            {
                NavigateTo(PageList[2], null, index > currentIndex);
            }
            else if (index is 3 && !Equals(GetCurrentPageType(), PageList[3]))
            {
                NavigateTo(PageList[3], null, index > currentIndex);
            }
            else if (index is 4 && !Equals(GetCurrentPageType(), PageList[4]))
            {
                NavigateTo(PageList[4], null, index > currentIndex);
            }
            else if (index is 5 && !Equals(GetCurrentPageType(), PageList[5]))
            {
                NavigateTo(PageList[5], null, index > currentIndex);
            }
            else if (index is 6 && !Equals(GetCurrentPageType(), PageList[6]))
            {
                NavigateTo(PageList[6], null, index > currentIndex);
            }
        }

        /// <summary>
        /// 导航完成后发生的事件
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < SettingsItemSelectorBar.Items.Count)
            {
                SelectedItem = SettingsItemSelectorBar.Items[index];
            }
        }

        /// <summary>
        /// 导航失败后发生的事件
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < SettingsItemSelectorBar.Items.Count)
            {
                SelectedItem = SettingsItemSelectorBar.Items[index];
            }

            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsItemPage), nameof(OnNavigationFailed), 1, args.Exception);
        }

        /// <summary>
        /// 打开重启应用确认的窗口对话框
        /// </summary>
        private async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new RestartAppsDialog());
        }

        /// <summary>
        /// 设置说明
        /// </summary>
        private void OnSettingsInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                settingsPage.ShowSettingsInstruction();
            }
        }

        /// <summary>
        /// 以管理员身份运行
        /// </summary>
        private async void OnRunAsAdministratorClicked(object sender, RoutedEventArgs args)
        {
            await RunAsAdministartorAsync();
        }

        /// <summary>
        /// 创建应用的桌面快捷方式
        /// </summary>
        private async void OnPinToDesktopClicked(object sender, RoutedEventArgs args)
        {
            bool isCreatedSuccessfully = await PinToDestkopAsync();
            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Desktop, isCreatedSuccessfully));
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        private async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool isPinnedSuccessfully = await PinToStartScreenAsync();
            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.StartScreen, isPinnedSuccessfully));
        }

        /// <summary>
        /// 将应用固定到任务栏
        /// </summary>
        private async void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            (bool needUnlock, LimitedAccessFeatureStatus limitedAccessFeatureStatus, bool isPinnedSuccessfully) pinnedResult = await PinToTaskbarAsync();

            if (!RuntimeHelper.IsElevated)
            {
                if (pinnedResult.needUnlock)
                {
                    if (pinnedResult.limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Available || pinnedResult.limitedAccessFeatureStatus is LimitedAccessFeatureStatus.AvailableWithoutToken)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Taskbar, pinnedResult.isPinnedSuccessfully));
                    }
                }
                else
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Taskbar, pinnedResult.isPinnedSuccessfully));
                }
            }
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        private async Task RunAsAdministartorAsync()
        {
            int result = await Task.Run(() =>
            {
                return Shell32Library.ShellExecute(nint.Zero, "runas", Path.Combine(InfoHelper.UserDataPath.LocalAppData, @"Microsoft\WindowsApps", Package.Current.Id.FamilyName, Path.GetFileName(Environment.ProcessPath)), null, null, WindowShowStyle.SW_SHOWNORMAL);
            });

            //返回值大于 32 代表函数执行成功
            if (result > 32)
            {
                Program.AppInstance.UnregisterKey();
                (Application.Current as MainApp).Dispose();
            }
        }

        /// <summary>
        /// 固定到桌面
        /// </summary>
        private async Task<bool> PinToDestkopAsync()
        {
            return await Task.Run(() =>
            {
                bool isCreatedSuccessfully = false;

                try
                {
                    if (StoreConfiguration.IsPinToDesktopSupported())
                    {
                        StoreConfiguration.PinToDesktop(Package.Current.Id.FamilyName);
                        isCreatedSuccessfully = true;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsItemPage), nameof(PinToDestkopAsync), 1, e);
                }

                return isCreatedSuccessfully;
            });
        }

        /// <summary>
        /// 固定到开始屏幕
        /// </summary>
        private async Task<bool> PinToStartScreenAsync()
        {
            return await Task.Run(async () =>
            {
                bool isPinnedSuccessfully = false;

                try
                {
                    IReadOnlyList<AppListEntry> appEntries = await Package.Current.GetAppListEntriesAsync();

                    if (appEntries[0] is AppListEntry defaultEntry)
                    {
                        StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                        isPinnedSuccessfully = await startScreenManager.RequestAddAppListEntryAsync(defaultEntry);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsItemPage), nameof(PinToStartScreenAsync), 1, e);
                }

                return isPinnedSuccessfully;
            });
        }

        /// <summary>
        /// 固定到任务栏
        /// </summary>
        private async Task<(bool, LimitedAccessFeatureStatus, bool)> PinToTaskbarAsync()
        {
            return await Task.Run(async () =>
            {
                LimitedAccessFeatureStatus limitedAccessFeatureStatus = LimitedAccessFeatureStatus.Unknown;
                bool needUnlock = false;
                bool isPinnedSuccessfully = false;

                if (RuntimeHelper.IsElevated)
                {
                    await Launcher.LaunchUriAsync(new("getstoreapppinner:"), new() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new()
                    {
                        {"Type", nameof(TaskbarManager) },
                        { "AppUserModelId", Package.Current.GetAppListEntries()[0].AppUserModelId },
                        { "PackageFullName", Package.Current.Id.FullName },
                    });
                }
                else
                {
                    try
                    {
                        if (ApiInformation.IsTypePresent("Windows.UI.Shell.ITaskbarManagerDesktopAppSupportStatics"))
                        {
                            string feature = "com.microsoft.windows.taskbar.pin";
                            string featureId = FeatureAccessHelper.GetFeatureId(feature);
                            if (!string.IsNullOrEmpty(featureId))
                            {
                                needUnlock = true;
                                string token = FeatureAccessHelper.GenerateTokenFromFeatureId(feature, featureId);
                                string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                                LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);

                                if (accessResult.Status is LimitedAccessFeatureStatus.Available || accessResult.Status is LimitedAccessFeatureStatus.AvailableWithoutToken)
                                {
                                    isPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                                }
                            }
                            else
                            {
                                isPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsItemPage), nameof(PinToTaskbarAsync), 1, e);
                    }

                    if (needUnlock && (limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Unavailable || limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Unknown) && !isPinnedSuccessfully)
                    {
                        await Launcher.LaunchUriAsync(new("getstoreapppinner:"), new() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new()
                        {
                            {"Type", nameof(TaskbarManager) },
                            { "AppUserModelId", Package.Current.GetAppListEntries()[0].AppUserModelId },
                            { "PackageFullName", Package.Current.Id.FullName },
                        });
                    }
                }

                return ValueTuple.Create(needUnlock, limitedAccessFeatureStatus, isPinnedSuccessfully);
            });
        }

        /// <summary>
        /// 页面向前导航
        /// </summary>
        internal void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                SettingsItemFrame.ContentTransitions = slideDirection.HasValue ? slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection : SuppressNavigationTransitionCollection;

                // 导航到该项目对应的页面
                SettingsItemFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsItemPage), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        internal Type GetCurrentPageType()
        {
            return SettingsItemFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 恢复页面默认导航设置
        /// </summary>
        internal void ResetFrameTransition()
        {
            SettingsItemFrame.ContentTransitions = SuppressNavigationTransitionCollection;
        }

        /// <summary>
        /// 设置要导航的内容
        /// </summary>
        internal void SetNavigateContent(bool needNavigate, Type navigateType, object navigateParameter = null, bool? slideDirection = null)
        {
            this.needNavigate = needNavigate;
            this.navigateType = navigateType;
            this.navigateParameter = navigateParameter;
            this.slideDirection = slideDirection;
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}
