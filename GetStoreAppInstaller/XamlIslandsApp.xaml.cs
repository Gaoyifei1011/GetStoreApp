using GetStoreAppInstaller.Views.Windows;
using Mile.Xaml;
using System;
using Windows.UI.Xaml;

namespace GetStoreAppInstaller
{
    public partial class XamlIslandsApp : Application, IDisposable
    {
        private bool isDisposed;

        public XamlIslandsApp()
        {
            this.ThreadInitialize();
            InitializeComponent();
            LoadComponent(this, new Uri("ms-appx:///XamlIslandsApp.xaml"));
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            //LogService.WriteLog(EventLevel.Warning, "Xaml islands UI Exception", args.Exception);
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
                if (disposing)
                {
                    this.ThreadUninitialize();

                    if (MainWindow.Current is not null && !MainWindow.Current.IsDisposed)
                    {
                        MainWindow.Current?.Close();
                    }

                    System.Windows.Forms.Application.Exit();
                }

                isDisposed = true;
            }
        }
    }
}
