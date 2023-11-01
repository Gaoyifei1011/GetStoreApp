using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace GetStoreApp.Views.CustomControls.Navigation
{
    /// <summary>
    /// 等距面板
    /// </summary>
    public partial class EqualPanel : Panel
    {
        private double maxItemWidth = 0;
        private double maxItemHeight = 0;
        private int visibleItemsCount = 0;

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }

            set { SetValue(SpacingProperty, value); }
        }

        public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
            nameof(Spacing),
            typeof(double),
            typeof(EqualPanel),
            new PropertyMetadata(default(double), OnSpacingChanged));

        public EqualPanel()
        {
            RegisterPropertyChangedCallback(HorizontalAlignmentProperty, OnHorizontalAlignmentChanged);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            maxItemWidth = 0;
            maxItemHeight = 0;

            IEnumerable<UIElement> elements = Children.Where(static e => e.Visibility == Visibility.Visible);
            visibleItemsCount = elements.Count();

            foreach (var child in elements)
            {
                child.Measure(availableSize);
                maxItemWidth = Math.Max(maxItemWidth, child.DesiredSize.Width);
                maxItemHeight = Math.Max(maxItemHeight, child.DesiredSize.Height);
            }

            if (visibleItemsCount > 0)
            {
                // 根据最宽的项返回相等的宽度
                // 在非常特定的边缘情况下，AvailableWidth 可能是无限的，从而导致崩溃。
                if (HorizontalAlignment is not HorizontalAlignment.Stretch || double.IsInfinity(availableSize.Width))
                {
                    return new Size((maxItemWidth * visibleItemsCount) + (Spacing * (visibleItemsCount - 1)), maxItemHeight);
                }
                else
                {
                    // 根据可用宽度相等列，调整间距
                    double totalWidth = availableSize.Width - (Spacing * (visibleItemsCount - 1));
                    maxItemWidth = totalWidth / visibleItemsCount;
                    return new Size(availableSize.Width, maxItemHeight);
                }
            }
            else
            {
                return new Size(0, 0);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0;

            // 检查是否有更多宽度可用 - 如果有，请重新计算
            if (finalSize.Width > visibleItemsCount * maxItemWidth + (Spacing * (visibleItemsCount - 1)))
            {
                MeasureOverride(finalSize);
            }

            IEnumerable<UIElement> elements = Children.Where(static e => e.Visibility == Visibility.Visible);
            foreach (var child in elements)
            {
                child.Arrange(new Rect(x, 0, maxItemWidth, maxItemHeight));
                x += maxItemWidth + Spacing;
            }
            return finalSize;
        }

        private void OnHorizontalAlignmentChanged(DependencyObject sender, DependencyProperty dp)
        {
            InvalidateMeasure();
        }

        private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var panel = (EqualPanel)d;
            panel.InvalidateMeasure();
        }
    }
}
