using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation;
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
        private bool isInitialized;
        private PackageManager searchAppsManager;
        private string cachedSearchText;
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

                await TeachingTipHelper.ShowAsync(new MainDataCopyTip(DataCopyKind.WinGetSearchInstall, copyResult));
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private void OnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is SearchAppsModel searchApps)
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

                // 添加任务
                WinGetInstance.InstallingAppsCollection.Add(new InstallingAppsModel()
                {
                    AppID = searchApps.AppID,
                    AppName = searchApps.AppName,
                    DownloadProgress = 0,
                    InstallProgressState = PackageInstallProgressState.Queued,
                    DownloadedFileSize = FileSizeHelper.ConvertFileSizeToString(0),
                    TotalFileSize = FileSizeHelper.ConvertFileSizeToString(0)
                });

                Task.Run(() =>
                {
                    try
                    {
                        IAsyncOperationWithProgress<InstallResult, InstallProgress> installPackageWithProgress = searchAppsManager.InstallPackageAsync(MatchResultList.Find(item => item.CatalogPackage.DefaultInstallVersion.Id == searchApps.AppID).CatalogPackage, new()
                        {
                            PackageInstallMode = Enum.TryParse(WinGetConfigService.WinGetInstallMode.Key, out PackageInstallMode packageInstallMode) ? packageInstallMode : PackageInstallMode.Default,
                            PackageInstallScope = PackageInstallScope.Any
                        });

                        // 第一部分：添加更新任务
                        WinGetInstance.installStateLock.Enter();

                        try
                        {
                            WinGetInstance.InstallingStateDict.Add(searchApps.AppID, installPackageWithProgress);
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                        finally
                        {
                            WinGetInstance.installStateLock.Exit();
                        }

                        // 第二部分：更新安装进度
                        installPackageWithProgress.Progress = (result, progress) => OnInstallPackageProgressing(result, progress, searchApps);

                        // 第三部分：安装已完成
                        installPackageWithProgress.Completed = (result, status) => OnInstallPackageCompleted(result, status, searchApps);
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "App installing failed.", e);
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
        /// 搜索应用控件初始化完成后触发的事件
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                isInitialized = true;

                try
                {
                    searchAppsManager = new();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Search apps information initialized failed.", e);
                }
            }
        }

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
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList.Clear();
            IsSearchCompleted = false;

            if (string.IsNullOrEmpty(cachedSearchText))
            {
                IsSearchCompleted = true;
                return;
            }
            await GetSearchAppsAsync();
            await InitializeDataAsync();
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private async void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                cachedSearchText = SearchText;
                NotSearched = false;
                IsSearchCompleted = false;
                await GetSearchAppsAsync();
                await InitializeDataAsync();
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

        #region 第三部分：搜索应用控件——自定义事件

        /// <summary>
        /// 应用安装状态发生变化时触发的事件
        /// </summary>
        private void OnInstallPackageProgressing(IAsyncOperationWithProgress<InstallResult, InstallProgress> result, InstallProgress progress, SearchAppsModel searchApps)
        {
            switch (progress.State)
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
                                    installingItem.DownloadProgress = Math.Round(progress.DownloadProgress * 100, 2); installingItem.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(progress.BytesDownloaded));
                                    installingItem.TotalFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(progress.BytesRequired));
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
        }

        /// 应用安装完成时时触发的事件
        /// </summary>
        private void OnInstallPackageCompleted(IAsyncOperationWithProgress<InstallResult, InstallProgress> result, AsyncStatus status, SearchAppsModel searchApps)
        {
            // 安装过程已顺利完成
            if (status is AsyncStatus.Completed)
            {
                InstallResult installResult = result.GetResults();

                // 应用安装成功
                if (installResult.Status is InstallResultStatus.Ok)
                {
                    // 显示 WinGet 应用安装成功通知
                    AppNotificationBuilder appNotificationBuilder = new();
                    appNotificationBuilder.AddArgument("action", "OpenApp");
                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallSuccessfully"), searchApps.AppName));
                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());

                    // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                    if (installResult.RebootRequired)
                    {
                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(WinGetOptionKind.UpgradeInstall, searchApps.AppName));

                            await Task.Run(() =>
                            {
                                if (contentDialogResult is ContentDialogResult.Primary)
                                {
                                    ShutdownHelper.Restart(ResourceService.GetLocalized("WinGet/RestartPC"), TimeSpan.FromSeconds(120));
                                }
                            });
                        });
                    }
                }
                else
                {
                    // 显示 WinGet 应用安装失败通知
                    AppNotificationBuilder appNotificationBuilder = new();
                    appNotificationBuilder.AddArgument("action", "OpenApp");
                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallFailed1"), searchApps.AppName));
                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetInstallFailed2"));
                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetInstallFailed3"));
                    AppNotificationButton installWithCommandButton = new(ResourceService.GetLocalized("Notification/InstallWithCommand"));
                    installWithCommandButton.Arguments.Add("action", string.Format("InstallWithCommand:{0}", searchApps.AppID));
                    AppNotificationButton openDownloadFolderButton = new(ResourceService.GetLocalized("Notification/OpenDownloadFolder"));
                    openDownloadFolderButton.Arguments.Add("action", "OpenDownloadFolder");
                    appNotificationBuilder.AddButton(installWithCommandButton);
                    appNotificationBuilder.AddButton(openDownloadFolderButton);
                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
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
            // 安装过程已被用户取消
            else if (status is AsyncStatus.Canceled)
            {
                LogService.WriteLog(LoggingLevel.Information, "App installing operation canceled.", new Exception());

                try
                {
                    WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);
                }
                catch (Exception exception)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(exception);
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
            // 安装过程发生错误
            else if (status is AsyncStatus.Error)
            {
                LogService.WriteLog(LoggingLevel.Error, "App installing failed.", result.ErrorCode);

                try
                {
                    WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);
                }
                catch (Exception exception)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(exception);
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

                // 显示 WinGet 应用安装失败通知
                AppNotificationBuilder appNotificationBuilder = new();
                appNotificationBuilder.AddArgument("action", "OpenApp");
                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallFailed1"), searchApps.AppName));
                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetInstallFailed2"));
                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetInstallFailed3"));
                AppNotificationButton installWithCommandButton = new("Notification/InstallWithCommand");
                installWithCommandButton.Arguments.Add("action", string.Format
                    ("InstallWithCommand:{0}", searchApps.AppID));
                AppNotificationButton openDownloadFolderButton = new("Notification/OpenDownloadFolder");
                openDownloadFolderButton.Arguments.Add("action", "OpenDownloadFolder");
                appNotificationBuilder.AddButton(installWithCommandButton);
                appNotificationBuilder.AddButton(openDownloadFolderButton);
                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
            }

            result.Close();
        }

        #endregion 第三部分：搜索应用控件——自定义事件

        public void InitializeWingetInstance(WinGetPage wingetInstance)
        {
            WinGetInstance = wingetInstance;
        }

        /// <summary>
        /// 搜索应用
        /// </summary>
        private async Task GetSearchAppsAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    IReadOnlyList<PackageCatalogReference> packageCatalogsList = searchAppsManager.GetPackageCatalogs();
                    CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = new();

                    for (int index = 0; index < packageCatalogsList.Count; index++)
                    {
                        PackageCatalogReference catalogReference = packageCatalogsList[index];
                        createCompositePackageCatalogOptions.Catalogs.Add(catalogReference);
                    }
                    PackageCatalogReference catalogRef = searchAppsManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);

                    if ((await catalogRef.ConnectAsync()).PackageCatalog is PackageCatalog searchCatalog)
                    {
                        FindPackagesOptions findPackagesOptions = new();
                        // 根据应用的名称寻找符合条件的结果
                        PackageMatchFilter nameMatchFilter = new()
                        {
                            Field = PackageMatchField.Name,
                            Option = PackageFieldMatchOption.ContainsCaseInsensitive,
                            Value = cachedSearchText
                        };
                        findPackagesOptions.Filters.Add(nameMatchFilter);
                        FindPackagesResult findResult = await searchCatalog.FindPackagesAsync(findPackagesOptions);

                        for (int index = 0; index < findResult.Matches.Count; index++)
                        {
                            MatchResultList.Add(findResult.Matches[index]);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Get search apps information failed.", e);
                }
            });
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        private async Task InitializeDataAsync()
        {
            SearchAppsCollection.Clear();
            if (MatchResultList.Count > 0)
            {
                List<SearchAppsModel> searchAppsList = [];
                await Task.Run(() =>
                {
                    try
                    {
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
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Initialize Searched apps data failed", e);
                    }
                });

                foreach (SearchAppsModel searchAppsItem in searchAppsList)
                {
                    SearchAppsCollection.Add(searchAppsItem);
                }
            }

            IsSearchCompleted = true;
        }
    }
}
