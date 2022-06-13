using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    public class HomeType : DependencyObject
    {
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(HomeType), new PropertyMetadata(""));

        public string InternalName
        {
            get { return (string)GetValue(InternalNameProperty); }
            set { SetValue(InternalNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalNameProperty =
            DependencyProperty.Register("InternalName", typeof(string), typeof(HomeType), new PropertyMetadata(""));
    }
}
