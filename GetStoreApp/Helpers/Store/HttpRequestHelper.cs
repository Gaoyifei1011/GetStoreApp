using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace GetStoreApp.Helpers.Store
{
    /// <summary>
    /// 网页请求辅助类
    /// </summary>
    public static partial class HtmlRequestHelper
    {
        private static readonly Uri API = new("https://store.rg-adguard.net/api/GetFiles");

        [GeneratedRegex("[\r\n]")]
        private static partial Regex WhiteSpaceRegex { get; }

        /// <summary>
        /// 生成要请求的内容
        /// </summary>
        public static string GenerateRequestContent(string type, string url, string ring)
        {
            return string.Format("type={0}&url={1}&ring={2}", type, url, ring);
        }

        /// <summary>
        /// 发送网页请求并获取结果
        /// </summary>
        public static async Task<RequestModel> HttpRequestAsync(string content)
        {
            RequestModel request = new();

            try
            {
                HttpStringContent httpStringContent = new(content);
                httpStringContent.TryComputeLength(out ulong length);
                httpStringContent.Headers.Expires = DateTime.Now;
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpStringContent.Headers.ContentLength = length;
                httpStringContent.Headers.ContentType.CharSet = "utf-8";

                // 默认超时时间是 20 秒
                HttpClient httpClient = new();
                HttpRequestResult httpRequestResult = await httpClient.TryPostAsync(API, httpStringContent);
                httpClient.Dispose();

                // 请求成功
                if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                {
                    request.RequestId = 0;
                    request.RequestStatusCode = Convert.ToString(httpRequestResult.ResponseMessage.StatusCode);
                    request.RequestContent = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

                    Dictionary<string, string> responseDict = new()
                    {
                        { "Headers", httpRequestResult.ResponseMessage.Headers is null ? string.Empty : WhiteSpaceRegex.Replace(Convert.ToString(httpRequestResult.ResponseMessage.Headers), string.Empty) },
                        { "Response message:", httpRequestResult.ResponseMessage.RequestMessage is null ? string.Empty : WhiteSpaceRegex.Replace(Convert.ToString(httpRequestResult.ResponseMessage.RequestMessage), string.Empty) }
                    };

                    LogService.WriteLog(LoggingLevel.Information, nameof(GetStoreApp), nameof(HtmlRequestHelper), nameof(HttpRequestAsync), 1, responseDict);
                }
                // 请求失败
                else
                {
                    request.RequestId = 1;
                    request.RequestStatusCode = string.Empty;
                    request.RequestExceptionContent = httpRequestResult.ExtendedError.Message;
                    request.RequestContent = string.Empty;
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(HtmlRequestHelper), nameof(HttpRequestAsync), 2, httpRequestResult.ExtendedError);
                }

                httpRequestResult.Dispose();
            }
            // 其他异常
            catch (Exception e)
            {
                request.RequestId = 2;
                request.RequestStatusCode = string.Empty;
                request.RequestExceptionContent = e.Message;
                request.RequestContent = string.Empty;
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(HtmlRequestHelper), nameof(HttpRequestAsync), 3, e);
            }

            return request;
        }

        /// <summary>
        /// 检查请求后的状态信息
        /// </summary>
        public static InfoBarSeverity CheckRequestState(RequestModel HttpRequestData)
        {
            // 服务器请求异常，返回错误状态值，成功下返回成功状态值，否则返回警告状态值
            return HttpRequestData.RequestId is not 0 ? InfoBarSeverity.Error : HttpRequestData.RequestContent.Contains("The links were successfully received from the Microsoft Store server.", StringComparison.OrdinalIgnoreCase) ? InfoBarSeverity.Success : InfoBarSeverity.Warning;
        }
    }
}
