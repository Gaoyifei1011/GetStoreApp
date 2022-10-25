using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Models.Controls.Home
{
    public class StatusBarStateModel : ModelBase
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
            DependencyProperty.Register("StateInfoText", typeof(string), typeof(StatusBarStateModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 信息状态栏进度环显示值
        /// </summary>
        public bool StatePrRingVisValue
        {
            get { return (bool)GetValue(StatePrRingVisValueProperty); }
            set { SetValue(StatePrRingVisValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatePrRingVisValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatePrRingVisValueProperty =
            DependencyProperty.Register("StatePrRingVisValue", typeof(bool), typeof(StatusBarStateModel), new PropertyMetadata(default(bool)));

        /// <summary>
        /// 信息状态栏进度环激活值
        /// </summary>
        public bool StatePrRingActValue
        {
            get { return (bool)GetValue(StatePrRingActValueProperty); }
            set { SetValue(StatePrRingActValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatePrRingActValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatePrRingActValueProperty =
            DependencyProperty.Register("StatePrRingActValue", typeof(bool), typeof(StatusBarStateModel), new PropertyMetadata(default(bool)));
    }
}
