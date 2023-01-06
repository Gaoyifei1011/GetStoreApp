using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

namespace GetStoreApp.Views.Controls
{
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
