using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Controls.Settings.Appearance
{
    public class LanguageModel : ModelBase
    {
        /// <summary>
        /// 应用语言设置显示名称
        /// </summary>
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(LanguageModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 应用语言设置内部名称
        /// </summary>
        public string InternalName
        {
            get { return (string)GetValue(InternalNameProperty); }
            set { SetValue(InternalNameProperty, value); }
        }

        public static readonly DependencyProperty InternalNameProperty =
            DependencyProperty.Register("InternalName", typeof(string), typeof(LanguageModel), new PropertyMetadata(string.Empty));
    }
}
