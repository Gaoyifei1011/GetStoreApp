using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store.Preview;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Shell;
using Windows.UI.StartScreen;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private AppNaviagtionArgs AboutNavigationArgs { get; set; } = AppNaviagtionArgs.None;

        public AboutPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (args.Parameter is not null)
            {
                AboutNavigationArgs = (AppNaviagtionArgs)Enum.Parse(typeof(AppNaviagtionArgs), Convert.ToString(args.Parameter));
            }
            else
            {
                AboutNavigationArgs = AppNaviagtionArgs.None;
            }
        }

        /// <summary>
        /// 创建应用的桌面快捷方式
        /// </summary>
        public void OnCreateDesktopShortcutClicked(object sender, RoutedEventArgs args)
        {
            bool IsCreatedSuccessfully = false;

            try
            {
                if (StoreConfiguration.IsPinToDesktopSupported())
                {
                    StoreConfiguration.PinToDesktop(Package.Current.Id.FamilyName);
                    IsCreatedSuccessfully = true;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Create desktop shortcut failed.", e);
            }
            finally
            {
                new QuickOperationNotification(this, QuickOperationType.DesktopShortcut, IsCreatedSuccessfully).Show();
            }
        }

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            double CurrentScrollPosition = AboutScroll.VerticalOffset;
            Point CurrentPoint = new Point(0, (int)CurrentScrollPosition);

            switch (AboutNavigationArgs)
            {
                case AppNaviagtionArgs.Instructions:
                    {
                        Point TargetPosition = Instructions.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                        AboutScroll.ChangeView(null, TargetPosition.Y, null);
                        break;
                    }
                case AppNaviagtionArgs.SettingsHelp:
                    {
                        Point TargetPosition = SettingsHelp.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                        AboutScroll.ChangeView(null, TargetPosition.Y, null);
                        break;
                    }
                default:
                    {
                        AboutScroll.ChangeView(null, 0, null);
                        break;
                    }
            }
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        public async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool IsPinnedSuccessfully = false;

            try
            {
                IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();

                AppListEntry DefaultEntry = AppEntries[0];

                if (DefaultEntry is not null)
                {
                    StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                    bool containsEntry = await startScreenManager.ContainsAppListEntryAsync(DefaultEntry);

                    if (!containsEntry)
                    {
                        await startScreenManager.RequestAddAppListEntryAsync(DefaultEntry);
                    }
                }
                IsPinnedSuccessfully = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Pin app to startscreen failed.", e);
            }
            finally
            {
                new QuickOperationNotification(this, QuickOperationType.StartScreen, IsPinnedSuccessfully).Show();
            }
        }

        /// <summary>
        /// 将应用固定到任务栏
        /// </summary>
        public async void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            bool IsPinnedSuccessfully = false;
            try
            {
                string featureId = "com.microsoft.windows.taskbar.pin";
                string token = FeatureAccessHelper.GenerateTokenFromFeatureId(featureId);
                string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);

                if (accessResult.Status is LimitedAccessFeatureStatus.Available)
                {
                    IsPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Pin app to taskbar failed.", e);
            }
            finally
            {
                new QuickOperationNotification(this, QuickOperationType.Taskbar, IsPinnedSuccessfully).Show();
            }
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        public async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new LicenseDialog(), this);
        }

        /// <summary>
        /// 查看更新日志
        /// </summary>
        public async void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        }
    }
}
