using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Dialogs.CommonDialogs.Settings
{
    public class TraceCleanupModel : ModelBase
    {
        /// <summary>
        /// 清理选项是否被选择的标志
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
        /// 标志该清理选项是否清理失败
        /// </summary>
        private bool _isCleanFailed;

        public bool IsCleanFailed
        {
            get { return _isCleanFailed; }

            set
            {
                _isCleanFailed = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 清理选项显示的名称
        /// </summary>
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(TraceCleanupModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 清理选项内部的名称
        /// </summary>
        public CleanArgs InternalName
        {
            get { return (CleanArgs)GetValue(InternalNameProperty); }
            set { SetValue(InternalNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalNameProperty =
            DependencyProperty.Register("InternalName", typeof(string), typeof(TraceCleanupModel), new PropertyMetadata(CleanArgs.History));

        /// <summary>
        /// 清理失败时详细的错误文字信息
        /// </summary>
        public string CleanFailedText
        {
            get { return (string)GetValue(CleanFailedTextProperty); }
            set { SetValue(CleanFailedTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CleanFailedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CleanFailedTextProperty =
            DependencyProperty.Register("CleanFailedText", typeof(string), typeof(TraceCleanupModel), new PropertyMetadata(string.Empty));
    }
}
