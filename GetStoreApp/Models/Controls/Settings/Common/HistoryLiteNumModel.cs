using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Controls.Settings.Common
{
    public class HistoryLiteNumModel : ModelBase
    {
        /// <summary>
        /// 主页面历史记录显示数量设置显示名称
        /// </summary>
        public string HistoryLiteNumName
        {
            get { return (string)GetValue(HistoryLiteNumNameProperty); }
            set { SetValue(HistoryLiteNumNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryLiteNumName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryLiteNumNameProperty =
            DependencyProperty.Register("HistoryLiteNumName", typeof(string), typeof(HistoryLiteNumModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 主页面历史记录显示数量设置内部名称
        /// </summary>
        public int HistoryLiteNumValue
        {
            get { return (int)GetValue(HistoryLiteNumValueProperty); }
            set { SetValue(HistoryLiteNumValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryLiteNumValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryLiteNumValueProperty =
            DependencyProperty.Register("HistoryLiteNum", typeof(int), typeof(HistoryLiteNumModel), new PropertyMetadata(default(int)));
    }
}
