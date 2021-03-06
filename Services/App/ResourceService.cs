using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;

namespace GetStoreApp.Services.App
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public class ResourceService : IResourceService
    {
        private string DefaultAppLanguage { get; set; }

        private string CurrentAppLanguage { get; set; }

        private ResourceContext DefaultResourceContext { get; set; } = new ResourceContext();

        private ResourceContext CurrentResourceContext { get; set; } = new ResourceContext();

        private ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");

        public List<ThemeModel> ThemeList { get; set; } = new List<ThemeModel>();

        public List<BackdropModel> BackdropList { get; set; } = new List<BackdropModel>();

        public List<HistoryItemValueModel> HistoryItemValueList { get; set; } = new List<HistoryItemValueModel>();

        public List<GetAppTypeModel> TypeList { get; set; } = new List<GetAppTypeModel>();

        public List<GetAppChannelModel> ChannelList { get; set; } = new List<GetAppChannelModel>();

        public List<StatusBarStateModel> StatusBarStateList { get; set; } = new List<StatusBarStateModel>();

        public async Task InitializeResourceAsync(string defaultAppLanguage, string currentAppLanguage)
        {
            DefaultAppLanguage = defaultAppLanguage;
            CurrentAppLanguage = currentAppLanguage;

            DefaultResourceContext.QualifierValues["Language"] = DefaultAppLanguage;
            CurrentResourceContext.QualifierValues["Language"] = CurrentAppLanguage;

            InitializeThemeList();
            InitializeBackdropList();
            InitializeHistoryItemValueList();
            InitializeTypeList();
            InitializeChannelList();
            InitializeStatusBarStateList();

            await Task.CompletedTask;
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

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>

        private void InitializeBackdropList()
        {
            ulong BuildNumber = InfoHelper.GetSystemVersion()["BuildNumber"];

            BackdropList.Add(new BackdropModel { DisplayName = GetLocalized("/Settings/BackdropDefault"), InternalName = "Default" });

            if (BuildNumber >= 22000)
            {
                BackdropList.Add(new BackdropModel { DisplayName = GetLocalized("/Settings/BackdropMica"), InternalName = "Mica" });
            }

            BackdropList.Add(new BackdropModel { DisplayName = GetLocalized("/Settings/BackdropArylic"), InternalName = "Acrylic" });
        }

        /// <summary>
        /// 初始化历史记录显示数量信息列表
        /// </summary>
        private void InitializeHistoryItemValueList()
        {
            HistoryItemValueList.Add(new HistoryItemValueModel
            {
                HistoryItemName = GetLocalized("/Settings/HistoryItemValueMin"),
                HistoryItemValue = 3
            });
            HistoryItemValueList.Add(new HistoryItemValueModel
            {
                HistoryItemName = GetLocalized("/Settings/HistoryItemValueMax"),
                HistoryItemValue = 5
            });
        }

        /// <summary>
        /// 初始化类型列表
        /// </summary>
        private void InitializeTypeList()
        {
            TypeList.Add(new GetAppTypeModel { DisplayName = GetLocalized("URL"), InternalName = "url" });
            TypeList.Add(new GetAppTypeModel { DisplayName = GetLocalized("ProductID"), InternalName = "ProductId" });
            TypeList.Add(new GetAppTypeModel { DisplayName = GetLocalized("PackageFamilyName"), InternalName = "PackageFamilyName" });
            TypeList.Add(new GetAppTypeModel { DisplayName = GetLocalized("CategoryID"), InternalName = "CategoryId" });
        }

        /// <summary>
        /// 初始化通道信息列表
        /// </summary>
        private void InitializeChannelList()
        {
            ChannelList.Add(new GetAppChannelModel { DisplayName = GetLocalized("Fast"), InternalName = "WIF" });
            ChannelList.Add(new GetAppChannelModel { DisplayName = GetLocalized("Slow"), InternalName = "WIS" });
            ChannelList.Add(new GetAppChannelModel { DisplayName = GetLocalized("RP"), InternalName = "RP" });
            ChannelList.Add(new GetAppChannelModel { DisplayName = GetLocalized("Retail"), InternalName = "Retail" });
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
                StatePrBarActValue = true,
                StatePrBarVisValue = true
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Success,
                StateInfoText = GetLocalized("/Home/StatusInfoSuccess"),
                StatePrBarActValue = false,
                StatePrBarVisValue = false
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Warning,
                StateInfoText = GetLocalized("/Home/StatusInfoWarning"),
                StatePrBarActValue = false,
                StatePrBarVisValue = false
            });
            StatusBarStateList.Add(new StatusBarStateModel
            {
                InfoBarSeverity = InfoBarSeverity.Error,
                StateInfoText = GetLocalized("/Home/StatusInfoError"),
                StatePrBarActValue = false,
                StatePrBarVisValue = false
            });
        }

        /// <summary>
        /// UI字符串本地化
        /// </summary>
        public string GetLocalized(string resource)
        {
            try
            {
                return resourceMap.GetValue(resource, CurrentResourceContext).ValueAsString;
            }
            catch (NullReferenceException)
            {
                try
                {
                    return resourceMap.GetValue(resource, DefaultResourceContext).ValueAsString;
                }
                catch (NullReferenceException)
                {
                    return resource;
                }
            }
        }
    }
}
