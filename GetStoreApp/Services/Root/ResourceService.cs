using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Models.Dialogs;
using Microsoft.Management.Deployment;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Diagnostics;
using Windows.Storage.Streams;

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

        private static readonly ResourceContext defaultResourceContext = new();
        private static readonly ResourceContext currentResourceContext = new();
        private static readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap;

        public static List<TypeModel> TypeList { get; } = [];

        public static List<ChannelModel> ChannelList { get; } = [];

        public static List<InfoBarModel> QueryLinksInfoList { get; } = [];

        public static List<InfoBarModel> SearchStoreInfoList { get; } = [];

        public static List<KeyValuePair<string, string>> BackdropList { get; } = [];

        public static List<KeyValuePair<string, string>> WebKernelList { get; } = [];

        public static List<KeyValuePair<string, string>> QueryLinksModeList { get; } = [];

        public static List<KeyValuePair<string, string>> InstallModeList { get; } = [];

        public static List<KeyValuePair<string, string>> ThemeList { get; } = [];

        public static List<TraceCleanupModel> TraceCleanupList { get; } = [];

        public static List<KeyValuePair<string, string>> WinGetInstallModeList { get; } = [];

        public static List<KeyValuePair<string, string>> DoEngineModeList { get; } = [];

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(KeyValuePair<string, string> defaultAppLanguage, KeyValuePair<string, string> currentAppLanguage)
        {
            _defaultAppLanguage = defaultAppLanguage;
            _currentAppLanguage = currentAppLanguage;

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
            InitializeBackdropList();
            InitializeWebKernelList();
            InitializeQueryLinksModeList();
            InitializeInstallModeList();
            InitializeThemeList();
            InitializeTraceCleanupList();
            InitializeWinGetInstallModeList();
            InitializeDoEngineModeList();
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
        /// 初始化应用背景色信息列表
        /// </summary>
        private static void InitializeBackdropList()
        {
            BackdropList.Add(KeyValuePair.Create(nameof(SystemBackdropTheme.Default), GetLocalized("Settings/BackdropDefault")));
            BackdropList.Add(KeyValuePair.Create(nameof(MicaKind) + nameof(MicaKind.Base), GetLocalized("Settings/BackdropMica")));
            BackdropList.Add(KeyValuePair.Create(nameof(MicaKind) + nameof(MicaKind.BaseAlt), GetLocalized("Settings/BackdropMicaAlt")));
            BackdropList.Add(KeyValuePair.Create(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Default), GetLocalized("Settings/BackdropAcrylic")));
            BackdropList.Add(KeyValuePair.Create(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Base), GetLocalized("Settings/BackdropAcrylicBase")));
            BackdropList.Add(KeyValuePair.Create(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Thin), GetLocalized("Settings/BackdropAcrylicThin")));
        }

        /// <summary>
        /// 初始化网页浏览器内核信息列表
        /// </summary>
        private static void InitializeWebKernelList()
        {
            WebKernelList.Add(KeyValuePair.Create("WebView", GetLocalized("Settings/WebKernelWebView")));
            WebKernelList.Add(KeyValuePair.Create("WebView2", GetLocalized("Settings/WebKernelWebView2")));
        }

        /// <summary>
        /// 初始化查询链接方式信息列表
        /// </summary>
        private static void InitializeQueryLinksModeList()
        {
            QueryLinksModeList.Add(KeyValuePair.Create("Official", GetLocalized("Settings/QueryLinksModeOfficial")));
            QueryLinksModeList.Add(KeyValuePair.Create("ThirdParty", GetLocalized("Settings/QueryLinksModeThirdParty")));
        }

        /// <summary>
        /// 初始化安装模式信息列表
        /// </summary>
        private static void InitializeInstallModeList()
        {
            InstallModeList.Add(KeyValuePair.Create("AppInstall", GetLocalized("Settings/AppInstall")));
            InstallModeList.Add(KeyValuePair.Create("CodeInstall", GetLocalized("Settings/CodeInstall")));
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(KeyValuePair.Create(nameof(ElementTheme.Default), GetLocalized("Settings/ThemeDefault")));
            ThemeList.Add(KeyValuePair.Create(nameof(ElementTheme.Light), GetLocalized("Settings/ThemeLight")));
            ThemeList.Add(KeyValuePair.Create(nameof(ElementTheme.Dark), GetLocalized("Settings/ThemeDark")));
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
        /// 初始化 WinGet 程序包安装模式信息列表
        /// </summary>
        private static void InitializeWinGetInstallModeList()
        {
            WinGetInstallModeList.Add(KeyValuePair.Create(nameof(PackageInstallMode.Interactive), GetLocalized("Settings/InteractiveInstall")));
            WinGetInstallModeList.Add(KeyValuePair.Create(nameof(PackageInstallMode.Silent), GetLocalized("Settings/SlientInstall")));
        }

        /// <summary>
        /// 初始化下载引擎方式信息列表
        /// </summary>
        private static void InitializeDoEngineModeList()
        {
            DoEngineModeList.Add(KeyValuePair.Create("DeliveryOptimization", GetLocalized("Settings/DoEngineDo")));
            DoEngineModeList.Add(KeyValuePair.Create("BITS", GetLocalized("Settings/DoEngineBits")));
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
                LogService.WriteLog(LoggingLevel.Warning, "Have you forgot to initialize app's resources?", new Exception());
                return resource;
            }
        }

        /// <summary>
        /// 获取嵌入的数据
        /// </summary>
        public static async Task<byte[]> GetEmbeddedDataAsync(string resource)
        {
            try
            {
                IRandomAccessStream randomAccessStream = await resourceMap.GetValue(resource).GetValueAsStreamAsync();
                DataReader dataReader = new(randomAccessStream);
                await dataReader.LoadAsync((uint)randomAccessStream.Size);
                byte[] bytesArray = new byte[randomAccessStream.Size];
                dataReader.ReadBytes(bytesArray);
                dataReader.Dispose();
                return bytesArray;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Get resource embedData failed.", e);
                return [];
            }
        }
    }
}
