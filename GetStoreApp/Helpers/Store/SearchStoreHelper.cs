﻿using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation.Diagnostics;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace GetStoreApp.Helpers.Store
{
    /// <summary>
    /// 搜索应用辅助类
    /// </summary>
    public static class SearchStoreHelper
    {
        private static readonly string storeLink = "https://apps.microsoft.com/store/detail/{0}";
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
        public static async Task<(bool requestResult, List<SearchStoreModel> searchStoreList)> SerachStoreAppsAsync(string generatedContent)
        {
            bool requestResult = false;
            List<SearchStoreModel> searchStoreList = [];

            try
            {
                HttpStringContent httpStringContent = new(generatedContent);
                httpStringContent.TryComputeLength(out ulong length);
                httpStringContent.Headers.Expires = DateTime.Now;
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("text/json");
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

                    LogService.WriteLog(LoggingLevel.Information, nameof(GetStoreApp), nameof(SearchStoreHelper), nameof(SerachStoreAppsAsync), 1, responseDict);
                    string responseString = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

                    if (JsonObject.TryParse(responseString, out JsonObject responseStringObject))
                    {
                        JsonArray dataArray = responseStringObject.GetNamedArray("Data");
                        foreach (IJsonValue jsonValue in dataArray)
                        {
                            JsonObject jobject = jsonValue.GetObject();

                            searchStoreList.Add(new SearchStoreModel()
                            {
                                StoreAppLink = string.Format(storeLink, jobject.GetNamedString("PackageIdentifier")),
                                StoreAppName = jobject.GetNamedString("PackageName"),
                                StoreAppPublisher = jobject.GetNamedString("Publisher")
                            });
                        }
                    }
                }
                // 请求失败
                else
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SearchStoreHelper), nameof(SerachStoreAppsAsync), 2, httpRequestResult.ExtendedError);
                }

                httpRequestResult.Dispose();
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SearchStoreHelper), nameof(SerachStoreAppsAsync), 3, e);
            }

            return ValueTuple.Create(requestResult, searchStoreList);
        }
    }
}
