using GetStoreAppWebView.Extensions.DataType.Enums;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using Windows.ApplicationModel;
using Windows.Storage;

namespace GetStoreAppWebView.Helpers.Root
{
    /// <summary>
    /// 运行时辅助类
    /// </summary>
    public static class RuntimeHelper
    {
        public static bool IsMSIX { get; private set; }

        public static bool IsElevated { get; } = Environment.IsPrivilegedProcess;

        public static WebView2Type WebView2Type { get; private set; }

        static RuntimeHelper()
        {
            IsInMsixContainer();
            GetWebView2Type();
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
        /// 获取 WebView2 运行时安装的类型
        /// </summary>
        private static void GetWebView2Type()
        {
            try
            {
                string webViewVersion = CoreWebView2Environment.GetAvailableBrowserVersionString();
                if (!string.IsNullOrEmpty(webViewVersion))
                {
                    WebView2Type = WebView2Type.User;
                }
                else
                {
                    string systemWebView2Path = Path.Combine(SystemDataPaths.GetDefault().System, "Microsoft-Edge-WebView");
                    string systemWebViewVersion = CoreWebView2Environment.GetAvailableBrowserVersionString(systemWebView2Path);
                    WebView2Type = !string.IsNullOrEmpty(systemWebViewVersion) ? WebView2Type.System : WebView2Type.None;
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
        }
    }
}
