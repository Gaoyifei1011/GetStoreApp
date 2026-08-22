using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.Views.Windows;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Foundation.Diagnostics;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace GetStoreAppInstaller
{
    /// <summary>
    /// 应用包安装器主程序
    /// </summary>
    public partial class InstallerApp : Application, IDisposable
    {
        #region 第一部分：常量、资源与状态字段

        private bool isDisposed;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        internal Window MainWindow { get; private set; }

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal InstallerApp()
        {
            InitializeComponent();
            DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
            UnhandledException += OnUnhandledException;
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 启动应用程序时调用，初始化应用主窗口
        /// </summary>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            MainWindow = new InstallerWindow();
            MainWindow.Activate();
            SetAppIcon(MainWindow.AppWindow);
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(InstallerApp), nameof(OnUnhandledException), 1, args.Exception);
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 设置应用窗口图标
        /// </summary>
        private void SetAppIcon(AppWindow appWindow)
        {
            // 选中文件中的图标总数
            int iconTotalCount = User32Library.PrivateExtractIcons(Environment.ProcessPath, 0, 0, 0, null, null, 0, 0);

            // 用于接收获取到的图标指针
            nint[] hIcons = new nint[iconTotalCount];

            // 对应的图标id
            int[] ids = new int[iconTotalCount];

            // 成功获取到的图标个数
            int successCount = User32Library.PrivateExtractIcons(Environment.ProcessPath, 0, 256, 256, hIcons, ids, iconTotalCount, 0);

            // GetStoreApp.exe 应用程序只有一个图标
            if (successCount >= 1 && hIcons[0] != nint.Zero)
            {
                appWindow.SetIcon(Win32Interop.GetIconIdFromIcon(hIcons[0]));
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~InstallerApp()
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

        #endregion 第六部分：数据操作与业务逻辑
    }
}
