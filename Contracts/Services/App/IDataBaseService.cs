using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.App
{
    public interface IDataBaseService
    {
        string DBName { get; }
        string HistoryTableName { get; }

        string DownloadTableName { get; }

        string DBpath { get; }

        Task InitializeDataBaseAsync();
    }
}
