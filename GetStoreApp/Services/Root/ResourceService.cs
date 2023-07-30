using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Models.Dialogs.Settings;
using GetStoreApp.Properties;
using Microsoft.Management.Deployment;
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

        private static GroupOptionsModel DefaultAppLanguage { get; set; }

        private static GroupOptionsModel CurrentAppLanguage { get; set; }

        private static ResourceContext DefaultResourceContext { get; set; } = new ResourceContext();

        private static ResourceContext CurrentResourceContext { get; set; } = new ResourceContext();

        private static ResourceMap ResourceMap { get; } = ResourceManager.Current.MainResourceMap;

        public static List<TypeModel> TypeList { get; } = new List<TypeModel>();

        public static List<ChannelModel> ChannelList { get; } = new List<ChannelModel>();

        public static List<StatusBarStateModel> StatusBarStateList { get; } = new List<StatusBarStateModel>();

        public static List<GroupOptionsModel> BackdropList { get; } = new List<GroupOptionsModel>();

        public static List<GroupOptionsModel> DownloadModeList { get; } = new List<GroupOptionsModel>();

        public static List<GroupOptionsModel> HistoryLiteNumList { get; } = new List<GroupOptionsModel>();

        public static List<GroupOptionsModel> HistoryJumpListNumList { get; } = new List<GroupOptionsModel>();

        public static List<GroupOptionsModel> InstallModeList { get; } = new List<GroupOptionsModel>();

        public static List<GroupOptionsModel> ThemeList { get; } = new List<GroupOptionsModel>();

        public static List<TraceCleanupModel> TraceCleanupList { get; } = new List<TraceCleanupModel>();

        public static List<GroupOptionsModel> WinGetInstallModeList { get; } = new List<GroupOptionsModel>();

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(GroupOptionsModel defaultAppLanguage, GroupOptionsModel currentAppLanguage)
        {
            DefaultAppLanguage = defaultAppLanguage;
            CurrentAppLanguage = currentAppLanguage;

            DefaultResourceContext.QualifierValues["Language"] = DefaultAppLanguage.SelectedValue;
            CurrentResourceContext.QualifierValues["Language"] = CurrentAppLanguage.SelectedValue;

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
            InitializeBackdropList();
            InitializeDownloadModeList();
            InitializeHistoryLiteNumList();
            InitializeHistoryJumpListNumList();
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
                StateInfoText = GetLocalized("Store/StatusInfoGetting"),
                StatePrRingActValue = true,
                StatePrRingVisValue = true
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Success,
                StateInfoText = GetLocalized("Store/StatusInfoSuccess"),
                StatePrRingActValue = false,
                StatePrRingVisValue = false
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Warning,
                StateInfoText = GetLocalized("Store/StatusInfoWarning"),
                StatePrRingActValue = false,
                StatePrRingVisValue = false
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Error,
                StateInfoText = GetLocalized("Store/StatusInfoError"),
                StatePrRingActValue = false,
                StatePrRingVisValue = false
            });
        }

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>
        private static void InitializeBackdropList()
        {
            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/BackdropDefault"),
                SelectedValue = "Default"
            });

            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/BackdropMica"),
                SelectedValue = "Mica"
            });

            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/BackdropMicaAlt"),
                SelectedValue = "MicaAlt"
            });

            BackdropList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/BackdropAcrylic"),
                SelectedValue = "Acrylic"
            });
        }

        /// <summary>
        /// 初始化应用下载方式列表
        /// </summary>
        private static void InitializeDownloadModeList()
        {
            DownloadModeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/DownloadInApp"),
                SelectedValue = "DownloadInApp"
            });
            DownloadModeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/DownloadWithBrowser"),
                SelectedValue = "DownloadWithBrowser"
            });
        }

        /// <summary>
        /// 初始化微软商店页面历史记录显示数量信息列表
        /// </summary>
        private static void InitializeHistoryLiteNumList()
        {
            HistoryLiteNumList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/HistoryLite3Items"),
                SelectedValue = "3"
            });
            HistoryLiteNumList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/HistoryLite5Items"),
                SelectedValue = "5"
            });
        }

        /// <summary>
        /// 初始化任务栏右键跳转列表历史记录显示数量信息列表
        /// </summary>
        private static void InitializeHistoryJumpListNumList()
        {
            HistoryJumpListNumList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/HistoryJumpList3Items"),
                SelectedValue = "3"
            });
            HistoryJumpListNumList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/HistoryJumpList5Items"),
                SelectedValue = "5"
            });
            HistoryJumpListNumList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/HistoryJumpList7Items"),
                SelectedValue = "7"
            });
            HistoryJumpListNumList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/HistoryJumpList9Items"),
                SelectedValue = "9"
            });
            HistoryJumpListNumList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/HistoryJumpListUnlimited"),
                SelectedValue = "Unlimited"
            });
        }

        /// <summary>
        /// 初始化安装模式信息列表
        /// </summary>
        private static void InitializeInstallModeList()
        {
            InstallModeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/AppInstall"),
                SelectedValue = "AppInstall"
            });
            InstallModeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/CodeInstall"),
                SelectedValue = "CodeInstall"
            });
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/ThemeDefault"),
                SelectedValue = Convert.ToString(ElementTheme.Default)
            });
            ThemeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/ThemeLight"),
                SelectedValue = Convert.ToString(ElementTheme.Light)
            });
            ThemeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/ThemeDark"),
                SelectedValue = Convert.ToString(ElementTheme.Dark)
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
                DisplayName = GetLocalized("Dialog/ActionCenter"),
                InternalName = CleanArgs.ActionCenter,
                CleanFailedText = GetLocalized("Dialog/ActionCenterError")
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
        /// 初始化 WinGet 程序包安装模式信息列表
        /// </summary>
        private static void InitializeWinGetInstallModeList()
        {
            WinGetInstallModeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/InteractiveInstall"),
                SelectedValue = Convert.ToString(PackageInstallMode.Interactive),
            });
            WinGetInstallModeList.Add(new GroupOptionsModel
            {
                DisplayMember = GetLocalized("Settings/SlientInstall"),
                SelectedValue = Convert.ToString(PackageInstallMode.Silent),
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
                    LogService.WriteLog(LogType.WARNING, string.Format("Get resource context with langauge {0} failed.", CurrentAppLanguage.SelectedValue), currentResourceException);
                    try
                    {
                        return ResourceMap.GetValue(resource, DefaultResourceContext).ValueAsString;
                    }
                    catch (Exception defaultResourceException)
                    {
                        LogService.WriteLog(LogType.WARNING, string.Format("Get resource context with langauge {0} failed.", DefaultAppLanguage.SelectedValue), defaultResourceException);
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
