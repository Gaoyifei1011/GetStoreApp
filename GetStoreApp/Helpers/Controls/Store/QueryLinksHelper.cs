using GetStoreApp.Helpers.Root;
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
using Windows.Data.Xml.Dom;
using Windows.Foundation.Diagnostics;
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
                requestContent = requestContent.Remove(0, requestContent.LastIndexOf('/') + 1);
            }
            if (requestContent.Contains('?'))
            {
                requestContent = requestContent.Remove(requestContent.IndexOf('?'));
            }
            return requestContent;
        }

        /// <summary>
        /// 获取微软商店服务器储存在用户本地终端上的数据
        /// </summary>
        public static async Task<string> GetCookieAsync()
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            string cookieResult = string.Empty;

            try
            {
                byte[] contentBytes = await ResourceService.GetEmbeddedDataAsync("Files/Assets/Embed/cookie.xml");

                HttpStringContent httpStringContent = new(Encoding.UTF8.GetString(contentBytes));
                httpStringContent.Headers.Expires = DateTime.Now;
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/soap+xml");
                httpStringContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                httpStringContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new();
                HttpResponseMessage responseMessage = await httpClient.PostAsync(cookieUri, httpStringContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (responseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", responseMessage.StatusCode.ToString() },
                        { "Headers", responseMessage.Headers is null ? string.Empty : responseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", responseMessage.RequestMessage is null ? string.Empty : responseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "Cookie request successfully.", responseDict);

                    string responseString = await responseMessage.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                    responseMessage.Dispose();

                    XmlDocument responseStringDocument = new();
                    responseStringDocument.LoadXml(responseString);

                    XmlNodeList encryptedDataList = responseStringDocument.GetElementsByTagName("EncryptedData");
                    if (encryptedDataList.Count > 0)
                    {
                        cookieResult = encryptedDataList[0].InnerText;
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
                LogService.WriteLog(LoggingLevel.Information, "Cookie request failed", e);
            }
            // 捕捉因访问超时引发的异常
            catch (TaskCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "Cookie request timeout", e);
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Cookie request unknown exception", e);
            }
            finally
            {
                cancellationTokenSource.Dispose();
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
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            Tuple<bool, AppInfoModel> appInformationResult = null;
            AppInfoModel appInfoModel = new();

            try
            {
                string categoryIDAPI = string.Format("https://storeedgefd.dsx.mp.microsoft.com/v9.0/products/{0}?market={1}&locale={2}&deviceFamily=Windows.Desktop", productId, StoreRegionService.StoreRegion.CodeTwoLetter, LanguageService.AppLanguage.Key);

                HttpClient httpClient = new();
                HttpResponseMessage responseMessage = await httpClient.GetAsync(new(categoryIDAPI)).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (responseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", responseMessage.StatusCode.ToString() },
                        { "Headers", responseMessage.Headers is null ? string.Empty : responseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", responseMessage.RequestMessage is null ? string.Empty : responseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "App Information request successfully.", responseDict);

                    string responseString = await responseMessage.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                    responseMessage.Dispose();

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
                            appInformationResult = new Tuple<bool, AppInfoModel>(true, appInfoModel);
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
                LogService.WriteLog(LoggingLevel.Information, "App Information request failed", e);
            }

            // 捕捉因访问超时引发的异常
            catch (TaskCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "App Information request timeout", e);
            }

            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "App Information request unknown exception", e);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            return appInformationResult is null ? new Tuple<bool, AppInfoModel>(false, appInfoModel) : appInformationResult;
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
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            string fileListXmlResult = string.Empty;

            try
            {
                byte[] wubytesArray = await ResourceService.GetEmbeddedDataAsync("Files/Assets/Embed/wu.xml");
                string fileListXml = Encoding.UTF8.GetString(wubytesArray).Replace("{1}", cookie).Replace("{2}", categoryId).Replace("{3}", ring);
                byte[] contentBytes = Encoding.UTF8.GetBytes(fileListXml);

                HttpStringContent httpStringContent = new(fileListXml);
                httpStringContent.Headers.Expires = DateTime.Now;
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/soap+xml");
                httpStringContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                httpStringContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new();
                HttpResponseMessage responseMessage = await httpClient.PostAsync(fileListXmlUri, httpStringContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (responseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", responseMessage.StatusCode.ToString() },
                        { "Headers", responseMessage.Headers is null ? string.Empty : responseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", responseMessage.RequestMessage is null ? string.Empty : responseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "FileListXml request successfully.", responseDict);

                    string responseString = await responseMessage.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                    responseMessage.Dispose();

                    fileListXmlResult = responseString.Replace("&lt;", "<").Replace("&gt;", ">");
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
                LogService.WriteLog(LoggingLevel.Information, "FileListXml request failed", e);
            }
            // 捕捉因访问超时引发的异常
            catch (TaskCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "FileListXml request timeout", e);
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Network state unknown exception", e);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            return fileListXmlResult;
        }

        /// <summary>
        /// 获取商店应用安装包
        /// </summary>
        /// <param name="fileListXml">文件信息的字符串</param>
        /// <param name="ring">通道</param>
        /// <returns>带解析后文件信息的列表</returns>
        public static List<QueryLinksModel> GetAppxPackages(string fileListXml, string ring)
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
                        string extension = fileNode.Attributes.GetNamedItem("FileName").InnerText.Remove(0, fileNode.Attributes.GetNamedItem("FileName").InnerText.LastIndexOf('.'));
                        string size = fileNode.Attributes.GetNamedItem("Size").InnerText;
                        string digest = fileNode.Attributes.GetNamedItem("Digest").InnerText;

                        if (!appxPackagesInfoDict.ContainsKey(name))
                        {
                            appxPackagesInfoDict.Add(name, new Tuple<string, string, string>(extension, size, digest));
                        }
                    }
                }

                Lock appxPackagesLock = new();
                XmlNodeList securedFragmentList = fileListDocument.DocumentElement.GetElementsByTagName("SecuredFragment");
                CountdownEvent countdownEvent = new(securedFragmentList.Count);

                foreach (IXmlNode securedFragmentNode in securedFragmentList)
                {
                    IXmlNode securedFragmentCloneNode = securedFragmentNode;
                    Task.Run(async () =>
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
                                    countdownEvent.Signal();
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
                            else
                            {
                                countdownEvent.Signal();
                            }
                        }
                    });
                }

                countdownEvent.Wait();
                countdownEvent.Dispose();
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
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            string urlResult = string.Empty;

            try
            {
                byte[] urlbytesArray = await ResourceService.GetEmbeddedDataAsync("Files/Assets/Embed/url.xml");
                string url = Encoding.UTF8.GetString(urlbytesArray).Replace("{1}", updateID).Replace("{2}", revisionNumber).Replace("{3}", ring);
                byte[] contentBytes = Encoding.UTF8.GetBytes(url);

                HttpStringContent httpContent = new(url);
                httpContent.Headers.Expires = DateTime.Now;
                httpContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/soap+xml");
                httpContent.Headers.ContentLength = Convert.ToUInt64(contentBytes.Length);
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpClient httpClient = new();
                HttpResponseMessage responseMessage = await httpClient.PostAsync(urlUri, httpContent).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (responseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", responseMessage.StatusCode.ToString() },
                        { "Headers", responseMessage.Headers is null ? string.Empty : responseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", responseMessage.RequestMessage is null ? string.Empty : responseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "Appx Url request successfully.", responseDict);

                    string responseString = await responseMessage.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                    responseMessage.Dispose();

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
                else
                {
                    httpClient.Dispose();
                    responseMessage.Dispose();
                }
            }
            // 捕捉因为网络失去链接获取信息时引发的异常
            catch (COMException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "Appx Url request failed", e);
            }
            // 捕捉因访问超时引发的异常
            catch (TaskCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "Appx Url request timeout", e);
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Appx Url request unknown exception", e);
            }
            finally
            {
                cancellationTokenSource.Dispose();
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
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            List<QueryLinksModel> nonAppxPackagesList = [];

            try
            {
                string url = string.Format("https://storeedgefd.dsx.mp.microsoft.com/v9.0/packageManifests/{0}?market={1}", productId, StoreRegionService.StoreRegion.CodeTwoLetter);

                HttpClient httpClient = new();
                HttpResponseMessage responseMessage = await httpClient.GetAsync(new(url)).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (responseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", responseMessage.StatusCode.ToString() },
                        { "Headers", responseMessage.Headers is null ? string.Empty : responseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", responseMessage.RequestMessage is null ? string.Empty : responseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "Non Appx Url request successfully.", responseDict);

                    string responseString = await responseMessage.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                    responseMessage.Dispose();

                    if (JsonObject.TryParse(responseString, out JsonObject responseStringObject))
                    {
                        JsonObject dataObject = responseStringObject.GetNamedValue("Data").GetObject();
                        JsonObject versionsObject = dataObject.GetNamedValue("Versions").GetArray()[0].GetObject();
                        JsonArray installersArray = versionsObject.GetNamedValue("Installers").GetArray();

                        Lock nonAppxPackagesLock = new();
                        CountdownEvent countdownEvent = new(installersArray.Count);

                        foreach (IJsonValue installer in installersArray)
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
                                        FileName = installerUrl.Remove(installerUrl.LastIndexOf('.')).Remove(0, installerUrl.LastIndexOf('/') + 1),
                                        FileLink = installerUrl,
                                        FileSize = fileSizeString,
                                        IsSelected = false,
                                        IsSelectMode = false,
                                    });
                                    countdownEvent.Signal();
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
                                    countdownEvent.Signal();
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
                        }

                        countdownEvent.Wait();
                        countdownEvent.Dispose();
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
                LogService.WriteLog(LoggingLevel.Information, "Non Appx Url request failed", e);
            }
            // 捕捉因访问超时引发的异常
            catch (TaskCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "Non Appx Url request timeout", e);
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Non Appx Url request unknown exception", e);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            return nonAppxPackagesList;
        }

        /// <summary>
        /// 获取非商店应用下载文件的大小
        /// </summary>
        private static async Task<string> GetNonAppxPackageFileSizeAsync(string url)
        {
            // 添加超时设置（半分钟后停止获取）
            CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            string fileSizeResult = "0";

            try
            {
                HttpClient httpClient = new();
                HttpRequestMessage requestMessage = new(HttpMethod.Head, new(url));
                HttpResponseMessage responseMessage = await httpClient.SendRequestAsync(requestMessage).AsTask(cancellationTokenSource.Token);

                // 请求成功
                if (responseMessage.IsSuccessStatusCode)
                {
                    Dictionary<string, string> responseDict = new()
                    {
                        { "Status code", responseMessage.StatusCode.ToString() },
                        { "Headers", responseMessage.Headers is null ? string.Empty : responseMessage.Headers.ToString().Replace('\r', ' ').Replace('\n', ' ') },
                        { "Response message:", responseMessage.RequestMessage is null ? string.Empty : responseMessage.RequestMessage.ToString().Replace('\r', ' ').Replace('\n', ' ') }
                    };

                    LogService.WriteLog(LoggingLevel.Information, "Non appx package file size request successfully.", responseDict);

                    fileSizeResult = Convert.ToString(responseMessage.Content.Headers.ContentLength);
                    httpClient.Dispose();
                    responseMessage.Dispose();
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
                LogService.WriteLog(LoggingLevel.Information, "FileListXml request failed", e);
            }
            // 捕捉因访问超时引发的异常
            catch (TaskCanceledException e)
            {
                LogService.WriteLog(LoggingLevel.Information, "FileListXml request timeout", e);
            }
            // 其他异常
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Network state unknown exception", e);
            }
            finally
            {
                cancellationTokenSource.Dispose();
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
