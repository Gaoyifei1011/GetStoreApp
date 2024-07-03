using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Models.Dialogs.Settings;
using Microsoft.Management.Deployment;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static bool isInitialized = false;

        private static DictionaryEntry _defaultAppLanguage;
        private static DictionaryEntry _currentAppLanguage;

        private static readonly ResourceManager resourceManager = new();
        private static readonly ResourceContext defaultResourceContext;
        private static readonly ResourceContext currentResourceContext;
        private static readonly ResourceMap resourceMap = new ResourceManager().MainResourceMap;

        public static List<TypeModel> TypeList { get; } = [];

        public static List<ChannelModel> ChannelList { get; } = [];

        public static List<InfoBarModel> QueryLinksInfoList { get; } = [];

        public static List<InfoBarModel> SearchStoreInfoList { get; } = [];

        public static List<DictionaryEntry> BackdropList { get; } = [];

        public static List<DictionaryEntry> WebKernelList { get; } = [];

        public static List<DictionaryEntry> QueryLinksModeList { get; } = [];

        public static List<DictionaryEntry> InstallModeList { get; } = [];

        public static List<DictionaryEntry> ThemeList { get; } = [];

        public static List<TraceCleanupModel> TraceCleanupList { get; } = [];

        public static List<DictionaryEntry> WinGetInstallModeList { get; } = [];

        public static List<DictionaryEntry> DoEngineModeList { get; } = [];

        static ResourceService()
        {
            defaultResourceContext = resourceManager.CreateResourceContext();
            currentResourceContext = resourceManager.CreateResourceContext();
            resourceMap = resourceManager.MainResourceMap;
            GlobalNotificationService.ApplicationExit += OnApplicationExit;
            resourceManager.ResourceNotFound += OnResourceNotFound;
        }

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private static void OnApplicationExit(object sender, EventArgs args)
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                resourceManager.ResourceNotFound -= OnResourceNotFound;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Unregister resource service event failed", e);
            }
        }

        /// <summary>
        /// 找不到指定的资源而尝试获取资源失败时发生的事件
        /// </summary>
        private static void OnResourceNotFound(ResourceManager sender, ResourceNotFoundEventArgs args)
        {
            Dictionary<string, string> loggingInformationDict = new()
            {
                { "ResourceName", args.Name },
                { "ResourceContext", args.Context.QualifierValues["language"] }
            };

            LogService.WriteLog(LoggingLevel.Warning, string.Format("No value for resource {0} could be found.", args.Name), loggingInformationDict);
        }

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(DictionaryEntry defaultAppLanguage, DictionaryEntry currentAppLanguage)
        {
            _defaultAppLanguage = defaultAppLanguage;
            _currentAppLanguage = currentAppLanguage;

            defaultResourceContext.QualifierValues["Language"] = _defaultAppLanguage.Value.ToString();
            currentResourceContext.QualifierValues["Language"] = _currentAppLanguage.Value.ToString();

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
            BackdropList.Add(new DictionaryEntry(GetLocalized("Settings/BackdropDefault"), nameof(SystemBackdropTheme.Default)));
            BackdropList.Add(new DictionaryEntry(GetLocalized("Settings/BackdropMica"), nameof(MicaKind) + nameof(MicaKind.Base)));
            BackdropList.Add(new DictionaryEntry(GetLocalized("Settings/BackdropMicaAlt"), nameof(MicaKind) + nameof(MicaKind.BaseAlt)));
            BackdropList.Add(new DictionaryEntry(GetLocalized("Settings/BackdropAcrylic"), nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Default)));
            BackdropList.Add(new DictionaryEntry(GetLocalized("Settings/BackdropAcrylicBase"), nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Base)));
            BackdropList.Add(new DictionaryEntry(GetLocalized("Settings/BackdropAcrylicThin"), nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Thin)));
        }

        /// <summary>
        /// 初始化网页浏览器内核信息列表
        /// </summary>
        private static void InitializeWebKernelList()
        {
            WebKernelList.Add(new DictionaryEntry(GetLocalized("Settings/WebKernelWebView"), "WebView"));
            WebKernelList.Add(new DictionaryEntry(GetLocalized("Settings/WebKernelWebView2"), "WebView2"));
        }

        /// <summary>
        /// 初始化查询链接方式信息列表
        /// </summary>
        private static void InitializeQueryLinksModeList()
        {
            QueryLinksModeList.Add(new DictionaryEntry(GetLocalized("Settings/QueryLinksModeOfficial"), "Official"));
            QueryLinksModeList.Add(new DictionaryEntry(GetLocalized("Settings/QueryLinksModeThirdParty"), "ThirdParty"));
        }

        /// <summary>
        /// 初始化安装模式信息列表
        /// </summary>
        private static void InitializeInstallModeList()
        {
            InstallModeList.Add(new DictionaryEntry(GetLocalized("Settings/AppInstall"), "AppInstall"));
            InstallModeList.Add(new DictionaryEntry(GetLocalized("Settings/CodeInstall"), "CodeInstall"));
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(new DictionaryEntry(GetLocalized("Settings/ThemeDefault"), nameof(ElementTheme.Default)));
            ThemeList.Add(new DictionaryEntry(GetLocalized("Settings/ThemeLight"), nameof(ElementTheme.Light)));
            ThemeList.Add(new DictionaryEntry(GetLocalized("Settings/ThemeDark"), nameof(ElementTheme.Dark)));
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
            WinGetInstallModeList.Add(new DictionaryEntry(GetLocalized("Settings/InteractiveInstall"), nameof(PackageInstallMode.Interactive)));
            WinGetInstallModeList.Add(new DictionaryEntry(GetLocalized("Settings/SlientInstall"), nameof(PackageInstallMode.Silent)));
        }

        /// <summary>
        /// 初始化下载引擎方式信息列表
        /// </summary>
        private static void InitializeDoEngineModeList()
        {
            DoEngineModeList.Add(new DictionaryEntry(GetLocalized("Settings/DoEngineDo"), "DeliveryOptimization"));
            DoEngineModeList.Add(new DictionaryEntry(GetLocalized("Settings/DoEngineBits"), "BITS"));
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
                    string currentResourceString = resourceMap.GetValue(resource, currentResourceContext).ValueAsString;

                    if (!string.IsNullOrEmpty(currentResourceString))
                    {
                        return currentResourceString;
                    }

                    string defaultResourceString = resourceMap.GetValue(resource, defaultResourceContext).ValueAsString;

                    if (!string.IsNullOrEmpty(defaultResourceString))
                    {
                        return defaultResourceString;
                    }

                    return resource;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, string.Format("Get resource context with resource {0} failed.", resource), e);
                    return resource;
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
        public static byte[] GetEmbeddedData(string resource)
        {
            try
            {
                return resourceMap.GetValue(resource).ValueAsBytes;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Get resource embedData failed.", e);
                return [];
            }
        }
    }
}
