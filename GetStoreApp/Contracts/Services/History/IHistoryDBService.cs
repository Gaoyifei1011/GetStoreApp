using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.History
{
    public interface IHistoryDBService
    {
        Task AddHistoryDataAsync(HistoryModel history);

        Task<Tuple<List<HistoryModel>, bool, bool>> QueryAllHistoryDataAsync(bool timeSortOrder = false, string typeFilter = "None", string channelFilter = "None");

        Task<List<HistoryModel>> QueryHistoryDataAsync(int value);

        Task<bool> DeleteHistoryDataAsync(List<HistoryModel> selectedHistoryDataList);

        Task<bool> ClearHistoryDataAsync();
    }
}
