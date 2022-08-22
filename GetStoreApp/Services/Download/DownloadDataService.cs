using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Download
{
    /// <summary>
    /// 下载记录数据库存储服务
    /// </summary>
    public class DownloadDataService : IDownloadDataService
    {
        private IDataBaseService DataBaseService { get; } = IOCHelper.GetService<IDataBaseService>();

        /// <summary>
        /// 判断下载记录表是否为空
        /// </summary>
        private async Task<bool> IsDownloadTableEmptyAsync()
        {
            int DownloadTableCount = 0;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand CountCommand = new SqliteCommand
                {
                    Connection = db,

                    CommandText = string.Format("SELECT COUNT(*) FROM {0}", DataBaseService.DownloadTableName)
                };

                SqliteDataReader Query = await CountCommand.ExecuteReaderAsync();

                if (await Query.ReadAsync())
                {
                    DownloadTableCount = Query.GetInt32(0);
                }

                await db.CloseAsync();
            }

            return Convert.ToBoolean(DownloadTableCount == 0);
        }

        /// <summary>
        /// 检查是否有相同键值的数据
        /// </summary>
        public async Task<bool> CheckDuplicatedDataAsync(string downloadKey)
        {
            bool IsExists = false;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand SearchCommand = new SqliteCommand
                {
                    Connection = db,

                    CommandText = string.Format("SELECT * FROM {0} WHERE DOWNLOADKEY LIKE '{1}'",
                        DataBaseService.DownloadTableName,
                        downloadKey
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
        /// 直接添加下载记录数据，并返回下载记录添加是否成功的结果
        /// </summary>
        public async Task<bool> AddDataAsync(DownloadModel download)
        {
            bool IsAddSuccessfully = false;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
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
                            InsertCommand.CommandText = string.Format("INSERT INTO {0} VALUES ({1},'{2}','{3}','{4}','{5}','{6}','{7}')",
                                DataBaseService.DownloadTableName,
                                download.CreateTimeStamp,
                                download.DownloadKey,
                                download.FileName,
                                download.FileLink,
                                download.FilePath,
                                download.FileSize,
                                download.DownloadFlag
                                );

                            await InsertCommand.ExecuteNonQueryAsync();

                            await transaction.CommitAsync();

                            IsAddSuccessfully = true;
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                        }
                    }
                }
                await db.CloseAsync();
            }
            return IsAddSuccessfully;
        }

        /// <summary>
        /// 存在重复的数据，只更新该记录的TimeStamp（相当于执行重新下载步骤）
        /// </summary>
        public async Task UpdateDataAsync(DownloadModel download)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
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
                            UpdateCommand.CommandText = string.Format("UPDATE {0} SET TIMESTAMP = {1}, DOWNLOADFLAG ={2} WHERE DOWNLOADKEY = '{3}'",
                                DataBaseService.DownloadTableName,
                                download.CreateTimeStamp,
                                download.DownloadFlag,
                                download.DownloadKey
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
        /// 获取所有的下载记录数据
        /// </summary>
        /// <param name="timeSortOrder">时间戳顺序，True为递增排序，False为递减排序</param>
        /// <param name="typeFilter"></param>
        /// <param name="channelFilter"></param>
        /// <returns>返回下载记录列表</returns>
        public async Task<Tuple<List<DownloadModel>, bool>> QueryAllDownloadDataAsync()
        {
            List<DownloadModel> DownloRawList = new List<DownloadModel>();

            //调用之前先判断历史记录表是否为空
            bool IsDownloadEmpty = await IsDownloadTableEmptyAsync();

            if (IsDownloadEmpty) return Tuple.Create(DownloRawList, IsDownloadEmpty);

            // 从数据库中获取数据
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand SelectCommand = new SqliteCommand
                {
                    Connection = db,

                    CommandText = string.Format("SELECT * FROM {0} ORDER BY TIMESTAMP DESC", DataBaseService.DownloadTableName)
                };

                SqliteDataReader Query = await SelectCommand.ExecuteReaderAsync();

                while (await Query.ReadAsync())
                {
                    DownloadModel downloadRawModel = new DownloadModel
                    {
                        CreateTimeStamp = Query.GetInt64(0),
                        DownloadKey = Query.GetString(1),
                        FileName = Query.GetString(2),
                        FileLink = Query.GetString(3),
                        FilePath = Query.GetString(4),
                        FileSHA1 = Query.GetString(5),
                        FileSize = Query.GetString(6),
                        DownloadFlag = Convert.ToInt32(Query.GetString(7))
                    };

                    DownloRawList.Add(downloadRawModel);
                }

                await db.CloseAsync();
            }
            return Tuple.Create(DownloRawList, IsDownloadEmpty);
        }

        /// <summary>
        /// 删除选定的下载记录数据
        /// </summary>
        public async Task DeleteDownloadDataAsync(List<DownloadModel> selectedDownloadDataList)
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
                            foreach (DownloadModel downloadItem in selectedDownloadDataList)
                            {
                                DeleteCommand.CommandText = string.Format("DELETE FROM {0} WHERE DOWNLOADKEY = '{1}'",
                                    DataBaseService.DownloadTableName,
                                    downloadItem.DownloadKey
                                    );

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
        /// 清空下载记录数据
        /// </summary>
        public async Task<bool> ClearDownloadDataAsync()
        {
            bool result = false;

            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
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
                            ClearCommand.CommandText = string.Format("DELETE FROM {0}", DataBaseService.DownloadTableName);
                            await ClearCommand.ExecuteNonQueryAsync();

                            await transaction.CommitAsync();
                            result = true;
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                        }
                    }
                }
                await db.CloseAsync();
                return result;
            }
        }
    }
}
