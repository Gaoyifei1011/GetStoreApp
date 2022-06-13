using GetStoreApp.Services.Settings;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using WinRT; // required to support Window.As<ICompositionSupportsSystemBackdrop>()

namespace GetStoreApp.Helpers
{
    public static class BackdropHelper
    {
        private static readonly WindowsSystemDispatcherQueueHelper WindowsSystemDispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper();

        private static MicaController MicaController { get; set; } = null;

        private static DesktopAcrylicController AcrylicController { get; set; } = null;

        private static SystemBackdropConfiguration BackdropConfigurationSource { get; set; } = null;

        public static ElementTheme CurrentTheme { get; set; } = ThemeSelectorService.Theme;

        public static string CurrentBackdrop { get; set; } = BackdropService.ApplicationBackdrop;

        static BackdropHelper()
        {
            WindowsSystemDispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();
        }

        /// <summary>
        /// 设置背景色
        /// </summary>
        public static void SetBackdrop()
        {
            ResetBackdrop();

            // 设置云母（Windows 11 22000以后的版本号支持）
            if (CurrentBackdrop == "Mica")
            {
                SetMicaBackdrop();
            }
            // 设置亚克力，为不支持云母的系统（Windows 10）提供背景选项
            else if (CurrentBackdrop == "Acrylic")
            {
                SetArylicBackdrop();
            }
        }

        /// <summary>
        /// 发生修改时，先对做出的修改进行重置
        /// </summary>
        private static void ResetBackdrop()
        {
            // Reset to default color. If the requested type is supported, we'll update to that.
            // Note: This sample completely removes any previous controller to reset to the default
            //       state. This is done so this sample can show what is expected to be the most
            //       common pattern of an app simply choosing one controller type which it sets at
            //       startup. If an app wants to toggle between Mica and Acrylic it could simply
            //       call RemoveSystemBackdropTarget() on the old controller and then setup the new
            //       controller, reusing any existing m_configurationSource and Activated/Closed
            //       event handlers.

            // 应用添加了设置后，如果发生更改，将之前的设置全部重置
            if (MicaController != null)
            {
                MicaController.Dispose();
                MicaController = null;
            }

            if (AcrylicController != null)
            {
                AcrylicController.Dispose();
                AcrylicController = null;
            }

            if (BackdropConfigurationSource != null)
            {
                BackdropConfigurationSource = null;
            }
        }

        /// <summary>
        /// 设置云母背景色
        /// </summary>
        private static bool SetMicaBackdrop()
        {
            if (MicaController.IsSupported())
            {
                // Hooking up the policy object
                BackdropConfigurationSource = new SystemBackdropConfiguration();

                App.MainWindow.Activated += Window_Activated;
                App.MainWindow.Closed += Window_Closed;

                // Initial configuration state.
                BackdropConfigurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                MicaController = new MicaController();

                // Enable the system backdrop
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                MicaController.AddSystemBackdropTarget(App.MainWindow.As<ICompositionSupportsSystemBackdrop>());
                MicaController.SetSystemBackdropConfiguration(BackdropConfigurationSource);

                // Set backdrop successfully
                return true;
            }

            // Mica is not supported on this system
            return false;
        }

        /// <summary>
        /// 设置亚克力背景色
        /// </summary>
        /// <returns></returns>
        private static bool SetArylicBackdrop()
        {
            if (DesktopAcrylicController.IsSupported())
            {
                // Hooking up the policy object
                BackdropConfigurationSource = new SystemBackdropConfiguration();

                App.MainWindow.Activated += Window_Activated;
                App.MainWindow.Closed += Window_Closed;

                // Initial configuration state.
                BackdropConfigurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                AcrylicController = new DesktopAcrylicController();

                // Enable the system backdrop
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                AcrylicController.AddSystemBackdropTarget(App.MainWindow.As<ICompositionSupportsSystemBackdrop>());
                AcrylicController.SetSystemBackdropConfiguration(BackdropConfigurationSource);

                // Set backdrop successfully
                return true;
            }

            // Acrylic is not supported on this system
            return false;
        }

        private static void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            BackdropConfigurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private static void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed so it doesn't try to use this closed window.
            App.MainWindow.Activated -= Window_Activated;

            ResetBackdrop();
        }

        private static void SetConfigurationSourceTheme()
        {
            switch (CurrentTheme)
            {
                case ElementTheme.Default: BackdropConfigurationSource.Theme = SystemBackdropTheme.Default; break;
                case ElementTheme.Light: BackdropConfigurationSource.Theme = SystemBackdropTheme.Light; break;
                case ElementTheme.Dark: BackdropConfigurationSource.Theme = SystemBackdropTheme.Dark; break;
                default: BackdropConfigurationSource.Theme = SystemBackdropTheme.Default; break;
            }
        }
    }
}
