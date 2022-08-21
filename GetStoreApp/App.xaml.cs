using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Win32.Foundation;
using WinUIEx;

namespace GetStoreApp
{
    public partial class App : Application
    {
        private IActivationService ActivationService { get; }

        public IAria2Service Aria2Service { get; }

        public static WindowEx MainWindow { get; } = new MainWindow();

        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr hWnd);

        public App()
        {
            InitializeComponent();

            IOCHelper.InitializeIOCService();

            ActivationService = IOCHelper.GetService<IActivationService>();
            Aria2Service = IOCHelper.GetService<IAria2Service>();

            UnhandledException += OnUnhandledException;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            await RunSingleInstanceAppAsync();
            Debug.WriteLine("11");
            await ActivationService.ActivateAsync(args);
        }

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        private async void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;

            await Aria2Service.CloseAria2Async();
        }

        /// <summary>
        /// 应用程序只运行单个实例
        /// </summary>
        private async Task RunSingleInstanceAppAsync()
        {
            // 获取已经激活的参数
            AppActivationArguments appArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

            // 获取或注册主实例
            AppInstance mainInstance = AppInstance.FindOrRegisterForKey("Main");

            // 如果主实例不是此当前实例
            if (!mainInstance.IsCurrent)
            {
                // 将激活重定向到该实例
                await mainInstance.RedirectActivationToAsync(appArgs);

                // 然后退出实例并停止
                Process.GetCurrentProcess().Kill();
                return;
            }

            // 否则将注册激活重定向
            AppInstance.GetCurrent().Activated += App_Activated;
        }

        /// <summary>
        /// 关闭其他实例后，按照原来的状态显示已经打开的实例窗口
        /// </summary>
        private void App_Activated(object sender, AppActivationArguments e)
        {
            // 将窗口置于前台前首先获取窗口句柄
            HWND hwnd = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(MainWindow);

            // 判断窗口状态是否处于最大化状态
            if (IsZoomed(hwnd))
            {
                Windows.Win32.PInvoke.ShowWindow(hwnd, Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD.SW_MAXIMIZE);
            }

            // 其他状态下窗口还原显示状态
            else
            {
                // 还原窗口（如果最小化）时，需要 Microsoft.Windows.CsWin32 NuGet 包和一个带有 ShowWindow() 方法的 NativeMethods.txt 文件
                Windows.Win32.PInvoke.ShowWindow(hwnd, Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD.SW_RESTORE);
            }

            // 将指定窗口的线程设置到前台时，需要 Microsoft.Windows.CsWin32 NuGet 包和一个具有 SetForegroundWindow() 方法的 NativeMethods.txt 文件
            Windows.Win32.PInvoke.SetForegroundWindow(hwnd);
        }
    }
}
