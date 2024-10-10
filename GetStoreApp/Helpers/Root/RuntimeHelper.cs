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
        public static bool IsMSIX { get; private set; }

        public static bool IsElevated { get; } = Environment.IsPrivilegedProcess;

        static RuntimeHelper()
        {
            IsInMsixContainer();
        }

        /// <summary>
        /// 判断应用是否在 Msix 容器中运行
        /// </summary>
        private static void IsInMsixContainer()
        {
            try
            {
                IsMSIX = Package.Current is not null;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                IsMSIX = false;
            }
        }
    }
}
