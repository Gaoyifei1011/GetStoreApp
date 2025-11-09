using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Helpers.WinGet;
using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// WinGet 程序包配置服务
    /// </summary>
    public static class WinGetConfigService
    {
        private static readonly string settingsKey = ConfigKey.WinGetSourceKey;
        private const string WinGetDataSource = "WinGetDataSource";
        private static readonly Lock wingetDataSourceLock = new();
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.GetDefault().LocalSettings;
        private static ApplicationDataContainer wingetDataSourceContainer;
        private static string defaultWinGetSource;

        public static string DefaultDownloadFolder { get; private set; }

        public static string WinGetSource { get; private set; }

        public static string CurrentWinGetSource { get; private set; }

        public static List<string> WinGetSourceList { get; } = ["BuiltinApp", "AppInstaller"];

        public static List<KeyValuePair<string, PredefinedPackageCatalog>> PredefinedPackageCatalogList { get; } = [];

        /// <summary>
        /// 应用在初始化前获取设置存储的是否使用开发版本布尔值和 WinGet 程序包安装方式值
        /// </summary>
        public static async Task InitializeWinGetConfigAsync()
        {
            defaultWinGetSource = WinGetSourceList[0];
            WinGetSource = GetWinGetSource();
            CurrentWinGetSource = Equals(WinGetSource, WinGetSourceList[0]) ? WinGetSource : WinGetFactoryHelper.IsExisted() ? WinGetSource : defaultWinGetSource;
            wingetDataSourceContainer = localSettingsContainer.CreateContainer(WinGetDataSource, ApplicationDataCreateDisposition.Always);
            DefaultDownloadFolder = (await ApplicationData.GetDefault().LocalCacheFolder.CreateFolderAsync("WinGet", Windows.Storage.CreationCollisionOption.OpenIfExists)).Path;

            // 每次获取时读取已经添加的安装源，并去除掉已经被删除的值
            await Task.Run(() =>
            {
                PackageManager packageManager = Equals(CurrentWinGetSource, WinGetSourceList[0]) ? new() : WinGetFactoryHelper.CreatePackageManager();
                wingetDataSourceLock.Enter();

                try
                {
                    // 获取内置数据源
                    foreach (PredefinedPackageCatalog predefinedPackageCatalog in Enum.GetValues<PredefinedPackageCatalog>())
                    {
                        PackageCatalogReference packageCatalogReference = packageManager.GetPredefinedPackageCatalog(predefinedPackageCatalog);
                        PredefinedPackageCatalogList.Add(KeyValuePair.Create(packageCatalogReference.Info.Name, predefinedPackageCatalog));
                    }

                    if (wingetDataSourceContainer.Values.TryGetValue(CurrentWinGetSource, out object value) && value is Windows.Storage.ApplicationDataCompositeValue compositeValue)
                    {
                        KeyValuePair<string, bool> winGetDataSourceName = KeyValuePair.Create(Convert.ToString(compositeValue["Name"]), Convert.ToBoolean(compositeValue["IsInternal"]));
                        wingetDataSourceContainer.Values.Remove(CurrentWinGetSource);
                        bool isModified = false;

                        // 保存检查完成后的数据
                        foreach (KeyValuePair<string, PredefinedPackageCatalog> predefinedPackageCatalogReferenceName in PredefinedPackageCatalogList)
                        {
                            if (string.Equals(winGetDataSourceName.Key, predefinedPackageCatalogReferenceName.Key) && winGetDataSourceName.Value)
                            {
                                wingetDataSourceContainer.Values[CurrentWinGetSource] = compositeValue;
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
                                    wingetDataSourceContainer.Values[CurrentWinGetSource] = compositeValue;
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
        /// 获取设置存储的 WinGet 来源值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetWinGetSource()
        {
            string winGetSource = LocalSettingsService.ReadSetting<string>(settingsKey);

            if (string.IsNullOrEmpty(winGetSource))
            {
                SetWinGetSource(defaultWinGetSource);
                return WinGetSourceList.Find(item => string.Equals(item, defaultWinGetSource, StringComparison.OrdinalIgnoreCase));
            }

            string selectedWinGetSource = WinGetSourceList.Find(item => string.Equals(item, winGetSource, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrEmpty(selectedWinGetSource) ? defaultWinGetSource : selectedWinGetSource;
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
                if (wingetDataSourceContainer.Values.TryGetValue(CurrentWinGetSource, out object value) && value is Windows.Storage.ApplicationDataCompositeValue compositeValue)
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
        ///  WinGet 来源发生修改时修改设置存储的 WinGet 来源值
        /// </summary>
        public static void SetWinGetSource(string winGetSource)
        {
            WinGetSource = winGetSource;
            LocalSettingsService.SaveSetting(settingsKey, winGetSource);
        }

        /// <summary>
        /// 设置 WinGet 数据源搜索时选择的所有名称
        /// </summary>
        public static void SetWinGetDataSourceName(KeyValuePair<string, bool> winGetDataSourceName)
        {
            wingetDataSourceLock.Enter();

            try
            {
                Windows.Storage.ApplicationDataCompositeValue compositeValue = new()
                {
                    ["Name"] = winGetDataSourceName.Key,
                    ["IsInternal"] = winGetDataSourceName.Value
                };

                wingetDataSourceContainer.Values[CurrentWinGetSource] = compositeValue;
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
                if (wingetDataSourceContainer.Values.TryGetValue(CurrentWinGetSource, out object value) && value is Windows.Storage.ApplicationDataCompositeValue compositeValue && compositeValue.TryGetValue("Name", out object nameValue) && string.Equals(Convert.ToString(nameValue), winGetDataSourceName.Key) && compositeValue.TryGetValue("IsInternal", out object isInternalValue) && Equals(Convert.ToBoolean(isInternalValue), winGetDataSourceName.Value))
                {
                    wingetDataSourceContainer.Values.Remove(CurrentWinGetSource);
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
