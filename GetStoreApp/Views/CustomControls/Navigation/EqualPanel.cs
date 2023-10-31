using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Windows.Foundation;

namespace GetStoreApp.Views.CustomControls.Navigation
{
    public partial class EqualPanel : Panel
    {
        private double _maxItemWidth = 0;
        private double _maxItemHeight = 0;
        private int _visibleItemsCount = 0;

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        /// <summary>
        /// Identifies the Spacing dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Spacing"/> dependency property.</returns>
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
            _maxItemWidth = 0;
            _maxItemHeight = 0;

            var elements = Children.Where(static e => e.Visibility == Visibility.Visible);
            _visibleItemsCount = elements.Count();

            foreach (var child in elements)
            {
                child.Measure(availableSize);
                _maxItemWidth = Math.Max(_maxItemWidth, child.DesiredSize.Width);
                _maxItemHeight = Math.Max(_maxItemHeight, child.DesiredSize.Height);
            }

            if (_visibleItemsCount > 0)
            {
                // Return equal widths based on the widest item
                // In very specific edge cases the AvailableWidth might be infinite resulting in a crash.
                if (HorizontalAlignment != HorizontalAlignment.Stretch || double.IsInfinity(availableSize.Width))
                {
                    return new Size((_maxItemWidth * _visibleItemsCount) + (Spacing * (_visibleItemsCount - 1)), _maxItemHeight);
                }
                else
                {
                    // Equal columns based on the available width, adjust for spacing
                    double totalWidth = availableSize.Width - (Spacing * (_visibleItemsCount - 1));
                    _maxItemWidth = totalWidth / _visibleItemsCount;
                    return new Size(availableSize.Width, _maxItemHeight);
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

            // Check if there's more width available - if so, recalculate (e.g. whenever Grid.Column is set to Auto)
            if (finalSize.Width > _visibleItemsCount * _maxItemWidth + (Spacing * (_visibleItemsCount - 1)))
            {
                MeasureOverride(finalSize);
            }

            var elements = Children.Where(static e => e.Visibility == Visibility.Visible);
            foreach (var child in elements)
            {
                child.Arrange(new Rect(x, 0, _maxItemWidth, _maxItemHeight));
                x += _maxItemWidth + Spacing;
            }
            return finalSize;
        }

        private void OnHorizontalAlignmentChanged(DependencyObject sender, DependencyProperty dp)
        {
            InvalidateMeasure();
        }

        private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (EqualPanel)d;
            panel.InvalidateMeasure();
        }
    }
}
