using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

namespace GetStoreApp.Views.CustomControls.Navigation
{
    /// <summary>
    /// 扩展后的导航控件项，附带命令绑定
    /// </summary>
    public partial class ExtendedNavigationViewItem : NavigationViewItem
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }

            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ExtendedNavigationViewItem), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }

            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ExtendedNavigationViewItem), new PropertyMetadata(null));

        public string ToolTip
        {
            get { return (string)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.Register("ToolTip", typeof(string), typeof(ExtendedNavigationViewItem), new PropertyMetadata(string.Empty));

        public ExtendedNavigationViewItem()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        ~ExtendedNavigationViewItem()
        {
            Loaded -= OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(ToolTip))
            {
                ToolTip NavigationItemToolTip = new ToolTip();
                NavigationItemToolTip.Content = string.Format("{0} ", ToolTip);
                NavigationItemToolTip.Placement = PlacementMode.Bottom;
                NavigationItemToolTip.VerticalOffset = 20;
                ToolTipService.SetToolTip(this, NavigationItemToolTip);
            }
        }

        /// <summary>
        /// 点击导航控件项时触发命令
        /// </summary>
        protected override void OnTapped(TappedRoutedEventArgs args)
        {
            base.OnTapped(args);

            ICommand clickCommand = Command;
            if (clickCommand is not null)
            {
                clickCommand.Execute(CommandParameter);
            }
        }
    }
}
