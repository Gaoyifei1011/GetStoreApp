using Microsoft.Web.WebView2.Core;

namespace GetStoreAppWebView.Models
{
    public struct XamlFocusChangeInfoModel
    {
        public CoreWebView2MoveFocusReason storedMoveFocusReason;

        public bool isPending;
    }
}
