using GetStoreApp.Extensions.DataType.Constant;
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
        private static readonly string winGetInstallModeSettingsKey = ConfigKey.WinGetInstallModeKey;
        private static readonly Lock wingetDataSourceLock = new();
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static PackageManager packageManager;
        private static ApplicationDataContainer wingetDataSourceContainer;

        public static bool IsWinGetInstalled { get; private set; }

        public static KeyValuePair<string, string> DefaultWinGetInstallMode { get; set; }

        public static KeyValuePair<string, string> WinGetInstallMode { get; set; }

        public static List<KeyValuePair<string, string>> WinGetInstallModeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的是否使用开发版本布尔值和 WinGet 程序包安装方式值
        /// </summary>
        public static void InitializeWinGetConfig()
        {
            WinGetInstallModeList = ResourceService.WinGetInstallModeList;

            DefaultWinGetInstallMode = WinGetInstallModeList.Find(item => item.Key.Equals(PackageInstallMode.Interactive.ToString(), StringComparison.OrdinalIgnoreCase));

            WinGetInstallMode = GetWinGetInstallMode();
            IsWinGetInstalled = GetWinGetInstalledState();

            wingetDataSourceContainer = localSettingsContainer.CreateContainer(WinetDataSource, ApplicationDataCreateDisposition.Always);

            // 每次获取时读取已经添加的安装源，并去除掉已经被删除的值
            Task.Run(() =>
            {
                if (IsWinGetInstalled)
                {
                    packageManager = new();
                    wingetDataSourceLock.Enter();

                    try
                    {
                        if (wingetDataSourceContainer.Values.Count > 0)
                        {
                            List<KeyValuePair<string, bool>> winGetDataSourceNameList = [];
                            for (int index = 0; index < wingetDataSourceContainer.Values.Count; index++)
                            {
                                if (wingetDataSourceContainer.Values.TryGetValue(WinetDataSource + index.ToString(), out object value) && value is ApplicationDataCompositeValue compositeValue)
                                {
                                    winGetDataSourceNameList.Add(KeyValuePair.Create(Convert.ToString(compositeValue["Name"]), Convert.ToBoolean(compositeValue["IsInternal"])));
                                }
                            }

                            wingetDataSourceContainer.Values.Clear();

                            // 检查内置数据源
                            List<string> predefinedPackageCatalogReferenceNameList = [];
                            foreach (PredefinedPackageCatalog predefinedPackageCatalog in Enum.GetValues<PredefinedPackageCatalog>())
                            {
                                predefinedPackageCatalogReferenceNameList.Add(packageManager.GetPredefinedPackageCatalog(predefinedPackageCatalog).Info.Name);
                            }

                            // 保存检查完成后的数据
                            foreach (string predefinedPackageCatalogReferenceName in predefinedPackageCatalogReferenceNameList)
                            {
                                foreach (KeyValuePair<string, bool> winGetDataSourceNameKeyValuePairItem in winGetDataSourceNameList)
                                {
                                    if (winGetDataSourceNameKeyValuePairItem.Key.Equals(predefinedPackageCatalogReferenceName) && winGetDataSourceNameKeyValuePairItem.Value)
                                    {
                                        ApplicationDataCompositeValue compositeValue = new()
                                        {
                                            ["Name"] = predefinedPackageCatalogReferenceName,
                                            ["IsInternal"] = winGetDataSourceNameKeyValuePairItem.Value
                                        };

                                        wingetDataSourceContainer.Values.TryAdd(WinetDataSource + wingetDataSourceContainer.Values.Count.ToString(), compositeValue);
                                    }
                                }
                            }

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
                                foreach (KeyValuePair<string, bool> winGetDataSourceNameKeyValuePairItem in winGetDataSourceNameList)
                                {
                                    if (winGetDataSourceNameKeyValuePairItem.Key.Equals(packageCatalogReferenceName) && !winGetDataSourceNameKeyValuePairItem.Value)
                                    {
                                        ApplicationDataCompositeValue compositeValue = new()
                                        {
                                            ["Name"] = packageCatalogReferenceName,
                                            ["IsInternal"] = winGetDataSourceNameKeyValuePairItem.Value
                                        };

                                        wingetDataSourceContainer.Values.TryAdd(WinetDataSource + wingetDataSourceContainer.Values.Count.ToString(), compositeValue);
                                    }
                                }
                            }
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
                }
            });
        }

        /// <summary>
        /// 获取设置存储的 WinGet 程序包安装方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static KeyValuePair<string, string> GetWinGetInstallMode()
        {
            string winGetInstallMode = LocalSettingsService.ReadSetting<string>(winGetInstallModeSettingsKey);

            if (string.IsNullOrEmpty(winGetInstallMode))
            {
                SetWinGetInstallMode(DefaultWinGetInstallMode);
                return WinGetInstallModeList.Find(item => item.Key.Equals(DefaultWinGetInstallMode.Key));
            }

            KeyValuePair<string, string> selectedWinGetInstallMode = WinGetInstallModeList.Find(item => item.Key.Equals(winGetInstallMode, StringComparison.OrdinalIgnoreCase));

            return string.IsNullOrEmpty(selectedWinGetInstallMode.Key) ? DefaultWinGetInstallMode : selectedWinGetInstallMode;
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的 WinGet 程序包安装方式
        /// </summary>
        public static void SetWinGetInstallMode(KeyValuePair<string, string> winGetInstallMode)
        {
            WinGetInstallMode = winGetInstallMode;

            LocalSettingsService.SaveSetting(winGetInstallModeSettingsKey, winGetInstallMode.Key);
        }

        /// <summary>
        /// 获取 WinGet 数据源搜索时选择的所有名称
        /// </summary>
        public static List<KeyValuePair<string, bool>> GetWinGetDataSourceNameList()
        {
            List<KeyValuePair<string, bool>> winGetDataSourceNameList = [];
            wingetDataSourceLock.Enter();

            try
            {
                if (wingetDataSourceContainer.Values.Count > 0)
                {
                    for (int index = 0; index < wingetDataSourceContainer.Values.Count; index++)
                    {
                        if (wingetDataSourceContainer.Values.TryGetValue(WinetDataSource + index.ToString(), out object value) && value is ApplicationDataCompositeValue compositeValue)
                        {
                            winGetDataSourceNameList.Add(KeyValuePair.Create(Convert.ToString(compositeValue["Name"]), Convert.ToBoolean(compositeValue["IsInternal"])));
                        }
                    }
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

            return winGetDataSourceNameList;
        }

        /// <summary>
        /// 设置 WinGet 数据源搜索时选择的所有名称
        /// </summary>
        public static void SetWinGetDataSourceNameList(List<KeyValuePair<string, bool>> winGetDataSourceNameList)
        {
            wingetDataSourceLock.Enter();

            try
            {
                wingetDataSourceContainer.Values.Clear();

                for (int index = 0; index < winGetDataSourceNameList.Count; index++)
                {
                    ApplicationDataCompositeValue compositeValue = new()
                    {
                        ["Name"] = winGetDataSourceNameList[index].Key,
                        ["IsInternal"] = winGetDataSourceNameList[index].Value
                    };

                    wingetDataSourceContainer.Values.TryAdd(WinetDataSource + index.ToString(), compositeValue);
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
