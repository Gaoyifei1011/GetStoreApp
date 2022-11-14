using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Controls.Home
{
    public class ResultModel : ModelBase
    {
        /// <summary>
        /// 在多选模式下，该行信息是否被选择的标志
        /// </summary>
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(ResultModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 文件下载链接
        /// </summary>
        public string FileLink
        {
            get { return (string)GetValue(FileLinkProperty); }
            set { SetValue(FileLinkProperty, value); }
        }

        public static readonly DependencyProperty FileLinkProperty =
            DependencyProperty.Register("FileLink", typeof(string), typeof(ResultModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 文件下载链接过期时间
        /// </summary>
        public string FileLinkExpireTime
        {
            get { return (string)GetValue(FileLinkExpireTimeProperty); }
            set { SetValue(FileLinkExpireTimeProperty, value); }
        }

        public static readonly DependencyProperty FileLinkExpireTimeProperty =
            DependencyProperty.Register("FileLinkExpireTime", typeof(string), typeof(ResultModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 文件SHA1值
        /// </summary>
        public string FileSHA1
        {
            get { return (string)GetValue(FileSHA1Property); }
            set { SetValue(FileSHA1Property, value); }
        }

        public static readonly DependencyProperty FileSHA1Property =
            DependencyProperty.Register("FileSHA1", typeof(string), typeof(ResultModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize
        {
            get { return (string)GetValue(FileSizeProperty); }
            set { SetValue(FileSizeProperty, value); }
        }

        public static readonly DependencyProperty FileSizeProperty =
            DependencyProperty.Register("FileSize", typeof(string), typeof(ResultModel), new PropertyMetadata(string.Empty));
    }
}
