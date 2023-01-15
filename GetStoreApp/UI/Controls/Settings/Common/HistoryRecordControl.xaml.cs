using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    /// <summary>
    /// 设置页面：历史记录设置用户控件视图
    /// </summary>
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
