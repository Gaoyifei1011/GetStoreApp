using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Download;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Download
{
    /// <summary>
    /// 下载记录数据库存储服务
    /// </summary>
    public class DownloadDBService : IDownloadDBService
    {
        private IDataBaseService DataBaseService { get; } = IOCHelper.GetService<IDataBaseService>();

        /// <summary>
        /// 检查是否有下载异常的记录，并将对应的下载状态值复原
        /// </summary>
        public async Task InitializeDownloadDBAsync()
        {
            // 将处于等待下载状态的任务调整为暂停下载状态
            List<BackgroundModel> DownloadWaitingList = await QueryWithFlagAsync(1);

            foreach (BackgroundModel backgroundItem in DownloadWaitingList)
            {
                await UpdateFlagAsync(backgroundItem.DownloadKey, 2);
            }

            // 将正在下载状态的任务调整为暂停下载状态
            List<BackgroundModel> DownloadingList = await QueryWithFlagAsync(3);

            foreach (BackgroundModel backgroundItem in DownloadingList)
            {
                await UpdateFlagAsync(backgroundItem.DownloadKey, 2);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// 直接添加下载记录数据，并返回下载记录添加是否成功的结果
        /// </summary>
        public async Task<bool> AddAsync(BackgroundModel backgroundItem)
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
                            InsertCommand.CommandText = string.Format("INSERT INTO {0} VALUES ('{1}','{2}','{3}','{4}','{5}',{6},{7})",
                                DataBaseService.DownloadTableName,
                                backgroundItem.DownloadKey,
                                backgroundItem.FileName,
                                backgroundItem.FileLink,
                                backgroundItem.FilePath,
                                backgroundItem.FileSHA1,
                                backgroundItem.TotalSize,
                                backgroundItem.DownloadFlag
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
        /// 存在重复的数据，只更新该记录的DownloadFlag（相当于执行重新下载步骤）
        /// </summary>
        public async Task<bool> UpdateFlagAsync(string downloadKey, int downloadFlag)
        {
            bool IsUpdateSuccessfully = false;

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
                            UpdateCommand.CommandText = string.Format("UPDATE {0} SET DOWNLOADFLAG ={1} WHERE DOWNLOADKEY = '{2}'",
                                DataBaseService.DownloadTableName,
                                downloadFlag,
                                downloadKey
                                );

                            await UpdateCommand.ExecuteNonQueryAsync();

                            await transaction.CommitAsync();

                            IsUpdateSuccessfully = true;
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                        }
                    }
                }
                await db.CloseAsync();
            }
            return IsUpdateSuccessfully;
        }

        /// <summary>
        /// 更新该记录对应的文件大小
        /// </summary>
        public async Task<bool> UpdateFileSizeAsync(string downloadKey, double fileSize)
        {
            bool IsUpdateSuccessfully = false;

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
                            UpdateCommand.CommandText = string.Format("UPDATE {0} SET FILESIZE ={1} WHERE DOWNLOADKEY = '{2}'",
                                DataBaseService.DownloadTableName,
                                fileSize,
                                downloadKey
                                );

                            await UpdateCommand.ExecuteNonQueryAsync();

                            await transaction.CommitAsync();

                            IsUpdateSuccessfully = true;
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                        }
                    }
                }
                await db.CloseAsync();
            }
            return IsUpdateSuccessfully;
        }

        /// <summary>
        /// 获取指定下载标志的下载记录数据
        /// </summary>
        /// <param name="downloadFlag">文件下载标志：0为下载失败，1为等待下载，2为暂停下载，3为正在下载，4为成功下载</param>
        /// <returns>返回指定下载标志记录列表</returns>
        public async Task<List<BackgroundModel>> QueryWithFlagAsync(int downloadFlag)
        {
            List<BackgroundModel> DownloadRawList = new List<BackgroundModel>();

            // 从数据库中获取数据
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand SelectCommand = new SqliteCommand
                {
                    Connection = db,

                    CommandText = string.Format("SELECT * FROM {0} WHERE DOWNLOADFLAG = {1}",
                        DataBaseService.DownloadTableName,
                        downloadFlag
                        )
                };

                SqliteDataReader Query = await SelectCommand.ExecuteReaderAsync();

                while (await Query.ReadAsync())
                {
                    BackgroundModel downloadRawModel = new BackgroundModel
                    {
                        DownloadKey = Query.GetString(0),
                        FileName = Query.GetString(1),
                        FileLink = Query.GetString(2),
                        FilePath = Query.GetString(3),
                        FileSHA1 = Query.GetString(4),
                        TotalSize = Convert.ToInt32(Query.GetString(5)),
                        DownloadFlag = Convert.ToInt32(Query.GetString(6))
                    };

                    DownloadRawList.Add(downloadRawModel);
                }

                await db.CloseAsync();
            }
            return DownloadRawList;
        }

        /// <summary>
        /// 获取指定下载键值的下载记录数据
        /// </summary>
        /// <param name="downloadKey">文件下载对应的唯一键值</param>
        /// <returns>返回指定下载键值对应的具体信息</returns>
        public async Task<BackgroundModel> QueryWithKeyAsync(string downloadKey)
        {
            BackgroundModel downloadRawModel = new BackgroundModel();

            // 从数据库中获取数据
            using (SqliteConnection db = new SqliteConnection($"Filename={DataBaseService.DBpath}"))
            {
                await db.OpenAsync();

                SqliteCommand SelectCommand = new SqliteCommand
                {
                    Connection = db,

                    CommandText = string.Format("SELECT * FROM {0} WHERE DOWNLOADKEY = '{1}'",
                        DataBaseService.DownloadTableName,
                        downloadKey
                        )
                };

                SqliteDataReader Query = await SelectCommand.ExecuteReaderAsync();

                while (await Query.ReadAsync())
                {
                    downloadRawModel.DownloadKey = Query.GetString(0);
                    downloadRawModel.FileName = Query.GetString(1);
                    downloadRawModel.FileLink = Query.GetString(2);
                    downloadRawModel.FilePath = Query.GetString(3);
                    downloadRawModel.FileSHA1 = Query.GetString(4);
                    downloadRawModel.TotalSize = Convert.ToInt32(Query.GetString(5));
                    downloadRawModel.DownloadFlag = Convert.ToInt32(Query.GetString(6));
                }

                await db.CloseAsync();
            }
            return downloadRawModel;
        }

        /// <summary>
        /// 检查是否存在相同键值的数据
        /// </summary>
        public async Task<bool> CheckDuplicatedAsync(string downloadKey)
        {
            bool IsExists = false;

            // 从数据库中获取数据
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
        /// 删除下载记录数据
        /// </summary>
        public async Task<bool> DeleteAsync(string downloadKey)
        {
            bool IsDeleteSuccessfully = true;

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
                            DeleteCommand.CommandText = string.Format("DELETE FROM {0} WHERE DOWNLOADKEY = '{1}'",
                                DataBaseService.DownloadTableName,
                                downloadKey
                                );

                            await DeleteCommand.ExecuteNonQueryAsync();

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
        /// 删除选定的下载记录数据
        /// </summary>
        public async Task<bool> DeleteSelectedAsync(List<string> selectedDownloadKeyList)
        {
            bool IsDeleteSuccessfully = true;

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
                            foreach (string downloadKey in selectedDownloadKeyList)
                            {
                                DeleteCommand.CommandText = string.Format("DELETE FROM {0} WHERE DOWNLOADKEY = '{1}'",
                                    DataBaseService.DownloadTableName,
                                    downloadKey
                                    );
                                await DeleteCommand.ExecuteNonQueryAsync();
                            }
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
        /// 清空下载记录数据（不清除正在下载和等待下载的数据）
        /// </summary>
        public async Task<bool> ClearAsync()
        {
            bool IsClearSuccessfully = true;

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
                            ClearCommand.CommandText = string.Format("DELETE FROM {0} WHERE DOWNLOADFLAG NOT IN (1,3)", DataBaseService.DownloadTableName);
                            await ClearCommand.ExecuteNonQueryAsync();

                            await transaction.CommitAsync();
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            IsClearSuccessfully = false;
                        }
                    }
                }
                await db.CloseAsync();
            }
            return IsClearSuccessfully;
        }
    }
}
