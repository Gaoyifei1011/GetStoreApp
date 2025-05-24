using GetStoreAppWebView.Extensions.DataType.Constant;
using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Root;
using System;
using System.Collections.Generic;

namespace GetStoreAppWebView.Services.Settings
{
    /// <summary>
    /// 网页浏览器内核选择设置服务
    /// </summary>
    public static class WebKernelService
    {
        private static readonly string settingsKey = ConfigKey.WebKernelKey;

        private static string defaultWebKernel;

        public static string WebKernel { get; set; }

        public static List<string> WebKernelList { get; } = ["WebView", "WebView2"];

        /// <summary>
        /// 应用在初始化前获取设置存储的网页浏览器内核选择值
        /// </summary>
        public static void InitializeWebKernel()
        {
            defaultWebKernel = WebKernelList.Find(item => item is "WebView2");

            WebKernel = GetWebKernel();
        }

        /// <summary>
        /// 获取设置存储的网页浏览器内核选择值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetWebKernel()
        {
            if (RuntimeHelper.IsWebView2Installed)
            {
                string webKernel = LocalSettingsService.ReadSetting<string>(settingsKey);

                if (string.IsNullOrEmpty(webKernel))
                {
                    return defaultWebKernel;
                }

                string selectedWebKernel = WebKernelList.Find(item => string.Equals(item, webKernel, StringComparison.OrdinalIgnoreCase));

                return string.IsNullOrEmpty(selectedWebKernel) ? defaultWebKernel : selectedWebKernel;
            }
            else
            {
                return WebKernelList.Find(item => Equals(item, "IE"));
            }
        }
    }
}
