using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.System.Threading;
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
        public static Tuple<bool, List<SearchStoreModel>> SerachStoreApps(string generatedContent)
        {
            bool requestResult = false;
            List<SearchStoreModel> searchStoreList = [];

            try
            {
                AutoResetEvent autoResetEvent = new(false);
                HttpStringContent httpStringContent = new(generatedContent);
                httpStringContent.TryComputeLength(out ulong length);
                httpStringContent.Headers.Expires = DateTime.Now;
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("text/json");
                httpStringContent.Headers.ContentLength = length;
                httpStringContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new();
                IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> httpPostProgress = httpClient.PostAsync(manifestSearchUri, httpStringContent);

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
                        LogService.WriteLog(LoggingLevel.Warning, "Search store apps cancel task failed", e);
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
                                requestResult = true;
                                Dictionary<string, string> responseDict = new()
                                {
                                    { "Status code", responseMessage.StatusCode.ToString() },
                                    { "Headers", responseMessage.Headers is null ? string.Empty : responseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                                    { "Response message:", responseMessage.RequestMessage is null ? string.Empty : responseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                                };

                                LogService.WriteLog(LoggingLevel.Information, "Search store apps request successfully.", responseDict);

                                string responseString = await responseMessage.Content.ReadAsStringAsync();

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

                            responseMessage.Dispose();
                        }
                        // 获取 POST 请求由于超时而被用户取消
                        else if (status is AsyncStatus.Canceled)
                        {
                            LogService.WriteLog(LoggingLevel.Information, "Search store apps request timeout", result.ErrorCode);
                        }
                        // 获取 POST 请求发生错误
                        else if (status is AsyncStatus.Error)
                        {
                            // 捕捉因为网络失去链接获取信息时引发的异常和可能存在的其他异常
                            LogService.WriteLog(LoggingLevel.Information, "Search store apps request failed", result.ErrorCode);
                        }
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Check update request completed, but an unknown exception has occured", e);
                    }
                    finally
                    {
                        httpClient.Dispose();
                        result.Close();
                        threadPoolTimer.Cancel();
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
                LogService.WriteLog(LoggingLevel.Warning, "Search store apps request unknown exception", e);
            }

            return new Tuple<bool, List<SearchStoreModel>>(requestResult, searchStoreList);
        }
    }
}
