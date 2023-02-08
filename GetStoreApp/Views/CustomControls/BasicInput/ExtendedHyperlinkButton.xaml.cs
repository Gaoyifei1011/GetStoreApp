using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

namespace GetStoreApp.Views.CustomControls.BasicInput
{
    /// <summary>
    /// 扩展后的超链接按钮，实现右键单击命令绑定
    /// </summary>
    public sealed partial class ExtendedHyperlinkButton : HyperlinkButton
    {
        public ICommand RightClickCommand
        {
            get { return (ICommand)GetValue(RightClickCommandProperty); }
            set { SetValue(RightClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightClickCommandProperty =
            DependencyProperty.Register("RightClickCommand", typeof(ICommand), typeof(ExtendedHyperlinkButton), new PropertyMetadata(null));

        public object RightClickCommandParameter
        {
            get { return GetValue(RightClickCommandParameterProperty); }
            set { SetValue(RightClickCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightClickCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightClickCommandParameterProperty =
            DependencyProperty.Register("RightClickCommandParameter", typeof(object), typeof(ExtendedHyperlinkButton), new PropertyMetadata(null));

        public ExtendedHyperlinkButton()
        {
            InitializeComponent();
        }

        protected override void OnRightTapped(RightTappedRoutedEventArgs args)
        {
            base.OnRightTapped(args);

            ICommand clickCommand = RightClickCommand;
            if (clickCommand is not null)
            {
                clickCommand.Execute(RightClickCommandParameter);
            }
        }
    }
}
