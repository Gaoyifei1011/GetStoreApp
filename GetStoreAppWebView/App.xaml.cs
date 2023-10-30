using GetStoreAppWebView.Services.Root;
using Mile.Xaml;
using System;
using Windows.Foundation.Diagnostics;

namespace GetStoreAppWebView
{
    public partial class App : Windows.UI.Xaml.Application, IDisposable
    {
        private bool isDisposed;

        public App()
        {
            this.ThreadInitialize();
            InitializeComponent();
            UnhandledException += OnUnhandledException;
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
        /// 关闭应用并释放所有资源
        /// </summary>
        public void CloseApp()
        {
            Program.MainWindow.Close();
            this.ThreadUninitialize();
            Environment.Exit(0);
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
            if (!isDisposed)
            {
                if (disposing)
                {
                    CloseApp();
                }

                isDisposed = true;
            }
        }
    }
}
