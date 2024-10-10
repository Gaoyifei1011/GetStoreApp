using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
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
        private static readonly string storeUri = "https://www.microsoft.com/store/productId/{0}";
        private static readonly Uri manifestSearchUri = new("https://storeedgefd.dsx.mp.microsoft.com/v9.0/manifestSearch");

        /// <summary>
        /// 生成搜索应用的所需的字符串
        /// </summary>
        public static string GenerateSearchString(string content)
        {
            try
            {
                JsonObject queryObject = new()
                {
                    ["Keyword"] = JsonValue.CreateStringValue(content),
                    ["MatchType"] = JsonValue.CreateStringValue("Substring")
                };

                JsonObject requestMatchObject = new()
                {
                    ["KeyWord"] = JsonValue.CreateStringValue(StoreRegionService.StoreRegion.CodeTwoLetter),
                    ["MatchType"] = JsonValue.CreateStringValue("CaseInsensitive")
                };

                JsonArray filtersArray =
                [
                    new JsonObject()
                    {
                        ["PackageMatchField"] = JsonValue.CreateStringValue("Market"),
                        ["RequestMatch"] = requestMatchObject
                    },
                ];

                JsonObject jsonObject = new()
                {
                    ["MaximunResults"] = JsonValue.CreateNumberValue(1000),
                    ["Filters"] = filtersArray,
                    ["Query"] = queryObject,
                };

                return jsonObject.Stringify();
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return string.Empty;
            }
        }

        /// <summary>
        /// 搜索商店应用
        /// </summary>
        public static async Task<Tuple<bool, List<SearchStoreModel>>> SerachStoreAppsAsync(string generatedContent)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            bool requestResult = false;
            List<SearchStoreModel> searchStoreList = [];

            try
            {
                byte[] contentBytes = Encoding.UTF8.GetBytes(generatedContent);

                HttpStringContent httpStringContent = new(generatedContent);
                httpStringContent.Headers.Expires = DateTime.Now;
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("text/json");
                httpStringContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                httpStringContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new();
                HttpResponseMessage responseMessage = await httpClient.PostAsync(manifestSearchUri, httpStringContent).AsTask(cancellationTokenSource.Token);

                if (responseMessage.IsSuccessStatusCode)
                {
                    requestResult = true;
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", responseMessage.StatusCode.ToString() },
                        { "Headers", responseMessage.Headers is null ? string.Empty : responseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", responseMessage.RequestMessage is null ? string.Empty : responseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "Search store apps request successfully.", responseDict);

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
