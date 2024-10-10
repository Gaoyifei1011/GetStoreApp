using Microsoft.Web.WebView2.Core;
using System;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppWebView.Helpers.Root
{
    /// <summary>
    /// 运行时辅助类
    /// </summary>
    public static class RuntimeHelper
    {
        public static bool IsWebView2Installed { get; private set; }

        static RuntimeHelper()
        {
            GetWebView2State();
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
