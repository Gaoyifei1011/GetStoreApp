using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    public class HistoryLiteNumModel : DependencyObject
    {
        /// <summary>
        /// 主页面历史记录显示数量设置显示名称
        /// </summary>
        public string HistoryLiteNumName
        {
            get { return (string)GetValue(HistoryItemNameProperty); }
            set { SetValue(HistoryItemNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryLiteNumName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryItemNameProperty =
            DependencyProperty.Register("HistoryLiteNumName", typeof(string), typeof(HistoryLiteNumModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 主页面历史记录显示数量设置内部名称
        /// </summary>
        public int HistoryLiteNumValue
        {
            get { return (int)GetValue(HistoryItemNumProperty); }
            set { SetValue(HistoryItemNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryLiteNumValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryItemNumProperty =
            DependencyProperty.Register("HistoryLiteNum", typeof(int), typeof(HistoryLiteNumModel), new PropertyMetadata(0));
    }
}
