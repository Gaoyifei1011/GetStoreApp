using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Services.Shell
{
    public static class RequestService
    {
        public static List<string> TypeList { get; } = new List<string>()
        {
            "url",
            "ProductId",
            "PackageFamilyName",
            "CategoryId"
        };

        public static List<string> ChannelList { get; } = new List<string>()
        {
            "WIF",
            "WIS",
            "RP",
            "Retail"
        };

        public static string SelectedType { get; set; }

        public static string SelectedChannel { get; set; }

        public static void InitializeWithoutQueryData()
        {
            SelectedType = Convert.ToInt32(ConsoleLaunchService.LaunchArgs["TypeName"]) == -1 ? TypeList[0] : TypeList[Convert.ToInt32(ConsoleLaunchService.LaunchArgs["TypeName"])];

            SelectedChannel = Convert.ToInt32(ConsoleLaunchService.LaunchArgs["ChannelName"]) == -1 ? ChannelList[0] : ChannelList[Convert.ToInt32(ConsoleLaunchService.LaunchArgs["ChannelName"])];
        }

        public static void InitializeQueryData(int typeIndex, int channelIndex, string link)
        {
            SelectedType = TypeList[typeIndex - 1];
            SelectedChannel = ChannelList[channelIndex - 1];
        }
    }
}
