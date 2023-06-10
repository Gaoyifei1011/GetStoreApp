using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：应用安装方式设置用户控件视图
    /// </summary>
    public sealed partial class InstallModeControl : Grid
    {
        public InstallModeControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }
    }
}
