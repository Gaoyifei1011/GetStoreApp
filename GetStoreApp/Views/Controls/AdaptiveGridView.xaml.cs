using GetStoreApp.Converters.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Windows.Input;
using Windows.Foundation.Collections;

namespace GetStoreApp.Views.Controls
{
    /// <summary>
    /// 自适应窗口宽度的网格控件
    /// </summary>
    public partial class AdaptiveGridView : GridView
    {
        private bool _isLoaded;

        private ScrollMode _savedVerticalScrollMode;

        private ScrollMode _savedHorizontalScrollMode;

        private ScrollBarVisibility _savedVerticalScrollBarVisibility;

        private ScrollBarVisibility _savedHorizontalScrollBarVisibility;

        private Orientation _savedOrientation;

        private bool _needToRestoreScrollStates;

        private bool _needContainerMarginForLayout;

        /// <summary>
        /// 获取或设置每个项的所需宽度
        /// </summary>
        public double DesiredWidth
        {
            get { return (double)GetValue(DesiredWidthProperty); }
            set { SetValue(DesiredWidthProperty, value); }
        }

        public static readonly DependencyProperty DesiredWidthProperty =
            DependencyProperty.Register("DesiredWidth", typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(double.NaN, DesiredWidthChanged));

        /// <summary>
        /// 获取或设置在单击项且 IsItemClickEnabled 属性为 true 时要执行的命令
        /// </summary>
        public ICommand ItemClickCommand
        {
            get { return (ICommand)GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(AdaptiveGridView), new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置网格中每个项的高度
        /// </summary>
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(double.NaN));

        /// <summary>
        /// 获取或设置网格中每个项的宽度
        /// </summary>
        private double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        private static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(AdaptiveGridView), new PropertyMetadata(double.NaN));

        /// <summary>
        /// 获取或设置一个值，该值指示是否只应显示一行
        /// </summary>
        public bool OneRowModeEnabled
        {
            get { return (bool)GetValue(OneRowModeEnabledProperty); }
            set { SetValue(OneRowModeEnabledProperty, value); }
        }

        public static readonly DependencyProperty OneRowModeEnabledProperty =
            DependencyProperty.Register("OneRowModeEnabled", typeof(bool), typeof(AdaptiveGridView), new PropertyMetadata(false, delegate (DependencyObject o, DependencyPropertyChangedEventArgs args)
            {
                OnOneRowModeEnabledChanged(o, args.NewValue);
            }));

        /// <summary>
        /// 获取或设置一个值，该值指示控件是否应拉伸内容以填充至少一行
        /// </summary>
        public bool StretchContentForSingleRow
        {
            get { return (bool)GetValue(StretchContentForSingleRowProperty); }
            set { SetValue(StretchContentForSingleRowProperty, value); }
        }

        public static readonly DependencyProperty StretchContentForSingleRowProperty =
            DependencyProperty.Register("StretchContentForSingleRow", typeof(bool), typeof(AdaptiveGridView), new PropertyMetadata(true, OnStretchContentForSingleRowPropertyChanged));

        public new ItemsPanelTemplate ItemsPanel => ItemsPanel;

        public AdaptiveGridView()
        {
            IsTabStop = false;
            SizeChanged += OnSizeChanged;
            ItemClick += OnItemClick;
            Items.VectorChanged += ItemsOnVectorChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            UseLayoutRounding = false;
        }

        ~AdaptiveGridView()
        {
            SizeChanged -= OnSizeChanged;
            ItemClick -= OnItemClick;
            Items.VectorChanged -= ItemsOnVectorChanged;
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
        }

        /// <summary>准备指定的元素以显示指定的项。</summary>
        /// <param name="obj">用于显示指定项的元素。</param>
        /// <param name="item">要显示的项。</param>
        protected override void PrepareContainerForItemOverride(DependencyObject obj, object item)
        {
            base.PrepareContainerForItemOverride(obj, item);
            if (obj is FrameworkElement frameworkElement)
            {
                Binding binding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("ItemHeight"),
                    Mode = BindingMode.TwoWay
                };
                Binding binding2 = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("ItemWidth"),
                    Mode = BindingMode.TwoWay
                };
                frameworkElement.SetBinding(HeightProperty, binding);
                frameworkElement.SetBinding(WidthProperty, binding2);
            }

            if (obj is ContentControl contentControl)
            {
                contentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                contentControl.VerticalContentAlignment = VerticalAlignment.Stretch;
            }

