using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

namespace GetStoreApp.Views.Controls
{
    /// <summary>
    /// 附带命令的导航控件项的容器
    /// </summary>
    public sealed partial class NavigationViewMenuItem : NavigationViewItem
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

        public NavigationViewMenuItem()
        {
            InitializeComponent();
            Tapped += OnTapped;
        }

        ~NavigationViewMenuItem()
        {
            Tapped -= OnTapped;
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
