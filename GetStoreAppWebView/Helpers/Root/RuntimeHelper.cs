using GetStoreAppWebView.WindowsAPI.PInvoke.Kernel32;
using Microsoft.Web.WebView2.Core;
using System;
using System.Security.Principal;

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
            int length = 0;
            IsMSIX = Kernel32Library.GetCurrentPackageFullName(ref length, null) != Kernel32Library.APPMODEL_ERROR_NO_PACKAGE;
        }

        /// <summary>
        /// 判断应用是否以管理员身份运行
        /// </summary>
        private static void IsRunningElevated()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            IsElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
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
