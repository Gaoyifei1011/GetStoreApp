using System;

namespace GetStoreAppWebView.UI.Controls
{
    /// <summary>
    /// 为 CoreWebView2Initialized 事件提供数据
    /// </summary>
    public class CoreWebView2InitializedEventArgs : EventArgs
    {
        /// <summary>
        /// 获取创建 WebView2 时引发的异常。
        /// </summary>
        public Exception Exception { get; }

        public CoreWebView2InitializedEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
