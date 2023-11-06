using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation.Diagnostics;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace GetStoreApp.Helpers.Controls.Store
{
    /// <summary>
    /// 搜索应用辅助类
    /// </summary>
    public static class SearchStoreHelper
    {
        private static string storeUri = "https://www.microsoft.com/store/productId/{0}";

        private static Uri ManifestSearchUri = new Uri("https://storeedgefd.dsx.mp.microsoft.com/v9.0/manifestSearch");

        /// <summary>
        /// 生成搜索应用的所需的字符串
        /// </summary>
        public static string GenerateSearchString(string content)
        {
            try
            {
                JsonObject queryObject = new JsonObject();
                queryObject["Keyword"] = JsonValue.CreateStringValue(content);
                queryObject["MatchType"] = JsonValue.CreateStringValue("Substring");

                JsonObject jsonObject = new JsonObject();
                jsonObject["Query"] = queryObject;

                return jsonObject.Stringify();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static async Task<Tuple<bool, List<SearchStoreModel>>> SerachStoreAppsAsync(string generatedContent)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            bool requestResult = false;
            List<SearchStoreModel> searchStoreList = new List<SearchStoreModel>();

            try
            {
                byte[] contentBytes = Encoding.UTF8.GetBytes(generatedContent);

                HttpStringContent httpStringContent = new HttpStringContent(generatedContent);
                httpStringContent.Headers.Expires = DateTime.Now;
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("text/json");
                httpStringContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                httpStringContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.PostAsync(ManifestSearchUri, httpStringContent).AsTask(cancellationTokenSource.Token);

                if (responseMessage.IsSuccessStatusCode)
                {
                    requestResult = true;
                    StringBuilder responseBuilder = new StringBuilder();

                    responseBuilder.Append("Status Code:");
                    responseBuilder.AppendLine(responseMessage.StatusCode.ToString());
                    responseBuilder.Append("Headers:");
                    responseBuilder.AppendLine(responseMessage.Headers is null ? "" : responseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' '));
                    responseBuilder.Append("ResponseMessage:");
                    responseBuilder.AppendLine(responseMessage.RequestMessage is null ? "" : responseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' '));

                    LogService.WriteLog(LoggingLevel.Information, "Search store apps request successfully.", responseBuilder);

                    string responseString = await responseMessage.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                    responseMessage.Dispose();

                    if (JsonObject.TryParse(responseString, out JsonObject responseStringObject))
                    {
                        JsonArray dataArray = responseStringObject.GetNamedArray("Data");
                        foreach (IJsonValue jsonValue in dataArray)
                        {
                            JsonObject jobject = jsonValue.GetObject();

                            searchStoreList.Add(new SearchStoreModel()
                            {
                                StoreAppLink = string.Format(storeUri, jobject.GetNamedString("PackageIdentifier")),
                                StoreAppName = jobject.GetNamedString("PackageName"),
                                StoreAppPublisher = jobject.GetNamedString("Publisher")
                            });
                        }
                    }
                }
                else
                {
                    httpClient.Dispose();
                    responseMessage.Dispose();
                }
            }
            // 捕捉因为网络失去链接获取信息时引发的异常
            catch (COMException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "Search store apps request failed", e);
            }
            // 捕捉因访问超时引发的异常
            catch (TaskCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "Search store apps request timeout", e);
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Search store apps request unknown exception", e);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            return new Tuple<bool, List<SearchStoreModel>>(requestResult, searchStoreList);
        }
    }
}
