using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Root;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace GetStoreApp.Helpers.Controls.Store
{
    /// <summary>
    /// 网页请求辅助类
    /// </summary>
    public static class HtmlRequestHelper
    {
        private const string API = "https://store.rg-adguard.net/api/GetFiles";

        // 数据的请求状态，0是正常状态，1是网络异常（WebException），2是超时异常（TimeOutException），3是其他异常（默认值）
        private static int RequestId;

        private static string RequestStatusCode;

        private static string RequestContent;

        private static string RequestExceptionContent;

        /// <summary>
        /// 发送网页请求并获取结果
        /// </summary>
        public static async Task<RequestModel> HttpRequestAsync(string content)
        {
            RequestModel HttpRequestResult = null;

            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                byte[] ContentBytes = Encoding.UTF8.GetBytes(content);

                HttpStringContent httpContent = new HttpStringContent(content);
                httpContent.Headers.Expires = DateTime.Now;
                httpContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpContent.Headers.ContentLength = Convert.ToUInt64(ContentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(API), httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    RequestId = 0;
                    RequestStatusCode = string.Format("{0}", response.StatusCode);
                    RequestContent = await response.Content.ReadAsStringAsync();

                    StringBuilder responseSuccessBuilder = new StringBuilder();
                    responseSuccessBuilder.Append("Headers:");
                    responseSuccessBuilder.Append(response.Headers is null ? "" : LogService.WhiteSpaceRegex.Replace(response.Headers.ToString(), ""));
                    responseSuccessBuilder.Append('\n');
                    responseSuccessBuilder.Append("ResponseMessage:");
                    responseSuccessBuilder.Append(response.RequestMessage is null ? "" : LogService.WhiteSpaceRegex.Replace(response.RequestMessage.ToString(), ""));
                    responseSuccessBuilder.Append('\n');

                    LogService.WriteLog(LogType.INFO, "Requested successfully.", responseSuccessBuilder);
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
                LogService.WriteLog(LogType.WARNING, "Network disconnected.", e);
            }

            // 捕捉因访问超时引发的异常
            catch (TaskCanceledException e)
            {
                RequestId = 2;
                RequestStatusCode = string.Empty;
                RequestExceptionContent = e.Message;
                RequestContent = string.Empty;
                LogService.WriteLog(LogType.WARNING, "Network access timeout.", e);
            }

            // 其他异常
            catch (Exception e)
            {
                RequestId = 3;
                RequestStatusCode = string.Empty;
                RequestExceptionContent = e.Message;
                RequestContent = string.Empty;
                LogService.WriteLog(LogType.WARNING, "Network state unknown.", e);
            }
            finally
            {
                HttpRequestResult = new RequestModel
                {
                    RequestId = RequestId,
                    RequestStatusCode = RequestStatusCode,
                    RequestContent = RequestContent,
                    RequestExceptionContent = RequestExceptionContent
                };
                cancellationTokenSource.Dispose();
            }

            return HttpRequestResult;
        }
    }
}
