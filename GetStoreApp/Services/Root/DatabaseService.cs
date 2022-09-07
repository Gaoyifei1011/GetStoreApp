using GetStoreApp.Contracts.Services.Root;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 数据库基础服务
    /// </summary>
    public class DataBaseService : IDataBaseService
    {
        public string DBName => "GetStoreApp.db";

        public string HistoryTableName => "HISTORY";

        public string DownloadTableName => "DOWNLOAD";

        public string DBpath => Path.Combine(ApplicationData.Current.LocalFolder.Path, "GetStoreApp.db");

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

                string CreateTimeStamp = "TIMESTAMP INTEGER NOT NULL UNIQUE";

                string HistoryKey = "HISTORYKEY CHAR(32) NOT NULL UNIQUE";

                string HistoryType = "TYPE VARCHAR(20) NOT NULL";

                string HistoryChannel = "CHANNEL VARCHAR(6) NOT NULL";

                string HistoryLink = "LINK TEXT";

                string HistoryTableCommand = string.Format("{0} {1} ({2},{3},{4},{5},{6})",
                    CreateTableString,
                    HistoryTableName,
                    CreateTimeStamp,
                    HistoryKey,
                    HistoryType,
                    HistoryChannel,
                    HistoryLink
                    );

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
            // 文件不存在，取消操作
            if (!File.Exists(DBpath))
            {
                return;
            }

            // 创建下载记录表
            using (SqliteConnection db = new SqliteConnection($"Filename={DBpath}"))
            {
                await db.OpenAsync();

                string CreateTableString = "CREATE TABLE IF NOT EXISTS";

                string DownloadKey = "DOWNLOADKEY CHAR(32) NOT NULL UNIQUE";

                string FileName = "FILENAME VARCHAR(100) NOT NULL";

                string FileLink = "FILELINK VARCHAR(300) NOT NULL";

                string FilePath = "FILEPATH VARCHAR(500) NOT NULL UNIQUE";

                string FileSHA1 = "FILESHA1 VARCHAR(32) NOT NULL UNIQUE";

                string FileSize = "FILESIZE VARCHAR(10) NOT NULL";

                string DownloadFlag = "DOWNLOADFLAG INTEGER NOT NULL";

                string DownloadTableCommand = string.Format("{0} {1} ({2},{3},{4},{5},{6},{7},{8})",
                    CreateTableString,
                    DownloadTableName,
                    DownloadKey,
                    FileName,
                    FileLink,
                    FilePath,
                    FileSHA1,
                    FileSize,
                    DownloadFlag
                    );

                SqliteCommand CreateTable = new SqliteCommand(DownloadTableCommand, db);

                CreateTable.ExecuteReader();

                await db.CloseAsync();
            }
        }
    }
}
