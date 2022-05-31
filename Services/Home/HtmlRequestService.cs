using GetStoreApp.Models;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Home
{
    public class HtmlRequestService
    {
        private const string API = "https://store.rg-adguard.net/api/GetFiles";

        private HttpRequestData httpRequestDataModel;

        // 数据的请求状态，0是正常状态，1是网络异常（WebExpection），2是超时异常（TimeOutExpection），3是其他异常（默认值）
        private int RequestId = 3;

        private string RequestStatusCode = string.Empty;

        private string RequestContent = string.Empty;

        private string RequestExpectionContent = string.Empty;

        public async Task<HttpRequestData> HttpRequestAsync(string content)
        {
            try
            {
                // method is depricated
                // 构建请求链接，请求方法（POST）和提交数据的方式
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                // 添加POST请求需要的内容
                byte[] data = Encoding.UTF8.GetBytes(content);
                request.ContentLength = data.Length;

                //HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri(API);
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                // 获取数据请求流
                var requestStream = await request.GetRequestStreamAsync();
                if (requestStream != null)
                {
                    requestStream.Write(data, 0, data.Length);
                }
                // 关闭流并释放连接以供重复使用
                requestStream.Close();

                // 请求获取数据
                HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

                // 获取数据响应流
                Stream stream = response.GetResponseStream();

                if (stream != null)
                {
                    // 获取网页响应的数据
                    StreamReader reader = new(stream, Encoding.UTF8);
                    string result = reader.ReadToEnd();

                    // 网页请求状态：正确状态
                    RequestId = 0;

                    // 网页请求状态码
                    RequestStatusCode = string.Format("{0}", response.StatusCode);

                    // 网页请求返回的内容
                    RequestContent = result;
                }
            }

            // 捕捉访问网络期间发生错误时引发的异常
            catch (WebException e)
            {
                var response = e.Response;
                if (response != null)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)response;

                    // 获取数据响应流
                    var stream = httpWebResponse.GetResponseStream();

                    if (stream != null)
                    {
                        // 获取数据响应回的内容
                        StreamReader reader = new(stream, Encoding.UTF8);
                        string result = reader.ReadToEnd();

                        // 网页请求状态：网络异常
                        RequestId = 1;

                        // 网页请求状态码
                        RequestStatusCode = string.Format("{0}", httpWebResponse.StatusCode);

                        // 网页请求返回的内容
                        RequestContent = result;

                        // 异常信息
                        if (e.Message != null)
                        {
                            RequestExpectionContent = e.Message;
                        }
                    }
                }
            }
            catch (TimeoutException e)
            {
                // 网页请求状态：超时异常
                RequestId = 2;

                // 异常信息
                if (e.Message != null)
                {
                    RequestExpectionContent = e.Message;
                }
            }
            catch (Exception e)
            {
                // 异常信息
                if (e.Message != null)
                {
                    RequestExpectionContent = e.Message;
                }
            }
            finally
            {
                httpRequestDataModel = new HttpRequestData()
                {
                    RequestId = RequestId,
                    RequestStatusCode = RequestStatusCode,
                    RequestContent = RequestContent,
                    RequestExpectionContent = RequestExpectionContent
                };
            }
            return httpRequestDataModel;
        }
    }
}
