using GetStoreAppWebView.Extensions.DataType.Enums;
using GetStoreAppWebView.Pages;
using GetStoreAppWebView.Services.Controls.Settings;
using GetStoreAppWebView.Services.Root;
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
using Windows.UI.Core;
using Windows.UI.Shell;
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
                if (protocolActivatedEventArgs.Uri.AbsoluteUri is "webbrowser:")
                {
                    if (appWindowList.Count is 0)
                    {
                        Window.Current.Content = new WebViewPage();
                        Window.Current.Activate();
                    }
                    else
                    {
                        return;
                    }
                }
                else if (protocolActivatedEventArgs.Uri.AbsoluteUri is "taskbarpinner:")
                {
                    AppWindow appWindow = await AppWindow.TryCreateAsync();
                    appWindowList.Add(appWindow.UIContext, appWindow);
                    appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                    ElementCompositionPreview.SetAppWindowContent(appWindow, new TaskbarPinPage());
                    appWindow.Title = ResourceService.GetLocalized("WebView/PinAppToTaskbar");
                    await appWindow.TryShowAsync();

                    if (Window.Current.Content is Grid)
                    {
                        await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                    }

                    appWindow.Presenter.RequestPresentation(AppWindowPresentationKind.CompactOverlay);
                    appWindow.RequestSize(new Size(400, 200));

                    bool pinResult = false;

                    try
                    {
                        if (protocolActivatedEventArgs.Data.Count is 2)
                        {
                            PackageManager packageManager = new();
                            Package package = packageManager.FindPackageForUser(string.Empty, protocolActivatedEventArgs.Data["PackageFullName"] as string);

                            if (package is not null)
                            {
                                foreach (AppListEntry applistItem in package.GetAppListEntries())
                                {
                                    if (applistItem.AppUserModelId.Equals(protocolActivatedEventArgs.Data["AppUserModelId"]))
                                    {
                                        pinResult = await TaskbarManager.GetDefault().RequestPinAppListEntryAsync(applistItem);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Pin app to taskbar failed", e);
                    }
                    finally
                    {
                        ResultService.SaveResult(StorageDataKind.TaskbarPinnedResult, pinResult.ToString());
                        ApplicationData.Current.SignalDataChanged();
                        await appWindow.CloseAsync();
                        appWindowList.Remove(appWindow.UIContext);
                    }
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
