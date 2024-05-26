using GetStoreAppWebView.WindowsAPI.PInvoke.Advapi32;
using Microsoft.Web.WebView2.Core;
using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;

namespace GetStoreAppWebView.Helpers.Root
{
    /// <summary>
    /// 运行时辅助类
    /// </summary>
    public static class RuntimeHelper
    {
        public static bool IsMSIX { get; private set; }

        public static bool IsElevated { get; private set; }

        public static bool IsWebView2Installed { get; private set; }

        static RuntimeHelper()
        {
            IsInMsixContainer();
            IsRunningElevated();
            GetWebView2State();
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
            uint tetSize = (uint)Marshal.SizeOf((int)token_elevation_type);

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

        /// <summary>
        /// 判断 WebView2 运行时是否已安装
        /// </summary>
        private static void GetWebView2State()
        {
            try
            {
                string webViewVersion = CoreWebView2Environment.GetAvailableBrowserVersionString();
                IsWebView2Installed = !string.IsNullOrEmpty(webViewVersion);
            }
            catch (Exception)
            {
                IsWebView2Installed = false;
            }
        }
    }
}
