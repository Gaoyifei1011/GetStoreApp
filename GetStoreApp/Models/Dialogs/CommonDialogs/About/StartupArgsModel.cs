using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Dialogs.CommonDialogs.About
{
    public class StartupArgsModel : ModelBase
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ArgumentName
        {
            get { return (string)GetValue(ArgumentNameProperty); }
            set { SetValue(ArgumentNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ArgumentName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArgumentNameProperty =
            DependencyProperty.Register("ArgumentName", typeof(string), typeof(StartupArgsModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 具体参数
        /// </summary>
        public string Argument
        {
            get { return (string)GetValue(ArgumentProperty); }
            set { SetValue(ArgumentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Argument.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArgumentProperty =
            DependencyProperty.Register("Argument", typeof(string), typeof(StartupArgsModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 参数是否必需要输入
        /// </summary>
        public string IsRequired
        {
            get { return (string)GetValue(IsRequiredProperty); }
            set { SetValue(IsRequiredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRequired.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register("IsRequired", typeof(string), typeof(StartupArgsModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 参数具体内容
        /// </summary>
        public string ArgumentContent
        {
            get { return (string)GetValue(ArgumentContentProperty); }
            set { SetValue(ArgumentContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ArgumentContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArgumentContentProperty =
            DependencyProperty.Register("ArgumentContent", typeof(string), typeof(StartupArgsModel), new PropertyMetadata(string.Empty));
    }
}
