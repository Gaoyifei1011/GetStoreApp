using GetStoreApp.Models;
using GetStoreApp.Services.App;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GetStoreApp.Services.History
{
    public static class HistoryDataService
    {
        // 添加历史记录数据
        public static async Task AddHistoryDataAsync(HistoryModel history)
        {
            // 文件不存在，取消操作
            if (!File.Exists(DataBaseService.DBpath))
            {
                return;
            }

            bool CheckResult = await CheckDuplicatedDataAsync(history.HistoryKey);

            // 如果存在相同的行数据，只更新TimeStamp值，没有，添加数据
            if (CheckResult)
            {
                await UpdateDataAsync(history);
            }
            else
            {
                await AddDataAsync(history);
            }
        }

        /// <summary>
        /// 检查是否有相同键值的数据
        /// </summary>
        private static async Task<bool> CheckDuplicatedDataAsync(string historyKey)
        {
            bool IsExists = false;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand SearchCommand = new SqliteCommand();
                SearchCommand.Connection = db;

                SearchCommand.CommandText = string.Format("SELECT * FROM {0} WHERE HISTORYKEY == {1}", DataBaseService.HistoryTableName, historyKey);

                SqliteDataReader Query = SearchCommand.ExecuteReader();

                if (Query.Read()) IsExists = true;

                await db.CloseAsync();
            }

            return IsExists;
        }

        /// <summary>
        /// 没有重复的数据，直接添加
        /// </summary>
        private static async Task AddDataAsync(HistoryModel history)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand InsertCommand = new SqliteCommand();
                InsertCommand.Connection = db;

                InsertCommand.CommandText = string.Format("INSERT INTO {0} VALUES ({1},{2},{3},{4},{5})", DataBaseService.HistoryTableName, history.CurrentTimeStamp, history.HistoryKey, history.HistoryType.InternalName, history.HistoryChannel.InternalName, history.HistoryLink);

                InsertCommand.ExecuteReader();

                await db.CloseAsync();
            }
        }

        /// <summary>
        /// 存在重复的数据，只更新该记录的TimeStamp
        /// </summary>
        private static async Task UpdateDataAsync(HistoryModel history)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand UpdateCommand = new SqliteCommand();
                UpdateCommand.Connection = db;

                UpdateCommand.CommandText = string.Format("UPDATE {0} SET TIMESTAMP = {1} WHERE HISTORYKEY = {2}", DataBaseService.HistoryTableName, history.CurrentTimeStamp, history.HistoryKey);

                UpdateCommand.ExecuteReader();

                await db.CloseAsync();
            }
        }

        /// <summary>
        /// 获取所有的历史记录数据
        /// </summary>
        public static async Task<List<HistoryRawModel>> QueryAllHistoryDataAsync()
        {
            List<HistoryRawModel> HistoryRawList = new List<HistoryRawModel>();

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand SelectCommand = new SqliteCommand();
                SelectCommand.Connection = db;

                SelectCommand.CommandText = string.Format("SELECT * from {0} ORDER BY TimeStamp Desc", DataBaseService.HistoryTableName);

                SqliteDataReader Query = SelectCommand.ExecuteReader();

                while (Query.Read())
                {
                    HistoryRawModel historyRawModel = new HistoryRawModel
                    {
                        CurrentTimeStamp = Query.GetInt64(0),
                        HistoryKey = Query.GetString(1),
                        HistoryType = Query.GetString(2),
                        HistoryChannel = Query.GetString(3),
                        HistoryLink = Query.GetString(4)
                    };

                    HistoryRawList.Add(historyRawModel);
                }

                await db.CloseAsync();
            }

            return HistoryRawList;
        }

        /// <summary>
        /// 获取一定数量的历史记录数据
        /// </summary>
        public static async Task<List<HistoryRawModel>> QueryHistoryDataAsync(int value)
        {
            List<HistoryRawModel> HistoryRawList = new List<HistoryRawModel>();

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand SelectCommand = new SqliteCommand();
                SelectCommand.Connection = db;

                SelectCommand.CommandText = string.Format("SELECT * from {0} LIMIT {1}", DataBaseService.HistoryTableName, value);

                SqliteDataReader Query = SelectCommand.ExecuteReader();

                while (Query.Read())
                {
                    HistoryRawModel historyRawModel = new HistoryRawModel
                    {
                        CurrentTimeStamp = Query.GetInt64(0),
                        HistoryKey = Query.GetString(1),
                        HistoryType = Query.GetString(2),
                        HistoryChannel = Query.GetString(3),
                        HistoryLink = Query.GetString(4)
                    };

                    HistoryRawList.Add(historyRawModel);
                }

                await db.CloseAsync();
            }

            return HistoryRawList;
        }

        /// <summary>
        /// 删除历史记录数据
        /// </summary>
        public static async Task DeleteHistoryDataAsync(string historyKey)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand DeleteCommand = new SqliteCommand();
                DeleteCommand.Connection = db;

                DeleteCommand.CommandText = string.Format("DELETE FROM {0} WHERE HISTORYKEY = {1}",DataBaseService.HistoryTableName,historyKey);

                DeleteCommand.ExecuteReader();

                await db.CloseAsync();
            }
        }

        /// <summary>
        /// 清空历史记录数据
        /// </summary>
        public static async Task ClearHistoryDataAsync()
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand DeleteCommand = new SqliteCommand();
                DeleteCommand.Connection = db;

                // 删除表中记录的同时修改他的自增列
                DeleteCommand.CommandText = string.Format("DELETE FROM {0};Update sqlite_sequence SET seq=0 WHERE name = '{0}'", DataBaseService.HistoryTableName);

                DeleteCommand.ExecuteReader();

                await db.CloseAsync();
            }
        }
    }
}
