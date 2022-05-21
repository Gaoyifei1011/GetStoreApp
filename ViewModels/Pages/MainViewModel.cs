using GetStoreApp.Core.Models;
using GetStoreApp.Services.Settings;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Pages
{
    public class MainViewModel : ObservableObject
    {
        private static readonly string Main_Type_URL = LanguageSettings.GetResources("Main_Type_URL");
        private static readonly string Main_Type_PID = LanguageSettings.GetResources("Main_Type_PID");
        private static readonly string Main_Type_PFN = LanguageSettings.GetResources("Main_Type_PFN");
        private static readonly string Main_Type_CID = LanguageSettings.GetResources("Main_Type_CID");

        private static readonly string Main_Channel_Fast = LanguageSettings.GetResources("Main_Channel_Fast");
        private static readonly string Main_Channel_Slow = LanguageSettings.GetResources("Main_Channel_Slow");
        private static readonly string Main_Channel_RP = LanguageSettings.GetResources("Main_Channel_RP");
        private static readonly string Main_Channel_Retail = LanguageSettings.GetResources("Main_Channel_Retail");

        // 列表数据
        // 初始化MainType列表
        public static IReadOnlyList<MainTypeModel> MainTypeList = new List<MainTypeModel>()
            {
                new MainTypeModel(DisplayName:Main_Type_URL,InternalName: "url"),
                new MainTypeModel(DisplayName: Main_Type_PID, InternalName: "ProductId"),
                new MainTypeModel(DisplayName: Main_Type_PFN, InternalName: "PackageFamilyName"),
                new MainTypeModel(DisplayName: Main_Type_CID, InternalName: "CategoryId")
            }.AsReadOnly();

        // 初始化MainChannel列表
        public static IReadOnlyList<MainChannelModel> MainChannelList = new List<MainChannelModel>()
            {
                new MainChannelModel(DisplayName:Main_Channel_Fast, InternalName:"WIF"),
                new MainChannelModel(DisplayName:Main_Channel_Slow, InternalName:"WIS"),
                new MainChannelModel(DisplayName:Main_Channel_RP, InternalName:"RP"),
                new MainChannelModel(DisplayName:Main_Channel_Retail, InternalName:"Retail")
            }.AsReadOnly();

        // RequestControl
        public static string GetLinks = LanguageSettings.GetResources("/MainResources/GetLinks");

        // ResultControl
        public static string SearchResult = LanguageSettings.GetResources("/MainResources/SearchResult");

        public static string BatchOperation = LanguageSettings.GetResources("/MainResources/BatchOperation");

        public static string SearchResultEmpty = LanguageSettings.GetResources("/MainResources/SearchResultEmpty");
    }
}
