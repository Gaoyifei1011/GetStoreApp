using GetStoreAppHelper.Services;
using GetStoreAppHelper.WindowsAPI.PInvoke.User32;
using Microsoft.VisualBasic.ApplicationServices;
using System;

namespace GetStoreAppHelper
{
    /// <summary>
    /// 单例应用程序管理器
    /// </summary>
    public class SingleInstanceManager : WindowsFormsApplicationBase, IDisposable
    {
        private bool isDisposed = false;

        public App ApplicationRoot { get; private set; }

        public MileWindow MainWindow { get; set; }

        public SingleInstanceManager()
        {
            IsSingleInstance = true;
        }

        protected override bool OnStartup(StartupEventArgs args)
        {
            ApplicationRoot = new App();

            MainWindow = new MileWindow();
            MainWindow.Content = new MainPage();
            MainWindow.Title = ResourceService.GetLocalized("HelperResources/Title");
            MainWindow.Position.X = unchecked((int)0x80000000);
            MainWindow.Position.Y = 0;
            MainWindow.Size.X = unchecked((int)0x80000000);
            MainWindow.Size.Y = 0;

            MainWindow.InitializeWindow();
            MainWindow.Activate();
            return base.OnStartup(args);
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs args)
        {
            User32Library.SetForegroundWindow(MainWindow.Handle);
            base.OnStartupNextInstance(args);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SingleInstanceManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            if (disposing && ApplicationRoot is not null)
            {
                ApplicationRoot.Dispose();
            }
            isDisposed = true;
        }

        public void Start(string[] commandline)
        {
            try
            {
                base.Run(commandline);
            }
            catch (NoStartupFormException)
            {
            }
        }
    }
}
