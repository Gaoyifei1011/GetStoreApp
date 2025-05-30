using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// WinGet 程序包配置服务
    /// </summary>
    public static class WinGetConfigService
    {
        private const string WinetDataSource = "WinetDataSource";
        private static readonly Lock wingetDataSourceLock = new();
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer wingetDataSourceContainer;

        public static StorageFolder DefaultDownloadFolder { get; private set; }

        public static List<KeyValuePair<string, PredefinedPackageCatalog>> PredefinedPackageCatalogList { get; } = [];

        /// <summary>
        /// 应用在初始化前获取设置存储的是否使用开发版本布尔值和 WinGet 程序包安装方式值
        /// </summary>
        public static async Task InitializeWinGetConfigAsync()
        {
            wingetDataSourceContainer = localSettingsContainer.CreateContainer(WinetDataSource, ApplicationDataCreateDisposition.Always);
            DefaultDownloadFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("WinGet", CreationCollisionOption.OpenIfExists);

            // 每次获取时读取已经添加的安装源，并去除掉已经被删除的值
            await Task.Run(() =>
            {
                PackageManager packageManager = new();
                wingetDataSourceLock.Enter();

                try
                {
                    if (wingetDataSourceContainer.Values.TryGetValue(WinetDataSource, out object value) && value is ApplicationDataCompositeValue compositeValue)
                    {
                        KeyValuePair<string, bool> winGetDataSourceName = KeyValuePair.Create(Convert.ToString(compositeValue["Name"]), Convert.ToBoolean(compositeValue["IsInternal"]));
                        wingetDataSourceContainer.Values.Clear();
                        bool isModified = false;

                        // 检查内置数据源

                        foreach (PredefinedPackageCatalog predefinedPackageCatalog in Enum.GetValues<PredefinedPackageCatalog>())
                        {
                            PackageCatalogReference packageCatalogReference = packageManager.GetPredefinedPackageCatalog(predefinedPackageCatalog);
                            PredefinedPackageCatalogList.Add(KeyValuePair.Create(packageCatalogReference.Info.Name, predefinedPackageCatalog));
                        }

                        // 保存检查完成后的数据
                        foreach (KeyValuePair<string, PredefinedPackageCatalog> predefinedPackageCatalogReferenceName in PredefinedPackageCatalogList)
                        {
                            if (string.Equals(winGetDataSourceName.Key, predefinedPackageCatalogReferenceName.Key) && winGetDataSourceName.Value)
                            {
                                wingetDataSourceContainer.Values[WinetDataSource] = compositeValue;
                                isModified = true;
                                break;
                            }
                        }

                        if (!isModified)
                        {
                            // 检查自定义数据源
                            IReadOnlyList<PackageCatalogReference> packageCatalogReferenceList = packageManager.GetPackageCatalogs();
                            List<string> packageCatalogReferenceNameList = [];
                            for (int index = 0; index < packageCatalogReferenceList.Count; index++)
                            {
                                packageCatalogReferenceNameList.Add(packageCatalogReferenceList[index].Info.Name);
                            }

                            // 保存检查完成后的数据
                            foreach (string packageCatalogReferenceName in packageCatalogReferenceNameList)
                            {
                                if (string.Equals(winGetDataSourceName.Key, packageCatalogReferenceName) && !winGetDataSourceName.Value)
                                {
                                    wingetDataSourceContainer.Values[WinetDataSource] = compositeValue;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetConfigService), nameof(InitializeWinGetConfigAsync), 1, e);
                }
                finally
                {
                    wingetDataSourceLock.Exit();
                }
            });
        }

        /// <summary>
        /// 获取 WinGet 数据源搜索时选择的所有名称
        /// </summary>
        public static KeyValuePair<string, bool> GetWinGetDataSourceName()
        {
            KeyValuePair<string, bool> winGetDataSourceName = default;
            wingetDataSourceLock.Enter();

            try
            {
                if (wingetDataSourceContainer.Values.TryGetValue(WinetDataSource, out object value) && value is ApplicationDataCompositeValue compositeValue)
                {
                    winGetDataSourceName = KeyValuePair.Create(Convert.ToString(compositeValue["Name"]), Convert.ToBoolean(compositeValue["IsInternal"]));
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetConfigService), nameof(GetWinGetDataSourceName), 1, e);
            }
            finally
            {
                wingetDataSourceLock.Exit();
            }

            return winGetDataSourceName;
        }

        /// <summary>
        /// 设置 WinGet 数据源搜索时选择的所有名称
        /// </summary>
        public static void SetWinGetDataSourceName(KeyValuePair<string, bool> winGetDataSourceName)
        {
            wingetDataSourceLock.Enter();

            try
            {
                ApplicationDataCompositeValue compositeValue = new()
                {
                    ["Name"] = winGetDataSourceName.Key,
                    ["IsInternal"] = winGetDataSourceName.Value
                };

                wingetDataSourceContainer.Values[WinetDataSource] = compositeValue;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetConfigService), nameof(SetWinGetDataSourceName), 1, e);
            }
            finally
            {
                wingetDataSourceLock.Exit();
            }
        }

        /// <summary>
        /// 移除 WinGet 自定义源设置
        /// </summary>
        public static void RemoveWinGetDataSourceName(KeyValuePair<string, bool> winGetDataSourceName)
        {
            wingetDataSourceLock.Enter();

            try
            {
                if (wingetDataSourceContainer.Values.TryGetValue(WinetDataSource, out object value) && value is ApplicationDataCompositeValue compositeValue && compositeValue.TryGetValue("Name", out object nameValue) && Equals(Convert.ToString(nameValue), winGetDataSourceName.Key) && compositeValue.TryGetValue("IsInternal", out object isInternalValue) && Equals(Convert.ToBoolean(isInternalValue), winGetDataSourceName.Value))
                {
                    wingetDataSourceContainer.Values.Remove(WinetDataSource);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetConfigService), nameof(RemoveWinGetDataSourceName), 1, e);
            }
            finally
            {
                wingetDataSourceLock.Exit();
            }
        }
    }
}
