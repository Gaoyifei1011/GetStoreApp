using Autofac;
using GetStoreAppConsole.Contracts;
using GetStoreAppConsole.Extensions.DataType.Enums;
using GetStoreAppConsole.Services;
using GetStoreAppWindowsAPI.PInvoke.User32;
using System;

namespace GetStoreAppConsole.Helpers
{
    /// <summary>
    /// 控制翻转/依赖注入
    /// </summary>
    public static class ContainerHelper
    {
        public static IContainer Container { get; set; }

        /// <summary>
        /// 获取已经在IOC容器注册类的实例
        /// </summary>
        public static T GetInstance<T>() where T : class
        {
            if (!Container.IsRegistered<T>())
            {
                MessageBoxResult Result = DllFunctions.MessageBox(
                    WinRT.Interop.WindowNative.GetWindowHandle(IntPtr.Zero),
                    $"应用启动失败。\n{typeof(T)} 需要在ContainerHelper.cs中的InitializeContainer()方法中注册。",
                    "获取商店应用",
                    MessageBoxOptions.MB_OK | MessageBoxOptions.MB_ICONERROR | MessageBoxOptions.MB_APPLMODAL | MessageBoxOptions.MB_TOPMOST
                    );

                if (Result == MessageBoxResult.IDOK)
                {
                    Environment.Exit(Convert.ToInt32(AppExitCode.Failed));
                }
            }

            return Container.Resolve<T>();
        }

        /// <summary>
        /// 初始化IOC容器
        /// </summary>
        public static void InitializeContainer()
        {
            ContainerBuilder Builder = new ContainerBuilder();

            Builder.RegisterType<ActivationService>().As<IActivationService>().SingleInstance();
            Builder.RegisterType<ConsoleService>().As<IConsoleService>().SingleInstance();
            Builder.RegisterType<LanguageService>().As<ILanguageService>().SingleInstance();
            Builder.RegisterType<ResourceService>().As<IResourceService>().SingleInstance();

            Container = Builder.Build();
        }
    }
}
