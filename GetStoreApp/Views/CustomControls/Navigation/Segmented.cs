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
        private int _internalSelectedIndex = -1;
        private bool _hasLoaded = false;

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
            if (!_hasLoaded)
            {
                SelectedIndex = _internalSelectedIndex;
                _hasLoaded = true;
            }

            PreviewKeyDown -= OnPreviewKeyDown;
            PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is SegmentedItem segmentedItem)
            {
                segmentedItem.Loaded += OnSegmentedItemLoaded;
            }
        }

        private void OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Left: e.Handled = MoveFocus(MoveDirection.Previous); break;
                case VirtualKey.Right: e.Handled = MoveFocus(MoveDirection.Next); break;
            }
        }

        private void OnSegmentedItemLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is SegmentedItem segmentedItem)
            {
                segmentedItem.Loaded -= OnSegmentedItemLoaded;
            }
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
        }

        private enum MoveDirection
        {
            Next,
            Previous
        }

        /// <summary>
        /// Adjust the selected item and range based on keyboard input.
        /// This is used to override the ListView behaviors for up/down arrow manipulation vs left/right for a horizontal control
        /// </summary>
        /// <param name="direction">direction to move the selection</param>
        /// <returns>True if the focus was moved, false otherwise</returns>
        private bool MoveFocus(MoveDirection direction)
        {
            bool retVal = false;
            var currentContainerItem = GetCurrentContainerItem();

            if (currentContainerItem != null)
            {
                var currentItem = ItemFromContainer(currentContainerItem);
                var previousIndex = Items.IndexOf(currentItem);
                var index = previousIndex;

                if (direction == MoveDirection.Previous)
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
                else if (direction == MoveDirection.Next)
                {
                    if (previousIndex < Items.Count - 1)
                    {
                        index += 1;
                    }
                }

                // Only do stuff if the index is actually changing
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
            // This is a workaround for https://github.com/microsoft/microsoft-ui-xaml/issues/8257
            if (_internalSelectedIndex == -1 && SelectedIndex > -1)
            {
                // We catch the correct SelectedIndex and save it.
                _internalSelectedIndex = SelectedIndex;
            }
        }
    }
}
