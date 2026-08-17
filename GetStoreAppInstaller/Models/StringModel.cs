using System;
using System.ComponentModel;

namespace GetStoreAppInstaller.Models
{
    /// <summary>
    /// 包文件索引字符串数据模型
    /// </summary>
    internal sealed partial class StringModel : INotifyPropertyChanged, IComparable<StringModel>
    {
        /// <summary>
        /// 是否已选择
        /// </summary>
        private bool _isSelected;

        internal bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
                }
            }
        }

        /// <summary>
        /// 字符串对应的键
        /// </summary>
        internal string Key { get; set; }

        /// <summary>
        /// 字符串对应的内容
        /// </summary>
        internal string Content { get; set; }

        /// <summary>
        /// 字符串对应的语言
        /// </summary>
        internal string Language { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public int CompareTo(StringModel other)
        {
            if (Key != other.Key)
            {
                return Key.CompareTo(other.Key);
            }
            else if (Language != other.Language)
            {
                return Language.CompareTo(other.Language);
            }
            else
            {
                return 0;
            }
        }
    }
}
