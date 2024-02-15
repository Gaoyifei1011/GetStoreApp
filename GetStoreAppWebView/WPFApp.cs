using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.Views.Windows;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace GetStoreAppWebView
{
    /// <summary>
    /// 网页浏览器程序
    /// </summary>
    public class WPFApp : Application, IDisposable
    {
        private bool isDisposed;

        public WPFApp()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 在调用 Run() 对象的 Application 方法时发生
        /// </summary>
        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);
            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        /// <summary>
        /// 应用程序在退出时引发的事件
        /// </summary>
        protected override void OnExit(ExitEventArgs args)
        {
            base.OnExit(args);
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

        /// <summary>
        /// 处理 WPF UI 线程异常
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLogEntryType.Error, "Unknown dispatcher unhandled exception.", args.Exception);
        }

        /// <summary>
        /// 处理 WPF 非 UI 线程异常
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(EventLogEntryType.Error, "Unknown unhandled exception.", args.ExceptionObject as Exception);
            Dispose();
        }

        ~WPFApp()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    isDisposed = true;
                    MainWindow?.Close();
                    Environment.Exit(Environment.ExitCode);
                }
            }
        }
    }
}
