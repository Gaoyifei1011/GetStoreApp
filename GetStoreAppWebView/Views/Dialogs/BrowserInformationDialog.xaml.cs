using Microsoft.UI.Xaml.Controls;

namespace GetStoreAppWebView.Views.Dialogs
{
    /// <summary>
    /// 浏览器内核信息对话框
    /// </summary>
    internal sealed partial class BrowserInformationDialog : ContentDialog
    {
        internal string BrowserRuntimeVersion { get; set; }

        internal BrowserInformationDialog(string browserRuntimeVersion)
        {
            InitializeComponent();
            BrowserRuntimeVersion = browserRuntimeVersion;
        }
    }
}
