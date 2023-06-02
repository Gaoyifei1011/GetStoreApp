using GetStoreApp.Helpers.Window;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Graphics;

namespace GetStoreApp.ViewModels.Controls.Window
{
    /// <summary>
    /// 应用标题栏用户控件视图模型
    /// </summary>
    public sealed class AppTitleBarViewModel : ViewModelBase
    {
        /// <summary>
        /// 初始化自定义标题栏
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            Grid appTitleBar = sender as Grid;
            if (appTitleBar is not null)
            {
                SetDragRectangles(Convert.ToInt32(appTitleBar.Margin.Left), appTitleBar.ActualWidth, appTitleBar.ActualHeight);
            }
        }

        /// <summary>
        /// 控件大小发生变化时，修改拖动区域
        /// </summary>
        public void OnSizeChanged(object sender, RoutedEventArgs args)
        {
            Grid appTitleBar = sender as Grid;
            if (appTitleBar is not null)
            {
                SetDragRectangles(Convert.ToInt32(appTitleBar.Margin.Left), appTitleBar.ActualWidth, appTitleBar.ActualHeight);
            }
        }

        /// <summary>
        /// 设置标题栏的可拖动区域
        /// </summary>
        private void SetDragRectangles(int leftMargin, double actualWidth, double actualHeight)
        {
            Program.ApplicationRoot.MainWindow.AppWindow.TitleBar.SetDragRectangles(new RectInt32[] { new RectInt32(leftMargin, 0, DPICalcHelper.ConvertEpxToPixel(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), Convert.ToInt32(actualWidth)), DPICalcHelper.ConvertEpxToPixel(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), Convert.ToInt32(actualHeight))) });
        }
    }
}
