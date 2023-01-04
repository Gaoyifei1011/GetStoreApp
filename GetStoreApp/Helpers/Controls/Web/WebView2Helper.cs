using Microsoft.Web.WebView2.Core;
using System;

namespace GetStoreApp.Helpers.Controls.Web
{
    public static class WebView2Helper
    {
        /// <summary>
        /// 检测Microsoft Edge WebView2 运行时是否已经安装
        /// </summary>
        public static bool IsInstalled()
        {
            try
            {
                string WebView2Version = CoreWebView2Environment.GetAvailableBrowserVersionString(browserExecutableFolder: default);

                if (string.IsNullOrEmpty(WebView2Version) || WebView2Version is "0.0.0.0")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
