using Microsoft.UI.Xaml.Controls;

namespace GetStoreAppWebView.Views.Dialogs
{
    /// <summary>
    /// 浏览器内核信息对话框
    /// </summary>
    public sealed partial class BrowserInformationDialog : ContentDialog
    {
        public string BrowserRuntimeVersion { get; set; }

        public BrowserInformationDialog(string browserRuntimeVersion)
        {
            InitializeComponent();
            BrowserRuntimeVersion = browserRuntimeVersion;
        }
    }
}
