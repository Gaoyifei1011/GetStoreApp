using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：搜索应用控件
    /// </summary>
    public sealed partial class SearchAppsControl : Grid, INotifyPropertyChanged
    {
        private readonly PackageManager SearchAppsManager;
        private string cachedSearchText;
        private AutoResetEvent autoResetEvent;
        private WinGetPage WinGetInstance;

        private string SearchedAppsCountInfo { get; } = ResourceService.GetLocalized("WinGet/SearchedAppsCountInfo");

        private bool _notSearched = true;

        public bool NotSearched
        {
            get { return _notSearched; }

            set
            {
                if (!Equals(_notSearched, value))
                {
                    _notSearched = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NotSearched)));
                }
            }
        }

        private bool _isSearchCompleted;

        public bool IsSearchCompleted
        {
            get { return _isSearchCompleted; }

            set
            {
                if (!Equals(_isSearchCompleted, value))
                {
                    _isSearchCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearchCompleted)));
                }
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private List<MatchResult> MatchResultList { get; } = [];

        private ObservableCollection<SearchAppsModel> SearchAppsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchAppsControl()
        {
            InitializeComponent();

            try
            {
                SearchAppsManager = WinGetService.CreatePackageManager();
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Search apps information initialized failed.", e);
            }
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制安装命令
        /// </summary>
        private async void OnCopyInstallTextExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appId && !string.IsNullOrEmpty(appId))
            {
                string copyContent = string.Format("winget install {0}", appId);
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyContent);

                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.WinGetSearchInstall, copyResult));
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private void OnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is SearchAppsModel searchApps)
            {
                Task.Run(async () =>
                {
                    AutoResetEvent autoResetEvent = new(false);
                    try
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            // 禁用当前应用的可安装状态
                            foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                            {
                                if (searchAppsItem.AppID == searchApps.AppID)
                                {
                                    searchAppsItem.IsInstalling = true;
                                    break;
                                }
                            }
                        });

                        InstallOptions installOptions = WinGetService.CreateInstallOptions();

                        installOptions.PackageInstallMode = Enum.TryParse(WinGetConfigService.WinGetInstallMode.Key, out PackageInstallMode packageInstallMode) ? packageInstallMode : PackageInstallMode.Default;
                        installOptions.PackageInstallScope = PackageInstallScope.Any;

                        // 更新安装进度
                        Progress<InstallProgress> progressCallBack = new((installProgress) =>
                        {
                            switch (installProgress.State)
                            {
                                // 处于等待中状态
                                case PackageInstallProgressState.Queued:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                            {
                                                if (installingItem.AppID == searchApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Queued;
                                                    break;
                                                }
                                            }
                                        });
                                        break;
                                    }
                                // 处于下载中状态
                                case PackageInstallProgressState.Downloading:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                            {
                                                if (installingItem.AppID == searchApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Downloading;
                                                    installingItem.DownloadProgress = Math.Round(installProgress.DownloadProgress * 100, 2); installingItem.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesDownloaded));
                                                    installingItem.TotalFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesRequired));
                                                    break;
                                                }
                                            }
                                        });

                                        break;
                                    }
                                // 处于安装中状态
                                case PackageInstallProgressState.Installing:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                            {
                                                if (installingItem.AppID == searchApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Installing;
                                                    installingItem.DownloadProgress = 100;
                                                    break;
                                                }
                                            }
                                        });

                                        break;
                                    }
                                // 安装完成后等待其他操作状态
                                case PackageInstallProgressState.PostInstall:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                            {
                                                if (installingItem.AppID == searchApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.PostInstall;
                                                    break;
                                                }
                                            }
                                        });

                                        break;
                                    }
                                // 处于安装完成状态
                                case PackageInstallProgressState.Finished:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                            {
                                                if (installingItem.AppID == searchApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Finished;
                                                    installingItem.DownloadProgress = 100;
                                                    break;
                                                }
                                            }
                                        });

                                        break;
                                    }
                            }
                        });

                        // 任务取消执行操作
                        CancellationTokenSource installTokenSource = new();

                        // 添加任务
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WinGetInstance.InstallingAppsCollection.Add(new InstallingAppsModel()
                            {
                                AppID = searchApps.AppID,
                                AppName = searchApps.AppName,
                                DownloadProgress = 0,
                                InstallProgressState = PackageInstallProgressState.Queued,
                                DownloadedFileSize = FileSizeHelper.ConvertFileSizeToString(0),
                                TotalFileSize = FileSizeHelper.ConvertFileSizeToString(0)
                            });
                        });

                        WinGetInstance.installStateLock.Enter();

                        try
                        {
                            WinGetInstance.InstallingStateDict.Add(searchApps.AppID, installTokenSource);
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                        finally
                        {
                            WinGetInstance.installStateLock.Exit();
                        }

                        InstallResult installResult = await SearchAppsManager.InstallPackageAsync(MatchResultList.Find(item => item.CatalogPackage.DefaultInstallVersion.Id == searchApps.AppID).CatalogPackage, installOptions).AsTask(installTokenSource.Token, progressCallBack);

                        // 获取安装完成后的结果信息
                        if (installResult.Status is InstallResultStatus.Ok)
                        {
                            ToastNotificationService.Show(NotificationKind.WinGetInstallSuccessfully, searchApps.AppName);

                            // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                            if (installResult.RebootRequired)
                            {
                                ContentDialogResult result = ContentDialogResult.None;
                                DispatcherQueue.TryEnqueue(async () =>
                                {
                                    result = await ContentDialogHelper.ShowAsync(new RebootDialog(WinGetOptionKind.UpgradeInstall, searchApps.AppName), this);
                                    autoResetEvent.Set();
                                });

                                autoResetEvent.WaitOne();
                                autoResetEvent.Dispose();

                                if (result is ContentDialogResult.Primary)
                                {
                                    Shell32Library.ShellExecute(IntPtr.Zero, "open", Path.Combine(InfoHelper.SystemDataPath.Windows, "System32", "Shutdown.exe"), "-r -t 120", null, WindowShowStyle.SW_SHOWNORMAL);
                                }
                            }
                        }
                        else
                        {
                            ToastNotificationService.Show(NotificationKind.WinGetInstallFailed, searchApps.AppName, searchApps.AppID);
                        }

                        try
                        {
                            WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                        finally
                        {
                            WinGetInstance.installStateLock.Exit();
                        }

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            // 应用安装失败，将当前任务状态修改为可安装状态
                            foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                            {
                                if (searchAppsItem.AppID == searchApps.AppID)
                                {
                                    searchAppsItem.IsInstalling = false;
                                    break;
                                }
                            }

                            // 完成任务后从任务管理中删除任务

                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                            {
                                if (installingAppsItem.AppID == searchApps.AppID)
                                {
                                    WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                    break;
                                }
                            }
                        });
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException e)
                    {
                        LogService.WriteLog(LoggingLevel.Information, "App installing operation canceled.", e);

                        WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            // 应用安装失败，将当前任务状态修改为可安装状态
                            foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                            {
                                if (searchAppsItem.AppID == searchApps.AppID)
                                {
                                    searchAppsItem.IsInstalling = false;
                                    break;
                                }
                            }

                            // 完成任务后从任务管理中删除任务

                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                            {
                                if (installingAppsItem.AppID == searchApps.AppID)
                                {
                                    WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                    break;
                                }
                            }
                        });
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "App installing failed.", e);

                        WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            // 应用安装失败，将当前任务状态修改为可安装状态
                            foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                            {
                                if (searchAppsItem.AppID == searchApps.AppID)
                                {
                                    searchAppsItem.IsInstalling = false;
                                    break;
                                }
                            }

                            // 完成任务后从任务管理中删除任务

                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                            {
                                if (installingAppsItem.AppID == searchApps.AppID)
                                {
                                    WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                    break;
                                }
                            }
                        });

                        ToastNotificationService.Show(NotificationKind.WinGetInstallFailed, searchApps.AppName, searchApps.AppID);
                    }
                });
            }
        }

        /// <summary>
        /// 使用命令安装
        /// </summary>
        private void OnInstallWithCmdExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appId && !string.IsNullOrEmpty(appId))
            {
                Task.Run(() =>
                {
                    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format("install {0}", appId), null, WindowShowStyle.SW_SHOWNORMAL);
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：搜索应用控件——挂载的事件

        /// <summary>
        /// 打开临时下载目录
        /// </summary>
        private async void OnOpenTempFolderClicked(object sender, RoutedEventArgs args)
        {
            string wingetTempPath = Path.Combine(Path.GetTempPath(), "WinGet");
            if (Directory.Exists(wingetTempPath))
            {
                await Launcher.LaunchFolderPathAsync(wingetTempPath);
            }
            else
            {
                await Launcher.LaunchFolderPathAsync(Path.GetTempPath());
            }
        }

        /// <summary>
        /// 更新已安装应用数据
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList.Clear();
            IsSearchCompleted = false;

            if (string.IsNullOrEmpty(cachedSearchText))
            {
                IsSearchCompleted = true;
                return;
            }
            GetSearchApps();
            InitializeData();
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                cachedSearchText = SearchText;
                NotSearched = false;
                IsSearchCompleted = false;
                GetSearchApps();
                InitializeData();
            }
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        private void OnTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender is AutoSuggestBox autoSuggestBox)
            {
                SearchText = autoSuggestBox.Text;
            }
        }

        #endregion 第二部分：搜索应用控件——挂载的事件

        public void InitializeWingetInstance(WinGetPage wingetInstance)
        {
            WinGetInstance = wingetInstance;
        }

        /// <summary>
        /// 搜索应用
        /// </summary>
        private void GetSearchApps()
        {
            try
            {
                autoResetEvent ??= new AutoResetEvent(false);
                Task.Run(async () =>
                {
                    IReadOnlyList<PackageCatalogReference> packageCatalogsList = SearchAppsManager.GetPackageCatalogs();
                    CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = WinGetService.CreateCreateCompositePackageCatalogOptions();

                    for (int index = 0; index < packageCatalogsList.Count; index++)
                    {
                        PackageCatalogReference catalogReference = packageCatalogsList[index];
                        createCompositePackageCatalogOptions.Catalogs.Add(catalogReference);
                    }
                    PackageCatalogReference catalogRef = SearchAppsManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);

                    if ((await catalogRef.ConnectAsync()).PackageCatalog is PackageCatalog searchCatalog)
                    {
                        FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                        PackageMatchFilter nameMatchFilter = WinGetService.CreatePacakgeMatchFilter();
                        // 根据应用的名称寻找符合条件的结果
                        nameMatchFilter.Field = PackageMatchField.Name;
                        nameMatchFilter.Option = PackageFieldMatchOption.ContainsCaseInsensitive;
                        nameMatchFilter.Value = cachedSearchText;
                        findPackagesOptions.Filters.Add(nameMatchFilter);
                        FindPackagesResult findResult = await searchCatalog.FindPackagesAsync(findPackagesOptions);

                        for (int index = 0; index < findResult.Matches.Count; index++)
                        {
                            MatchResultList.Add(findResult.Matches[index]);
                        }
                    }
                    autoResetEvent?.Set();
                });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Get search apps information failed.", e);
            }
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        private void InitializeData()
        {
            SearchAppsCollection.Clear();

            Task.Run(() =>
            {
                autoResetEvent?.WaitOne();
                autoResetEvent?.Dispose();
                autoResetEvent = null;

                if (MatchResultList.Count > 0)
                {
                    List<SearchAppsModel> searchAppsList = [];
                    foreach (MatchResult matchItem in MatchResultList)
                    {
                        if (matchItem.CatalogPackage.DefaultInstallVersion is not null)
                        {
                            bool isInstalling = false;
                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                            {
                                if (matchItem.CatalogPackage.DefaultInstallVersion.Id == installingAppsItem.AppID)
                                {
                                    isInstalling = true;
                                    break;
                                }
                            }
                            searchAppsList.Add(new SearchAppsModel()
                            {
                                AppID = matchItem.CatalogPackage.DefaultInstallVersion.Id,
                                AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.DisplayName) || matchItem.CatalogPackage.DefaultInstallVersion.DisplayName.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.DisplayName,
                                AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Publisher) || matchItem.CatalogPackage.DefaultInstallVersion.Publisher.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Publisher,
                                AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Version) || matchItem.CatalogPackage.DefaultInstallVersion.Version.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Version,
                                IsInstalling = isInstalling,
                            });
                        }
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        foreach (SearchAppsModel searchAppsItem in searchAppsList)
                        {
                            SearchAppsCollection.Add(searchAppsItem);
                        }

                        IsSearchCompleted = true;
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        IsSearchCompleted = true;
                    });
                }
            });
        }
    }
}
