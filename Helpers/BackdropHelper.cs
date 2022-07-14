using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using System;
using WinRT;

namespace GetStoreApp.Helpers
{
    public static class BackdropHelper
    {
        private static readonly WindowsSystemDispatcherQueueHelper WindowsSystemDispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper();

        private static MicaController MicaController { get; set; } = null;

        private static DesktopAcrylicController AcrylicController { get; set; } = null;

        private static SystemBackdropConfiguration BackdropConfigurationSource { get; set; } = null;

        private static string AppTheme;

        static BackdropHelper()
        {
            WindowsSystemDispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();
        }

        /// <summary>
        /// 设置背景色
        /// </summary>
        public static void SetBackdrop(string appTheme, string appBackdrop)
        {
            AppTheme = appTheme;
            ResetBackdrop();

            // 设置云母（Windows 11 22000以后的版本号支持）
            if (appBackdrop == "Mica")
            {
                SetMicaBackdrop();
            }
            // 设置亚克力，为不支持云母的系统（Windows 10）提供背景选项
            else if (appBackdrop == "Acrylic")
            {
                SetArylicBackdrop();
            }
        }

        /// <summary>
        /// 发生修改时，先对做出的修改进行重置
        /// </summary>
        private static void ResetBackdrop()
        {
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
            BackdropConfigurationSource.Theme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme) switch
            {
                ElementTheme.Default => SystemBackdropTheme.Default,
                ElementTheme.Light => SystemBackdropTheme.Light,
                ElementTheme.Dark => SystemBackdropTheme.Dark,
                _ => SystemBackdropTheme.Default,
            };
        }
    }
}
