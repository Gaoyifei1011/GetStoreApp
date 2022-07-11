using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IHistoryItemValueService
    {
        string HistoryItemValue { get; set; }

        List<HistoryItemValueModel> HistoryItemValueList { get; set; }

        Task InitializeHistoryItemValueAsync();

        Task SetHistoryItemValueAsync(string historyItemValue);
    }
}
