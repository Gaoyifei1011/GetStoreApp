using CommunityToolkit.Mvvm.ComponentModel;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Pages
{
    public class HomeViewModel : ObservableRecipient
    {
        public static string Title { get; } = LanguageService.GetResources("/Home/Title");

        public static string UseInstruction { get; } = LanguageService.GetResources("/Home/UseInstruction");

        public static string TitleDescription1 { get; } = LanguageService.GetResources("/Home/TitleDescription1");

        public static string TitleDescription2 { get; } = LanguageService.GetResources("/Home/TitleDescription2");

        private static string TypeURL { get; } = LanguageService.GetResources("/Home/TypeURL");

        private static string TypePID { get; } = LanguageService.GetResources("/Home/TypePID");

        private static string TypePFN { get; } = LanguageService.GetResources("/Home/TypePFN");

        private static string TypeCID { get; } = LanguageService.GetResources("/Home/TypeCID");

        private static string ChannelFast { get; } = LanguageService.GetResources("/Home/ChannelFast");

        private static string ChannelSlow { get; } = LanguageService.GetResources("/Home/ChannelSlow");

        private static string ChannelRP { get; } = LanguageService.GetResources("/Home/ChannelRP");

        private static string ChannelRetail { get; } = LanguageService.GetResources("/Home/ChannelRetail");

        public static string SampleTitle { get; } = LanguageService.GetResources("/Home/SampleTitle");

        public static string GetLinks { get; } = LanguageService.GetResources("/Home/GetLinks");

        public static string StatusInfoGetting { get; } = LanguageService.GetResources("/Home/StatusInfoGetting");

        public static string StatusInfoWelcome { get; } = LanguageService.GetResources("/Home/StatusInfoWelcome");

        public static string StatusInfoError { get; } = LanguageService.GetResources("/Home/StatusInfoError");

        public static string StatusInfoWarning { get; } = LanguageService.GetResources("/Home/StatusInfoWarning");

        public static string StatusInfoSuccess { get; } = LanguageService.GetResources("/Home/StatusInfoSuccess");

        public static string SearchResult { get; } = LanguageService.GetResources("/Home/SearchResult");

        public static string BatchOperation { get; } = LanguageService.GetResources("/Home/BatchOperation");

        public static string SearchResultEmpty { get; } = LanguageService.GetResources("/Home/SearchResultEmpty");

        public static string CategoryId { get; } = LanguageService.GetResources("/Home/CategoryId");

        public static string ResultCountInfo { get; } = LanguageService.GetResources("/Home/ResultCountInfo");

        public static string SerialNumber { get; } = LanguageService.GetResources("/Home/SerialNumber");

        public static string FileName { get; } = LanguageService.GetResources("/Home/FileName");

        public static string LinkExpireTime { get; } = LanguageService.GetResources("/Home/LinkExpireTime");

        public static string FileSHA1 { get; } = LanguageService.GetResources("/Home/FileSHA1");

        public static string FileSize { get; } = LanguageService.GetResources("/Home/FileSize");

        public static string CopyToolTip { get; } = LanguageService.GetResources("/Home/CopyToolTip");

        public static string ResultBottom { get; } = LanguageService.GetResources("/Home/ResultBottom");

        // 列表数据
        // 初始化HomeType列表
        public static IReadOnlyList<HomeTypeModel> TypeList { get; } = new List<HomeTypeModel>()
        {
            new HomeTypeModel(TypeURL,"url"),
            new HomeTypeModel(TypePID,"ProductId"),
            new HomeTypeModel(TypePFN,"PackageFamilyName"),
            new HomeTypeModel(TypeCID,"CategoryId")
        };

        // 初始化Channel列表
        public static IReadOnlyList<HomeChannelModel> ChannelList { get; } = new List<HomeChannelModel>()
        {
            new HomeChannelModel(ChannelFast,"WIF"),
            new HomeChannelModel(ChannelSlow,"WIS"),
            new HomeChannelModel(ChannelRP,"RP"),
            new HomeChannelModel(ChannelRetail,"Retail")
        };

        // 初始化SampleLink列表
        public static IReadOnlyList<string> SampleLinkList { get; } = new List<string>
        {
            "https://www.microsoft.com/store/productId/9NSWSBXN8K03",
            "9NKSQGP7F2NH",
            "Microsoft.WindowsStore_8wekyb3d8bbwe",
            "d58c3a5f-ca63-4435-842c-7814b5ff91b7"
        };
    }
}