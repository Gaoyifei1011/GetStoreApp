using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    public class GetAppChannelModel : DependencyObject
    {
        /// <summary>
        /// 获取应用通道类型显示名称
        /// </summary>
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(GetAppChannelModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 获取应用通道类型内部名称
        /// </summary>
        public string InternalName
        {
            get { return (string)GetValue(InternalNameProperty); }
            set { SetValue(InternalNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalNameProperty =
            DependencyProperty.Register("InternalName", typeof(string), typeof(GetAppChannelModel), new PropertyMetadata(string.Empty));
    }
}
