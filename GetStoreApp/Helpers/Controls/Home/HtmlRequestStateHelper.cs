using GetStoreApp.Models.Controls.Home;
using System;

namespace GetStoreApp.Helpers.Controls.Home
{
    /// <summary>
    /// 网页请求状态解析服务
    /// </summary>
    public static class HtmlRequestStateHelper
    {
        public static int CheckRequestState(RequestModel HttpRequestData)
        {
            // 服务器请求异常，返回状态值3
            if (HttpRequestData.RequestId != 0)
            {
                return 3;
            }

            // 服务器请求成功
            else
            {
                // 成功下返回值为成功，否则返回警告
                if (HttpRequestData.RequestContent.Contains("The links were successfully received from the Microsoft Store server.",StringComparison.Ordinal))
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
