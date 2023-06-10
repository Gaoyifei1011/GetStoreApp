using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Store
{
    /// <summary>
    /// 微软商店页面：请求用户控件视图
    /// </summary>
    public sealed partial class RequestControl : Grid
    {
        public RequestControl()
        {
            InitializeComponent();
        }

        public bool IsTypeItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        public bool IsChannelItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }
    }
}
