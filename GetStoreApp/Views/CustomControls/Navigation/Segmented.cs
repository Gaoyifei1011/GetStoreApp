using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace GetStoreApp.Views.CustomControls.Navigation
{
    /// <summary>
    /// 分段控件
    /// </summary>
    public partial class Segmented : ListViewBase
    {
        private int internalSelectedIndex = -1;
        private bool hasLoaded = false;

        public Segmented()
        {
            DefaultStyleKey = typeof(Segmented);
            RegisterPropertyChangedCallback(SelectedIndexProperty, OnSelectedIndexChanged);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SegmentedItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SegmentedItem;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!hasLoaded)
            {
                SelectedIndex = internalSelectedIndex;
                hasLoaded = true;
            }

            PreviewKeyDown -= OnPreviewKeyDown;
            PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is SegmentedItem segmentedItem)
            {
                segmentedItem.Loaded += OnSegmentedItemLoaded;
            }
        }

        private void OnPreviewKeyDown(object sender, KeyRoutedEventArgs args)
        {
            switch (args.Key)
            {
                case VirtualKey.Left: args.Handled = MoveFocus(false); break;
                case VirtualKey.Right: args.Handled = MoveFocus(true); break;
            }
        }

        private void OnSegmentedItemLoaded(object sender, RoutedEventArgs args)
        {
            if (sender is SegmentedItem segmentedItem)
            {
                segmentedItem.Loaded -= OnSegmentedItemLoaded;
            }
        }

        /// <summary>
        /// 根据键盘输入调整所选项目和范围
        /// 这用于重写向上/向下箭头操作的 ListView 行为与水平控件的左/右操作的 ListView 行为
        /// </summary>
        /// <param name="direction">移动选区的方向，向前移动为 true，否则为 false</param>
        /// <returns>如果焦点已移动，则为 true，否则为 false</returns>
        private bool MoveFocus(bool moveForward)
        {
            bool retVal = false;
            SegmentedItem currentContainerItem = GetCurrentContainerItem();

            if (currentContainerItem != null)
            {
                object currentItem = ItemFromContainer(currentContainerItem);
                int previousIndex = Items.IndexOf(currentItem);
                int index = previousIndex;

                if (moveForward == true)
                {
                    if (previousIndex < Items.Count - 1)
                    {
                        index += 1;
                    }
                }
                else
                {
                    if (previousIndex > 0)
                    {
                        index -= 1;
                    }
                    else
                    {
                        retVal = true;
                    }
                }

                // 只有当索引实际发生变化时才做一些事情
                if (index != previousIndex && ContainerFromIndex(index) is SegmentedItem newItem)
                {
                    newItem.Focus(FocusState.Keyboard);
                    retVal = true;
                }
            }

            return retVal;
        }

        private SegmentedItem GetCurrentContainerItem()
        {
            if (XamlRoot is not null)
            {
                return FocusManager.GetFocusedElement(XamlRoot) as SegmentedItem;
            }
            else
            {
                return FocusManager.GetFocusedElement() as SegmentedItem;
            }
        }

        private void OnSelectedIndexChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (internalSelectedIndex == -1 && SelectedIndex > -1)
            {
                internalSelectedIndex = SelectedIndex;
            }
        }
    }
}
