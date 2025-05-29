using GetStoreAppWebView.Extensions.DataType.Enums;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.Views.Pages;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace GetStoreAppWebView
{
    /// <summary>
    /// 获取商店应用 网页浏览器程序
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private bool isDisposed;
        private readonly string PinningAppString = ResourceService.GetLocalized("Pinner/PinningApp");
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

            if (args.Kind is ActivationKind.Protocol && args is ProtocolActivatedEventArgs protocolActivatedEventArgs)
            {
                if (protocolActivatedEventArgs.Uri.AbsoluteUri is "getstoreappwebbrowser:")
                {
                    if (appWindowList.Count is 0)
                    {
                        if (Window.Current.Content is not Page)
                        {
                            Window.Current.Content = new WebViewPage();
                            Window.Current.Activate();
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else if (protocolActivatedEventArgs.Uri.AbsoluteUri is "getstoreapppinner:")
                {
                    AppWindow appWindow = await AppWindow.TryCreateAsync();
                    appWindowList.Add(appWindow.UIContext, appWindow);
                    appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                    ElementCompositionPreview.SetAppWindowContent(appWindow, new PinnerPage(appWindow));
                    appWindow.Title = PinningAppString;
                    await appWindow.TryShowAsync();

                    if (Window.Current.Content is not Page)
                    {
                        await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                    }

                    appWindow.Presenter.RequestPresentation(AppWindowPresentationKind.CompactOverlay);
                    appWindow.RequestSize(new Size(400, 200));

                    bool isPinnedSuccessfully = false;

                    if (protocolActivatedEventArgs.Data.Count is 3)
                    {
                        if (protocolActivatedEventArgs.Data["Type"] is nameof(SecondaryTile))
                        {
                            string tag = Convert.ToString(protocolActivatedEventArgs.Data["Tag"]);
                            string displayName = Convert.ToString(protocolActivatedEventArgs.Data["DisplayName"]);

                            try
                            {
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
                                isPinnedSuccessfully = await secondaryTile.RequestCreateAsync();
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Use SecondaryTile api to pin app to taskbar failed.", e);
                            }
                            finally
                            {
                                List<string> dataList = [];
                                dataList.Add(Convert.ToString(isPinnedSuccessfully));
                                ResultService.SaveResult(StorageDataKind.SecondaryTile, dataList);
                                ApplicationData.Current.SignalDataChanged();
                                await appWindow.CloseAsync();
                                appWindowList.Remove(appWindow.UIContext);
                            }
                        }
                        else if (protocolActivatedEventArgs.Data["Type"] is nameof(TaskbarManager))
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
                                            isPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinAppListEntryAsync(appListEntryItem);
                                            break;
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Use TaskbarManager api to pin app to taskbar failed.", e);
                            }
                            finally
                            {
                                List<string> dataList = [];
                                dataList.Add(Convert.ToString(isPinnedSuccessfully));
                                ResultService.SaveResult(StorageDataKind.TaskbarManager, dataList);
                                ApplicationData.Current.SignalDataChanged();
                                await appWindow.CloseAsync();
                                appWindowList.Remove(appWindow.UIContext);
                            }
                        }
                        else
                        {
                            await appWindow.CloseAsync();
                            appWindowList.Remove(appWindow.UIContext);
                        }
                    }
                    else
                    {
                        await appWindow.CloseAsync();
                        appWindowList.Remove(appWindow.UIContext);
                    }
                }
                else
                {
                    if (Window.Current.Content is not Page)
                    {
                        await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                    }
                }
            }
            else
            {
                if (Window.Current.Content is not Page)
                {
                    await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                }
            }
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Error, "Unknown unhandled exception.", args.Exception);
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
                isDisposed = true;
            }

            LogService.CloseLog();
            Environment.Exit(Environment.ExitCode);
        }
    }
}
