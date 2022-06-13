using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Models
{
    public class StatusBarState : DependencyObject
    {
        public InfoBarSeverity InfoBarSeverity
        {
            get { return (InfoBarSeverity)GetValue(InfoBarSeverityProperty); }
            set { SetValue(InfoBarSeverityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InfoBarSeverity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InfoBarSeverityProperty =
            DependencyProperty.Register("InfoBarSeverity", typeof(InfoBarSeverity), typeof(StatusBarState), new PropertyMetadata(InfoBarSeverity.Informational));

        public string StateInfoText
        {
            get { return (string)GetValue(StateInfoTextProperty); }
            set { SetValue(StateInfoTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StateInfoText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateInfoTextProperty =
            DependencyProperty.Register("StateInfoText", typeof(string), typeof(StatusBarState), new PropertyMetadata(""));

        public bool StatePrRingVisValue
        {
            get { return (bool)GetValue(StatePrRingVisValueProperty); }
            set { SetValue(StatePrRingVisValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatePrRingVisValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatePrRingVisValueProperty =
            DependencyProperty.Register("StatePrRingVisValue", typeof(bool), typeof(StatusBarState), new PropertyMetadata(false));

        public bool StatePrRingActValue
        {
            get { return (bool)GetValue(StatePrRingActValueProperty); }
            set { SetValue(StatePrRingActValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatePrRingActValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatePrRingActValueProperty =
            DependencyProperty.Register("StatePrRingActValue", typeof(bool), typeof(StatusBarState), new PropertyMetadata(false));
    }
}
