using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Models.Dialogs.Settings;
using Microsoft.Management.Deployment;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static bool IsInitialized = false;

        private static DictionaryEntry DefaultAppLanguage;
        private static DictionaryEntry CurrentAppLanguage;

        private static ResourceContext DefaultResourceContext = new ResourceContext();
        private static ResourceContext CurrentResourceContext = new ResourceContext();

        private static ResourceMap ResourceMap = ResourceManager.Current.MainResourceMap;

        public static List<TypeModel> TypeList { get; } = new List<TypeModel>();

        public static List<ChannelModel> ChannelList { get; } = new List<ChannelModel>();

        public static List<InfoBarModel> QueryLinksInfoList { get; } = new List<InfoBarModel>();

        public static List<InfoBarModel> SearchStoreInfoList { get; } = new List<InfoBarModel>();

        public static List<DictionaryEntry> BackdropList { get; } = new List<DictionaryEntry>();

        public static List<DictionaryEntry> DownloadModeList { get; } = new List<DictionaryEntry>();

        public static List<DictionaryEntry> WebKernelList { get; } = new List<DictionaryEntry>();

        public static List<DictionaryEntry> InstallModeList { get; } = new List<DictionaryEntry>();

        public static List<DictionaryEntry> ThemeList { get; } = new List<DictionaryEntry>();

        public static List<TraceCleanupModel> TraceCleanupList { get; } = new List<TraceCleanupModel>();

        public static List<DictionaryEntry> WinGetInstallModeList { get; } = new List<DictionaryEntry>();

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(DictionaryEntry defaultAppLanguage, DictionaryEntry currentAppLanguage)
        {
            DefaultAppLanguage = defaultAppLanguage;
            CurrentAppLanguage = currentAppLanguage;

            DefaultResourceContext.QualifierValues["Language"] = DefaultAppLanguage.Value.ToString();
            CurrentResourceContext.QualifierValues["Language"] = CurrentAppLanguage.Value.ToString();

            IsInitialized = true;
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
            InitializeDownloadModeList();
            InitializeWebKernelList();
            InitializeInstallModeList();
            InitializeThemeList();
            InitializeTraceCleanupList();
            InitializeWinGetInstallModeList();
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
            BackdropList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/BackdropDefault"),
                Value = nameof(SystemBackdropTheme.Default)
            });

            BackdropList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/BackdropMica"),
                Value = nameof(MicaKind) + nameof(MicaKind.Base)
            });

            BackdropList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/BackdropMicaAlt"),
                Value = nameof(MicaKind) + nameof(MicaKind.BaseAlt)
            });

            BackdropList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/BackdropAcrylic"),
                Value = nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Default)
            });

            BackdropList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/BackdropAcrylicBase"),
                Value = nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Base)
            });

            BackdropList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/BackdropAcrylicThin"),
                Value = nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Thin)
            });
        }

        /// <summary>
        /// 初始化应用下载方式列表
        /// </summary>
        private static void InitializeDownloadModeList()
        {
            DownloadModeList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/DownloadInApp"),
                Value = "DownloadInApp"
            });
            DownloadModeList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/DownloadWithBrowser"),
                Value = "DownloadWithBrowser"
            });
        }

        /// <summary>
        /// 初始化网页浏览器内核信息列表
        /// </summary>
        private static void InitializeWebKernelList()
        {
            WebKernelList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/WebKernelIE"),
                Value = "IE"
            });
            WebKernelList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/WebKernelWebView2"),
                Value = "WebView2"
            });
        }

        /// <summary>
        /// 初始化安装模式信息列表
        /// </summary>
        private static void InitializeInstallModeList()
        {
            InstallModeList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/AppInstall"),
                Value = "AppInstall"
            });
            InstallModeList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/CodeInstall"),
                Value = "CodeInstall"
            });
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/ThemeDefault"),
                Value = nameof(ElementTheme.Default)
            });
            ThemeList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/ThemeLight"),
                Value = nameof(ElementTheme.Light)
            });
            ThemeList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/ThemeDark"),
                Value = nameof(ElementTheme.Dark)
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
        /// 初始化 WinGet 程序包安装模式信息列表
        /// </summary>
        private static void InitializeWinGetInstallModeList()
        {
            WinGetInstallModeList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/InteractiveInstall"),
                Value = nameof(PackageInstallMode.Interactive),
            });
            WinGetInstallModeList.Add(new DictionaryEntry
            {
                Key = GetLocalized("Settings/SlientInstall"),
                Value = nameof(PackageInstallMode.Silent),
            });
        }

        /// <summary>
        /// 字符串本地化
        /// </summary>
        public static string GetLocalized(string resource)
        {
            if (IsInitialized)
            {
                try
                {
                    return ResourceMap.GetValue(resource, CurrentResourceContext).ValueAsString;
                }
                catch (Exception currentResourceException)
                {
                    LogService.WriteLog(LoggingLevel.Warning, string.Format("Get resource context with langauge {0} failed.", CurrentAppLanguage.Value), currentResourceException);
                    try
                    {
                        return ResourceMap.GetValue(resource, DefaultResourceContext).ValueAsString;
                    }
                    catch (Exception defaultResourceException)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, string.Format("Get resource context with langauge {0} failed.", DefaultAppLanguage.Value), defaultResourceException);
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
    }
}
