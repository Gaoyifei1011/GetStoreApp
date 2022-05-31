using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Models
{
    public class StatusBarState
    {
        public InfoBarSeverity InfoBarSeverity { get; set; }

        public string StateInfoText { get; set; }

        public bool StatePrRingVisValue { get; set; }

        public bool StatePrRingActValue { get; set; }
    }
}
