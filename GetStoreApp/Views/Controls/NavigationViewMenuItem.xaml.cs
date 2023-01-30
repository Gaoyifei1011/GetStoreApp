using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

namespace GetStoreApp.Views.Controls
{
    /// <summary>
    /// 附带命令的导航控件项的容器
    /// </summary>
    public partial class NavigationViewMenuItem : NavigationViewItem
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }

            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("ActivatedCommand", typeof(ICommand), typeof(NavigationViewMenuItem), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }

            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(NavigationViewMenuItem), new PropertyMetadata(null));

        public string ToolTip
        {
            get { return (string)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.Register("ToolTip", typeof(string), typeof(NavigationViewMenuItem), new PropertyMetadata(string.Empty));

        public NavigationViewMenuItem()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Tapped += OnTapped;
        }

        ~NavigationViewMenuItem()
        {
            Loaded -= OnLoaded;
            Tapped -= OnTapped;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(ToolTip))
            {
                ToolTip NavigationItemToolTip = new ToolTip();
                NavigationItemToolTip.Content = string.Format("{0} ", ToolTip);
                NavigationItemToolTip.Placement = PlacementMode.Mouse;
                NavigationItemToolTip.VerticalOffset = 10;
                ToolTipService.SetToolTip(this,NavigationItemToolTip);
            }
        }

        /// <summary>
        /// 点击导航控件项时触发命令
        /// </summary>
        private void OnTapped(object sender, TappedRoutedEventArgs args)
        {
            ICommand clickCommand = Command;
            if (clickCommand is not null)
            {
                clickCommand.Execute(CommandParameter);
            }
        }
    }
}
