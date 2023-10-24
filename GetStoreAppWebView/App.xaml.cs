using GetStoreAppWebView.Services.Root;
using Mile.Xaml;
using System;
using System.Windows.Forms;

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
            MessageBox.Show(
                ResourceService.GetLocalized("WebView/ErrorTitle") + Environment.NewLine +
                ResourceService.GetLocalized("WebView/Content1") + Environment.NewLine +
                ResourceService.GetLocalized("WebView/Content2"),
                ResourceService.GetLocalized("WebView/AppDisplayName"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
                );
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
