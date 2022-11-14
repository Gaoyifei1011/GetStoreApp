using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Controls.Settings.Common
{
    public class NotificationModel : ModelBase
    {
        /// <summary>
        /// 显示通知设置显示名称
        /// </summary>
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(NotificationModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 显示通知设置内部名称
        /// </summary>
        public string InternalName
        {
            get { return (string)GetValue(InternalNameProperty); }
            set { SetValue(InternalNameProperty, value); }
        }

        public static readonly DependencyProperty InternalNameProperty =
            DependencyProperty.Register("InternalName", typeof(string), typeof(NotificationModel), new PropertyMetadata(string.Empty));
    }
}