            if (_needContainerMarginForLayout)
            {
                _needContainerMarginForLayout = false;
                RecalculateLayout(ActualWidth);
            }
        }

        /// <summary>计算网格项的宽度。</summary>
        /// <param name="containerWidth">容器控件的宽度。</param>
        /// <returns>计算的项目宽度。</returns>
        protected virtual double CalculateItemWidth(double containerWidth)
        {
            if (double.IsNaN(DesiredWidth))
            {
                return DesiredWidth;
            }

            int num = CalculateColumns(containerWidth, DesiredWidth);
            if (Items != null && Items.Count > 0 && Items.Count < num && StretchContentForSingleRow)
            {
                num = Items.Count;
            }

            Thickness thickness = default;
            Thickness itemMargin = AdaptiveHeightValueConverter.GetItemMargin(this, thickness);
            if (itemMargin.Equals(thickness))
            {
                _needContainerMarginForLayout = true;
            }

            return containerWidth / num - itemMargin.Left - itemMargin.Right;
        }

        /// 每当应用程序代码或内部进程（如重建布局传递）调用 ApplyTemplate 时调用。简单来说，这意味着在应用中显示 UI 元素之前调用该方法。重写此方法以影响类的默认后模板逻辑。
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnOneRowModeEnabledChanged(this, OneRowModeEnabled);
        }

        private void ItemsOnVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            if (!double.IsNaN(ActualWidth))
            {
                RecalculateLayout(ActualWidth);
            }
        }

        private void OnItemClick(object sender, ItemClickEventArgs args)
        {
            ICommand itemClickCommand = ItemClickCommand;
            if (itemClickCommand != null && itemClickCommand.CanExecute(args.ClickedItem))
            {
                itemClickCommand.Execute(args.ClickedItem);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                int num = CalculateColumns(args.PreviousSize.Width, DesiredWidth);
                int num2 = CalculateColumns(args.NewSize.Width, DesiredWidth);
                if (num != num2)
                {
                    RecalculateLayout(args.NewSize.Width);
                }
            }
            else if (args.PreviousSize.Width != args.NewSize.Width)
            {
                RecalculateLayout(args.NewSize.Width);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            _isLoaded = true;
            DetermineOneRowMode();
        }

        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            _isLoaded = false;
        }

        private void DetermineOneRowMode()
        {
            if (!_isLoaded)
            {
                return;
            }

            ItemsWrapGrid itemsWrapGrid = ItemsPanelRoot as ItemsWrapGrid;
            if (OneRowModeEnabled)
            {
                Binding binding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("ItemHeight"),
                    Converter = new AdaptiveHeightValueConverter(),
                    ConverterParameter = this
                };
                if (itemsWrapGrid != null)
                {
                    _savedOrientation = itemsWrapGrid.Orientation;
                    itemsWrapGrid.Orientation = Orientation.Vertical;
                }

                SetBinding(MaxHeightProperty, binding);
                _savedHorizontalScrollMode = ScrollViewer.GetHorizontalScrollMode(this);
                _savedVerticalScrollMode = ScrollViewer.GetVerticalScrollMode(this);
                _savedHorizontalScrollBarVisibility = ScrollViewer.GetHorizontalScrollBarVisibility(this);
                _savedVerticalScrollBarVisibility = ScrollViewer.GetVerticalScrollBarVisibility(this);
                _needToRestoreScrollStates = true;
                ScrollViewer.SetVerticalScrollMode(this, ScrollMode.Disabled);
                ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Hidden);
                ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Visible);
                ScrollViewer.SetHorizontalScrollMode(this, ScrollMode.Enabled);
                return;
            }

            ClearValue(MaxHeightProperty);
            if (_needToRestoreScrollStates)
            {
                _needToRestoreScrollStates = false;
                if (itemsWrapGrid != null)
                {
                    itemsWrapGrid.Orientation = _savedOrientation;
                }

                ScrollViewer.SetVerticalScrollMode(this, _savedVerticalScrollMode);
                ScrollViewer.SetVerticalScrollBarVisibility(this, _savedVerticalScrollBarVisibility);
                ScrollViewer.SetHorizontalScrollBarVisibility(this, _savedHorizontalScrollBarVisibility);
                ScrollViewer.SetHorizontalScrollMode(this, _savedHorizontalScrollMode);
            }
        }

        private void RecalculateLayout(double containerWidth)
        {
            Panel itemsPanelRoot = ItemsPanelRoot;
            double num = ((itemsPanelRoot != null) ? (itemsPanelRoot.Margin.Left + itemsPanelRoot.Margin.Right) : 0.0);
            double num2 = Padding.Left + Padding.Right;
            double num3 = BorderThickness.Left + BorderThickness.Right;
            containerWidth = containerWidth - num2 - num - num3;
            if (containerWidth > 0.0)
            {
                double d = CalculateItemWidth(containerWidth);
                ItemWidth = Math.Floor(d);
            }
        }

        private static void OnOneRowModeEnabledChanged(DependencyObject d, object newValue)
        {
            (d as AdaptiveGridView).DetermineOneRowMode();
        }

        private static void DesiredWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            AdaptiveGridView obj = d as AdaptiveGridView;
            obj.RecalculateLayout(obj.ActualWidth);
        }

        private static void OnStretchContentForSingleRowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            AdaptiveGridView obj = d as AdaptiveGridView;
            obj.RecalculateLayout(obj.ActualWidth);
        }

        private static int CalculateColumns(double containerWidth, double itemWidth)
        {
            int num = (int)Math.Round(containerWidth / itemWidth);
            if (num == 0)
            {
                num = 1;
            }

            return num;
        }
    }
}
