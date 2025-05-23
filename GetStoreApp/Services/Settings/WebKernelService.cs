﻿using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 网页浏览器内核选择设置服务
    /// </summary>
    public static class WebKernelService
    {
        private static readonly string webKernelSettingsKey = ConfigKey.WebKernelKey;

        private static KeyValuePair<string, string> defaultWebKernel;

        public static KeyValuePair<string, string> WebKernel { get; set; }

        public static List<KeyValuePair<string, string>> WebKernelList { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的网页浏览器内核选择值
        /// </summary>
        public static void InitializeWebKernel()
        {
            WebKernelList = ResourceService.WebKernelList;

            defaultWebKernel = WebKernelList.Find(item => item.Key is "WebView2");

            WebKernel = GetWebKernel();
        }

        /// <summary>
        /// 获取设置存储的网页浏览器内核选择值，如果设置没有存储，使用默认值
        /// </summary>
        private static KeyValuePair<string, string> GetWebKernel()
        {
            string webKernel = LocalSettingsService.ReadSetting<string>(webKernelSettingsKey);

            if (string.IsNullOrEmpty(webKernel))
            {
                SetWebKernel(defaultWebKernel);
                return defaultWebKernel;
            }

            KeyValuePair<string, string> selectedWebKernel = WebKernelList.Find(item => string.Equals(item.Key, webKernel, StringComparison.OrdinalIgnoreCase));

            return string.IsNullOrEmpty(selectedWebKernel.Key) ? defaultWebKernel : selectedWebKernel;
        }

        /// <summary>
        /// 网页浏览器内核发生修改时修改设置存储的网页浏览器内核值
        /// </summary>
        public static void SetWebKernel(KeyValuePair<string, string> webKernel)
        {
            WebKernel = webKernel;

            LocalSettingsService.SaveSetting(webKernelSettingsKey, webKernel.Key);
        }
    }
}
