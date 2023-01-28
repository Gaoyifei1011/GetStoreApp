using GetStoreApp.Models.Controls.History;
using GetStoreApp.Services.Root;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.History
{
    /// <summary>
    /// 历史记录数据库存储服务
    /// </summary>
    public static class HistoryDBService
    {
        /// <summary>
        /// 判断历史记录表是否为空
        /// </summary>
        private static async Task<bool> IsTableEmptyAsync()
        {
            int HistoryTableCount = 0;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBFile.Path}"))
            {
                await db.OpenAsync();

                SqliteCommand CountCommand = new SqliteCommand
                {
                    Connection = db,

                    CommandText = string.Format("SELECT COUNT(*) FROM {0}", DataBaseService.HistoryTableName)
                };

                SqliteDataReader Query = await CountCommand.ExecuteReaderAsync();

                if (await Query.ReadAsync())
                {
                    HistoryTableCount = Query.GetInt32(0);
                }

                await db.CloseAsync();
            }
            return Convert.ToBoolean(HistoryTableCount is 0);
        }

        /// <summary>
        /// 检查是否有相同键值的数据
        /// </summary>
        private static async Task<bool> CheckDuplicatedAsync(string historyKey)
        {
            bool IsExists = false;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBFile.Path}"))
            {
                await db.OpenAsync();

                SqliteCommand SearchCommand = new SqliteCommand
                {
                    Connection = db,

                    CommandText = string.Format("SELECT * FROM {0} WHERE HISTORYKEY LIKE '{1}'",
                        DataBaseService.HistoryTableName,
                        historyKey
                        )
                };

                SqliteDataReader Query = await SearchCommand.ExecuteReaderAsync();

                if (await Query.ReadAsync())
                {
                    IsExists = true;
                }

                await db.CloseAsync();
            }
            return IsExists;
        }

        /// <summary>
        /// 添加历史记录数据
        /// </summary>
        public static async Task AddAsync(HistoryModel historyItem)
        {
            bool CheckResult = await CheckDuplicatedAsync(historyItem.HistoryKey);

            // 如果存在相同的行数据，只更新TimeStamp值，没有，添加数据
            if (CheckResult)
            {
                await UpdateDataAsync(historyItem);
            }
            else
            {
                await AddDataAsync(historyItem);
            }
        }

        /// <summary>
        /// 没有重复的数据，直接添加
        /// </summary>
        private static async Task AddDataAsync(HistoryModel historyItem)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBFile.Path}"))
            {
                await db.OpenAsync();

                using (SqliteTransaction transaction = db.BeginTransaction())
                {
                    using (SqliteCommand InsertCommand = new SqliteCommand())
                    {
                        InsertCommand.Connection = db;
                        InsertCommand.Transaction = transaction;

                        try
                        {
                            InsertCommand.CommandText = string.Format("INSERT INTO {0} VALUES ({1},'{2}','{3}','{4}','{5}')",
                                DataBaseService.HistoryTableName,
                                historyItem.CreateTimeStamp,
                                historyItem.HistoryKey,
                                historyItem.HistoryType,
                                historyItem.HistoryChannel,
                                historyItem.HistoryLink
                                );

                            await InsertCommand.ExecuteNonQueryAsync();

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
        /// 存在重复的数据，只更新该记录的时间戳
        /// </summary>
        private static async Task UpdateDataAsync(HistoryModel historyItem)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBFile.Path}"))
            {
                await db.OpenAsync();

                using (SqliteTransaction transaction = db.BeginTransaction())
                {
                    using (SqliteCommand UpdateCommand = new SqliteCommand())
                    {
                        UpdateCommand.Connection = db;
                        UpdateCommand.Transaction = transaction;

                        try
                        {
                            UpdateCommand.CommandText = string.Format("UPDATE {0} SET TIMESTAMP = {1} WHERE HISTORYKEY = '{2}'",
                                DataBaseService.HistoryTableName,
                                historyItem.CreateTimeStamp,
                                historyItem.HistoryKey
                                );

                            await UpdateCommand.ExecuteNonQueryAsync();

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
        /// 获取所有的历史记录数据
        /// </summary>
        /// <param name="timeSortOrder">时间戳顺序，True为递增排序，False为递减排序</param>
        /// <param name="typeFilter"></param>
        /// <param name="channelFilter"></param>
        /// <returns>返回历史记录列表</returns>
        public static async Task<(List<HistoryModel>, bool, bool)> QueryAllAsync(bool timeSortOrder = false, string typeFilter = "None", string channelFilter = "None")
        {
            List<HistoryModel> HistoryRawList = new List<HistoryModel>();

            //调用之前先判断历史记录表是否为空
            bool IsHistoryEmpty = await IsTableEmptyAsync();

            if (IsHistoryEmpty)
            {
                return (HistoryRawList, IsHistoryEmpty, true);
            }

            // 从数据库中获取数据
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBFile.Path}"))
            {
                await db.OpenAsync();

                SqliteCommand SelectCommand = new SqliteCommand
                {
                    Connection = db,

                    CommandText = GenerateSelectSQL(timeSortOrder, typeFilter, channelFilter)
                };

                SqliteDataReader Query = await SelectCommand.ExecuteReaderAsync();

                while (await Query.ReadAsync())
                {
                    HistoryModel historyRawModel = new HistoryModel
                    {
                        CreateTimeStamp = Query.GetInt64(0),
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
            bool IsHistoryEmptyAfterFilter = HistoryRawList.Count is 0;

            return (HistoryRawList, IsHistoryEmpty, IsHistoryEmptyAfterFilter);
        }

        /// <summary>
        /// 根据选定的条件生成相应的SQL语句
        /// </summary>
        private static string GenerateSelectSQL(bool timeSortOrder, string typeFilter, string channelFilter)
        {
            string SQL = string.Empty;

            if (typeFilter is "None" && channelFilter is "None")
            {
                SQL = string.Format("SELECT * FROM {0} ORDER BY TIMESTAMP", DataBaseService.HistoryTableName);
            }
            else if (typeFilter is not "None" && channelFilter is not "None")
            {
                SQL = string.Format("SELECT * FROM {0} WHERE TYPE = '{1}' AND CHANNEL = '{2}' ORDER BY TIMESTAMP",
                    DataBaseService.HistoryTableName,
                    typeFilter,
                    channelFilter
                    );
            }
            else if (typeFilter is not "None")
            {
                SQL = string.Format("SELECT * FROM {0} WHERE TYPE = '{1}' ORDER BY TIMESTAMP",
                    DataBaseService.HistoryTableName,
                    typeFilter
                    );
            }
            else if (channelFilter is not "None")
            {
                SQL = string.Format("SELECT * FROM {0} WHERE CHANNEL = '{1}' ORDER BY TIMESTAMP",
                    DataBaseService.HistoryTableName,
                    channelFilter
                    );
            }

            if (!timeSortOrder)
            {
                SQL = string.Format("{0} {1}", SQL, "DESC");
            }

            return SQL;
        }

        /// <summary>
        /// 获取一定数量的历史记录数据
        /// </summary>
        public static async Task<List<HistoryModel>> QueryAsync(int value)
        {
            List<HistoryModel> HistoryRawList = new List<HistoryModel>();

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBFile.Path}"))
            {
                await db.OpenAsync();

                SqliteCommand SelectCommand = new SqliteCommand
                {
                    Connection = db,

                    CommandText = string.Format("SELECT * FROM {0} ORDER BY TIMESTAMP DESC LIMIT {1}",
                        DataBaseService.HistoryTableName,
                        value
                        )
                };

                SqliteDataReader Query = await SelectCommand.ExecuteReaderAsync();

                while (await Query.ReadAsync())
                {
                    HistoryModel historyRawModel = new HistoryModel
                    {
                        CreateTimeStamp = Query.GetInt64(0),
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
        /// 删除选定的历史记录数据
        /// </summary>
        public static async Task<bool> DeleteAsync(List<HistoryModel> selectedHistoryDataList)
        {
            bool IsDeleteSuccessfully = true;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBFile.Path}"))
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
                            selectedHistoryDataList.ForEach(async historyItem =>
                            {
                                DeleteCommand.CommandText = string.Format("DELETE FROM {0} WHERE HISTORYKEY = '{1}'",
                                    DataBaseService.HistoryTableName,
                                    historyItem.HistoryKey
                                    );

                                await DeleteCommand.ExecuteNonQueryAsync();
                            });
                            await transaction.CommitAsync();
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            IsDeleteSuccessfully = false;
                        }
                    }
                }
                await db.CloseAsync();
            }
            return IsDeleteSuccessfully;
        }

        /// <summary>
        /// 清空历史记录数据
        /// </summary>
        public static async Task<bool> ClearAsync()
        {
            bool IsClearSuccessfully = false;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBFile.Path}"))
            {
                await db.OpenAsync();

                using (SqliteTransaction transaction = db.BeginTransaction())
                {
                    using (SqliteCommand ClearCommand = new SqliteCommand())
                    {
                        ClearCommand.Connection = db;
                        ClearCommand.Transaction = transaction;

                        try
                        {
                            ClearCommand.CommandText = string.Format("DELETE FROM {0}", DataBaseService.HistoryTableName);
                            await ClearCommand.ExecuteNonQueryAsync();

                            await transaction.CommitAsync();
                            IsClearSuccessfully = true;
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                        }
                    }
                }
                await db.CloseAsync();
            }
            return IsClearSuccessfully;
        }
    }
}
