using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static bool isInitialized;

        private static KeyValuePair<string, string> _defaultAppLanguage;
        private static KeyValuePair<string, string> _currentAppLanguage;

        private static readonly ResourceManager resourceManager = new();
        private static ResourceContext defaultResourceContext;
        private static ResourceContext currentResourceContext;
        private static ResourceMap resourceMap;

        public static List<TypeModel> TypeList { get; } = [];

        public static List<ChannelModel> ChannelList { get; } = [];

        public static List<InfoBarModel> QueryLinksInfoList { get; } = [];

        public static List<InfoBarModel> SearchStoreInfoList { get; } = [];

        public static List<TraceCleanupModel> TraceCleanupList { get; } = [];

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(KeyValuePair<string, string> defaultAppLanguage, KeyValuePair<string, string> currentAppLanguage)
        {
            _defaultAppLanguage = defaultAppLanguage;
            _currentAppLanguage = currentAppLanguage;

            defaultResourceContext = resourceManager.CreateResourceContext();
            currentResourceContext = resourceManager.CreateResourceContext();
            resourceMap = resourceManager.MainResourceMap;

            defaultResourceContext.QualifierValues["Language"] = _defaultAppLanguage.Key.ToString();
            currentResourceContext.QualifierValues["Language"] = _currentAppLanguage.Key.ToString();

            isInitialized = true;
        }

        /// <summary>
        /// 初始化应用本地化信息
        /// </summary>
        public static void LocalizeReosurce()
        {
            InitializeTypeList();
            InitializeChannelList();
            InitializeQueryLinksInfoList();
            InitializeSearchStoreInfoList();
            InitializeTraceCleanupList();
        }

        /// <summary>
        /// 初始化类型列表
        /// </summary>
        private static void InitializeTypeList()
        {
            TypeList.Add(new TypeModel
            {
                DisplayName = GetLocalized("Resources/URL"),
                InternalName = "url",
                ShortName = "url"
            });

            TypeList.Add(new TypeModel
            {
                DisplayName = GetLocalized("Resources/ProductID"),
                InternalName = "ProductId",
                ShortName = "pid"
            });
        }

        /// <summary>
        /// 初始化通道信息列表
        /// </summary>
        private static void InitializeChannelList()
        {
            ChannelList.Add(new ChannelModel
            {
                DisplayName = GetLocalized("Resources/Fast"),
                InternalName = "WIF",
                ShortName = "wif"
            });

            ChannelList.Add(new ChannelModel
            {
                DisplayName = GetLocalized("Resources/Slow"),
                InternalName = "WIS",
                ShortName = "wis"
            });

            ChannelList.Add(new ChannelModel
            {
                DisplayName = GetLocalized("Resources/RP"),
                InternalName = "RP",
                ShortName = "rp"
            });

            ChannelList.Add(new ChannelModel
            {
                DisplayName = GetLocalized("Resources/Retail"),
                InternalName = "Retail",
                ShortName = "rt"
            });
        }

        /// <summary>
        /// 初始化查询链接信息状态栏信息列表
        /// </summary>
        private static void InitializeQueryLinksInfoList()
        {
            QueryLinksInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Informational,
                Message = GetLocalized("Store/QueryLinksInfoGetting"),
                PrRingActValue = true,
                PrRingVisValue = true
            });
            QueryLinksInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Success,
                Message = GetLocalized("Store/QueryLinksInfoSuccess"),
                PrRingActValue = false,
                PrRingVisValue = false
            });
            QueryLinksInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Warning,
                Message = GetLocalized("Store/QueryLinksInfoWarning"),
                PrRingActValue = false,
                PrRingVisValue = false
            });
            QueryLinksInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Error,
                Message = GetLocalized("Store/QueryLinksInfoError"),
                PrRingActValue = false,
                PrRingVisValue = false
            });
        }

        /// <summary>
        /// 初始化查询链接信息状态栏信息列表
        /// </summary>
        private static void InitializeSearchStoreInfoList()
        {
            SearchStoreInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Informational,
                Message = GetLocalized("Store/SearchStoreInfoGetting"),
                PrRingActValue = true,
                PrRingVisValue = true
            });
            SearchStoreInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Success,
                Message = GetLocalized("Store/SearchStoreInfoSuccess"),
                PrRingActValue = false,
                PrRingVisValue = false
            });
            SearchStoreInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Warning,
                Message = GetLocalized("Store/SearchStoreInfoWarning"),
                PrRingActValue = false,
                PrRingVisValue = false
            });
            SearchStoreInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Error,
                Message = GetLocalized("Store/SearchStoreInfoError"),
                PrRingActValue = false,
                PrRingVisValue = false
            });
        }

        /// <summary>
        /// 初始化痕迹清理列表
        /// </summary>
        private static void InitializeTraceCleanupList()
        {
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("Dialog/HistoryRecord"),
                InternalName = CleanKind.History,
                CleanFailedText = GetLocalized("Dialog/HistoryCleanError")
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("Dialog/ActionCenter"),
                InternalName = CleanKind.ActionCenter,
                CleanFailedText = GetLocalized("Dialog/ActionCenterError")
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("Dialog/DownloadRecord"),
                InternalName = CleanKind.Download,
                CleanFailedText = GetLocalized("Dialog/DownloadCleanError")
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("Dialog/LocalFile"),
                InternalName = CleanKind.LocalFile,
                CleanFailedText = GetLocalized("Dialog/LocalFileCleanError")
            });
        }

        /// <summary>
        /// 字符串本地化
        /// </summary>
        public static string GetLocalized(string resource)
        {
            if (isInitialized)
            {
                try
                {
                    return resourceMap.GetValue(resource, currentResourceContext).ValueAsString;
                }
                catch (Exception currentResourceException)
                {
                    LogService.WriteLog(LoggingLevel.Warning, string.Format("Get resource context with langauge {0} failed.", _currentAppLanguage.Key), currentResourceException);
                    try
                    {
                        return resourceMap.GetValue(resource, defaultResourceContext).ValueAsString;
                    }
                    catch (Exception defaultResourceException)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, string.Format("Get resource context string with langauge {0} failed.", _defaultAppLanguage.Key), defaultResourceException);
                        return resource;
                    }
                }
            }
            else
            {
                LogService.WriteLog(LoggingLevel.Warning, "Have you forgot to initialize app's resources?", new NullReferenceException());
                return resource;
            }
        }

        /// <summary>
        /// 获取嵌入的数据
        /// </summary>
        public static byte[] GetEmbeddedData(string resource)
        {
            try
            {
                return resourceMap.GetValue(resource).ValueAsBytes;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Get resource embedData failed.", e);
                return default;
            }
        }
    }
}
