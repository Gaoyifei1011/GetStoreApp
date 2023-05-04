using GetStoreApp.Models.Controls.Store;
using System;

namespace GetStoreApp.Helpers.Controls.Store
{
    /// <summary>
    /// 网页请求状态解析辅助类
    /// </summary>
    public static class HtmlRequestStateHelper
    {
        /// <summary>
        /// 检查请求后的状态信息
        /// </summary>
        public static int CheckRequestState(RequestModel HttpRequestData)
        {
            // 服务器请求异常，返回状态值3
            if (HttpRequestData.RequestId is not 0)
            {
                return 3;
            }

            // 服务器请求成功
            else
            {
                // 成功下返回值为成功，否则返回警告
                if (HttpRequestData.RequestContent.Contains("The links were successfully received from the Microsoft Store server.", StringComparison.Ordinal))
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }
    }
}
