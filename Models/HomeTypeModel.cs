namespace GetStoreApp.Models
{
    public class HomeTypeModel
    {
        public string DisplayName { get; set; }
        public string InternalName { get; set; }

        public HomeTypeModel(string displayName, string internalName)
        {
            DisplayName = displayName;
            InternalName = internalName;
        }
    }
}