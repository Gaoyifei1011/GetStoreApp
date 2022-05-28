namespace GetStoreApp.Models
{
    public class HistoryDisplayDataModel
    {
        public string TypeName { get; set; }
        public string TypeInternalName { get; set; }
        public string ChannelName { get; set; }
        public string ChannelInternalName { get; set; }
        public string LinkName { get; set; }

        public HistoryDisplayDataModel(string typeName, string typeInternalName, string channelName, string channelInternalName, string linkName)
        {
            TypeName = typeName;
            TypeInternalName = typeInternalName;
            ChannelName = channelName;
            ChannelInternalName = channelInternalName;
            LinkName = linkName;
        }
    }
}