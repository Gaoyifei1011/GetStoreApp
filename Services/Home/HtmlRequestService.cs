using GetStoreApp.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Home
{
    public class HtmlRequestService
    {
        private const string API = "https://store.rg-adguard.net/api/GetFiles";

        private RequestModel HttpRequestResult;

        // 数据的请求状态，0是正常状态，1是网络异常（WebExpection），2是超时异常（TimeOutExpection），3是其他异常（默认值）
        private int RequestId = 3;

        private string RequestStatusCode = string.Empty;

        private string RequestContent = string.Empty;

        private string RequestExpectionContent = string.Empty;

        public async Task<RequestModel> HttpRequestAsync(string content)
        {
            try
            {
                byte[] ContentBytes = Encoding.UTF8.GetBytes(content);

                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.Expires = DateTime.Now;
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpContent.Headers.ContentLength = ContentBytes.Length;
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(API);
                httpClient.Timeout = new TimeSpan(0, 0, 30);

                HttpResponseMessage response = await httpClient.PostAsync(API, httpContent);

                // 请求成功
                if (response.IsSuccessStatusCode)
                {
                    RequestId = 0;
                    RequestStatusCode = string.Format("{0}", response.StatusCode);
                    RequestContent = await response.Content.ReadAsStringAsync();
                }

                // 请求失败
                else
                {
                    throw new HttpRequestException();
                }
            }

            // 捕捉访问网络期间发生错误时引发的异常
            catch (HttpRequestException httpRequestException)
            {
                RequestId = 1;
                RequestStatusCode = Convert.ToString(httpRequestException.StatusCode);
                RequestExpectionContent = httpRequestException.Message;
            }

            // 捕捉因访问超时引发的异常
            catch(TaskCanceledException taskCanceledExpection)
            {
                RequestId = 2;
                RequestExpectionContent = taskCanceledExpection.Message;
            }

            // 其他未知异常
            catch(Exception expection)
            {
                RequestExpectionContent = expection.Message;
            }

            finally
            {
                HttpRequestResult = new RequestModel
                {
                    RequestId = RequestId,
                    RequestStatusCode = RequestStatusCode,
                    RequestContent = RequestContent,
                    RequestExpectionContent = RequestExpectionContent
                };
            }

            return HttpRequestResult;
        }
    }
}
