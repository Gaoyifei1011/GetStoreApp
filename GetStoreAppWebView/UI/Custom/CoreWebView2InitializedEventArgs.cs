using System;

namespace GetStoreAppWebView.UI.Custom
{
    /// <summary>
    /// 为 CoreWebView2Initialized 事件提供数据
    /// </summary>
    public class CoreWebView2InitializedEventArgs : EventArgs
    {
        /// <summary>
        /// 获取创建 WebView2 时引发的异常。
        /// </summary>
        private readonly Exception _exception;

        public Exception Exception
        {
            get { return _exception; }
        }

        public CoreWebView2InitializedEventArgs(Exception exception)
        {
            _exception = exception;
        }
    }
}
