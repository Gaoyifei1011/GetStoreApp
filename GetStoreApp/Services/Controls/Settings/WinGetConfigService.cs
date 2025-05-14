using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreApp.Services.Controls.Settings
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

        public static bool IsWinGetInstalled { get; private set; }

        public static StorageFolder DefaultDownloadFolder { get; private set; }

        public static List<KeyValuePair<string, PredefinedPackageCatalog>> PredefinedPackageCatalogList { get; } = [];

        /// <summary>
        /// 应用在初始化前获取设置存储的是否使用开发版本布尔值和 WinGet 程序包安装方式值
        /// </summary>
        public static async Task InitializeWinGetConfigAsync()
        {
            wingetDataSourceContainer = localSettingsContainer.CreateContainer(WinetDataSource, ApplicationDataCreateDisposition.Always);
            DefaultDownloadFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("WinGet", CreationCollisionOption.OpenIfExists);
            IsWinGetInstalled = GetWinGetInstalledState();

            // 每次获取时读取已经添加的安装源，并去除掉已经被删除的值
            await Task.Run(() =>
            {
                if (IsWinGetInstalled)
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
                                if (winGetDataSourceName.Key.Equals(predefinedPackageCatalogReferenceName.Key) && winGetDataSourceName.Value)
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
                                    if (winGetDataSourceName.Key.Equals(packageCatalogReferenceName) && !winGetDataSourceName.Value)
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
                        LogService.WriteLog(LoggingLevel.Error, "Initialize winget data source settings data failed", e);
                    }
                    finally
                    {
                        wingetDataSourceLock.Exit();
                    }
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
                LogService.WriteLog(LoggingLevel.Error, "Get winget data source settings data failed", e);
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
                LogService.WriteLog(LoggingLevel.Error, "Set winget data source settings data failed", e);
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
                if (wingetDataSourceContainer.Values.TryGetValue(WinetDataSource, out object value) && value is ApplicationDataCompositeValue compositeValue)
                {
                    if (compositeValue.TryGetValue("Name", out object nameValue) && Convert.ToString(nameValue).Equals(winGetDataSourceName.Key) && compositeValue.TryGetValue("IsInternal", out object isInternalValue) && Convert.ToBoolean(isInternalValue).Equals(winGetDataSourceName.Value))
                    {
                        wingetDataSourceContainer.Values.Remove(WinetDataSource);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Set winget data source settings data failed", e);
            }
            finally
            {
                wingetDataSourceLock.Exit();
            }
        }

        /// <summary>
        /// 获取 WinGet 的安装状态
        /// </summary>
        private static bool GetWinGetInstalledState()
        {
            Windows.Management.Deployment.PackageManager packageManager = new();
            foreach (Package package in packageManager.FindPackagesForUser(string.Empty))
            {
                if (package.Id.FullName.Contains("Microsoft.DesktopAppInstaller") && File.Exists(Path.Combine(package.InstalledPath, "WinGet.exe")))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
