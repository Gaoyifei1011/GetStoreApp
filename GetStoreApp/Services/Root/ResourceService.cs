using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Models.Dialogs.CommonDialogs.Settings;
using GetStoreApp.Properties;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources.Core;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static bool IsInitialized { get; set; } = false;

        private static LanguageModel DefaultAppLanguage { get; set; }

        private static LanguageModel CurrentAppLanguage { get; set; }

        private static ResourceContext DefaultResourceContext { get; set; } = new ResourceContext();

        private static ResourceContext CurrentResourceContext { get; set; } = new ResourceContext();

        private static ResourceMap ResourceMap { get; } = ResourceManager.Current.MainResourceMap;

        public static List<TypeModel> TypeList { get; } = new List<TypeModel>();

        public static List<ChannelModel> ChannelList { get; } = new List<ChannelModel>();

        public static List<StatusBarStateModel> StatusBarStateList { get; } = new List<StatusBarStateModel>();

        public static List<AppExitModel> AppExitList { get; } = new List<AppExitModel>();

        public static List<BackdropModel> BackdropList { get; } = new List<BackdropModel>();

        public static List<DownloadModeModel> DownloadModeList { get; } = new List<DownloadModeModel>();

        public static List<HistoryLiteNumModel> HistoryLiteNumList { get; } = new List<HistoryLiteNumModel>();

        public static List<HistoryJumpListNumModel> HistoryJumpListNumList { get; } = new List<HistoryJumpListNumModel>();

        public static List<InstallModeModel> InstallModeList { get; } = new List<InstallModeModel>();

        public static List<ThemeModel> ThemeList { get; } = new List<ThemeModel>();

        public static List<NotifyIconMenuThemeModel> NotifyIconMenuThemeList { get; } = new List<NotifyIconMenuThemeModel>();

        public static List<TraceCleanupModel> TraceCleanupList { get; } = new List<TraceCleanupModel>();

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(LanguageModel defaultAppLanguage, LanguageModel currentAppLanguage)
        {
            DefaultAppLanguage = defaultAppLanguage;
            CurrentAppLanguage = currentAppLanguage;

            DefaultResourceContext.QualifierValues["Language"] = DefaultAppLanguage.InternalName;
            CurrentResourceContext.QualifierValues["Language"] = CurrentAppLanguage.InternalName;

            IsInitialized = true;
        }

        /// <summary>
        /// 初始化应用本地化信息
        /// </summary>
        public static void LocalizeReosurce()
        {
            InitializeTypeList();
            InitializeChannelList();
            InitializeStatusBarStateList();
            InitializeAppExitList();
            InitializeBackdropList();
            InitializeDownloadModeList();
            InitializeHistoryLiteNumList();
            InitializeHistoryJumpListNumList();
            InitializeInstallModeList();
            InitializeThemeList();
            InitializeNotifyIconMenuThemeList();
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

            TypeList.Add(new TypeModel
            {
                DisplayName = GetLocalized("Resources/PackageFamilyName"),
                InternalName = "PackageFamilyName",
                ShortName = "pfn"
            });

            TypeList.Add(new TypeModel
            {
                DisplayName = GetLocalized("Resources/CategoryID"),
                InternalName = "CategoryId",
                ShortName = "cid"
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
        /// 初始化状态栏信息列表
        /// </summary>
        private static void InitializeStatusBarStateList()
        {
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Informational,
                StateInfoText = GetLocalized("Home/StatusInfoGetting"),
                StatePrRingActValue = true,
                StatePrRingVisValue = true
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Success,
                StateInfoText = GetLocalized("Home/StatusInfoSuccess"),
                StatePrRingActValue = false,
                StatePrRingVisValue = false
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Warning,
                StateInfoText = GetLocalized("Home/StatusInfoWarning"),
                StatePrRingActValue = false,
                StatePrRingVisValue = false
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Error,
                StateInfoText = GetLocalized("Home/StatusInfoError"),
                StatePrRingActValue = false,
                StatePrRingVisValue = false
            });
        }

        /// <summary>
        /// 初始化应用退出内容列表
        /// </summary>
        private static void InitializeAppExitList()
        {
            AppExitList.Add(new AppExitModel
            {
                DisplayName = GetLocalized("Settings/HideToTray"),
                InternalName = "HideToTray"
            });
            AppExitList.Add(new AppExitModel
            {
                DisplayName = GetLocalized("Settings/CloseApp"),
                InternalName = "CloseApp"
            });
        }

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>
        private static void InitializeBackdropList()
        {
            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("Settings/BackdropDefault"),
                InternalName = "Default"
            });

            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("Settings/BackdropMica"),
                InternalName = "Mica"
            });

            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("Settings/BackdropMicaAlt"),
                InternalName = "MicaAlt"
            });

            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("Settings/BackdropAcrylic"),
                InternalName = "Acrylic"
            });
        }

        /// <summary>
        /// 初始化应用下载方式列表
        /// </summary>
        private static void InitializeDownloadModeList()
        {
            DownloadModeList.Add(new DownloadModeModel
            {
                DisplayName = GetLocalized("Settings/DownloadInApp"),
                InternalName = "DownloadInApp"
            });
            DownloadModeList.Add(new DownloadModeModel
            {
                DisplayName = GetLocalized("Settings/DownloadWithBrowser"),
                InternalName = "DownloadWithBrowser"
            });
        }

        /// <summary>
        /// 初始化主页面历史记录显示数量信息列表
        /// </summary>
        private static void InitializeHistoryLiteNumList()
        {
            HistoryLiteNumList.Add(new HistoryLiteNumModel
            {
                HistoryLiteNumName = GetLocalized("Settings/HistoryLite3Items"),
                HistoryLiteNumValue = 3
            });
            HistoryLiteNumList.Add(new HistoryLiteNumModel
            {
                HistoryLiteNumName = GetLocalized("Settings/HistoryLite5Items"),
                HistoryLiteNumValue = 5
            });
        }

        /// <summary>
        /// 初始化任务栏右键跳转列表历史记录显示数量信息列表
        /// </summary>
        private static void InitializeHistoryJumpListNumList()
        {
            HistoryJumpListNumList.Add(new HistoryJumpListNumModel
            {
                HistoryJumpListNumName = GetLocalized("Settings/HistoryJumpList3Items"),
                HistoryJumpListNumValue = "3"
            });
            HistoryJumpListNumList.Add(new HistoryJumpListNumModel
            {
                HistoryJumpListNumName = GetLocalized("Settings/HistoryJumpList5Items"),
                HistoryJumpListNumValue = "5"
            });
            HistoryJumpListNumList.Add(new HistoryJumpListNumModel
            {
                HistoryJumpListNumName = GetLocalized("Settings/HistoryJumpList7Items"),
                HistoryJumpListNumValue = "7"
            });
            HistoryJumpListNumList.Add(new HistoryJumpListNumModel
            {
                HistoryJumpListNumName = GetLocalized("Settings/HistoryJumpList9Items"),
                HistoryJumpListNumValue = "9"
            });
            HistoryJumpListNumList.Add(new HistoryJumpListNumModel
            {
                HistoryJumpListNumName = GetLocalized("Settings/HistoryJumpListUnlimited"),
                HistoryJumpListNumValue = "Unlimited"
            });
        }

        /// <summary>
        /// 初始化安装模式信息列表
        /// </summary>
        private static void InitializeInstallModeList()
        {
            InstallModeList.Add(new InstallModeModel
            {
                DisplayName = GetLocalized("Settings/AppInstall"),
                InternalName = "AppInstall"
            });
            InstallModeList.Add(new InstallModeModel
            {
                DisplayName = GetLocalized("Settings/CodeInstall"),
                InternalName = "CodeInstall"
            });
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(new ThemeModel
            {
                DisplayName = GetLocalized("Settings/ThemeDefault"),
                InternalName = Convert.ToString(ElementTheme.Default)
            });
            ThemeList.Add(new ThemeModel
            {
                DisplayName = GetLocalized("Settings/ThemeLight"),
                InternalName = Convert.ToString(ElementTheme.Light)
            });
            ThemeList.Add(new ThemeModel
            {
                DisplayName = GetLocalized("Settings/ThemeDark"),
                InternalName = Convert.ToString(ElementTheme.Dark)
            });
        }

        /// <summary>
        /// 初始化通知区域右键菜单主题信息列表
        /// </summary>
        private static void InitializeNotifyIconMenuThemeList()
        {
            NotifyIconMenuThemeList.Add(new NotifyIconMenuThemeModel
            {
                DisplayName = GetLocalized("Settings/NotifyIconMenuAppTheme"),
                InternalName = "NotifyIconMenuAppTheme"
            });
            NotifyIconMenuThemeList.Add(new NotifyIconMenuThemeModel
            {
                DisplayName = GetLocalized("Settings/NotifyIconMenuSystemTheme"),
                InternalName = "NotifyIconMenuSystemTheme"
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
                InternalName = CleanArgs.History,
                CleanFailedText = GetLocalized("Dialog/HistoryCleanError")
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("Dialog/JumpList"),
                InternalName = CleanArgs.JumpList,
                CleanFailedText = GetLocalized("Dialog/JumpListCleanError")
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("Dialog/DownloadRecord"),
                InternalName = CleanArgs.Download,
                CleanFailedText = GetLocalized("Dialog/DownloadCleanError")
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("Dialog/LocalFile"),
                InternalName = CleanArgs.LocalFile,
                CleanFailedText = GetLocalized("Dialog/LocalFileCleanError")
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
                catch (Exception)
                {
                    try
                    {
                        return ResourceMap.GetValue(resource, DefaultResourceContext).ValueAsString;
                    }
                    catch (Exception)
                    {
                        return resource;
                    }
                }
            }
            else
            {
                throw new ApplicationException(Resources.ResourcesInitializeFailed);
            }
        }
    }
}
