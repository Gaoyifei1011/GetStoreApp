using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    public class HistoryItemValueModel : DependencyObject
    {
        public string HistoryItemName
        {
            get { return (string)GetValue(HistoryItemNameProperty); }
            set { SetValue(HistoryItemNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryItemName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryItemNameProperty =
            DependencyProperty.Register("HistoryItemName", typeof(string), typeof(HistoryItemValueModel), new PropertyMetadata(string.Empty));

        public int HistoryItemValue
        {
            get { return (int)GetValue(HistoryItemNumProperty); }
            set { SetValue(HistoryItemNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryItemValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryItemNumProperty =
            DependencyProperty.Register("HistoryItemValue", typeof(int), typeof(HistoryItemValueModel), new PropertyMetadata(0));
    }
}
