using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.CustomControls.BasicInput
{
    /// <summary>
    /// 扩展后的拆分按钮，可以在主次按钮上各自显示自己的工具提示信息
    /// </summary>
    public sealed partial class ExtendedSplitButton : SplitButton
    {
        public ExtendedSplitButton()
        {
            DefaultStyleKey = typeof(ExtendedSplitButton);
        }

        public bool IsPrimaryButtonEnabled
        {
            get { return (bool)GetValue(IsPrimaryButtonEnabledProperty); }
            set { SetValue(IsPrimaryButtonEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsPrimaryButtonEnabledProperty =
            DependencyProperty.Register("IsPrimaryButtonEnabled", typeof(bool), typeof(ExtendedSplitButton), new PropertyMetadata(true));

        public bool IsSecondaryButtonEnabled
        {
            get { return (bool)GetValue(IsSecondaryButtonEnabledProperty); }
            set { SetValue(IsSecondaryButtonEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsSecondaryButtonEnabledProperty =
            DependencyProperty.Register("IsSecondaryButtonEnabled", typeof(bool), typeof(ExtendedSplitButton), new PropertyMetadata(true));

        public object PrimaryButtonToolTip
        {
            get { return GetValue(PrimaryButtonToolTipProperty); }
            set { SetValue(PrimaryButtonToolTipProperty, value); }
        }

        public static readonly DependencyProperty PrimaryButtonToolTipProperty =
            DependencyProperty.Register("PrimaryButtonToolTip", typeof(object), typeof(ExtendedSplitButton), new PropertyMetadata(null));

        public object SecondaryButtonToolTip
        {
            get { return GetValue(SecondaryButtonToolTipProperty); }
            set { SetValue(SecondaryButtonToolTipProperty, value); }
        }

        public static readonly DependencyProperty SecondaryButtonToolTipProperty =
            DependencyProperty.Register("SecondaryButtonToolTip", typeof(object), typeof(ExtendedSplitButton), new PropertyMetadata(null));
    }
}
