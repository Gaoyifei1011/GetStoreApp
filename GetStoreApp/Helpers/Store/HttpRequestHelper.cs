using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace GetStoreApp.Helpers.Store
{
    /// <summary>
    /// 网页请求辅助类
    /// </summary>
    internal static partial class HtmlRequestHelper
    {
        private static readonly string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/149.0.0.0 Safari/537.36 Edg/149.0.0.0";
        private static readonly Uri apiUri = new("https://store.rg-adguard.net/api/GetFiles");

        [GeneratedRegex("[\r\n]")]
        private static partial Regex WhiteSpaceRegex { get; }

        /// <summary>
        /// 生成要请求的内容
        /// </summary>
        internal static async Task<string> GenerateRequestContentAsync(string type, string url, string ring)
        {
            HttpFormUrlEncodedContent httpFormUrlEncodedContent = new((ReadOnlyCollection<KeyValuePair<string, string>>)
            [
                new("type", type),
                new("url", url),
                new("ring", ring)
            ]);
            return await httpFormUrlEncodedContent.ReadAsStringAsync();
        }

        /// <summary>
        /// 发送网页请求并获取结果
        /// </summary>
        internal static async Task<HttpRequestData> HttpRequestAsync(string content)
        {
            HttpRequestData httpRequestData = new();

            try
            {
                HttpStringContent httpStringContent = new(content, UnicodeEncoding.Utf8);
                httpStringContent.TryComputeLength(out ulong length);
                httpStringContent.Headers.Expires = DateTimeOffset.Now;
                httpStringContent.Headers.ContentType = new("application/x-www-form-urlencoded");
                httpStringContent.Headers.ContentLength = length;
                httpStringContent.Headers.ContentType.CharSet = "utf-8";

                // 默认超时时间是 20 秒
                HttpClient httpClient = new();
                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgent);
                httpClient.DefaultRequestHeaders.Referer = apiUri;
                httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", apiUri.GetLeftPart(UriPartial.Authority));

                HttpRequestResult httpRequestResult = await httpClient.TryPostAsync(apiUri, httpStringContent);

                httpClient.Dispose();

                // 请求成功
                if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                {
                    httpRequestData.RequestId = 0;
                    httpRequestData.RequestStatusCode = Convert.ToString(httpRequestResult.ResponseMessage.StatusCode);
                    httpRequestData.RequestContent = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

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
                    httpRequestData.RequestId = 1;
                    httpRequestData.RequestStatusCode = string.Empty;
                    httpRequestData.RequestExceptionContent = httpRequestResult.ExtendedError.Message;
                    httpRequestData.RequestContent = string.Empty;
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(HtmlRequestHelper), nameof(HttpRequestAsync), 2, httpRequestResult.ExtendedError);
                }

                httpRequestResult.Dispose();
            }
            // 其他异常
            catch (Exception e)
            {
                httpRequestData.RequestId = 2;
                httpRequestData.RequestStatusCode = string.Empty;
                httpRequestData.RequestExceptionContent = e.Message;
                httpRequestData.RequestContent = string.Empty;
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(HtmlRequestHelper), nameof(HttpRequestAsync), 3, e);
            }

            return httpRequestData;
        }

        /// <summary>
        /// 获取请求后的状态信息
        /// </summary>
        internal static InfoBarSeverity GetRequestStateSeverity(HttpRequestData httpRequestData)
        {
            if (httpRequestData is null)
            {
                return default;
            }

            // 服务器请求异常，返回错误状态值，成功下返回成功状态值，否则返回警告状态值
            return httpRequestData.RequestId is not 0 ? InfoBarSeverity.Error : httpRequestData.RequestContent.Contains("The links were successfully received from the Microsoft Store server.", StringComparison.OrdinalIgnoreCase) ? InfoBarSeverity.Success : InfoBarSeverity.Warning;
        }
    }
}
