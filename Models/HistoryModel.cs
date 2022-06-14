using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    public class HistoryModel : DependencyObject
    {
        /// <summary>
        /// 历史记录生成时对应的时间戳，本地存储时使用的是格林尼治标准时间（GMT+0）
        /// </summary>
        public string CurrentTimeStamp
        {
            get { return (string)GetValue(CurrentTimeStampProperty); }
            set { SetValue(CurrentTimeStampProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTimeStamp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeStampProperty =
            DependencyProperty.Register("CurrentTimeStamp", typeof(string), typeof(HistoryModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 历史记录的索引键值
        /// </summary>
        public string HistoryKey
        {
            get { return (string)GetValue(HistoryKeyProperty); }
            set { SetValue(HistoryKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryKeyProperty =
            DependencyProperty.Register("HistoryKey", typeof(string), typeof(HistoryModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 历史记录中包含的类型
        /// </summary>
        public GetAppTypeModel HistoryType
        {
            get { return (GetAppTypeModel)GetValue(HistoryTypeProperty); }
            set { SetValue(HistoryTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryTypeProperty =
            DependencyProperty.Register("HistoryType", typeof(GetAppTypeModel), typeof(HistoryModel), new PropertyMetadata(new GetAppTypeModel()));

        /// <summary>
        /// 历史记录中包含的通道
        /// </summary>
        public GetAppChannelModel HistoryChannel
        {
            get { return (GetAppChannelModel)GetValue(HistoryChannelProperty); }
            set { SetValue(HistoryChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryChannelProperty =
            DependencyProperty.Register("HistoryChannel", typeof(GetAppChannelModel), typeof(HistoryModel), new PropertyMetadata(new GetAppChannelModel()));

        /// <summary>
        /// 历史记录包含的链接
        /// </summary>
        public string HistoryLink
        {
            get { return (string)GetValue(HistoryLinkProperty); }
            set { SetValue(HistoryLinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryLink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryLinkProperty =
            DependencyProperty.Register("HistoryLink", typeof(string), typeof(HistoryModel), new PropertyMetadata(string.Empty));
    }
}
