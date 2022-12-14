using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.Models.Window
{
    public class NavigationModel : ModelBase
    {
        /// <summary>
        /// 页面标签
        /// </summary>
        public string NavigationTag { get; set; }

        public NavigationViewItem NavigationItem { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public Type NavigationPage { get; set; }
    }
}
