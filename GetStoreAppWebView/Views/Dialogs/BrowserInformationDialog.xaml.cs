using Microsoft.UI.Xaml.Controls;

namespace GetStoreAppWebView.Views.Dialogs
{
    /// <summary>
    /// 浏览器内核信息对话框
    /// </summary>
    internal sealed partial class BrowserInformationDialog : ContentDialog
    {
        #region 第一部分：属性、集合与事件

        private string BrowserRuntimeVersion { get; set; }

        #endregion 第一部分：属性、集合与事件

        #region 第二部分：构造函数

        internal BrowserInformationDialog(string browserRuntimeVersion)
        {
            InitializeComponent();
            BrowserRuntimeVersion = browserRuntimeVersion;
        }

        #endregion 第二部分：构造函数
    }
}
