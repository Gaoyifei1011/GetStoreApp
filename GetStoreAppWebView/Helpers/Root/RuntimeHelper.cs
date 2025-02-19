using Microsoft.Web.WebView2.Core;
using System;
using System.Runtime.InteropServices.Marshalling;
using Windows.ApplicationModel;

namespace GetStoreAppWebView.Helpers.Root
{
    /// <summary>
    /// 运行时辅助类
    /// </summary>
    public static class RuntimeHelper
    {
        public static bool IsMSIX { get; private set; }

        public static bool IsWebView2Installed { get; private set; }

        static RuntimeHelper()
        {
            IsInMsixContainer();
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
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                IsMSIX = false;
            }
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
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                IsWebView2Installed = false;
            }
        }
    }
}
