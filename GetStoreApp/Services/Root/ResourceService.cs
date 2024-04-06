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
        private static bool isInitialized = false;

        private static DictionaryEntry _defaultAppLanguage;
        private static DictionaryEntry _currentAppLanguage;

        private static ResourceContext defaultResourceContext = new ResourceContext();
        private static ResourceContext currentResourceContext = new ResourceContext();

        private static ResourceMap resourceMap = ResourceManager.Current.MainResourceMap;

        public static List<TypeModel> TypeList { get; } = new List<TypeModel>();

        public static List<ChannelModel> ChannelList { get; } = new List<ChannelModel>();

        public static List<InfoBarModel> QueryLinksInfoList { get; } = new List<InfoBarModel>();

        public static List<InfoBarModel> SearchStoreInfoList { get; } = new List<InfoBarModel>();

        public static List<DictionaryEntry> BackdropList { get; } = new List<DictionaryEntry>();

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
            WebKernelList.Add(new DictionaryEntry(GetLocalized("Settings/WebKernelIE"), "IE"));
            WebKernelList.Add(new DictionaryEntry(GetLocalized("Settings/WebKernelWebView2"), "WebView2"));
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
                    LogService.WriteLog(LoggingLevel.Warning, string.Format("Get resource context with langauge {0} failed.", _currentAppLanguage.Value), currentResourceException);
                    try
                    {
                        return resourceMap.GetValue(resource, defaultResourceContext).ValueAsString;
                    }
                    catch (Exception defaultResourceException)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, string.Format("Get resource context string with langauge {0} failed.", _defaultAppLanguage.Value), defaultResourceException);
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
                DataReader dataReader = new DataReader(randomAccessStream);
                await dataReader.LoadAsync((uint)randomAccessStream.Size);
                byte[] bytesArray = new byte[randomAccessStream.Size];
                dataReader.ReadBytes(bytesArray);
                dataReader.Dispose();
                return bytesArray;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, string.Format("Get resource embedData failed."), e);
                return [];
            }
        }
    }
}
