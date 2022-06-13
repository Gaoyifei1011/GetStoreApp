using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    public class HistoryItemData : DependencyObject
    {
        public string HistoryItemKey
        {
            get { return (string)GetValue(HistoryItemKeyProperty); }
            set { SetValue(HistoryItemKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryItemKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryItemKeyProperty =
            DependencyProperty.Register("HistoryItemKey", typeof(string), typeof(HistoryItemData), new PropertyMetadata(""));

        public HomeType HistoryItemType
        {
            get { return (HomeType)GetValue(HistoryItemTypeProperty); }
            set { SetValue(HistoryItemTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryItemType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryItemTypeProperty =
            DependencyProperty.Register("HistoryItemType", typeof(HomeType), typeof(HistoryItemData), new PropertyMetadata(new HomeType()));

        public HomeChannel HistoryItemChannel
        {
            get { return (HomeChannel)GetValue(HistoryItemChannelProperty); }
            set { SetValue(HistoryItemChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryItemChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryItemChannelProperty =
            DependencyProperty.Register("HistoryItemChannel", typeof(HomeChannel), typeof(HistoryItemData), new PropertyMetadata(new HomeChannel()));

        public string HistoryItemLink
        {
            get { return (string)GetValue(HistoryItemLinkProperty); }
            set { SetValue(HistoryItemLinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryItemLink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryItemLinkProperty =
            DependencyProperty.Register("HistoryItemLink", typeof(string), typeof(HistoryItemData), new PropertyMetadata(""));

        //public DateTime RequestedTime { get; set; }
    }
}
