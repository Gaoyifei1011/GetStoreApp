using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.CustomControls.Navigation
{
    /// <summary>
    /// 表示分段控件中项的容器
    /// </summary>
    public partial class SegmentedItem : ListViewItem
    {
        private const string IconLeftState = "IconLeft";
        private const string IconOnlyState = "IconOnly";
        private const string ContentOnlyState = "ContentOnly";

        private Thickness LeftItemMargin = new Thickness(3, 3, 1, 3);
        private Thickness MiddleItemMargin = new Thickness(1, 3, 1, 3);
        private Thickness RightItemMargin = new Thickness(1, 3, 3, 3);

        private Border PART_Hover_Border;

        /// <summary>
        /// 获取或设置图标
        /// </summary>
        public IconElement Icon
        {
            get { return (IconElement)GetValue(IconProperty); }

            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(IconElement),
            typeof(SegmentedItem),
            new PropertyMetadata(defaultValue: null, (d, e) => ((SegmentedItem)d).OnIconPropertyChanged((IconElement)e.OldValue, (IconElement)e.NewValue)));

        public SegmentedItem()
        {
            DefaultStyleKey = typeof(SegmentedItem);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnIconChanged();
            ContentChanged();

            PART_Hover_Border = GetTemplateChild("PART_Hover") as Border;
            if (PART_Hover_Border is not null)
            {
                ItemsControl listView = ItemsControl.ItemsControlFromItemContainer(this);

                int index = listView.IndexFromContainer(this);

                if (index == 0)
                {
                    PART_Hover_Border.Margin = LeftItemMargin;
                }
                else if (index == listView.Items.Count - 1)
                {
                    PART_Hover_Border.Margin = RightItemMargin;
                }
                else
                {
                    PART_Hover_Border.Margin = MiddleItemMargin;
                }
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            ContentChanged();
        }

        protected virtual void OnIconPropertyChanged(IconElement oldValue, IconElement newValue)
        {
            OnIconChanged();
        }

        private void ContentChanged()
        {
            if (Content is not null)
            {
                VisualStateManager.GoToState(this, IconLeftState, true);
            }
            else
            {
                VisualStateManager.GoToState(this, IconOnlyState, true);
            }
        }

        private void OnIconChanged()
        {
            if (Icon != null)
            {
                VisualStateManager.GoToState(this, IconLeftState, true);
            }
            else
            {
                VisualStateManager.GoToState(this, ContentOnlyState, true);
            }
        }
    }
}
