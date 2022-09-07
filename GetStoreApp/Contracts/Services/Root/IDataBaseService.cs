using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Root
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
