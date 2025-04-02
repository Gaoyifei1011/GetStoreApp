using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Diagnostics;
using Windows.Security.Cryptography;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace GetStoreApp.Helpers.Controls.Store
{
    /// <summary>
    /// 查询链接辅助类
    /// </summary>
    public static class QueryLinksHelper
    {
        private static readonly Uri cookieUri = new("https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx");
        private static readonly Uri fileListXmlUri = new("https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx");
        private static readonly Uri urlUri = new("https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx/secured");

        /// <summary>
        /// 解析输入框输入的字符串
        /// </summary>
        public static string ParseRequestContent(string requestContent)
        {
            if (requestContent.Contains('/'))
            {
                requestContent = requestContent[(requestContent.LastIndexOf('/') + 1)..];
            }
            if (requestContent.Contains('?'))
            {
                requestContent = requestContent[..requestContent.IndexOf('?')];
            }
            return requestContent;
        }

        /// <summary>
        /// 获取微软商店服务器储存在用户本地终端上的数据
        /// </summary>
        public static async Task<string> GetCookieAsync()
        {
            string cookieResult = string.Empty;

            try
            {
                byte[] cookieByteArray = ResourceService.GetEmbeddedData("Files/Assets/Embed/cookie.xml");

                HttpStringContent httpStringContent = new(CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, CryptographicBuffer.CreateFromByteArray(cookieByteArray)));
                httpStringContent.TryComputeLength(out ulong length);
                httpStringContent.Headers.Expires = DateTime.Now;
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/soap+xml");
                httpStringContent.Headers.ContentLength = length;
                httpStringContent.Headers.ContentType.CharSet = "utf-8";

                // 默认超时时间是 20 秒
                HttpClient httpClient = new();
                HttpRequestResult httpRequestResult = await httpClient.TryPostAsync(cookieUri, httpStringContent);
                httpClient.Dispose();

                // 请求成功
                if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", httpRequestResult.ResponseMessage.StatusCode.ToString() },
                        { "Headers", httpRequestResult.ResponseMessage.Headers is null ? string.Empty : httpRequestResult.ResponseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", httpRequestResult.ResponseMessage.RequestMessage is null ? string.Empty : httpRequestResult.ResponseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "Cookie request successfully.", responseDict);

                    string responseString = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseString))
                    {
                        XmlDocument responseStringDocument = new();
                        responseStringDocument.LoadXml(responseString);

                        XmlNodeList encryptedDataList = responseStringDocument.GetElementsByTagName("EncryptedData");
                        if (encryptedDataList.Count > 0)
                        {
                            cookieResult = encryptedDataList[0].InnerText;
                        }
                    }
                }
                // 请求失败
                else
                {
                    LogService.WriteLog(LoggingLevel.Information, "Cookie request failed", httpRequestResult.ExtendedError);
                }

                httpRequestResult.Dispose();
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Cookie request unknown exception", e);
            }

            return cookieResult;
        }

        /// <summary>
        /// 获取应用信息
        /// </summary>
        /// <param name="productId">应用的产品 ID</param>
        /// <returns>打包应用：有，非打包应用：无</returns>
        public static async Task<Tuple<bool, AppInfoModel>> GetAppInformationAsync(string productId)
        {
            bool requestResult = false;
            AppInfoModel appInfoModel = new();

            try
            {
                string categoryIDAPI = string.Format("https://storeedgefd.dsx.mp.microsoft.com/v9.0/products/{0}?market={1}&locale={2}&deviceFamily=Windows.Desktop", productId, StoreRegionService.StoreRegion.CodeTwoLetter, LanguageService.AppLanguage.Key);

                // 默认超时时间是 20 秒
                HttpClient httpClient = new();
                HttpRequestResult httpRequestResult = await httpClient.TryGetAsync(new(categoryIDAPI));
                httpClient.Dispose();

                // 请求成功
                if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                {
                    requestResult = true;
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", httpRequestResult.ResponseMessage.StatusCode.ToString() },
                        { "Headers", httpRequestResult.ResponseMessage.Headers is null ? string.Empty : httpRequestResult.ResponseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", httpRequestResult.ResponseMessage.RequestMessage is null ? string.Empty : httpRequestResult.ResponseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "App Information request successfully.", responseDict);

                    string responseString = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

                    if (JsonObject.TryParse(responseString, out JsonObject responseStringObject))
                    {
                        JsonObject payLoadObject = responseStringObject.GetNamedValue("Payload").GetObject();

                        appInfoModel.Name = payLoadObject.GetNamedString("Title");
                        appInfoModel.Publisher = payLoadObject.GetNamedString("PublisherName");
                        appInfoModel.Description = payLoadObject.GetNamedString("Description");
                        appInfoModel.CategoryID = string.Empty;
                        appInfoModel.ProductID = productId;

                        JsonArray skusArray = payLoadObject.GetNamedArray("Skus");

                        if (skusArray.Count > 0)
                        {
                            appInfoModel.CategoryID = string.Empty;
                            JsonObject jsonObject = skusArray[0].GetObject();
                            if (jsonObject.TryGetValue("FulfillmentData", out IJsonValue jsonValue))
                            {
                                string fulfillmentData = jsonValue.GetString();
                                if (JsonObject.TryParse(fulfillmentData, out JsonObject fulfillmentDataObject))
                                {
                                    appInfoModel.CategoryID = fulfillmentDataObject.GetNamedString("WuCategoryId");
                                }
                            }
                        }
                    }
                }
                // 请求失败
                else
                {
                    LogService.WriteLog(LoggingLevel.Information, "App Information request failed", httpRequestResult.ExtendedError);
                }

                httpRequestResult.Dispose();
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "App Information request unknown exception", e);
            }

            return Tuple.Create(requestResult, appInfoModel);
        }

        /// <summary>
        /// 获取文件信息字符串
        /// </summary>
        /// <param name="cookie">cookie 数据</param>
        /// <param name="categoryId">category ID</param>
        /// <param name="ring">通道</param>
        /// <returns>文件信息的字符串</returns>
        public static async Task<string> GetFileListXmlAsync(string cookie, string categoryId, string ring)
        {
            string fileListXmlResult = string.Empty;

            try
            {
                byte[] wubyteArray = ResourceService.GetEmbeddedData("Files/Assets/Embed/wu.xml");
                string fileListXml = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, CryptographicBuffer.CreateFromByteArray(wubyteArray)).Replace("{1}", cookie).Replace("{2}", categoryId).Replace("{3}", ring);

                HttpStringContent httpStringContent = new(fileListXml);
                httpStringContent.TryComputeLength(out ulong length);
                httpStringContent.Headers.Expires = DateTime.Now;
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/soap+xml");
                httpStringContent.Headers.ContentLength = length;
                httpStringContent.Headers.ContentType.CharSet = "utf-8";

                // 默认超时时间是 20 秒
                HttpClient httpClient = new();
                HttpRequestResult httpRequestResult = await httpClient.TryPostAsync(fileListXmlUri, httpStringContent);
                httpClient.Dispose();

                // 请求成功
                if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", httpRequestResult.ResponseMessage.StatusCode.ToString() },
                        { "Headers", httpRequestResult.ResponseMessage.Headers is null ? string.Empty : httpRequestResult.ResponseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", httpRequestResult.ResponseMessage.RequestMessage is null ? string.Empty : httpRequestResult.ResponseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "FileListXml request successfully.", responseDict);

                    string responseString = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

                    fileListXmlResult = responseString.Replace("&lt;", "<").Replace("&gt;", ">");
                }
                // 请求失败
                else
                {
                    LogService.WriteLog(LoggingLevel.Information, "FileListXml request failed", httpRequestResult.ExtendedError);
                }

                httpRequestResult.Dispose();
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "FileListXml request unknown exception", e);
            }

            return fileListXmlResult;
        }

        /// <summary>
        /// 获取商店应用安装包
        /// </summary>
        /// <param name="fileListXml">文件信息的字符串</param>
        /// <param name="ring">通道</param>
        /// <returns>带解析后文件信息的列表</returns>
        public static async Task<List<QueryLinksModel>> GetAppxPackagesAsync(string fileListXml, string ring)
        {
            List<QueryLinksModel> appxPackagesList = [];

            try
            {
                XmlDocument fileListDocument = new();
                fileListDocument.LoadXml(fileListXml);

                Dictionary<string, Tuple<string, string, string>> appxPackagesInfoDict = [];
                XmlNodeList fileList = fileListDocument.GetElementsByTagName("File");

                foreach (IXmlNode fileNode in fileList)
                {
                    if (fileNode.Attributes.GetNamedItem("InstallerSpecificIdentifier") is not null)
                    {
                        string name = fileNode.Attributes.GetNamedItem("InstallerSpecificIdentifier").InnerText;
                        string extension = fileNode.Attributes.GetNamedItem("FileName").InnerText[fileNode.Attributes.GetNamedItem("FileName").InnerText.LastIndexOf('.')..];
                        string size = fileNode.Attributes.GetNamedItem("Size").InnerText;
                        string digest = fileNode.Attributes.GetNamedItem("Digest").InnerText;

                        if (!appxPackagesInfoDict.ContainsKey(name))
                        {
                            appxPackagesInfoDict.Add(name, Tuple.Create(extension, size, digest));
                        }
                    }
                }

                Lock appxPackagesLock = new();
                XmlNodeList securedFragmentList = fileListDocument.DocumentElement.GetElementsByTagName("SecuredFragment");
                List<Task> taskList = [];

                foreach (IXmlNode securedFragmentNode in securedFragmentList)
                {
                    IXmlNode securedFragmentCloneNode = securedFragmentNode;
                    taskList.Add(Task.Run(async () =>
                    {
                        IXmlNode xmlNode = securedFragmentCloneNode.ParentNode.ParentNode;

                        if (xmlNode.GetElementsByName("ApplicabilityRules").GetElementsByName("Metadata").GetElementsByName("AppxPackageMetadata").GetElementsByName("AppxMetadata").Attributes.GetNamedItem("PackageMoniker") is not null)
                        {
                            string name = xmlNode.GetElementsByName("ApplicabilityRules").GetElementsByName("Metadata").GetElementsByName("AppxPackageMetadata").GetElementsByName("AppxMetadata").Attributes.GetNamedItem("PackageMoniker").InnerText;

                            if (appxPackagesInfoDict.TryGetValue(name, out Tuple<string, string, string> value))
                            {
                                string fileName = name + value.Item1;
                                string fileSize = value.Item2;
                                string digest = value.Item3;
                                string revisionNumber = xmlNode.GetElementsByName("UpdateIdentity").Attributes.GetNamedItem("RevisionNumber").InnerText;
                                string updateID = xmlNode.GetElementsByName("UpdateIdentity").Attributes.GetNamedItem("UpdateID").InnerText;
                                string uri = await GetAppxUrlAsync(updateID, revisionNumber, ring, digest);
                                string fileSizeString = FileSizeHelper.ConvertFileSizeToString(double.TryParse(fileSize, out double size) ? size : 0);

                                appxPackagesLock.Enter();

                                try
                                {
                                    appxPackagesList.Add(new QueryLinksModel()
                                    {
                                        FileName = fileName,
                                        FileLink = uri,
                                        FileSize = fileSizeString,
                                        IsSelected = false,
                                        IsSelectMode = false
                                    });
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                                finally
                                {
                                    appxPackagesLock.Exit();
                                }
                            }
                        }
                    }));
                }

                await Task.WhenAll(taskList);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Information, "FileListXml parse failed", e);
            }

            return appxPackagesList;
        }

        /// <summary>
        /// 获取商店应用安装包对应的下载链接
        /// </summary>
        /// <returns>商店应用安装包对应的下载链接</returns>
        private static async Task<string> GetAppxUrlAsync(string updateID, string revisionNumber, string ring, string digest)
        {
            string urlResult = string.Empty;

            try
            {
                byte[] urlbyteArray = ResourceService.GetEmbeddedData("Files/Assets/Embed/url.xml");
                string url = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, CryptographicBuffer.CreateFromByteArray(ResourceService.GetEmbeddedData("Files/Assets/Embed/url.xml"))).Replace("{1}", updateID).Replace("{2}", revisionNumber).Replace("{3}", ring);

                HttpStringContent httpContent = new(url);
                httpContent.TryComputeLength(out ulong length);
                httpContent.Headers.Expires = DateTime.Now;
                httpContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/soap+xml");
                httpContent.Headers.ContentLength = length;
                httpContent.Headers.ContentType.CharSet = "utf-8";

                // 默认超时时间是 20 秒
                HttpClient httpClient = new();
                HttpRequestResult httpRequestResult = await httpClient.TryPostAsync(urlUri, httpContent);
                httpClient.Dispose();

                // 请求成功
                if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", httpRequestResult.ResponseMessage.StatusCode.ToString() },
                        { "Headers", httpRequestResult.ResponseMessage.Headers is null ? string.Empty : httpRequestResult.ResponseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", httpRequestResult.ResponseMessage.RequestMessage is null ? string.Empty : httpRequestResult.ResponseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "Appx Url request successfully.", responseDict);

                    string responseString = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseString))
                    {
                        XmlDocument responseStringDocument = new();
                        responseStringDocument.LoadXml(responseString);

                        XmlNodeList fileLocationList = responseStringDocument.DocumentElement.GetElementsByTagName("FileLocation");

                        foreach (IXmlNode fileLocationNode in fileLocationList)
                        {
                            if (Equals(fileLocationNode.GetElementsByName("FileDigest").InnerText, digest))
                            {
                                urlResult = fileLocationNode.GetElementsByName("Url").InnerText;
                                break;
                            }
                        }
                    }
                }
                // 请求失败
                else
                {
                    LogService.WriteLog(LoggingLevel.Information, "Appx Url request failed", httpRequestResult.ExtendedError);
                }

                httpRequestResult.Dispose();
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Appx Url request unknown exception", e);
            }

            return urlResult;
        }

        /// <summary>
        /// 获取非商店应用的安装包
        /// </summary>
        /// <param name="productId">产品 ID</param>
        /// <returns>带解析后文件信息的列表</returns>
        public static async Task<List<QueryLinksModel>> GetNonAppxPackagesAsync(string productId)
        {
            List<QueryLinksModel> nonAppxPackagesList = [];

            try
            {
                string url = string.Format("https://storeedgefd.dsx.mp.microsoft.com/v9.0/packageManifests/{0}?market={1}", productId, StoreRegionService.StoreRegion.CodeTwoLetter);

                // 默认超时时间是 20 秒
                HttpClient httpClient = new();
                HttpRequestResult httpRequestResult = await httpClient.TryGetAsync(new(url));
                httpClient.Dispose();

                // 请求成功
                if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", httpRequestResult.ResponseMessage.StatusCode.ToString() },
                        { "Headers", httpRequestResult.ResponseMessage.Headers is null ? string.Empty : httpRequestResult.ResponseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", httpRequestResult.ResponseMessage.RequestMessage is null ? string.Empty : httpRequestResult.ResponseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "Non Appx Url request successfully.", responseDict);

                    string responseString = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

                    if (JsonObject.TryParse(responseString, out JsonObject responseStringObject))
                    {
                        JsonObject dataObject = responseStringObject.GetNamedValue("Data").GetObject();
                        JsonObject versionsObject = dataObject.GetNamedValue("Versions").GetArray()[0].GetObject();
                        JsonArray installersArray = versionsObject.GetNamedValue("Installers").GetArray();

                        Lock nonAppxPackagesLock = new();
                        List<Task> taskList = [];

                        foreach (IJsonValue installer in installersArray)
                        {
                            taskList.Add(Task.Run(async () =>
                            {
                                JsonObject installerObject = installer.GetObject();

                                string installerType = installerObject.GetNamedString("InstallerType");
                                string installerUrl = installerObject.GetNamedString("InstallerUrl");
                                string fileSize = await GetNonAppxPackageFileSizeAsync(installerUrl);
                                string fileSizeString = FileSizeHelper.ConvertFileSizeToString(double.TryParse(fileSize, out double size) ? size : 0);

                                if (string.IsNullOrEmpty(installerType) || installerUrl.ToLower().EndsWith(".exe") || installerUrl.ToLower().EndsWith(".msi"))
                                {
                                    nonAppxPackagesLock.Enter();

                                    try
                                    {
                                        nonAppxPackagesList.Add(new QueryLinksModel()
                                        {
                                            FileName = installerUrl[..installerUrl.LastIndexOf('.')][(installerUrl.LastIndexOf('/') + 1)..],
                                            FileLink = installerUrl,
                                            FileSize = fileSizeString,
                                            IsSelected = false,
                                            IsSelectMode = false,
                                        });
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        nonAppxPackagesLock.Exit();
                                    }
                                }
                                else
                                {
                                    string name = installerUrl.Split('/')[^1];

                                    nonAppxPackagesLock.Enter();

                                    try
                                    {
                                        nonAppxPackagesList.Add(new QueryLinksModel()
                                        {
                                            FileName = string.Format("{0} ({1}).{2}", name, installerObject.GetNamedString("InstallerLocale"), installerType),
                                            FileLink = installerUrl,
                                            FileSize = fileSizeString,
                                            IsSelected = false,
                                            IsSelectMode = false,
                                        });
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        nonAppxPackagesLock.Exit();
                                    }
                                }
                            }));
                        }

                        await Task.WhenAll(taskList);
                    }
                }
                // 请求失败
                else
                {
                    LogService.WriteLog(LoggingLevel.Information, "Non Appx Url request failed", httpRequestResult.ExtendedError);
                }

                httpRequestResult.Dispose();
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Non Appx Url request unknown exception", e);
            }

            return nonAppxPackagesList;
        }

        /// <summary>
        /// 获取非商店应用下载文件的大小
        /// </summary>
        private static async Task<string> GetNonAppxPackageFileSizeAsync(string url)
        {
            string fileSizeResult = "0";

            try
            {
                // 默认超时时间是 20 秒
                HttpClient httpClient = new();
                HttpRequestMessage requestMessage = new(HttpMethod.Head, new(url));
                HttpRequestResult httpRequestResult = await httpClient.TrySendRequestAsync(requestMessage);
                httpClient.Dispose();

                // 请求成功
                if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", httpRequestResult.ResponseMessage.StatusCode.ToString() },
                        { "Headers", httpRequestResult.ResponseMessage.Headers is null ? string.Empty : httpRequestResult.ResponseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", httpRequestResult.ResponseMessage.RequestMessage is null ? string.Empty : httpRequestResult.ResponseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "Non appx package file size request successfully.", responseDict);

                    fileSizeResult = Convert.ToString(httpRequestResult.ResponseMessage.Content.Headers.ContentLength);
                }
                // 请求失败
                else
                {
                    LogService.WriteLog(LoggingLevel.Information, "Non appx package file size request failed", httpRequestResult.ExtendedError);
                }

                httpRequestResult.Dispose();
            }

            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Non appx package file size request unknown exception", e);
            }

            return fileSizeResult;
        }

        private static IXmlNode GetElementsByName(this IXmlNode xmlNode, string name)
        {
            foreach (IXmlNode node in xmlNode.ChildNodes)
            {
                if (Equals(node.NodeName, name))
                {
                    return node;
                }
            }

            return null;
        }
    }
}
