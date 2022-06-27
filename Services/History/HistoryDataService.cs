using GetStoreApp.Models;
using GetStoreApp.Services.App;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace GetStoreApp.Services.History
{
    public static class HistoryDataService
    {
        /// <summary>
        /// 判断历史记录表是否为空
        /// </summary>
        private static async Task<bool> IsHistoryTableEmptyAsync()
        {
            int HistoryTableCount = 0;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand CountCommand = new SqliteCommand();
                CountCommand.Connection = db;

                CountCommand.CommandText = string.Format("SELECT COUNT(*) FROM {0}", DataBaseService.HistoryTableName);

                SqliteDataReader Query = await CountCommand.ExecuteReaderAsync();

                if (await Query.ReadAsync()) HistoryTableCount = Query.GetInt32(0);

                await db.CloseAsync();
            }

            return Convert.ToBoolean(HistoryTableCount == 0);
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

                SearchCommand.CommandText = string.Format("SELECT * FROM {0} WHERE HISTORYKEY LIKE '{1}'", DataBaseService.HistoryTableName, historyKey);

                SqliteDataReader Query = await SearchCommand.ExecuteReaderAsync();

                if (await Query.ReadAsync()) IsExists = true;

                await db.CloseAsync();
            }

            return IsExists;
        }

        /// <summary>
        /// 添加历史记录数据
        /// </summary>
        public static async void AddHistoryDataAsync(HistoryModel history)
        {
            // 文件不存在，取消操作
            if (!File.Exists(DataBaseService.DBpath)) return;

            bool CheckResult = await CheckDuplicatedDataAsync(history.HistoryKey);

            // 如果存在相同的行数据，只更新TimeStamp值，没有，添加数据
            if (CheckResult) await UpdateDataAsync(history);
            else await AddDataAsync(history);
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

                InsertCommand.CommandText = string.Format("INSERT INTO {0} VALUES ({1},'{2}','{3}','{4}','{5}')", DataBaseService.HistoryTableName, history.CurrentTimeStamp, history.HistoryKey, history.HistoryType, history.HistoryChannel, history.HistoryLink);

                await InsertCommand.ExecuteNonQueryAsync();

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

                UpdateCommand.CommandText = string.Format("UPDATE {0} SET TIMESTAMP = {1} WHERE HISTORYKEY = '{2}'", DataBaseService.HistoryTableName, history.CurrentTimeStamp, history.HistoryKey);

                await UpdateCommand.ExecuteNonQueryAsync();

                await db.CloseAsync();
            }
        }

        /// <summary>
        /// 获取所有的历史记录数据
        /// </summary>
        /// <param name="timeSortOrder">时间戳顺序，True为递增排序，False为递减排序</param>
        /// <param name="typeFilter"></param>
        /// <param name="channelFilter"></param>
        /// <returns>返回历史记录列表，并在原函数修改对应的参数值</returns>
        public static async Task<Tuple<List<HistoryModel>, bool, bool>> QueryAllHistoryDataAsync(bool timeSortOrder = false, string typeFilter = "None", string channelFilter = "None")
        {
            List<HistoryModel> HistoryRawList = new List<HistoryModel>();

            //调用之前先判断历史记录表是否为空
            bool IsHistoryEmpty = await IsHistoryTableEmptyAsync();

            if (IsHistoryEmpty) return Tuple.Create(HistoryRawList, IsHistoryEmpty, true);

            // 从数据库中获取数据
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand SelectCommand = new SqliteCommand();
                SelectCommand.Connection = db;

                SelectCommand.CommandText = GenerateSelectSQL(timeSortOrder, typeFilter, channelFilter);

                SqliteDataReader Query = await SelectCommand.ExecuteReaderAsync();

                while (await Query.ReadAsync())
                {
                    HistoryModel historyRawModel = new HistoryModel
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

            // 判断经过过滤后历史记录表是否为空
            bool IsHistoryEmptyAfterFilter = HistoryRawList.Count == 0;

            return Tuple.Create(HistoryRawList, IsHistoryEmpty, IsHistoryEmptyAfterFilter);
        }

        /// <summary>
        /// 根据选定的条件生成相应的SQL语句
        /// </summary>
        private static string GenerateSelectSQL(bool timeSortOrder, string typeFilter, string channelFilter)
        {
            string SQL = string.Empty;

            if (typeFilter == "None" && channelFilter == "None")
                SQL = string.Format("SELECT * FROM {0} ORDER BY TIMESTAMP", DataBaseService.HistoryTableName);
            else if (typeFilter != "None" && channelFilter != "None")
                SQL = string.Format("SELECT * FROM {0} WHERE TYPE = '{1}' AND CHANNEL = '{2}' ORDER BY TIMESTAMP", DataBaseService.HistoryTableName, typeFilter, channelFilter);
            else if (typeFilter != "None")
                SQL = string.Format("SELECT * FROM {0} WHERE TYPE = '{1}' ORDER BY TIMESTAMP", DataBaseService.HistoryTableName, typeFilter, channelFilter);
            else if (channelFilter != "None")
                SQL = string.Format("SELECT * FROM {0} WHERE CHANNEL = '{1}' ORDER BY TIMESTAMP", DataBaseService.HistoryTableName, channelFilter, channelFilter);

            if (!timeSortOrder)
                SQL = string.Format("{0} {1}", SQL, "DESC");

            return SQL;
        }

        /// <summary>
        /// 获取一定数量的历史记录数据
        /// </summary>
        public static async Task<List<HistoryModel>> QueryHistoryDataAsync(int value)
        {
            List<HistoryModel> HistoryRawList = new List<HistoryModel>();

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand SelectCommand = new SqliteCommand();
                SelectCommand.Connection = db;

                SelectCommand.CommandText = string.Format("SELECT * FROM {0} ORDER BY TIMESTAMP DESC LIMIT {1}", DataBaseService.HistoryTableName, value);

                SqliteDataReader Query = await SelectCommand.ExecuteReaderAsync();

                while (await Query.ReadAsync())
                {
                    HistoryModel historyRawModel = new HistoryModel
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
        public static async Task DeleteHistoryDataAsync(List<HistoryModel> selectedHistoryDataList)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                using (SqliteTransaction transaction = db.BeginTransaction())
                {
                    using (SqliteCommand DeleteCommand = new SqliteCommand())
                    {
                        DeleteCommand.Connection = db;
                        DeleteCommand.Transaction = transaction;

                        try
                        {
                            foreach (var item in selectedHistoryDataList)
                            {
                                DeleteCommand.CommandText = string.Format("DELETE FROM {0} WHERE HISTORYKEY = '{1}'", DataBaseService.HistoryTableName, item.HistoryKey);
                                await DeleteCommand.ExecuteNonQueryAsync();
                            }
                            await transaction.CommitAsync();
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                        }
                    }
                }
                await db.CloseAsync();
            }
        }

        /// <summary>
        /// 清空历史记录数据
        /// </summary>
        public static async Task<bool> ClearHistoryDataAsync()
        {
            bool result = false;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();
                Debug.WriteLine("01");
                using (SqliteTransaction transaction = db.BeginTransaction())
                {
                    using(SqliteCommand ClearCommand = new SqliteCommand())
                    {
                        ClearCommand.Connection = db;
                        ClearCommand.Transaction = transaction;
                        Debug.WriteLine("02");
                        try
                        {
                            ClearCommand.CommandText = string.Format("DELETE FROM {0}",DataBaseService.HistoryTableName);
                            await ClearCommand.ExecuteNonQueryAsync();
                            Debug.WriteLine("03");
                            await transaction.CommitAsync();
                            result = true;
                            Debug.WriteLine("06");
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            Debug.WriteLine("07");
                        }
                    }
                }
                await db.CloseAsync();
                Debug.WriteLine("08");
                return result;
            }
        }
    }
}
