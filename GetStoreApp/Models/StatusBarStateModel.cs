using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Models
{
    public class StatusBarStateModel : DependencyObject
    {
        /// <summary>
        /// 信息状态栏严重程度值
        /// </summary>
        public InfoBarSeverity InfoBarSeverity
        {
            get { return (InfoBarSeverity)GetValue(InfoBarSeverityProperty); }
            set { SetValue(InfoBarSeverityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InfoBarSeverity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InfoBarSeverityProperty =
            DependencyProperty.Register("InfoBarSeverity", typeof(InfoBarSeverity), typeof(StatusBarStateModel), new PropertyMetadata(InfoBarSeverity.Informational));

        /// <summary>
        /// 信息状态栏文字内容
        /// </summary>
        public string StateInfoText
        {
            get { return (string)GetValue(StateInfoTextProperty); }
            set { SetValue(StateInfoTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StateInfoText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateInfoTextProperty =
            DependencyProperty.Register("StateInfoText", typeof(string), typeof(StatusBarStateModel), new PropertyMetadata(""));

        /// <summary>
        /// 信息状态栏进度栏显示值
        /// </summary>
        public bool StatePrBarVisValue
        {
            get { return (bool)GetValue(StatePrRingVisValueProperty); }
            set { SetValue(StatePrRingVisValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatePrBarVisValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatePrRingVisValueProperty =
            DependencyProperty.Register("StatePrBarVisValue", typeof(bool), typeof(StatusBarStateModel), new PropertyMetadata(false));

        /// <summary>
        /// 信息状态栏进度栏激活值
        /// </summary>
        public bool StatePrBarActValue
        {
            get { return (bool)GetValue(StatePrRingActValueProperty); }
            set { SetValue(StatePrRingActValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatePrBarActValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatePrRingActValueProperty =
            DependencyProperty.Register("StatePrBarActValue", typeof(bool), typeof(StatusBarStateModel), new PropertyMetadata(false));
    }
}
