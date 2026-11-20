using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation.Diagnostics;
using Windows.Web.Http;

namespace GetStoreApp.Helpers.Store
{
    /// <summary>
    /// 搜索应用辅助类
    /// </summary>
    internal static class SearchAppsHelper
    {
        private static readonly string storeLink = "https://apps.microsoft.com/store/detail/{0}";
        private static readonly Uri manifestSearchUri = new("https://storeedgefd.dsx.mp.microsoft.com/v9.0/manifestSearch");
        private static readonly Uri storeExactSearchUri = new("https://storeedgefd.dsx.mp.microsoft.com/v9.0/pages/searchResults");

        /// <summary>
        /// 生成清单搜索应用的所需的字符串
        /// </summary>
        internal static string GenerateManifestSearchString(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return default;
            }

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
                    ["MaximumResults"] = JsonValue.CreateNumberValue(1000),
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
        /// 使用商店精准搜索应用接口搜索应用
        /// </summary>
        internal static async Task<(bool, ReadOnlyCollection<SearchAppsResultModel>)> StoreExactSearchAsync(string content)
        {
            bool requestResult = false;
            List<SearchAppsResultModel> searchAppsResultList = [];

            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    Uri storeConcatExactSearchUri = new(string.Format("{0}?market={1}&locale={2}&deviceFamily=windows.desktop&query={3}", storeExactSearchUri, StoreRegionService.StoreRegion.CodeTwoLetter, LanguageService.AppLanguage.Key, content));

                    // 默认超时时间是 20 秒
                    HttpClient httpClient = new();
                    HttpRequestResult httpRequestResult = await httpClient.TryGetAsync(storeConcatExactSearchUri);
                    httpClient.Dispose();

                    // 请求成功
                    if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                    {
                        requestResult = true;
                        Dictionary<string, string> responseDict = new()
                        {
                            { "Status code", Convert.ToString(httpRequestResult.ResponseMessage.StatusCode) },
                            { "Headers", httpRequestResult.ResponseMessage.Headers is null ? string.Empty : Convert.ToString(httpRequestResult.ResponseMessage.Headers).Replace('\r', ' ').Replace('\n', ' ') },
                            { "Response message:", httpRequestResult.ResponseMessage.RequestMessage is null ? string.Empty : Convert.ToString(httpRequestResult.ResponseMessage.RequestMessage).Replace('\r', ' ').Replace('\n', ' ') }
                        };

                        LogService.WriteLog(LoggingLevel.Information, nameof(GetStoreApp), nameof(SearchAppsHelper), nameof(StoreExactSearchAsync), 1, responseDict);
                        string responseString = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

                        if (JsonArray.TryParse(responseString, out JsonArray responseStringArray) &&
                            responseStringArray.Count is 2)
                        {
                            JsonObject jsonObject = responseStringArray.GetObjectAt(1);
                            if (jsonObject.TryGetValue("Payload", out IJsonValue payloadJsonValue) &&
                                payloadJsonValue.ValueType is JsonValueType.Object)
                            {
                                JsonObject payloadObject = payloadJsonValue.GetObject();
                                if (payloadObject.TryGetValue("$type", out IJsonValue typeJsonValue) &&
                                    string.Equals(typeJsonValue.GetString(), "Microsoft.Marketplace.Storefront.Contracts.V9.SearchResponse, Microsoft.Marketplace.Storefront.Contracts", StringComparison.OrdinalIgnoreCase) &&
                                    payloadObject.TryGetValue("SearchResults", out IJsonValue searchResultsJsonValue) &&
                                    searchResultsJsonValue.ValueType is JsonValueType.Array)
                                {
                                    JsonArray searchResultsArray = searchResultsJsonValue.GetArray();
                                    foreach (IJsonValue serarchResults in searchResultsArray)
                                    {
                                        JsonObject searchResultsObject = serarchResults.GetObject();
                                        if (searchResultsObject.TryGetValue("$type", out typeJsonValue) &&
                                            typeJsonValue.ValueType is JsonValueType.String &&
                                            string.Equals(typeJsonValue.GetString(), "Microsoft.Marketplace.Storefront.Contracts.V8.One.CardModel, Microsoft.Marketplace.Storefront.Contracts", StringComparison.OrdinalIgnoreCase))
                                        {
                                            string productId = searchResultsObject.TryGetValue("ProductId", out IJsonValue productIdJsonValue) && productIdJsonValue.ValueType is JsonValueType.String ? productIdJsonValue.GetString() : string.Empty;
                                            if (Uri.TryCreate(string.Format(storeLink, productId), UriKind.Absolute, out Uri storeAppLink))
                                            {
                                                string title = searchResultsObject.TryGetValue("Title", out IJsonValue titleJsonValue) && titleJsonValue.ValueType is JsonValueType.String ? titleJsonValue.GetString() : string.Empty;
                                                string publisherName = searchResultsObject.TryGetValue("PublisherName", out IJsonValue publisherNameJsonValue) && publisherNameJsonValue.ValueType is JsonValueType.String ? publisherNameJsonValue.GetString() : string.Empty;

                                                searchAppsResultList.Add(new()
                                                {
                                                    StoreAppLink = storeAppLink,
                                                    StoreAppName = title,
                                                    StoreAppPublisher = publisherName
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // 请求失败
                    else
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SearchAppsHelper), nameof(StoreExactSearchAsync), 2, httpRequestResult.ExtendedError);
                    }

                    httpRequestResult.Dispose();
                }
                // 其他异常
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SearchAppsHelper), nameof(StoreExactSearchAsync), 3, e);
                }
            }

            return ValueTuple.Create(requestResult, searchAppsResultList.AsReadOnly());
        }

        /// <summary>
        /// 按照清单方式搜索应用
        /// </summary>
        internal static async Task<(bool, ReadOnlyCollection<SearchAppsResultModel>)> ManifestSearchAsync(string generatedContent)
        {
            bool requestResult = false;
            List<SearchAppsResultModel> searchAppsResultList = [];

            if (!string.IsNullOrEmpty(generatedContent))
            {
                try
                {
                    HttpStringContent httpStringContent = new(generatedContent);
                    httpStringContent.TryComputeLength(out ulong length);
                    httpStringContent.Headers.Expires = DateTimeOffset.Now;
                    httpStringContent.Headers.ContentType = new("text/json");
                    httpStringContent.Headers.ContentLength = length;
                    httpStringContent.Headers.ContentType.CharSet = "utf-8";

                    // 默认超时时间是 20 秒
                    HttpClient httpClient = new();
                    HttpRequestResult httpRequestResult = await httpClient.TryPostAsync(manifestSearchUri, httpStringContent);
                    httpClient.Dispose();

                    // 请求成功
                    if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                    {
                        requestResult = true;
                        Dictionary<string, string> responseDict = new()
                        {
                            { "Status code", Convert.ToString(httpRequestResult.ResponseMessage.StatusCode) },
                            { "Headers", httpRequestResult.ResponseMessage.Headers is null ? string.Empty : Convert.ToString(httpRequestResult.ResponseMessage.Headers).Replace('\r', ' ').Replace('\n', ' ') },
                            { "Response message:", httpRequestResult.ResponseMessage.RequestMessage is null ? string.Empty : Convert.ToString(httpRequestResult.ResponseMessage.RequestMessage).Replace('\r', ' ').Replace('\n', ' ') }
                        };

                        LogService.WriteLog(LoggingLevel.Information, nameof(GetStoreApp), nameof(SearchAppsHelper), nameof(ManifestSearchAsync), 1, responseDict);
                        string responseString = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

                        if (JsonObject.TryParse(responseString, out JsonObject responseStringObject) &&
                            responseStringObject.TryGetValue("Data", out IJsonValue dataJsonValue) &&
                            dataJsonValue.ValueType is JsonValueType.Array)
                        {
                            JsonArray dataArray = dataJsonValue.GetArray();
                            foreach (IJsonValue jsonValue in dataArray)
                            {
                                JsonObject jsonObject = jsonValue.GetObject();
                                if (jsonObject.TryGetValue("PackageIdentifier", out IJsonValue packageIdentifierJsonValue) &&
                                    packageIdentifierJsonValue.ValueType is JsonValueType.String &&
                                    Uri.TryCreate(string.Format(storeLink, packageIdentifierJsonValue.GetString()), UriKind.Absolute, out Uri storeAppLink))
                                {
                                    searchAppsResultList.Add(new()
                                    {
                                        StoreAppLink = storeAppLink,
                                        StoreAppName = jsonObject.TryGetValue("PackageName", out IJsonValue packageNameJsonValue) && packageNameJsonValue.ValueType is JsonValueType.String ? packageNameJsonValue.GetString() : string.Empty,
                                        StoreAppPublisher = jsonObject.TryGetValue("Publisher", out IJsonValue publisherJsonValue) && publisherJsonValue.ValueType is JsonValueType.String ? publisherJsonValue.GetString() : string.Empty
                                    });
                                }
                            }
                        }
                    }
                    // 请求失败
                    else
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SearchAppsHelper), nameof(ManifestSearchAsync), 2, httpRequestResult.ExtendedError);
                    }

                    httpRequestResult.Dispose();
                }
                // 其他异常
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SearchAppsHelper), nameof(ManifestSearchAsync), 3, e);
                }
            }

            return ValueTuple.Create(requestResult, searchAppsResultList.AsReadOnly());
        }
    }
}
