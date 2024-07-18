using GetStoreAppWebView.Extensions.DataType.Constant;
using GetStoreAppWebView.Pages;
using GetStoreAppWebView.Services.Root;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.UI.Shell;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace GetStoreAppWebView
{
    /// <summary>
    /// 获取商店应用 网页浏览器程序
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private bool isDisposed;

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

            if (args.Kind is ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs protocolActivatedEventArgs = args as ProtocolActivatedEventArgs;

                if (protocolActivatedEventArgs.PreviousExecutionState is not ApplicationExecutionState.Running)
                {
                    if (protocolActivatedEventArgs.Uri.AbsoluteUri is "webbrowser:")
                    {
                        Window.Current.Content = new MainPage();
                        Window.Current.Activate();
                    }
                    else if (protocolActivatedEventArgs.Uri.AbsoluteUri is "taskbarpinner:")
                    {
                        ApplicationView applicationView = ApplicationView.GetForCurrentView();
                        CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
                        Window.Current.Content = new TaskbarPinPage();
                        applicationView.Title = ResourceService.GetLocalized("WebView/PinAppToTaskbar");
                        Window.Current.Activate();
                        ViewModePreferences preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                        preferences.CustomSize = new Size(400, 200);
                        await applicationView.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
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
                            ResultService.SaveResult(ConfigKey.TaskbarPinnedResultKey, pinResult);
                            ApplicationData.Current.SignalDataChanged();

                            Dispose();
                        }
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
            Dispose();
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
            Environment.Exit(0);
        }
    }
}
