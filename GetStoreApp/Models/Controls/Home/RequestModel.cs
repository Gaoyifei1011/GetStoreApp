using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Controls.Home
{
    public class RequestModel : ModelBase
    {
        /// <summary>
        /// 网页请求返回的ID值
        /// </summary>
        public int RequestId
        {
            get { return (int)GetValue(RequestIdProperty); }
            set { SetValue(RequestIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestIdProperty =
            DependencyProperty.Register("RequestId", typeof(int), typeof(RequestModel), new PropertyMetadata(default(int)));

        /// <summary>
        /// 网页请求返回的状态码
        /// </summary>
        public string RequestStatusCode
        {
            get { return (string)GetValue(RequestStatusCodeProperty); }
            set { SetValue(RequestStatusCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestStatusCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestStatusCodeProperty =
            DependencyProperty.Register("RequestStatusCode", typeof(string), typeof(RequestModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 正常网页请求返回的信息
        /// </summary>
        public string RequestContent
        {
            get { return (string)GetValue(RequestContentProperty); }
            set { SetValue(RequestContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestContentProperty =
            DependencyProperty.Register("RequestContent", typeof(string), typeof(RequestModel), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 异常网页请求返回的信息
        /// </summary>
        public string RequestExceptionContent
        {
            get { return (string)GetValue(RequestExceptionContentProperty); }
            set { SetValue(RequestExceptionContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestExceptionContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestExceptionContentProperty =
            DependencyProperty.Register("RequestExceptionContent", typeof(string), typeof(RequestModel), new PropertyMetadata(string.Empty));
    }
}
