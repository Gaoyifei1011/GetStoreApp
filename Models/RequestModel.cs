using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 网页数据请求内容数据模型
    /// Web page data request content data model
    /// </summary>
    public class RequestModel : DependencyObject
    {
        public int RequestId
        {
            get { return (int)GetValue(RequestIdProperty); }
            set { SetValue(RequestIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestIdProperty =
            DependencyProperty.Register("RequestId", typeof(int), typeof(RequestModel), new PropertyMetadata(0));

        public string RequestStatusCode
        {
            get { return (string)GetValue(RequestStatusCodeProperty); }
            set { SetValue(RequestStatusCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestStatusCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestStatusCodeProperty =
            DependencyProperty.Register("RequestStatusCode", typeof(string), typeof(RequestModel), new PropertyMetadata(string.Empty));

        public string RequestContent
        {
            get { return (string)GetValue(RequestContentProperty); }
            set { SetValue(RequestContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestContentProperty =
            DependencyProperty.Register("RequestContent", typeof(string), typeof(RequestModel), new PropertyMetadata(string.Empty));

        public string RequestExpectionContent
        {
            get { return (string)GetValue(RequestExpectionContentProperty); }
            set { SetValue(RequestExpectionContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestExpectionContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestExpectionContentProperty =
            DependencyProperty.Register("RequestExpectionContent", typeof(string), typeof(RequestModel), new PropertyMetadata(string.Empty));
    }
}
