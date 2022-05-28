namespace GetStoreApp.Models
{
    public class HistoryItemValueModel
    {
        public string HistoryItemName { get; set; }
        public int HistoryItemValue { get; set; }

        public HistoryItemValueModel(string historyItemDisplayName, int historyItemValue)
        {
            HistoryItemName = historyItemDisplayName;
            HistoryItemValue = historyItemValue;
        }
    }
}