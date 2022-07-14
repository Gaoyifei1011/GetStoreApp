using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IHistoryItemValueService
    {
        int HistoryItemValue { get; set; }

        List<HistoryItemValueModel> HistoryItemValueList { get; set; }

        Task InitializeHistoryItemValueAsync();

        Task SetHistoryItemValueAsync(int historyItemValue);
    }
}
