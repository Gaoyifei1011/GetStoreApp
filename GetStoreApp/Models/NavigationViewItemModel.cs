using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 导航控件项数据模型
    /// </summary>
    internal sealed class NavigationViewItemModel
    {
        /// <summary>
        /// 导航控件项具体类型
        /// </summary>
        internal NavigationViewItemKind NavigationViewItemKind { get; set; }

        /// <summary>
        /// 导航图标
        /// </summary>
        internal IconElement NavigationIcon { get; set; }

        /// <summary>
        /// 导航标题
        /// </summary>
        internal string NavigationTitle { get; set; }

        /// <summary>
        /// 导航标签
        /// </summary>
        internal string NavigationTag { get; set; }

        /// <summary>
        /// 导航类型
        /// </summary>
        internal Type NavigationPage { get; set; }

        /// <summary>
        /// 右键菜单显示状态
        /// </summary>
        internal Visibility ContextMenuVisibleState { get; set; }
    }
}
