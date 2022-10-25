using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Models.Dialogs.CommonDialogs.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public class ResourceService : IResourceService
    {
        private LanguageModel DefaultAppLanguage { get; set; }

        private LanguageModel CurrentAppLanguage { get; set; }

        private ResourceContext DefaultResourceContext { get; set; } = new ResourceContext();

        private ResourceContext CurrentResourceContext { get; set; } = new ResourceContext();

        private ResourceMap ResourceMap { get; } = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");

        public List<TypeModel> TypeList { get; } = new List<TypeModel>();

        public List<ChannelModel> ChannelList { get; } = new List<ChannelModel>();

        public List<StatusBarStateModel> StatusBarStateList { get; } = new List<StatusBarStateModel>();

        public List<AppExitModel> AppExitList { get; } = new List<AppExitModel>();

        public List<BackdropModel> BackdropList { get; } = new List<BackdropModel>();

        public List<DownloadModeModel> DownloadModeList { get; } = new List<DownloadModeModel>();

        public List<HistoryLiteNumModel> HistoryLiteNumList { get; } = new List<HistoryLiteNumModel>();

        public List<InstallModeModel> InstallModeList { get; } = new List<InstallModeModel>();

        public List<ThemeModel> ThemeList { get; } = new List<ThemeModel>();

        public List<TraceCleanupModel> TraceCleanupList { get; } = new List<TraceCleanupModel>();

        public async Task InitializeResourceAsync(LanguageModel defaultAppLanguage, LanguageModel currentAppLanguage)
        {
            DefaultAppLanguage = defaultAppLanguage;
            CurrentAppLanguage = currentAppLanguage;

            DefaultResourceContext.QualifierValues["Language"] = DefaultAppLanguage.InternalName;
            CurrentResourceContext.QualifierValues["Language"] = CurrentAppLanguage.InternalName;

            InitializeTypeList();
            InitializeChannelList();
            InitializeStatusBarStateList();
            InitializeAppExitList();
            InitializeBackdropList();
            InitializeDownloadModeList();
            InitializeHistoryLiteNumList();
            InitializeInstallModeList();
            InitializeThemeList();
            InitializeTraceCleanupList();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 初始化类型列表
        /// </summary>
        private void InitializeTypeList()
        {
            TypeList.Add(new TypeModel
            {
                DisplayName = GetLocalized("URL"),
                InternalName = "url",
                ShortName = "url"
            });

            TypeList.Add(new TypeModel
            {
                DisplayName = GetLocalized("ProductID"),
                InternalName = "ProductId",
                ShortName = "pid"
            });

            TypeList.Add(new TypeModel
            {
                DisplayName = GetLocalized("PackageFamilyName"),
                InternalName = "PackageFamilyName",
                ShortName = "pfn"
            });

            TypeList.Add(new TypeModel
            {
                DisplayName = GetLocalized("CategoryID"),
                InternalName = "CategoryId",
                ShortName = "cid"
            });
        }

        /// <summary>
        /// 初始化通道信息列表
        /// </summary>
        private void InitializeChannelList()
        {
            ChannelList.Add(new ChannelModel
            {
                DisplayName = GetLocalized("Fast"),
                InternalName = "WIF",
                ShortName = "wif"
            });

            ChannelList.Add(new ChannelModel
            {
                DisplayName = GetLocalized("Slow"),
                InternalName = "WIS",
                ShortName = "wis"
            });

            ChannelList.Add(new ChannelModel
            {
                DisplayName = GetLocalized("RP"),
                InternalName = "RP",
                ShortName = "rp"
            });

            ChannelList.Add(new ChannelModel
            {
                DisplayName = GetLocalized("Retail"),
                InternalName = "Retail",
                ShortName = "rt"
            });
        }

        /// <summary>
        /// 初始化状态栏信息列表
        /// </summary>
        private void InitializeStatusBarStateList()
        {
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Informational,
                StateInfoText = GetLocalized("/Home/StatusInfoGetting"),
                StatePrRingActValue = true,
                StatePrRingVisValue = true
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Success,
                StateInfoText = GetLocalized("/Home/StatusInfoSuccess"),
                StatePrRingActValue = false,
                StatePrRingVisValue = false
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Warning,
                StateInfoText = GetLocalized("/Home/StatusInfoWarning"),
                StatePrRingActValue = false,
                StatePrRingVisValue = false
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Error,
                StateInfoText = GetLocalized("/Home/StatusInfoError"),
                StatePrRingActValue = false,
                StatePrRingVisValue = false
            });
        }

        /// <summary>
        /// 初始化应用退出内容列表
        /// </summary>
        private void InitializeAppExitList()
        {
            AppExitList.Add(new AppExitModel
            {
                DisplayName = GetLocalized("/Settings/HideToTray"),
                InternalName = "HideToTray"
            });
            AppExitList.Add(new AppExitModel
            {
                DisplayName = GetLocalized("/Settings/CloseApp"),
                InternalName = "CloseApp"
            });
        }

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>
        private void InitializeBackdropList()
        {
            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("/Settings/BackdropDefault"),
                InternalName = "Default"
            });

            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("/Settings/BackdropMica"),
                InternalName = "Mica"
            });

            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("/Settings/BackdropMicaAlt"),
                InternalName = "MicaAlt"
            });

            BackdropList.Add(new BackdropModel
            {
                DisplayName = GetLocalized("/Settings/BackdropAcrylic"),
                InternalName = "Acrylic"
            });
        }

        /// <summary>
        /// 初始化应用下载方式列表
        /// </summary>
        private void InitializeDownloadModeList()
        {
            DownloadModeList.Add(new DownloadModeModel
            {
                DisplayName = GetLocalized("/Settings/DownloadInApp"),
                InternalName = "DownloadInApp"
            });
            DownloadModeList.Add(new DownloadModeModel
            {
                DisplayName = GetLocalized("/Settings/DownloadWithBrowser"),
                InternalName = "DownloadWithBrowser"
            });
        }

        /// <summary>
        /// 初始化历史记录显示数量信息列表
        /// </summary>
        private void InitializeHistoryLiteNumList()
        {
            HistoryLiteNumList.Add(new HistoryLiteNumModel
            {
                HistoryLiteNumName = GetLocalized("/Settings/HistoryLiteNumMin"),
                HistoryLiteNumValue = 3
            });
            HistoryLiteNumList.Add(new HistoryLiteNumModel
            {
                HistoryLiteNumName = GetLocalized("/Settings/HistoryLiteNumMax"),
                HistoryLiteNumValue = 5
            });
        }

        /// <summary>
        /// 初始化安装模式信息列表
        /// </summary>
        private void InitializeInstallModeList()
        {
            InstallModeList.Add(new InstallModeModel
            {
                DisplayName = GetLocalized("/Settings/AppInstall"),
                InternalName = "AppInstall"
            });
            InstallModeList.Add(new InstallModeModel
            {
                DisplayName = GetLocalized("/Settings/CodeInstall"),
                InternalName = "CodeInstall"
            });
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private void InitializeThemeList()
        {
            ThemeList.Add(new ThemeModel
            {
                DisplayName = GetLocalized("/Settings/ThemeDefault"),
                InternalName = Convert.ToString(ElementTheme.Default)
            });
            ThemeList.Add(new ThemeModel
            {
                DisplayName = GetLocalized("/Settings/ThemeLight"),
                InternalName = Convert.ToString(ElementTheme.Light)
            });
            ThemeList.Add(new ThemeModel
            {
                DisplayName = GetLocalized("/Settings/ThemeDark"),
                InternalName = Convert.ToString(ElementTheme.Dark)
            });
        }

        private void InitializeTraceCleanupList()
        {
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("/Dialog/HistoryRecord"),
                InternalName = CleanArgs.History,
                CleanFailedText = GetLocalized("/Dialog/HistoryCleanError")
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("/Dialog/DownloadRecord"),
                InternalName = CleanArgs.Download,
                CleanFailedText = GetLocalized("/Dialog/DownloadCleanError")
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("/Dialog/LocalFile"),
                InternalName = CleanArgs.LocalFile,
                CleanFailedText = GetLocalized("/Dialog/LocalFileCleanError")
            });
            TraceCleanupList.Add(new TraceCleanupModel
            {
                DisplayName = GetLocalized("/Dialog/WebCache"),
                InternalName = CleanArgs.WebCache,
                CleanFailedText = GetLocalized("/Dialog/WebCacheCleanError")
            });
        }

        /// <summary>
        /// UI字符串本地化
        /// </summary>
        public string GetLocalized(string resource)
        {
            try
            {
                return ResourceMap.GetValue(resource, CurrentResourceContext).ValueAsString;
            }
            catch (NullReferenceException)
            {
                try
                {
                    return ResourceMap.GetValue(resource, DefaultResourceContext).ValueAsString;
                }
                catch (NullReferenceException)
                {
                    return resource;
                }
            }
        }
    }
}
