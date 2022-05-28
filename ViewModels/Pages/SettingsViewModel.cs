using CommunityToolkit.Mvvm.ComponentModel;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System.Collections.Generic;
using Windows.ApplicationModel;

namespace GetStoreApp.ViewModels.Pages
{
    public class SettingsViewModel : ObservableRecipient
    {
        public static string Title { get; } = LanguageService.GetResources("/Settings/Title");

        public static string Appearance { get; } = LanguageService.GetResources("/Settings/Appearance");

        public static string Theme { get; } = LanguageService.GetResources("/Settings/Theme");

        public static string ThemeDescription { get; } = LanguageService.GetResources("/Settings/ThemeDescription");

        public static string ThemeLight { get; } = LanguageService.GetResources("/Settings/ThemeLight");

        public static string ThemeDark { get; } = LanguageService.GetResources("/Settings/ThemeDark");

        public static string ThemeDefault { get; } = LanguageService.GetResources("/Settings/ThemeDefault");

        public static string Language { get; } = LanguageService.GetResources("/Settings/Language");

        public static string LanguageDescription { get; } = LanguageService.GetResources("/Settings/LanguageDescription");

        public static string LanguageTip { get; } = LanguageService.GetResources("/Settings/LanguageTip");

        public static string LanguageTipDescription { get; } = LanguageService.GetResources("/Settings/LanguageTipDescription");

        public static string LaunchSettingsInstalledApps { get; } = LanguageService.GetResources("/Settings/LaunchSettingsInstalledApps");

        public static string General { get; } = LanguageService.GetResources("/Settings/General");

        public static string UseInstructionButton { get; } = LanguageService.GetResources("/Settings/UseInstructionButton");

        public static string UseInstructionButtonDescription { get; } = LanguageService.GetResources("/Settings/UseInstructionButtonDescription");

        public static string UseInstructionOffContent { get; } = LanguageService.GetResources("/Settings/UseInstructionOffContent");

        public static string UseInstructionOnContent { get; } = LanguageService.GetResources("/Settings/UseInstructionOnContent");

        public static string HistoryItem { get; } = LanguageService.GetResources("/Settings/HistoryItem");

        public static string HistoryItemDescription { get; } = LanguageService.GetResources("/Settings/HistoryItemDescription");

        public static string HistoryItemValueMin { get; } = LanguageService.GetResources("/Settings/HistoryItemValueMin");

        public static string HistoryItemValueMax { get; } = LanguageService.GetResources("/Settings/HistoryItemValueMax");

        public static string Region { get; } = LanguageService.GetResources("/Settings/Region");

        public static string RegionDescription { get; } = LanguageService.GetResources("/Settings/RegionDescription");

        public static string LinkFilter { get; } = LanguageService.GetResources("/Settings/LinkFilter");

        public static string LinkFilterDescription { get; } = LanguageService.GetResources("/Settings/LinkFilterDescription");

        public static string LinkFilterInstruction { get; } = LanguageService.GetResources("/Settings/LinkFilterInstruction");

        public static string StartsWithEInstruction { get; } = LanguageService.GetResources("/Settings/StartsWithEInstruction");

        public static string StartsWithEButton { get; } = LanguageService.GetResources("/Settings/StartsWithEButton");

        public static string BlockMapInstruction { get; } = LanguageService.GetResources("/Settings/BlockMapInstruction");

        public static string BlockMapButton { get; } = LanguageService.GetResources("/Settings/BlockMapButton");

        public static string StartsWithE { get; } = LanguageService.GetResources("/Settings/StartsWithE");

        public static string StartsWithEDescription { get; } = LanguageService.GetResources("/Settings/StartsWithEDescription");

        public static string StartsWithEOffContent { get; } = LanguageService.GetResources("/Settings/StartsWithEOffContent");

        public static string StartsWithEOnContent { get; } = LanguageService.GetResources("/Settings/StartsWithEOnContent");

        public static string BlockMap { get; } = LanguageService.GetResources("/Settings/BlockMap");

        public static string BlockMapDescription { get; } = LanguageService.GetResources("/Settings/BlockMapDescription");

        public static string BlockMapOffContent { get; } = LanguageService.GetResources("/Settings/BlockMapOffContent");

        public static string BlockMapOnContent { get; } = LanguageService.GetResources("/Settings/BlockMapOnContent");

        // 语言列表
        public static List<LanguageModel> LanguageList { get; } = LanguageService.LanguageList;

        public static List<HistoryItemValueModel> HistoryItemValueList { get; } = new List<HistoryItemValueModel>()
            {
                new HistoryItemValueModel(HistoryItemValueMin, 3),
                new HistoryItemValueModel(HistoryItemValueMax, 5)
            };

        // 区域列表
        public static List<GeographicalLocationModel> RegionList { get; } = RegionService.AppGlobalLocations;

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { SetProperty(ref _versionDescription, value); }
        }

        public SettingsViewModel()
        {
            VersionDescription = GetVersionDescription();
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var version = Package.Current.Id.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}