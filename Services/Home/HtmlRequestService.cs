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
        /// <summary>
        /// 请求数据时需要使用的API链接
        /// Api links that need to be used when requesting data
        /// </summary>
        private const string API = "https://store.rg-adguard.net/api/GetFiles";

        /// <summary>
        /// 数据请求信息
        /// Data request information
        /// </summary>
        private HttpRequestDataModel httpRequestDataModel;

        /// <summary>
        /// 数据的请求状态，0是正常状态，1是网络异常（WebExpection），2是超时异常（TimeOutExpection），3是其他异常（默认值）
        /// The request status of the data, 0 is the normal state, 1 is the network exception (WebExpection), 2 is the timeout exception (TimeOutExpection), 3 is the other exception (default)
        /// </summary>
        private int RequestId = 3;

        /// <summary>
        /// 网页状态请求码
        /// The page status request code
        /// </summary>
        private string RequestStatusCode = string.Empty;

        /// <summary>
        /// 网页正常请求时返回的内容
        /// The content that is returned when the page is normally requested
        /// </summary>
        private string RequestContent = string.Empty;

        /// <summary>
        /// 网页异常请求时返回的内容
        /// The content returned when a web page is requested unexpectedly
        /// </summary>
        private string RequestExpectionContent = string.Empty;

        /// <summary>
        /// 生成要请求的content内容
        /// Generate the content to be requested
        /// </summary>
        /// <param name="type"></param>
        /// <param name="url"></param>
        /// <param name="ring"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GenerateContent(string type, string url, string ring, string language)
        {
            return string.Format("type={0}&url={1}&ring={2}&lang={3}", type, url, ring, language);
        }

        public async Task<HttpRequestDataModel> HttpRequestAsync(string content)
        {
            // 将语句执行块放在try中，可以捕捉到异常
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
                var response = await request.GetResponseAsync() as HttpWebResponse;

                // 获取数据响应流
                var stream = response.GetResponseStream();

                if (stream != null)
                {
                    // 获取网页响应的数据
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
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
                    var httpWebResponse = (HttpWebResponse)response;

                    // 获取数据响应流
                    var stream = httpWebResponse.GetResponseStream();

                    if (stream != null)
                    {
                        // 获取数据响应回的内容
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
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
            // 捕捉访问网络期间超时引发的异常
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
            // 添加数据
            finally
            {
                httpRequestDataModel = new HttpRequestDataModel(RequestId, RequestStatusCode, RequestContent, RequestExpectionContent);
            }
            return httpRequestDataModel;
        }
    }
}