using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Controls.Settings.Advanced
{
    public class InstallModeModel : DependencyObject
    {
        /// <summary>
        /// 应用安装方式设置显示名称
        /// </summary>
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(InstallModeModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 应用安装方式设置内部名称
        /// </summary>
        public string InternalName
        {
            get { return (string)GetValue(InternalNameProperty); }
            set { SetValue(InternalNameProperty, value); }
        }

        public static readonly DependencyProperty InternalNameProperty =
            DependencyProperty.Register("InternalName", typeof(string), typeof(InstallModeModel), new PropertyMetadata(string.Empty));
    }
}
