using System;
using System.Runtime.InteropServices.Marshalling;
using Windows.ApplicationModel;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 运行时辅助类
    /// </summary>
    public static class RuntimeHelper
    {
        public static bool IsMSIX { get; }

        public static bool IsElevated { get; } = Environment.IsPrivilegedProcess;

        public static bool IsStoreVersion { get; private set; }

        static RuntimeHelper()
        {
            IsMSIX = IsInMsixContainerApp();
            IsStoreVersion = IsStoreApp();
        }

        /// <summary>
        /// 判断应用是否在 Msix 容器中运行
        /// </summary>
        private static bool IsInMsixContainerApp()
        {
            try
            {
                return Package.Current is not null;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return false;
            }
        }

        /// <summary>
        /// 判断应用是否为商店版本
        /// </summary>
        private static bool IsStoreApp()
        {
            try
            {
                return Package.Current.Id.FamilyName.StartsWith("055B5CA4.");
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return false;
            }
        }
    }
}
