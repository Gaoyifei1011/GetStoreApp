using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 文件内容为空时，先初始化文件内容。
    /// 下载记录数据库存储服务
    /// </summary>
    public static class DownloadXmlService
    {
        private const string DownloadItem = "DownloadItem";

        private const string DownloadKey = "DownloadKey";

        private const string FileName = "FileName";

        private const string FileLink = "FileLink";

        private const string FilePath = "FilePath";

        private const string FileSHA1 = "FileSHA1";

        private const string FileSize = "FileSize";

        private const string DownloadFlag = "DownloadFlag";

        private static readonly object DownloadXmlFileLock = new object();

        private static bool isReadingAndWriting = false;

        /// <summary>
        /// 检查是否有下载异常的记录，并将对应的下载状态值复原
        /// </summary>
        public static async Task InitializeDownloadXmlAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    FileInfo DownloadFileInfo = new FileInfo(XmlStorageService.DownloadXmlFile.Path);
                    if (DownloadFileInfo.Exists && DownloadFileInfo.Length is 0)
                    {
                        await XmlStorageService.InitializeDownloadXmlFileAsync();
                    }
                    else
                    {
                        while (isReadingAndWriting) await Task.Delay(10);
                        lock (DownloadXmlFileLock) isReadingAndWriting = true;

                        XmlDocument DownloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                        lock (DownloadXmlFileLock) isReadingAndWriting = false;

                        if (DownloadFileDocument.HasChildNodes())
                        {
                            IXmlNode DownloadRootElement = DownloadFileDocument.ChildNodes[0];

                            if (DownloadRootElement.HasChildNodes())
                            {
                                bool isModified = false;
                                foreach (IXmlNode downloadItemElement in DownloadRootElement.ChildNodes)
                                {
                                    if (downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText.Equals(Convert.ToString(1)) ||
                                       downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText.Equals(Convert.ToString(3))
                                       )
                                    {
                                        isModified = true;
                                        downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText = Convert.ToString(2);
                                    }
                                }

                                if (isModified)
                                {
                                    while (isReadingAndWriting) await Task.Delay(10);
                                    lock (DownloadXmlFileLock) isReadingAndWriting = true;

                                    await DownloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                                    lock (DownloadXmlFileLock) isReadingAndWriting = false;
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 直接添加下载记录数据，并返回下载记录添加是否成功的结果
        /// </summary>
        public static async Task<bool> AddAsync(BackgroundModel backgroundItem)
        {
            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (DownloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument DownloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (DownloadXmlFileLock) isReadingAndWriting = false;

                if (DownloadFileDocument.HasChildNodes())
                {
                    IXmlNode DownloadRootElement = DownloadFileDocument.ChildNodes[0];

                    XmlElement DownloadItemElement = DownloadFileDocument.CreateElement(DownloadItem);
                    DownloadItemElement.SetAttribute(DownloadKey, backgroundItem.DownloadKey);
                    DownloadItemElement.SetAttribute(FileName, backgroundItem.FileName);
                    DownloadItemElement.SetAttribute(FileLink, backgroundItem.FileLink);
                    DownloadItemElement.SetAttribute(FilePath, backgroundItem.FilePath);
                    DownloadItemElement.SetAttribute(FileSHA1, backgroundItem.FileSHA1);
                    DownloadItemElement.SetAttribute(FileSize, Convert.ToString(backgroundItem.TotalSize));
                    DownloadItemElement.SetAttribute(DownloadFlag, Convert.ToString(backgroundItem.DownloadFlag));
                    DownloadRootElement.AppendChild(DownloadItemElement);

                    while (isReadingAndWriting) await Task.Delay(10);
                    lock (DownloadXmlFileLock) isReadingAndWriting = true;

                    await DownloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                    lock (DownloadXmlFileLock) isReadingAndWriting = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 存在重复的数据，只更新该记录的DownloadFlag
        /// </summary>
        public static async Task<bool> UpdateFlagAsync(string downloadKey, int downloadFlag)
        {
            bool isUpdatedSuccessfully = false;
            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (DownloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument DownloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (DownloadXmlFileLock) isReadingAndWriting = false;

                if (DownloadFileDocument.HasChildNodes())
                {
                    IXmlNode DownloadRootElement = DownloadFileDocument.ChildNodes[0];

                    if (DownloadRootElement.HasChildNodes())
                    {
                        bool isModified = false;
                        foreach (IXmlNode downloadItemElement in DownloadRootElement.ChildNodes)
                        {
                            if (downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText.Equals(downloadKey, StringComparison.OrdinalIgnoreCase))
                            {
                                downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText = Convert.ToString(downloadFlag);
                                isModified = true;
                                break;
                            }
                        }

                        if (isModified)
                        {
                            while (isReadingAndWriting) await Task.Delay(10);
                            lock (DownloadXmlFileLock) isReadingAndWriting = true;

                            await DownloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                            lock (DownloadXmlFileLock) isReadingAndWriting = false;
                            isUpdatedSuccessfully = true;
                        }
                    }
                }
            }
            catch (Exception) { }
            return isUpdatedSuccessfully;
        }

        /// <summary>
        /// 更新该记录对应的文件大小
        /// </summary>
        public static async Task<bool> UpdateFileSizeAsync(string downloadKey, double fileSize)
        {
            bool isUpdatedSuccessfully = false;
            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (DownloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument DownloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (DownloadXmlFileLock) isReadingAndWriting = false;

                if (DownloadFileDocument.HasChildNodes())
                {
                    IXmlNode DownloadRootElement = DownloadFileDocument.ChildNodes[0];

                    if (DownloadRootElement.HasChildNodes())
                    {
                        bool isModified = false;
                        foreach (IXmlNode downloadItemElement in DownloadRootElement.ChildNodes)
                        {
                            if (downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText.Equals(downloadKey, StringComparison.OrdinalIgnoreCase))
                            {
                                downloadItemElement.Attributes.GetNamedItem(FileSize).InnerText = Convert.ToString(fileSize);
                                isModified = true;
                                break;
                            }
                        }

                        if (isModified)
                        {
                            while (isReadingAndWriting) await Task.Delay(10);
                            lock (DownloadXmlFileLock) isReadingAndWriting = true;

                            await DownloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                            lock (DownloadXmlFileLock) isReadingAndWriting = false;
                            isUpdatedSuccessfully = true;
                        }
                    }
                }
            }
            catch (Exception) { }
            return isUpdatedSuccessfully;
        }

        /// <summary>
        /// 获取指定下载标志的下载记录数据
        /// </summary>
        /// <param name="downloadFlag">文件下载标志：0为下载失败，1为等待下载，2为暂停下载，3为正在下载，4为成功下载</param>
        /// <returns>返回指定下载标志记录列表</returns>
        public static async Task<List<BackgroundModel>> QueryWithFlagAsync(int downloadFlag)
        {
            List<BackgroundModel> DownloadRawList = new List<BackgroundModel>();

            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (DownloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument DownloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (DownloadXmlFileLock) isReadingAndWriting = false;

                if (DownloadFileDocument.HasChildNodes())
                {
                    IXmlNode DownloadRootElement = DownloadFileDocument.ChildNodes[0];

                    if (DownloadRootElement.HasChildNodes())
                    {
                        foreach (IXmlNode downloadItemElement in DownloadRootElement.ChildNodes)
                        {
                            if (downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText.Equals(Convert.ToString(downloadFlag)))
                            {
                                BackgroundModel downloadRawModel = new BackgroundModel
                                {
                                    DownloadKey = downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText,
                                    FileName = downloadItemElement.Attributes.GetNamedItem(FileName).InnerText,
                                    FileLink = downloadItemElement.Attributes.GetNamedItem(FileLink).InnerText,
                                    FilePath = downloadItemElement.Attributes.GetNamedItem(FilePath).InnerText,
                                    FileSHA1 = downloadItemElement.Attributes.GetNamedItem(FileSHA1).InnerText,
                                    TotalSize = Convert.ToInt32(downloadItemElement.Attributes.GetNamedItem(FileSize).InnerText),
                                    DownloadFlag = Convert.ToInt32(downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText)
                                };
                                DownloadRawList.Add(downloadRawModel);
                            }
                        }
                    }
                }
                return DownloadRawList;
            }
            catch (Exception)
            {
                return DownloadRawList;
            }
        }

        /// <summary>
        /// 获取指定下载键值的下载记录数据
        /// </summary>
        /// <param name="downloadKey">文件下载对应的唯一键值</param>
        /// <returns>返回指定下载键值对应的具体信息</returns>
        public static async Task<BackgroundModel> QueryWithKeyAsync(string downloadKey)
        {
            BackgroundModel downloadRawModel = new BackgroundModel();

            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (DownloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument DownloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (DownloadXmlFileLock) isReadingAndWriting = false;

                if (DownloadFileDocument.HasChildNodes())
                {
                    IXmlNode DownloadRootElement = DownloadFileDocument.ChildNodes[0];

                    if (DownloadRootElement.HasChildNodes())
                    {
                        foreach (IXmlNode downloadItemElement in DownloadRootElement.ChildNodes)
                        {
                            if (downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText.Equals(Convert.ToString(downloadKey)))
                            {
                                downloadRawModel.DownloadKey = downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText;
                                downloadRawModel.FileName = downloadItemElement.Attributes.GetNamedItem(FileName).InnerText;
                                downloadRawModel.FileLink = downloadItemElement.Attributes.GetNamedItem(FileLink).InnerText;
                                downloadRawModel.FilePath = downloadItemElement.Attributes.GetNamedItem(FilePath).InnerText;
                                downloadRawModel.FileSHA1 = downloadItemElement.Attributes.GetNamedItem(FileSHA1).InnerText;
                                downloadRawModel.TotalSize = Convert.ToInt32(downloadItemElement.Attributes.GetNamedItem(FileSize).InnerText);
                                downloadRawModel.DownloadFlag = Convert.ToInt32(downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText);
                                break;
                            }
                        }
                    }
                }
                return downloadRawModel;
            }
            catch (Exception)
            {
                return downloadRawModel;
            }
        }

        /// <summary>
        /// 检查是否存在相同键值的数据
        /// </summary>
        public static async Task<DuplicatedDataInfoArgs> CheckDuplicatedAsync(string downloadKey)
        {
            DuplicatedDataInfoArgs duplicatedDataInfo = DuplicatedDataInfoArgs.None;

            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (DownloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument DownloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (DownloadXmlFileLock) isReadingAndWriting = false;

                if (DownloadFileDocument.HasChildNodes())
                {
                    IXmlNode DownloadRootElement = DownloadFileDocument.ChildNodes[0];

                    if (DownloadRootElement.HasChildNodes())
                    {
                        foreach (IXmlNode downloadItemElement in DownloadRootElement.ChildNodes)
                        {
                            if (downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText.Equals(Convert.ToString(downloadKey)))
                            {
                                int downloadFlag = Convert.ToInt32(downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText);
                                if (downloadFlag is 4)
                                {
                                    duplicatedDataInfo = DuplicatedDataInfoArgs.Completed;
                                }
                                else
                                {
                                    duplicatedDataInfo = DuplicatedDataInfoArgs.Unfinished;
                                }
                            }
                        }
                    }
                }
                return duplicatedDataInfo;
            }
            catch (Exception)
            {
                return duplicatedDataInfo;
            }
        }

        /// <summary>
        /// 删除下载记录数据
        /// </summary>
        public static async Task<bool> DeleteAsync(string downloadKey)
        {
            bool isDeleteSuccessfully = false;
            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (DownloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument DownloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (DownloadXmlFileLock) isReadingAndWriting = false;

                if (DownloadFileDocument.HasChildNodes())
                {
                    IXmlNode DownloadRootElement = DownloadFileDocument.ChildNodes[0];

                    if (DownloadRootElement.HasChildNodes())
                    {
                        bool isModified = false;
                        foreach (IXmlNode downloadItemElement in DownloadRootElement.ChildNodes)
                        {
                            if (downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText.Equals(Convert.ToString(downloadKey)))
                            {
                                DownloadRootElement.RemoveChild(downloadItemElement);
                                isModified = true;
                                break;
                            }
                        }

                        if (isModified)
                        {
                            while (isReadingAndWriting) await Task.Delay(10);
                            lock (DownloadXmlFileLock) isReadingAndWriting = true;

                            await DownloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                            lock (DownloadXmlFileLock) isReadingAndWriting = false;
                            isDeleteSuccessfully = true;
                        }
                    }
                }
            }
            catch (Exception) { }
            return isDeleteSuccessfully;
        }

        /// <summary>
        /// 删除选定的下载记录数据
        /// </summary>
        public static async Task<bool> DeleteSelectedAsync(List<BackgroundModel> selectedDownloadList)
        {
            bool isDeleteSuccessfully = false;
            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (DownloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument DownloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (DownloadXmlFileLock) isReadingAndWriting = false;

                if (DownloadFileDocument.HasChildNodes())
                {
                    IXmlNode DownloadRootElement = DownloadFileDocument.ChildNodes[0];

                    if (DownloadRootElement.HasChildNodes())
                    {
                        bool isModified = false;
                        selectedDownloadList.ForEach((backgroundItem) =>
                        {
                            foreach (IXmlNode downloadItemElement in DownloadRootElement.ChildNodes)
                            {
                                if (downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText.Equals(Convert.ToString(backgroundItem.DownloadKey)))
                                {
                                    DownloadRootElement.RemoveChild(downloadItemElement);
                                    isModified = true;
                                    break;
                                }
                            }
                        });

                        if (isModified)
                        {
                            while (isReadingAndWriting) await Task.Delay(10);
                            lock (DownloadXmlFileLock) isReadingAndWriting = true;

                            await DownloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                            lock (DownloadXmlFileLock) isReadingAndWriting = false;
                            isDeleteSuccessfully = true;
                        }
                    }
                }
            }
            catch (Exception) { }
            return isDeleteSuccessfully;
        }

        /// <summary>
        /// 清空下载记录数据（不清除正在下载和等待下载的数据）
        /// </summary>
        public static async Task<bool> ClearAsync()
        {
            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (DownloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument DownloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (DownloadXmlFileLock) isReadingAndWriting = false;

                if (DownloadFileDocument.HasChildNodes())
                {
                    IXmlNode DownloadRootElement = DownloadFileDocument.ChildNodes[0];

                    if (DownloadRootElement.HasChildNodes())
                    {
                        while (DownloadRootElement.ChildNodes.Count > 0)
                        {
                            DownloadRootElement.RemoveChild(DownloadRootElement.ChildNodes[0]);
                        }

                        while (isReadingAndWriting) await Task.Delay(10);
                        lock (DownloadXmlFileLock) isReadingAndWriting = true;

                        await DownloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                        lock (DownloadXmlFileLock) isReadingAndWriting = false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
