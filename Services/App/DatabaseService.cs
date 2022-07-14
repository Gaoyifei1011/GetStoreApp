using GetStoreApp.Contracts.Services.App;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.App
{
    public class DataBaseService : IDataBaseService
    {
        public string DBName { get; } = "GetStoreApp.db";

        public string HistoryTableName { get; } = "HISTORY";

        public string DownloadTableName { get; } = "Download";

        public string DBpath { get; } = Path.Combine(ApplicationData.Current.LocalFolder.Path, "GetStoreApp.db");

        /// <summary>
        /// 历史记录表不存在时，自动创建历史记录表
        /// </summary>
        public async Task InitializeDataBaseAsync()
        {
            // 创建数据库文件
            await ApplicationData.Current.LocalFolder.CreateFileAsync(DBName, CreationCollisionOption.OpenIfExists);

            // 初始化历史记录表
            await InitializeHistoryTableAsync();

            // 初始化下载记录表
            await InitializeDownloadTableAsync();
        }

        /// <summary>
        /// 初始化历史记录表
        /// </summary>
        private async Task InitializeHistoryTableAsync()
        {
            // 文件不存在，取消操作
            if (!File.Exists(DBpath))
            {
                return;
            }

            // 创建历史记录表
            using (SqliteConnection db = new SqliteConnection($"Filename={DBpath}"))
            {
                await db.OpenAsync();

                string CreateTableString = "CREATE TABLE IF NOT EXISTS";

                string CurrentTimeStampKey = "TIMESTAMP INTEGER NOT NULL UNIQUE";

                string HistoryKey = "HISTORYKEY CHAR(32) NOT NULL UNIQUE";

                string HistoryType = "TYPE VARCHAR(20) NOT NULL";

                string HistoryChannel = "CHANNEL VARCHAR(6) NOT NULL";

                string HistoryLink = "LINK TEXT";

                string HistoryTableCommand = string.Format("{0} {1} ({2},{3},{4},{5},{6})", CreateTableString, HistoryTableName, CurrentTimeStampKey, HistoryKey, HistoryType, HistoryChannel, HistoryLink);

                SqliteCommand CreateTable = new SqliteCommand(HistoryTableCommand, db);

                CreateTable.ExecuteReader();

                await db.CloseAsync();
            }
        }

        /// <summary>
        /// 下载记录表不存在时，初始化下载记录表
        /// </summary>
        private async Task InitializeDownloadTableAsync()
        {
            await Task.CompletedTask;
        }
    }
}
