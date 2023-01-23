using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using WinRT;

namespace GetStoreApp.Helpers.Window
{
    /// <summary>
    /// 应用背景色辅助类
    /// </summary>
    public static class BackdropHelper
    {
        private static WindowsSystemDispatcherQueueHelper m_wsdqHelper;

        private static MicaController m_micaController;
        private static DesktopAcrylicController m_acrylicController;
        private static SystemBackdropConfiguration m_configurationSource;

        /// <summary>
        /// 设置背景色为云母
        /// </summary>
        public static bool TrySetMicaBackdrop()
        {
            if (MicaController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // 挂接策略对象
                m_configurationSource = new SystemBackdropConfiguration();
                ((FrameworkElement)Program.ApplicationRoot.MainWindow.Content).ActualThemeChanged += OnActualThemeChanged;

                // 初始化配置状态
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_micaController = new MicaController() { Kind = MicaKind.Base };

                // 启用系统背景色
                // 注意： 请确保使用“using:WinRT;”来支持 Window.As<...>() 调用。
                m_micaController.AddSystemBackdropTarget(Program.ApplicationRoot.MainWindow.As<ICompositionSupportsSystemBackdrop>());
                m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // 成功
            }

            return false; // 此系统不支持云母背景色设置
        }

        /// <summary>
        /// 设置背景色为云母 Alt
        /// </summary>
        public static bool TrySetMicaAltBackdrop()
        {
            if (MicaController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // 挂接策略对象
                m_configurationSource = new SystemBackdropConfiguration();
                ((FrameworkElement)Program.ApplicationRoot.MainWindow.Content).ActualThemeChanged += OnActualThemeChanged;

                // 初始化配置状态
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_micaController = new MicaController() { Kind = MicaKind.BaseAlt };

                // 启用系统背景色
                // 注意： 请确保使用“using:WinRT;”来支持 Window.As<...>() 调用。
                m_micaController.AddSystemBackdropTarget(Program.ApplicationRoot.MainWindow.As<ICompositionSupportsSystemBackdrop>());
                m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // 成功
            }

            return false; // 此系统不支持云母Alt背景色设置
        }

        /// <summary>
        /// 设置背景色为亚克力
        /// </summary>
        public static bool TrySetAcrylicBackdrop()
        {
            if (DesktopAcrylicController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // 挂接策略对象
                m_configurationSource = new SystemBackdropConfiguration();
                ((FrameworkElement)Program.ApplicationRoot.MainWindow.Content).ActualThemeChanged += OnActualThemeChanged;

                // 初始化配置状态
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_acrylicController = new DesktopAcrylicController();

                // 启用系统背景色
                // 注意： 请确保使用“using:WinRT;”来支持 Window.As<...>() 调用。
                m_acrylicController.AddSystemBackdropTarget(Program.ApplicationRoot.MainWindow.As<ICompositionSupportsSystemBackdrop>());
                m_acrylicController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // 成功
            }

            return false; // 此系统不支持亚克力背景色设置
        }

        /// <summary>
        /// 设置窗口处于非激活状态时，窗口的状态
        /// </summary>
        public static void SetBackdropState(bool alwaysShowBackdrop, WindowActivatedEventArgs args)
        {
            if (m_configurationSource is null)
            {
                return;
            }

            if (alwaysShowBackdrop)
            {
                m_configurationSource.IsInputActive = true;
            }
            else
            {
                m_configurationSource.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;
            }
        }

        /// <summary>
        /// 关闭应用的背景色
        /// </summary>
        public static void ReleaseBackdrop()
        {
            // 确保任何云母/亚克力控制器被处置，这样它就不会试图使用这个关闭的窗口。
            if (m_micaController is not null)
            {
                m_micaController.Dispose();
                m_micaController = null;
            }

            if (m_acrylicController is not null)
            {
                m_acrylicController.Dispose();
                m_acrylicController = null;
            }

            ((FrameworkElement)Program.ApplicationRoot.MainWindow.Content).ActualThemeChanged -= OnActualThemeChanged;
            m_configurationSource = null;
        }

        /// <summary>
        /// 窗口主题色变化时修改背景色的主题色
        /// </summary>
        private static void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource is not null)
            {
                SetConfigurationSourceTheme();
            }
        }

        /// <summary>
        /// 设置窗口的主题色
        /// </summary>
        private static void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)Program.ApplicationRoot.MainWindow.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = SystemBackdropTheme.Default; break;
            }
        }
    }
}
