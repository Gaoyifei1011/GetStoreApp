﻿using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 文件内容为空时，先初始化文件内容
    /// 下载记录数据库存储服务
    /// </summary>
    public static class DownloadXmlService
    {
        private static readonly object downloadXmlFileLock = new object();

        private static bool isReadingAndWriting = false;

        private const string DownloadItem = "DownloadItem";
        private const string DownloadKey = "DownloadKey";
        private const string FileName = "FileName";
        private const string FileLink = "FileLink";
        private const string FilePath = "FilePath";
        private const string FileSHA1 = "FileSHA1";
        private const string FileSize = "FileSize";
        private const string DownloadFlag = "DownloadFlags";

        /// <summary>
        /// 检查是否有下载异常的记录，并将对应的下载状态值复原
        /// </summary>
        public static async Task InitializeDownloadXmlAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    FileInfo downloadFileInfo = new FileInfo(XmlStorageService.DownloadXmlFile.Path);
                    if (downloadFileInfo.Exists && downloadFileInfo.Length is 0)
                    {
                        await XmlStorageService.InitializeDownloadXmlFileAsync();
                    }
                    else
                    {
                        while (isReadingAndWriting) await Task.Delay(10);
                        lock (downloadXmlFileLock) isReadingAndWriting = true;

                        XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                        lock (downloadXmlFileLock) isReadingAndWriting = false;

                        if (downloadFileDocument.HasChildNodes())
                        {
                            IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                            if (downloadRootElement.HasChildNodes())
                            {
                                bool isModified = false;
                                foreach (IXmlNode downloadItemElement in downloadRootElement.ChildNodes)
                                {
                                    if (downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText.Equals(1.ToString()) ||
                                       downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText.Equals(3.ToString())
                                       )
                                    {
                                        isModified = true;
                                        downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText = 2.ToString();
                                    }
                                }

                                if (isModified)
                                {
                                    while (isReadingAndWriting) await Task.Delay(10);
                                    lock (downloadXmlFileLock) isReadingAndWriting = true;

                                    await downloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                                    lock (downloadXmlFileLock) isReadingAndWriting = false;
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "InitializeWebKernel download record state failed.", e);
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
                lock (downloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (downloadXmlFileLock) isReadingAndWriting = false;

                if (downloadFileDocument.HasChildNodes())
                {
                    IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                    XmlElement downloadItemElement = downloadFileDocument.CreateElement(DownloadItem);
                    downloadItemElement.SetAttribute(DownloadKey, backgroundItem.DownloadKey);
                    downloadItemElement.SetAttribute(FileName, backgroundItem.FileName);
                    downloadItemElement.SetAttribute(FileLink, backgroundItem.FileLink);
                    downloadItemElement.SetAttribute(FilePath, backgroundItem.FilePath);
                    downloadItemElement.SetAttribute(FileSize, Convert.ToString(backgroundItem.TotalSize));
                    downloadItemElement.SetAttribute(DownloadFlag, Convert.ToString(backgroundItem.DownloadFlag));
                    downloadRootElement.AppendChild(downloadItemElement);

                    while (isReadingAndWriting) await Task.Delay(10);
                    lock (downloadXmlFileLock) isReadingAndWriting = true;

                    await downloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                    lock (downloadXmlFileLock) isReadingAndWriting = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Add download record failed.", e);
                return false;
            }
        }

        /// <summary>
        /// 直接添加下载记录数据，并返回下载记录添加是否成功的结果
        /// </summary>
        public static async Task<bool> AddAsync(DownloadSchedulerModel downloadSchedulerItem)
        {
            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (downloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (downloadXmlFileLock) isReadingAndWriting = false;

                if (downloadFileDocument.HasChildNodes())
                {
                    IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                    XmlElement downloadItemElement = downloadFileDocument.CreateElement(DownloadItem);
                    downloadItemElement.SetAttribute(DownloadKey, downloadSchedulerItem.DownloadKey);
                    downloadItemElement.SetAttribute(FileName, downloadSchedulerItem.FileName);
                    downloadItemElement.SetAttribute(FileLink, downloadSchedulerItem.FileLink);
                    downloadItemElement.SetAttribute(FilePath, downloadSchedulerItem.FilePath);
                    downloadItemElement.SetAttribute(FileSize, Convert.ToString(downloadSchedulerItem.TotalSize));
                    downloadItemElement.SetAttribute(DownloadFlag, Convert.ToString(downloadSchedulerItem.DownloadFlag));
                    downloadRootElement.AppendChild(downloadItemElement);

                    while (isReadingAndWriting) await Task.Delay(10);
                    lock (downloadXmlFileLock) isReadingAndWriting = true;

                    await downloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                    lock (downloadXmlFileLock) isReadingAndWriting = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Add download record failed.", e);
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
                lock (downloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (downloadXmlFileLock) isReadingAndWriting = false;

                if (downloadFileDocument.HasChildNodes())
                {
                    IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                    if (downloadRootElement.HasChildNodes())
                    {
                        bool isModified = false;
                        foreach (IXmlNode downloadItemElement in downloadRootElement.ChildNodes)
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
                            lock (downloadXmlFileLock) isReadingAndWriting = true;

                            await downloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                            lock (downloadXmlFileLock) isReadingAndWriting = false;
                            isUpdatedSuccessfully = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Update download record flag failed.", e);
            }
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
                lock (downloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (downloadXmlFileLock) isReadingAndWriting = false;

                if (downloadFileDocument.HasChildNodes())
                {
                    IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                    if (downloadRootElement.HasChildNodes())
                    {
                        bool isModified = false;
                        foreach (IXmlNode downloadItemElement in downloadRootElement.ChildNodes)
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
                            lock (downloadXmlFileLock) isReadingAndWriting = true;

                            await downloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                            lock (downloadXmlFileLock) isReadingAndWriting = false;
                            isUpdatedSuccessfully = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Update file size failed.", e);
            }
            return isUpdatedSuccessfully;
        }

        /// <summary>
        /// 获取指定下载标志的下载记录数据
        /// </summary>
        /// <param name="downloadFlag">文件下载标志：0为下载失败，1为等待下载，2为暂停下载，3为正在下载，4为成功下载</param>
        /// <returns>返回指定下载标志记录列表</returns>
        public static async Task<List<BackgroundModel>> QueryWithFlagAsync(int downloadFlag)
        {
            List<BackgroundModel> downloadRawList = new List<BackgroundModel>();

            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (downloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (downloadXmlFileLock) isReadingAndWriting = false;

                if (downloadFileDocument.HasChildNodes())
                {
                    IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                    if (downloadRootElement.HasChildNodes())
                    {
                        foreach (IXmlNode downloadItemElement in downloadRootElement.ChildNodes)
                        {
                            if (downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText.Equals(Convert.ToString(downloadFlag)))
                            {
                                BackgroundModel downloadRawModel = new BackgroundModel
                                {
                                    DownloadKey = downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText,
                                    FileName = downloadItemElement.Attributes.GetNamedItem(FileName).InnerText,
                                    FileLink = downloadItemElement.Attributes.GetNamedItem(FileLink).InnerText,
                                    FilePath = downloadItemElement.Attributes.GetNamedItem(FilePath).InnerText,
                                    TotalSize = Convert.ToInt32(downloadItemElement.Attributes.GetNamedItem(FileSize).InnerText),
                                    DownloadFlag = Convert.ToInt32(downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText)
                                };
                                downloadRawList.Add(downloadRawModel);
                            }
                        }
                    }
                }
                return downloadRawList;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Query download record with flag failed.", e);
                return downloadRawList;
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
                lock (downloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (downloadXmlFileLock) isReadingAndWriting = false;

                if (downloadFileDocument.HasChildNodes())
                {
                    IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                    if (downloadRootElement.HasChildNodes())
                    {
                        foreach (IXmlNode downloadItemElement in downloadRootElement.ChildNodes)
                        {
                            if (downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText.Equals(Convert.ToString(downloadKey)))
                            {
                                downloadRawModel.DownloadKey = downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText;
                                downloadRawModel.FileName = downloadItemElement.Attributes.GetNamedItem(FileName).InnerText;
                                downloadRawModel.FileLink = downloadItemElement.Attributes.GetNamedItem(FileLink).InnerText;
                                downloadRawModel.FilePath = downloadItemElement.Attributes.GetNamedItem(FilePath).InnerText;
                                downloadRawModel.TotalSize = Convert.ToInt32(downloadItemElement.Attributes.GetNamedItem(FileSize).InnerText);
                                downloadRawModel.DownloadFlag = Convert.ToInt32(downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText);
                                break;
                            }
                        }
                    }
                }
                return downloadRawModel;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Query download record with downloadKey failed.", e);
                return downloadRawModel;
            }
        }

        /// <summary>
        /// 检查是否存在相同键值的数据
        /// </summary>
        public static async Task<DuplicatedDataKind> CheckDuplicatedAsync(string downloadKey)
        {
            DuplicatedDataKind duplicatedDataInfo = DuplicatedDataKind.None;

            try
            {
                while (isReadingAndWriting) await Task.Delay(10);
                lock (downloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (downloadXmlFileLock) isReadingAndWriting = false;

                if (downloadFileDocument.HasChildNodes())
                {
                    IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                    if (downloadRootElement.HasChildNodes())
                    {
                        foreach (IXmlNode downloadItemElement in downloadRootElement.ChildNodes)
                        {
                            if (downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText.Equals(Convert.ToString(downloadKey)))
                            {
                                int downloadFlag = Convert.ToInt32(downloadItemElement.Attributes.GetNamedItem(DownloadFlag).InnerText);
                                if (downloadFlag is 4)
                                {
                                    duplicatedDataInfo = DuplicatedDataKind.Completed;
                                }
                                else
                                {
                                    duplicatedDataInfo = DuplicatedDataKind.Unfinished;
                                }
                            }
                        }
                    }
                }
                return duplicatedDataInfo;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Check duplicated download data record failed.", e);
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
                lock (downloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (downloadXmlFileLock) isReadingAndWriting = false;

                if (downloadFileDocument.HasChildNodes())
                {
                    IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                    if (downloadRootElement.HasChildNodes())
                    {
                        bool isModified = false;
                        foreach (IXmlNode downloadItemElement in downloadRootElement.ChildNodes)
                        {
                            if (downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText.Equals(Convert.ToString(downloadKey)))
                            {
                                downloadRootElement.RemoveChild(downloadItemElement);
                                isModified = true;
                                break;
                            }
                        }

                        if (isModified)
                        {
                            while (isReadingAndWriting) await Task.Delay(10);
                            lock (downloadXmlFileLock) isReadingAndWriting = true;

                            await downloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                            lock (downloadXmlFileLock) isReadingAndWriting = false;
                            isDeleteSuccessfully = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Delete download record failed.", e);
            }
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
                lock (downloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (downloadXmlFileLock) isReadingAndWriting = false;

                if (downloadFileDocument.HasChildNodes())
                {
                    IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                    if (downloadRootElement.HasChildNodes())
                    {
                        bool isModified = false;
                        selectedDownloadList.ForEach((backgroundItem) =>
                        {
                            foreach (IXmlNode downloadItemElement in downloadRootElement.ChildNodes)
                            {
                                if (downloadItemElement.Attributes.GetNamedItem(DownloadKey).InnerText.Equals(Convert.ToString(backgroundItem.DownloadKey)))
                                {
                                    downloadRootElement.RemoveChild(downloadItemElement);
                                    isModified = true;
                                    break;
                                }
                            }
                        });

                        if (isModified)
                        {
                            while (isReadingAndWriting) await Task.Delay(10);
                            lock (downloadXmlFileLock) isReadingAndWriting = true;

                            await downloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                            lock (downloadXmlFileLock) isReadingAndWriting = false;
                            isDeleteSuccessfully = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Delete selected download record failed.", e);
            }
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
                lock (downloadXmlFileLock) isReadingAndWriting = true;

                XmlDocument downloadFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.DownloadXmlFile);

                lock (downloadXmlFileLock) isReadingAndWriting = false;

                if (downloadFileDocument.HasChildNodes())
                {
                    IXmlNode downloadRootElement = downloadFileDocument.ChildNodes[0];

                    if (downloadRootElement.HasChildNodes())
                    {
                        while (downloadRootElement.ChildNodes.Count > 0)
                        {
                            downloadRootElement.RemoveChild(downloadRootElement.ChildNodes[0]);
                        }

                        while (isReadingAndWriting) await Task.Delay(10);
                        lock (downloadXmlFileLock) isReadingAndWriting = true;

                        await downloadFileDocument.SaveToFileAsync(XmlStorageService.DownloadXmlFile);

                        lock (downloadXmlFileLock) isReadingAndWriting = false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Clear download record failed.", e);
                return false;
            }
        }
    }
}
