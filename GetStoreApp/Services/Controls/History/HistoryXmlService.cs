using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.History;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace GetStoreApp.Services.Controls.History
{
    /// <summary>
    /// 历史记录XML存储服务
    /// </summary>
    public static class HistoryXmlService
    {
        private const string HistoryItem = "HistoryItem";

        private const string CreateTimeStamp = "TimeStamp";

        private const string HistoryKey = "HistoryKey";

        private const string HistoryType = "Type";

        private const string HistoryChannel = "Channel";

        private const string HistoryLink = "Link";

        /// <summary>
        /// 文件内容为空时，先初始化文件内容。
        /// 添加历史记录数据
        /// </summary>
        public static async Task AddAsync(HistoryModel historyItem)
        {
            try
            {
                await Task.Run(async () =>
                {
                    FileInfo HistoryFileInfo = new FileInfo(XmlStorageService.HistoryXmlFile.Path);
                    if (HistoryFileInfo.Exists && HistoryFileInfo.Length is 0)
                    {
                        await XmlStorageService.InitializeHistoryXmlFileAsync();
                    }

                    XmlDocument HistoryFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.HistoryXmlFile);

                    if (HistoryFileDocument.HasChildNodes())
                    {
                        bool isDuplicated = false;
                        IXmlNode HistoryRootElement = HistoryFileDocument.ChildNodes[0];
                        // 如果存在相同的行数据，只更新TimeStamp值，没有，添加数据
                        if (HistoryRootElement.HasChildNodes())
                        {
                            foreach (IXmlNode historyItemElement in HistoryRootElement.ChildNodes)
                            {
                                if (historyItemElement.Attributes.GetNamedItem(HistoryKey).InnerText.Equals(historyItem.HistoryKey, StringComparison.OrdinalIgnoreCase))
                                {
                                    historyItemElement.Attributes.GetNamedItem(CreateTimeStamp).InnerText = Convert.ToString(historyItem.CreateTimeStamp);
                                    isDuplicated = true;
                                    break;
                                }
                            }
                        }

                        if (!isDuplicated)
                        {
                            XmlElement HistoryItemElement = HistoryFileDocument.CreateElement(HistoryItem);
                            HistoryItemElement.SetAttribute(CreateTimeStamp, Convert.ToString(historyItem.CreateTimeStamp));
                            HistoryItemElement.SetAttribute(HistoryKey, historyItem.HistoryKey);
                            HistoryItemElement.SetAttribute(HistoryType, historyItem.HistoryType);
                            HistoryItemElement.SetAttribute(HistoryChannel, historyItem.HistoryChannel);
                            HistoryItemElement.SetAttribute(HistoryLink, historyItem.HistoryLink);
                            HistoryRootElement.AppendChild(HistoryItemElement);
                        }
                        await HistoryFileDocument.SaveToFileAsync(XmlStorageService.HistoryXmlFile);
                    }
                });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.WARNING, "Add history record failed.", e);
                return;
            }
        }

        /// <summary>
        /// 获取所有的历史记录数据
        /// </summary>
        /// <param name="timeSortOrder">时间戳顺序，True为递增排序，False为递减排序</param>
        /// <param name="typeFilter">选择过滤的类型，默认为None，不过滤</param>
        /// <param name="channelFilter">选择过的通道，默认为None，不过滤</param>
        /// <returns>返回历史记录列表</returns>
        public static async Task<(List<HistoryModel>, bool, bool)> QueryAllAsync(bool timeSortOrder = false, string typeFilter = "None", string channelFilter = "None")
        {
            List<HistoryModel> historyRawList = new List<HistoryModel>();
            bool isHistoryEmpty = true;
            bool isHistoryEmptyAfterFilter = false;

            try
            {
                await Task.Run(async () =>
                {
                    XmlDocument HistoryFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.HistoryXmlFile);

                    //调用之前先判断历史记录表是否为空
                    if (HistoryFileDocument.HasChildNodes())
                    {
                        IXmlNode HistoryRootElement = HistoryFileDocument.ChildNodes[0];

                        if (HistoryRootElement.HasChildNodes())
                        {
                            // 按照指定条件过滤
                            IEnumerable<IXmlNode> FilteredResultList = HistoryRootElement.ChildNodes;

                            bool isfiltered = false;
                            // 类型过滤
                            if (typeFilter is not "None")
                            {
                                FilteredResultList = FilteredResultList.Where(item => item.Attributes.GetNamedItem(HistoryType).InnerText.Equals(typeFilter, StringComparison.OrdinalIgnoreCase));
                                isfiltered = true;
                            }

                            // 通道过滤
                            if (channelFilter is not "None")
                            {
                                FilteredResultList = FilteredResultList.Where(item => item.Attributes.GetNamedItem(HistoryChannel).InnerText.Equals(channelFilter, StringComparison.OrdinalIgnoreCase));
                                isfiltered = true;
                            }

                            // 时间戳排序
                            if (timeSortOrder)
                            {
                                FilteredResultList = FilteredResultList.OrderBy(historyItemElement => Convert.ToInt64(historyItemElement.Attributes.GetNamedItem(CreateTimeStamp).InnerText));
                            }
                            else
                            {
                                FilteredResultList = FilteredResultList.OrderByDescending(historyItemElement =>
                                Convert.ToInt64(historyItemElement.Attributes.GetNamedItem(CreateTimeStamp).InnerText));
                            }

                            foreach (IXmlNode historyItemElement in FilteredResultList)
                            {
                                HistoryModel historyRawModel = new HistoryModel
                                {
                                    CreateTimeStamp = Convert.ToInt64(historyItemElement.Attributes.GetNamedItem(CreateTimeStamp).InnerText),
                                    HistoryKey = historyItemElement.Attributes.GetNamedItem(HistoryKey).InnerText,
                                    HistoryType = historyItemElement.Attributes.GetNamedItem(HistoryType).InnerText,
                                    HistoryChannel = historyItemElement.Attributes.GetNamedItem(HistoryChannel).InnerText,
                                    HistoryLink = historyItemElement.Attributes.GetNamedItem(HistoryLink).InnerText
                                };
                                historyRawList.Add(historyRawModel);
                            }

                            // 判断经过过滤后历史记录表是否为空
                            if (isfiltered)
                            {
                                isHistoryEmpty = false;
                                isHistoryEmptyAfterFilter = historyRawList.Count is 0;
                            }
                            else
                            {
                                isHistoryEmpty = false;
                                isHistoryEmptyAfterFilter = false;
                            }
                        }
                        else
                        {
                            isHistoryEmpty = true;
                            isHistoryEmptyAfterFilter = true;
                        }
                    }
                    else
                    {
                        isHistoryEmpty = true;
                        isHistoryEmptyAfterFilter = true;
                    }
                });
                return (historyRawList, isHistoryEmpty, isHistoryEmptyAfterFilter);
            }
            catch (Exception e)
            {
                isHistoryEmpty = true;
                isHistoryEmptyAfterFilter = true;
                LogService.WriteLog(LogType.WARNING, "Query history record with condition failed.", e);
                return (historyRawList, isHistoryEmpty, isHistoryEmptyAfterFilter);
            }
        }

        /// <summary>
        /// 获取一定数量的历史记录数据
        /// </summary>
        public static async Task<List<HistoryModel>> QueryAsync(int value)
        {
            List<HistoryModel> HistoryRawList = new List<HistoryModel>();

            try
            {
                await Task.Run(async () =>
                {
                    XmlDocument HistoryFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.HistoryXmlFile);

                    //调用之前先判断历史记录表是否为空
                    if (HistoryFileDocument.HasChildNodes())
                    {
                        IXmlNode HistoryRootElement = HistoryFileDocument.ChildNodes[0];

                        if (HistoryRootElement.HasChildNodes())
                        {
                            IEnumerable<IXmlNode> HistoryItemElementList = HistoryRootElement.ChildNodes;

                            HistoryItemElementList = HistoryItemElementList.OrderByDescending(historyItemElement =>
                                Convert.ToInt64(historyItemElement.Attributes.GetNamedItem(CreateTimeStamp).InnerText));

                            int index = 0;
                            foreach (IXmlNode historyItemElement in HistoryItemElementList)
                            {
                                if (index < value)
                                {
                                    HistoryModel historyRawModel = new HistoryModel
                                    {
                                        CreateTimeStamp = Convert.ToInt64(historyItemElement.Attributes.GetNamedItem(CreateTimeStamp).InnerText),
                                        HistoryKey = historyItemElement.Attributes.GetNamedItem(HistoryKey).InnerText,
                                        HistoryType = historyItemElement.Attributes.GetNamedItem(HistoryType).InnerText,
                                        HistoryChannel = historyItemElement.Attributes.GetNamedItem(HistoryChannel).InnerText,
                                        HistoryLink = historyItemElement.Attributes.GetNamedItem(HistoryLink).InnerText
                                    };

                                    HistoryRawList.Add(historyRawModel);
                                    index++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                });

                return HistoryRawList;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.WARNING, "Query history record failed.", e);
                return HistoryRawList;
            }
        }

        /// <summary>
        /// 删除选定的历史记录数据
        /// </summary>
        public static async Task<bool> DeleteAsync(List<HistoryModel> selectedHistoryDataList)
        {
            try
            {
                await Task.Run(async () =>
                {
                    XmlDocument HistoryFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.HistoryXmlFile);

                    //调用之前先判断历史记录表是否为空
                    if (HistoryFileDocument.HasChildNodes())
                    {
                        IXmlNode HistoryRootElement = HistoryFileDocument.ChildNodes[0];

                        if (HistoryRootElement.HasChildNodes())
                        {
                            bool isModified = false;
                            selectedHistoryDataList.ForEach(historyItem =>
                            {
                                foreach (IXmlNode historyItemElement in HistoryRootElement.ChildNodes)
                                {
                                    if (historyItemElement.Attributes.GetNamedItem(HistoryKey).InnerText.Equals(historyItem.HistoryKey, StringComparison.OrdinalIgnoreCase))
                                    {
                                        HistoryRootElement.RemoveChild(historyItemElement);
                                        isModified = true;
                                        break;
                                    }
                                }
                            });

                            if (isModified)
                            {
                                await HistoryFileDocument.SaveToFileAsync(XmlStorageService.HistoryXmlFile);
                            }
                        }
                    }
                });
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.WARNING, "Delete history record failed.", e);
                return false;
            }
        }

        /// <summary>
        /// 清空历史记录数据
        /// </summary>
        public static async Task<bool> ClearAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    XmlDocument HistoryFileDocument = await XmlDocument.LoadFromFileAsync(XmlStorageService.HistoryXmlFile);

                    //调用之前先判断历史记录表是否为空
                    if (HistoryFileDocument.HasChildNodes())
                    {
                        IXmlNode HistoryRootElement = HistoryFileDocument.ChildNodes[0];

                        if (HistoryRootElement.HasChildNodes())
                        {
                            while (HistoryRootElement.ChildNodes.Count > 0)
                            {
                                HistoryRootElement.RemoveChild(HistoryRootElement.ChildNodes[0]);
                            }
                            await HistoryFileDocument.SaveToFileAsync(XmlStorageService.HistoryXmlFile);
                        }
                    }
                });

                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.WARNING, "Clear history record failed.", e);
                return false;
            }
        }
    }
}
