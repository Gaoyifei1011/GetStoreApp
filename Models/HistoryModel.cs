using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 该类用来存放数据库原始数据
    /// </summary>
    public class HistoryModel : DependencyObject, INotifyPropertyChanged, IComparable<HistoryModel>
    {
        /// <summary>
        /// 在多选模式下，该行历史记录是否被选择的标志
        /// </summary>
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        /// <summary>
        /// 历史记录生成时对应的时间戳，本地存储时使用的是格林尼治标准时间（GMT+0）
        /// </summary>
        public long CurrentTimeStamp
        {
            get { return (long)GetValue(CurrentTimeStampProperty); }
            set { SetValue(CurrentTimeStampProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTimeStamp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeStampProperty =
            DependencyProperty.Register("CurrentTimeStamp", typeof(string), typeof(HistoryModel), new PropertyMetadata(0));

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
        /// 历史记录中包含的类型，数据库存储的原始名称
        /// </summary>
        public string HistoryType
        {
            get { return (string)GetValue(HistoryTypeProperty); }
            set { SetValue(HistoryTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryTypeProperty =
            DependencyProperty.Register("HistoryType", typeof(string), typeof(HistoryModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 历史记录中包含的通道
        /// </summary>
        public string HistoryChannel
        {
            get { return (string)GetValue(HistoryChannelProperty); }
            set { SetValue(HistoryChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HistoryChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryChannelProperty =
            DependencyProperty.Register("HistoryChannel", typeof(string), typeof(HistoryModel), new PropertyMetadata(string.Empty));

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

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 按时间升序排列
        /// </summary>
        /// <param name="other"></param>
        public int CompareTo(HistoryModel other)
        {
            return this.CurrentTimeStamp.CompareTo(other.CurrentTimeStamp);
        }
    }
}
