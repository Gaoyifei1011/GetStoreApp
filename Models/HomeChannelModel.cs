namespace GetStoreApp.Models
{
    public class HomeChannelModel
    {
        public string DisplayName { get; set; }
        public string InternalName { get; set; }

        public HomeChannelModel(string displayName, string internalName)
        {
            DisplayName = displayName;
            InternalName = internalName;
        }
    }
}