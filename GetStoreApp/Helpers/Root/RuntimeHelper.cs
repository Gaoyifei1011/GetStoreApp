using GetStoreApp.WindowsAPI.PInvoke.Advapi32;
using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;

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
                IsMSIX = Package.Current is not null;
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
            bool success = Advapi32Library.OpenProcessToken(-1, 0x0008, out IntPtr tokenHandle);

            TOKEN_ELEVATION_TYPE token_elevation_type = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
            uint tetSize = sizeof(int);

            if (success)
            {
                IntPtr token_elevation_type_Ptr = Marshal.AllocHGlobal((int)tetSize);
                try
                {
                    if (Advapi32Library.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevationType, token_elevation_type_Ptr, tetSize, out uint returnLength))
                    {
                        token_elevation_type = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(token_elevation_type_Ptr);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(token_elevation_type_Ptr);
                }
            }

            IsElevated = token_elevation_type == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
        }
    }
}
