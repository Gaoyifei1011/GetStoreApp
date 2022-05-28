namespace GetStoreApp.Models
{
    public class LanguageModel
    {
        public string DisplayName { get; set; }
        public string CodeName { get; set; }

        public LanguageModel(string displayName, string codeName)
        {
            DisplayName = displayName;
            CodeName = codeName;
        }
    }
}