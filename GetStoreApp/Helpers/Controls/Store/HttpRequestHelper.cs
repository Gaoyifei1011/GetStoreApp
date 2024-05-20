using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
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
        private static partial Regex WhiteSpace();

        private static readonly Regex WhiteSpaceRegex = WhiteSpace();

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
            RequestModel httpRequestResult = null;

            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                byte[] ContentBytes = Encoding.UTF8.GetBytes(content);

                HttpStringContent httpContent = new(content);
                httpContent.Headers.Expires = DateTime.Now;
                httpContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpContent.Headers.ContentLength = Convert.ToUInt64(ContentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(API), httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    RequestId = 0;
                    RequestStatusCode = string.Format("{0}", response.StatusCode);
                    RequestContent = await response.Content.ReadAsStringAsync();

                    StringBuilder responseSuccessBuilder = new();
                    responseSuccessBuilder.Append("Headers:");
                    responseSuccessBuilder.Append(response.Headers is null ? "" : WhiteSpaceRegex.Replace(response.Headers.ToString(), ""));
                    responseSuccessBuilder.Append('\n');
                    responseSuccessBuilder.Append("ResponseMessage:");
                    responseSuccessBuilder.Append(response.RequestMessage is null ? "" : WhiteSpaceRegex.Replace(response.RequestMessage.ToString(), ""));
                    responseSuccessBuilder.Append('\n');

                    LogService.WriteLog(LoggingLevel.Information, "Requested successfully.", responseSuccessBuilder);
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

            // 捕捉因为网络失去链接获取信息时引发的异常
            catch (COMException e)
            {
                RequestId = 1;
                RequestStatusCode = string.Empty;
                RequestExceptionContent = e.Message;
                RequestContent = string.Empty;
                LogService.WriteLog(LoggingLevel.Warning, "Network disconnected.", e);
            }

            // 捕捉因访问超时引发的异常
            catch (TaskCanceledException e)
            {
                RequestId = 2;
                RequestStatusCode = string.Empty;
                RequestExceptionContent = e.Message;
                RequestContent = string.Empty;
                LogService.WriteLog(LoggingLevel.Warning, "Network access timeout.", e);
            }

            // 其他异常
            catch (Exception e)
            {
                RequestId = 3;
                RequestStatusCode = string.Empty;
                RequestExceptionContent = e.Message;
                RequestContent = string.Empty;
                LogService.WriteLog(LoggingLevel.Warning, "Network state unknown.", e);
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
                cancellationTokenSource.Dispose();
            }

            return httpRequestResult;
        }

        /// <summary>
        /// 检查请求后的状态信息
        /// </summary>
        public static InfoBarSeverity CheckRequestState(RequestModel HttpRequestData)
        {
            // 服务器请求异常，返回状态值3
            if (HttpRequestData.RequestId is not 0)
            {
                return InfoBarSeverity.Error;
            }

            // 服务器请求成功
            else
            {
                // 成功下返回值为成功，否则返回警告
                return HttpRequestData.RequestContent.Contains("The links were successfully received from the Microsoft Store server.", StringComparison.OrdinalIgnoreCase) ? InfoBarSeverity.Success : InfoBarSeverity.Warning;
            }
        }
    }
}
