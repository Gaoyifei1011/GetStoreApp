using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IHistoryItemValueService
    {
        HistoryItemValueModel HistoryItem { get; set; }

        List<HistoryItemValueModel> HistoryItemValueList { get; set; }

        Task InitializeHistoryItemValueAsync();

        Task SetHistoryItemValueAsync(HistoryItemValueModel historyItemValue);
    }
}
