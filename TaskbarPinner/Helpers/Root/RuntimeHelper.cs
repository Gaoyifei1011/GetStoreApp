using TaskbarPinner.WindowsAPI.PInvoke.Kernel32;

namespace TaskbarPinner.Helpers.Root
{
    /// <summary>
    /// 运行时辅助类
    /// </summary>
    public static class RuntimeHelper
    {
        public static bool IsMSIX { get; private set; }

        static RuntimeHelper()
        {
            IsInMsixContainer();
        }

        /// <summary>
        /// 判断应用是否在 Msix 容器中运行
        /// </summary>
        private static void IsInMsixContainer()
        {
            int length = 0;
            IsMSIX = Kernel32Library.GetCurrentPackageFullName(ref length, null) != Kernel32Library.APPMODEL_ERROR_NO_PACKAGE;
        }
    }
}
