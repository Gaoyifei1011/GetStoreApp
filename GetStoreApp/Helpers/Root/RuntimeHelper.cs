using System;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 运行时辅助类
    /// </summary>
    public static class RuntimeHelper
    {
        public static bool IsMSIX { get; private set; }

        public static bool IsElevated { get; private set; }

        static RuntimeHelper()
        {
            IsInMsixContainer();
            IsRunningElevated();
        }

        /// <summary>
        /// 判断应用是否在 Msix 容器中运行
        /// </summary>
        private static void IsInMsixContainer()
        {
            try
            {
                if (Package.Current is not null)
                {
                    IsMSIX = true;
                }
                else
                {
                    IsMSIX = false;
                }
            }
            catch (Exception)
            {
                IsMSIX = false;
            }
        }

        /// <summary>
        /// 判断应用是否以管理员身份运行
        /// </summary>
        private static void IsRunningElevated()
        {
            try
            {
                new PackageManager().FindPackages();
                IsElevated = true;
            }
            catch (Exception)
            {
                IsElevated = false;
            }
        }
    }
}
