using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store.Preview;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Shell;
using Windows.UI.StartScreen;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置项页面
    /// </summary>
    public sealed partial class SettingsItemPage : Page, INotifyPropertyChanged
    {
        private SelectorBarItem _selectedItem;

        public SelectorBarItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        private List<Type> PageList { get; } = [typeof(SettingsGeneralPage), typeof(SettingsStoreAndUpdatePage), typeof(SettingsWinGetPage), typeof(SettingsDownloadPage), typeof(SettingsAppInstallPage), typeof(SettingsAdvancedPage), typeof(SettingsAboutPage)];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsItemPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            SettingsItemFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (GetCurrentPageType() is null)
            {
                NavigateTo(PageList[0], args.Parameter, null);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：设置项页面——挂载的事件

        /// <summary>
        /// 点击选择器栏发生的事件
        /// </summary>
        private void OnSelectorBarTapped(object sender, TappedRoutedEventArgs args)
        {
            if (sender is SelectorBarItem selectorBarItem && selectorBarItem.Tag is Type pageType)
            {
                int index = PageList.IndexOf(pageType);
                int currentIndex = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

                if (index is 0 && !Equals(GetCurrentPageType(), PageList[0]))
                {
                    NavigateTo(PageList[0], null, index > currentIndex);
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
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < SettingsItemSelectorBar.Items.Count)
            {
                SelectedItem = SettingsItemSelectorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < SettingsItemSelectorBar.Items.Count)
            {
                SelectedItem = SettingsItemSelectorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
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
            int result = await Task.Run(() =>
            {
                return Shell32Library.ShellExecute(IntPtr.Zero, "runas", Environment.ProcessPath, null, null, WindowShowStyle.SW_SHOWNORMAL);
            });

            //返回值大于 32 代表函数执行成功
            if (result > 32)
            {
                (Application.Current as WinUIApp).Dispose();
            }
        }

        /// <summary>
        /// 创建应用的桌面快捷方式
        /// </summary>
        private async void OnPinToDesktopClicked(object sender, RoutedEventArgs args)
        {
            bool isCreatedSuccessfully = false;

            await Task.Run(() =>
            {
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsItemPage), nameof(OnPinToDesktopClicked), 1, e);
                }
            });

            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.Desktop, isCreatedSuccessfully));
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        private async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool isPinnedSuccessfully = false;

            await Task.Run(async () =>
            {
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsItemPage), nameof(OnPinToStartScreenClicked), 1, e);
                }
            });

            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.StartScreen, isPinnedSuccessfully));
        }

        /// <summary>
        /// 将应用固定到任务栏
        /// </summary>
        private async void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            (LimitedAccessFeatureStatus limitedAccessFeatureStatus, bool isPinnedSuccessfully) pinnedRsult = await Task.Run(async () =>
            {
                LimitedAccessFeatureStatus limitedAccessFeatureStatus = LimitedAccessFeatureStatus.Unknown;
                bool isPinnedSuccessfully = false;

                if (!RuntimeHelper.IsElevated)
                {
                    try
                    {
                        if (ApiInformation.IsTypePresent("Windows.UI.Shell.ITaskbarManagerDesktopAppSupportStatics"))
                        {
                            string featureId = "com.microsoft.windows.taskbar.pin";
                            string token = FeatureAccessHelper.GenerateTokenFromFeatureId(featureId);
                            string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                            LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);
                            limitedAccessFeatureStatus = accessResult.Status;

                            if (limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Available)
                            {
                                isPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsItemPage), nameof(OnPinToTaskbarClicked), 1, e);
                    }
                }

                if ((limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Unavailable || limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Unknown) && !isPinnedSuccessfully)
                {
                    await Launcher.LaunchUriAsync(new Uri("getstoreapppinner:"), new LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
                        {
                            {"Type", nameof(TaskbarManager) },
                            { "AppUserModelId", Package.Current.GetAppListEntries()[0].AppUserModelId },
                            { "PackageFullName", Package.Current.Id.FullName },
                        });
                }

                return ValueTuple.Create(limitedAccessFeatureStatus, isPinnedSuccessfully);
            });

            if (pinnedRsult.limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Available || pinnedRsult.limitedAccessFeatureStatus is LimitedAccessFeatureStatus.AvailableWithoutToken)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.Taskbar, pinnedRsult.isPinnedSuccessfully));
            }
        }

        #endregion 第二部分：设置项页面——挂载的事件

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    SettingsItemFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

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
        private Type GetCurrentPageType()
        {
            return SettingsItemFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 恢复页面默认导航设置
        /// </summary>
        public void ResetFrameTransition()
        {
            SettingsItemFrame.ContentTransitions = SuppressNavigationTransitionCollection;
        }
    }
}
