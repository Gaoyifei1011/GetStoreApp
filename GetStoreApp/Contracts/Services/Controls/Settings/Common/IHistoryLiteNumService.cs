using GetStoreApp.Models.Controls.Settings.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Controls.Settings.Common
{
    public interface IHistoryLiteNumService
    {
        HistoryLiteNumModel HistoryLiteNum { get; set; }

        List<HistoryLiteNumModel> HistoryLiteNumList { get; set; }

        Task InitializeHistoryLiteNumAsync();

        Task SetHistoryLiteNumAsync(HistoryLiteNumModel historyLiteNum);
    }
}
