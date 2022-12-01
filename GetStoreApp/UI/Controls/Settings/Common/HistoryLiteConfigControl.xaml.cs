using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    public sealed partial class HistoryLiteConfigControl : UserControl
    {
        public HistoryLiteConfigControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(int selectedIHistoryLiteNumValue, int historyLiteNumValue)
        {
            return selectedIHistoryLiteNumValue == historyLiteNumValue;
        }
    }
}
