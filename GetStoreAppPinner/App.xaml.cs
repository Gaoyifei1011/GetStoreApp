using GetStoreAppPinner.Helpers.Root;
using GetStoreAppPinner.Services.Root;
using GetStoreAppPinner.Views.Pages;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.UI;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using WinRT;

namespace GetStoreAppPinner
{
    /// <summary>
    /// 固定辅助程序
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private bool isDisposed;
        private readonly string PinningAppString = ResourceService.GetLocalized("Pinner/PinningApp");
        private ApplicationView applicationView;
        private readonly Dictionary<UIContext, AppWindow> appWindowList = [];

        public App()
        {
            InitializeComponent();
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 在通过常规启动以外的某种方式激活应用程序时调用。
        /// </summary>
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            applicationView = ApplicationView.GetForCurrentView();

            if (args.Kind is ActivationKind.Protocol && args.As<ProtocolActivatedEventArgs>() is ProtocolActivatedEventArgs protocolActivatedEventArgs)
            {
                if (protocolActivatedEventArgs.Uri.AbsoluteUri is "getstoreapppinner:")
                {
                    AppWindow appWindow = await AppWindow.TryCreateAsync();
                    appWindowList.Add(appWindow.UIContext, appWindow);
                    appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                    ElementCompositionPreview.SetAppWindowContent(appWindow, new PinnerPage(appWindow));
                    appWindow.Title = PinningAppString;
                    await appWindow.TryShowAsync();
                    await applicationView.TryConsolidateAsync();
                    appWindow.Presenter.RequestPresentation(AppWindowPresentationKind.CompactOverlay);
                    appWindow.RequestSize(new Size(400, 200));

                    if (protocolActivatedEventArgs.Data.Count is 4 && protocolActivatedEventArgs.Data["Type"] is nameof(SecondaryTile))
                    {
                        string tag = Convert.ToString(protocolActivatedEventArgs.Data["Tag"]);
                        string displayName = Convert.ToString(protocolActivatedEventArgs.Data["DisplayName"]);
                        string position = Convert.ToString(protocolActivatedEventArgs.Data["Position"]);
                        SecondaryTile secondaryTile = new("GetStoreApp" + tag)
                        {
                            DisplayName = displayName,
                            Arguments = "SecondaryTile " + tag
                        };

                        secondaryTile.VisualElements.BackgroundColor = Colors.Transparent;
                        secondaryTile.VisualElements.Square150x150Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.Square71x71Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.Square44x44Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                        if (position is "StartScreen")
                        {
                            try
                            {
                                await secondaryTile.RequestCreateAsync();
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppPinner), nameof(App), nameof(OnActivated), 1, e);
                            }
                        }
                        else if (position is "Taskbar")
                        {
                            string feature = "com.microsoft.windows.taskbar.requestPinSecondaryTile";
                            string featureId = FeatureAccessHelper.GetFeatureId(feature);
                            if (!string.IsNullOrEmpty(featureId))
                            {
                                string token = FeatureAccessHelper.GenerateTokenFromFeatureId(feature, featureId);
                                string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                                LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);

                                if (accessResult.Status is LimitedAccessFeatureStatus.Available || accessResult.Status is LimitedAccessFeatureStatus.AvailableWithoutToken)
                                {
                                    await TaskbarManager.GetDefault().RequestPinSecondaryTileAsync(secondaryTile);
                                }
                            }
                            else
                            {
                                await TaskbarManager.GetDefault().RequestPinSecondaryTileAsync(secondaryTile);
                            }
                        }
                    }
                    else if (protocolActivatedEventArgs.Data.Count is 3 && protocolActivatedEventArgs.Data["Type"] is nameof(TaskbarManager))
                    {
                        try
                        {
                            PackageManager packageManager = new();
                            Package package = packageManager.FindPackageForUser(string.Empty, protocolActivatedEventArgs.Data["PackageFullName"] as string);

                            if (package is not null)
                            {
                                foreach (AppListEntry appListEntryItem in package.GetAppListEntries())
                                {
                                    if (Equals(appListEntryItem.AppUserModelId, protocolActivatedEventArgs.Data["AppUserModelId"]))
                                    {
                                        await TaskbarManager.GetDefault().RequestPinAppListEntryAsync(appListEntryItem);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppPinner), nameof(App), nameof(OnActivated), 2, e);
                        }
                    }

                    await appWindow.CloseAsync();
                    appWindowList.Remove(appWindow.UIContext);
                }
                else
                {
                    await applicationView.TryConsolidateAsync();
                }
            }
            else
            {
                await applicationView.TryConsolidateAsync();
            }
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppPinner), nameof(App), nameof(OnUnhandledException), 1, args.Exception);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~App()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                LogService.CloseLog();
                isDisposed = true;
            }

            Exit();
        }
    }
}
