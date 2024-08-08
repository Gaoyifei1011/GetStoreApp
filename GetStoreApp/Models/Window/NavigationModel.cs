using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.Models.Window
{
    /// <summary>
    /// 页面导航数据模型
    /// </summary>
    public sealed class NavigationModel
    {
        /// <summary>
        /// 页面导航标签
        /// </summary>
        public string NavigationTag { get; set; }

        /// <summary>
        /// 页面导航控件中项的容器
        /// </summary>
        public NavigationViewItem NavigationItem { get; set; }

        /// <summary>
        /// 页面导航类型
        /// </summary>
        public Type NavigationPage { get; set; }
    }
}
