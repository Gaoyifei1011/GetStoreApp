using GetStoreAppInstaller.Services.Root;
using System;
using Windows.Foundation.Diagnostics;
using Windows.UI.Xaml;

namespace GetStoreAppInstaller
{
    public partial class XamlIslandsApp : Application, IDisposable
    {
        private bool isDisposed;

        public XamlIslandsApp()
        {
            InitializeComponent();
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            LogService.WriteLog(LoggingLevel.Warning, "Xaml islands UI Exception", args.Exception);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~XamlIslandsApp()
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
                isDisposed = true;
            }
        }
    }
}
