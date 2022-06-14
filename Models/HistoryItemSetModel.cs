using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    public class HistoryItemSetModel : DependencyObject
    {
        public string HistoryItemName
        {
            get { return (string)GetValue(HistoryItemNameProperty); }
            set { SetValue(HistoryItemNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryItemName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryItemNameProperty =
            DependencyProperty.Register("HistoryItemName", typeof(string), typeof(HistoryItemSetModel), new PropertyMetadata(string.Empty));

        public int HistoryItemNum
        {
            get { return (int)GetValue(HistoryItemNumProperty); }
            set { SetValue(HistoryItemNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryItemNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryItemNumProperty =
            DependencyProperty.Register("HistoryItemNum", typeof(int), typeof(HistoryItemSetModel), new PropertyMetadata(0));
    }
}
