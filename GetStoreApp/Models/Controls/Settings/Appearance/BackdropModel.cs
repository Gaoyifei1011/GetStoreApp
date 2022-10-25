using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Controls.Settings.Appearance
{
    public class BackdropModel : ModelBase
    {
        /// <summary>
        /// 应用背景色设置显示名称
        /// </summary>
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(BackdropModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 应用背景色设置内部名称
        /// </summary>
        public string InternalName
        {
            get { return (string)GetValue(InternalNameProperty); }
            set { SetValue(InternalNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalNameProperty =
            DependencyProperty.Register("InternalName", typeof(string), typeof(BackdropModel), new PropertyMetadata(string.Empty));
    }
}
