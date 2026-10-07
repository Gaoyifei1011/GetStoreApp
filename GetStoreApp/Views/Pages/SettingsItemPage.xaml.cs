using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WinRT;

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

        private readonly string AboutString = ResourceService.GetLocalized("SettingsItem/About");
        private readonly string AdvancedString = ResourceService.GetLocalized("SettingsItem/Advanced");
        private readonly string AppInstallerString = ResourceService.GetLocalized("SettingsItem/AppInstaller");
        private readonly string DownloadString = ResourceService.GetLocalized("SettingsItem/Download");
        private readonly string GeneralString = ResourceService.GetLocalized("SettingsItem/General");
        private readonly string StoreAndUpdateString = ResourceService.GetLocalized("SettingsItem/StoreAndUpdate");
        private readonly string WinGetString = ResourceService.GetLocalized("SettingsItem/WinGet");

        private bool needNavigate;
        private Type navigateType;
        private object navigateParameter;
        private bool? slideDirection;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private int _selectedIndex;

        private int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                if (!Equals(_selectedIndex, value))
                {
                    _selectedIndex = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedIndex)));
                }
            }
        }

        internal ReadOnlyCollection<Type> PageCollection { get; } = [typeof(SettingsGeneralPage), typeof(SettingsStoreAndUpdatePage), typeof(SettingsWinGetPage), typeof(SettingsDownloadPage), typeof(SettingsAppInstallPage), typeof(SettingsAdvancedPage), typeof(SettingsAboutPage)];

        private List<string> SettingsItemTabList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal SettingsItemPage()
        {
            InitializeComponent();
            InitializeData();
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
                if (!Equals(GetCurrentPageType(), PageCollection[3]))
                {
                    NavigateTo(PageCollection[3]);
                }
            }
            else if (args.Parameter is AppNaviagtionArgs.AppInstall)
            {
                if (!Equals(GetCurrentPageType(), PageCollection[4]))
                {
                    NavigateTo(PageCollection[4]);
                }
            }
            else
            {
                // 第一次导航
                if (GetCurrentPageType() is null)
                {
                    NavigateTo(PageCollection[0]);
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
        /// 点击选择器栏选中项发生变化后发生的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(TabView))]
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is TabView tabView && !Equals(SelectedIndex, tabView.SelectedIndex))
            {
                SelectedIndex = tabView.SelectedIndex;
            }

            if (SelectedIndex >= 0 && SelectedIndex < SettingsItemTabList.Count)
            {
                int index = SelectedIndex;
                Type currentPage = GetCurrentPageType();
                int currentIndex = -1;
                for (int i = 0; i < PageCollection.Count; i++)
                {
                    if (Equals(PageCollection[i], currentPage))
                    {
                        currentIndex = i;
                        break;
                    }
                }

                if (index is 0)
                {
                    if (currentPage is null)
                    {
                        NavigateTo(PageCollection[0]);
                    }
                    else if (!Equals(currentPage, PageCollection[0]))
                    {
                        NavigateTo(PageCollection[0], null, index > currentIndex);
                    }
                }
                else if (index is 1 && !Equals(GetCurrentPageType(), PageCollection[1]))
                {
                    NavigateTo(PageCollection[1], null, index > currentIndex);
                }
                else if (index is 2 && !Equals(GetCurrentPageType(), PageCollection[2]))
                {
                    NavigateTo(PageCollection[2], null, index > currentIndex);
                }
                else if (index is 3 && !Equals(GetCurrentPageType(), PageCollection[3]))
                {
                    NavigateTo(PageCollection[3], null, index > currentIndex);
                }
                else if (index is 4 && !Equals(GetCurrentPageType(), PageCollection[4]))
                {
                    NavigateTo(PageCollection[4], null, index > currentIndex);
                }
                else if (index is 5 && !Equals(GetCurrentPageType(), PageCollection[5]))
                {
                    NavigateTo(PageCollection[5], null, index > currentIndex);
                }
                else if (index is 6 && !Equals(GetCurrentPageType(), PageCollection[6]))
                {
                    NavigateTo(PageCollection[6], null, index > currentIndex);
                }
            }
        }

        /// <summary>
        /// 导航完成后发生的事件
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            int index = -1;
            for (int i = 0; i < PageCollection.Count; i++)
            {
                if (Equals(PageCollection[i], GetCurrentPageType()))
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0 && index < SettingsItemTabList.Count)
            {
                SelectedIndex = index;
            }
        }

        /// <summary>
        /// 导航失败后发生的事件
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = -1;
            for (int i = 0; i < PageCollection.Count; i++)
            {
                if (Equals(PageCollection[i], GetCurrentPageType()))
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0 && index < SettingsItemTabList.Count)
            {
                SelectedIndex = index;
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
        private async void OnSettingsInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                await settingsPage.ShowSettingsInstructionAsync(false);
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
            (bool needUnlock, LimitedAccessFeatureStatus limitedAccessFeatureStatus, bool isPinnedSuccessfully) = await PinToTaskbarAsync();

            if (!RuntimeHelper.IsElevated)
            {
                if (needUnlock)
                {
                    if (limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Available || limitedAccessFeatureStatus is LimitedAccessFeatureStatus.AvailableWithoutToken)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Taskbar, isPinnedSuccessfully));
                    }
                }
                else
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Taskbar, isPinnedSuccessfully));
                }
            }
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            SettingsItemTabList.Add(GeneralString);
            SettingsItemTabList.Add(StoreAndUpdateString);
            SettingsItemTabList.Add(WinGetString);
            SettingsItemTabList.Add(DownloadString);
            SettingsItemTabList.Add(AppInstallerString);
            SettingsItemTabList.Add(AdvancedString);
            SettingsItemTabList.Add(AboutString);
        }

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
                    IReadOnlyList<AppListEntry> appEntryList = await Package.Current.GetAppListEntriesAsync();

                    if (appEntryList[0] is AppListEntry defaultEntry)
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
                                string attestation = FeatureAccessHelper.GenerateAttestation(feature);
                                LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(feature, token, attestation);

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
