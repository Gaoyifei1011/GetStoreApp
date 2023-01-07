using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    public sealed partial class HistoryRecordControl : UserControl
    {
        public HistoryRecordControl()
        {
            InitializeComponent();
        }

        public bool IsHistoryLiteItemChecked(int selectedIHistoryLiteNumValue, int historyLiteNumValue)
        {
            return selectedIHistoryLiteNumValue == historyLiteNumValue;
        }

        public bool IsHistoryJumpListItemChecked(string selectedIHistoryJumpListNumValue, string historyJumpListNumValue)
        {
            return selectedIHistoryJumpListNumValue == historyJumpListNumValue;
        }
    }
}
