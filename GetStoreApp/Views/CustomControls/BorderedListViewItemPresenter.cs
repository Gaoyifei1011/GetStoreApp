using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using WinRT;

namespace GetStoreApp.Views.CustomControls
{
    /// <summary>
    /// 带边框的表示列表项的视觉元素
    /// </summary>
    internal sealed partial class BorderedListViewItemPresenter : ListViewItemPresenter
    {
        [DynamicWindowsRuntimeCast(typeof(Border))]
        internal BorderedListViewItemPresenter()
        {
            Loaded += (sender, args) =>
            {
                if (VisualTreeHelper.GetChildrenCount(this) > 0 && VisualTreeHelper.GetChild(this, 0) is Border listViewItemPresenterBorder)
                {
                    listViewItemPresenterBorder.BorderBrush = BorderBrush;
                    listViewItemPresenterBorder.BorderThickness = BorderThickness;
                    listViewItemPresenterBorder.Margin = new(0);
                }
            };
        }
    }
}
