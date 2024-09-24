using GetStoreAppShellExtension.WindowsAPI.PInvoke.Kernel32;

namespace GetStoreAppShellExtension.Helpers.Root
{
    /// <summary>
    /// 应用信息辅助类
    /// </summary>
    public static partial class InfoHelper
    {
        // 应用安装根目录
        public static string AppInstalledLocation { get; } = string.Empty;

        static unsafe InfoHelper()
        {
            uint length = 0;
            int result = Kernel32Library.GetCurrentPackagePath(ref length, null);

            if (result is 122)
            {
                char[] buffer = new char[length];
                fixed (char* pBuffer = buffer)
                {
                    result = Kernel32Library.GetCurrentPackagePath(ref length, pBuffer);
                    if (result is 0)
                    {
                        if (length >= 1)
                        {
                            AppInstalledLocation = new string(pBuffer, 0, (int)length - 1);
                        }
                    }
                }
            }
        }
    }
}
