using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace GetStoreApp.Helpers.Controls.Store
{
    /// <summary>
    /// 网页请求辅助类
    /// </summary>
    public static partial class HtmlRequestHelper
    {
        private const string API = "https://store.rg-adguard.net/api/GetFiles";

        // 数据的请求状态，0是正常状态，1是网络异常（WebException），2是超时异常（TimeOutException），3是其他异常（默认值）
        private static int RequestId;

        private static string RequestStatusCode;

        private static string RequestContent;

        private static string RequestExceptionContent;

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
        public static RequestModel HttpRequest(string content)
        {
            RequestModel httpRequestResult = null;

            try
            {
                AutoResetEvent autoResetEvent = new(false);

                IBuffer contentBuffer = CryptographicBuffer.ConvertStringToBinary(content, BinaryStringEncoding.Utf8);

                HttpStringContent httpContent = new(content);
                httpContent.Headers.Expires = DateTime.Now;
                httpContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpContent.Headers.ContentLength = Convert.ToUInt64(contentBuffer.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new();
                IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> httpPostProgress = httpClient.PostAsync(new Uri(API), httpContent);

                // 添加超时设置（半分钟后停止获取）
                ThreadPoolTimer threadPoolTimer = ThreadPoolTimer.CreateTimer((_) => { }, TimeSpan.FromSeconds(30), (_) =>
                {
                    try
                    {
                        if (httpPostProgress is not null && (httpPostProgress.Status is not AsyncStatus.Canceled || httpPostProgress.Status is not AsyncStatus.Completed || httpPostProgress.Status is not AsyncStatus.Error))
                        {
                            httpPostProgress.Cancel();
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Http request task cancel failed", e);
                    }
                });

                // HTTP POST 请求过程已完成
                httpPostProgress.Completed += async (result, status) =>
                {
                    httpPostProgress = null;
                    try
                    {
                        // 获取 POST 请求已完成
                        if (status is AsyncStatus.Completed)
                        {
                            HttpResponseMessage responseMessage = result.GetResults();

                            // 请求成功
                            if (responseMessage.IsSuccessStatusCode)
                            {
                                RequestId = 0;
                                RequestStatusCode = responseMessage.StatusCode.ToString();
                                RequestContent = await responseMessage.Content.ReadAsStringAsync();

                                Dictionary<string, string> responseDict = new()
                                {
                                    { "Headers", responseMessage.Headers is null ? string.Empty : WhiteSpaceRegex.Replace(responseMessage.Headers.ToString(), string.Empty) },
                                    { "Response message:", responseMessage.RequestMessage is null ? string.Empty : WhiteSpaceRegex.Replace(responseMessage.RequestMessage.ToString(), string.Empty) }
                                };

                                LogService.WriteLog(LoggingLevel.Information, "Http request requested successfully.", responseDict);
                            }
                            // 请求失败
                            else
                            {
                                RequestId = 3;
                                RequestStatusCode = string.Empty;
                                RequestExceptionContent = string.Empty;
                                RequestContent = string.Empty;
                            }
                        }
                        // 获取 POST 请求由于超时而被用户取消
                        else if (status is AsyncStatus.Canceled)
                        {
                            RequestId = 2;
                            RequestStatusCode = string.Empty;
                            RequestExceptionContent = result.ErrorCode.Message;
                            RequestContent = string.Empty;
                            LogService.WriteLog(LoggingLevel.Warning, "Http request timeout.", result.ErrorCode);
                        }
                        // 获取 POST 请求发生错误
                        else if (status is AsyncStatus.Error)
                        {
                            // 捕捉因为网络失去链接获取信息时引发的异常和可能存在的其他异常
                            RequestId = 1;
                            RequestStatusCode = string.Empty;
                            RequestExceptionContent = result.ErrorCode.Message;
                            RequestContent = string.Empty;
                            LogService.WriteLog(LoggingLevel.Warning, "Http request failed.", result.ErrorCode);
                        }
                    }
                    catch (Exception)
                    {
                        RequestId = 3;
                        RequestStatusCode = string.Empty;
                        RequestExceptionContent = result.ErrorCode.Message;
                        RequestContent = string.Empty;
                        LogService.WriteLog(LoggingLevel.Warning, "Http request unknown exception", result.ErrorCode);
                    }
                    finally
                    {
                        result.Close();
                        autoResetEvent.Set();
                    }
                };

                autoResetEvent.WaitOne();
                autoResetEvent.Dispose();
                autoResetEvent = null;
            }
            // 其他异常
            catch (Exception e)
            {
                RequestId = 3;
                RequestStatusCode = string.Empty;
                RequestExceptionContent = e.Message;
                RequestContent = string.Empty;
                LogService.WriteLog(LoggingLevel.Warning, "Http request unknown exception", e);
            }
            finally
            {
                httpRequestResult = new RequestModel
                {
                    RequestId = RequestId,
                    RequestStatusCode = RequestStatusCode,
                    RequestContent = RequestContent,
                    RequestExceptionContent = RequestExceptionContent
                };
            }

            return httpRequestResult;
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
