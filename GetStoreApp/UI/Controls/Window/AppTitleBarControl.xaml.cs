using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;

namespace GetStoreApp.UI.Controls.Window
{
    /// <summary>
    /// 应用标题栏用户控件视图
    /// </summary>
    public sealed partial class AppTitleBarControl : Grid
    {
        public AppTitleBarControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 右键菜单关闭时返回标题栏默认拖拽区域的高度
        /// </summary>
        public void OnClosing(object sender, FlyoutBaseClosingEventArgs args)
        {
            ViewModel.SetDragRectangles(Convert.ToInt32(Margin.Left), ActualWidth, ActualHeight);
        }

        /// <summary>
        /// 右键菜单打开时设置标题栏默认拖拽区域的高度为30（默认标题栏高度）
        /// </summary>
        public void OnOpening(object sender, object args)
        {
            ViewModel.SetDragRectangles(Convert.ToInt32(Margin.Left), ActualWidth, 30);
        }
    }
}
