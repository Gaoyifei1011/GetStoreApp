using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    /// <summary>
    /// 设置页面：下载管理设置用户控件视图
    /// </summary>
    public sealed partial class DownloadOptionsControl : UserControl
    {
        public DownloadOptionsControl()
        {
            InitializeComponent();
        }

        public bool IsDownloadItemChecked(int selectedIndex, int index)
        {
            return selectedIndex == index;
        }

        public bool IsDownloadModeChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }
    }
}
