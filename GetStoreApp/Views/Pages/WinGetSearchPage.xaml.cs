using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.Windows;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
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

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 搜索应用界面
    /// </summary>
    public sealed partial class WinGetSearchPage : Page, INotifyPropertyChanged
    {
        private readonly string SearchedAppsCountInfo = ResourceService.GetLocalized("WinGet/SearchedAppsCountInfo");
        private readonly string Unknown = ResourceService.GetLocalized("WinGet/Unknown");
        private readonly string SearchAppsEmptyDescription = ResourceService.GetLocalized("WinGet/SearchAppsEmptyDescription");
        private readonly string SearchAppsFailed = ResourceService.GetLocalized("WinGet/SearchAppsFailed");
        private readonly string SearchFindAppsFailed = ResourceService.GetLocalized("WinGet/SearchFindAppsFailed");
        private readonly string SearchCatalogReferenceFailed = ResourceService.GetLocalized("WinGet/SearchCatalogReferenceFailed");
        private readonly string SearchNotSelectSource = ResourceService.GetLocalized("WinGet/SearchNotSelectSource");
        private string cachedSearchText;
        private WinGetPage WinGetPageInstance;

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

        private bool _isIncrease = true;

        public bool IsIncrease
        {
            get { return _isIncrease; }

            set
            {
                if (!Equals(_isIncrease, value))
                {
                    _isIncrease = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIncrease)));
                }
            }
        }

        private SearchAppsResultKind _searchAppsResultKind = SearchAppsResultKind.NotSearch;

        public SearchAppsResultKind SearchAppsResultKind
        {
            get { return _searchAppsResultKind; }

            set
            {
                if (!Equals(_searchAppsResultKind, value))
                {
                    _searchAppsResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchAppsResultKind)));
                }
            }
        }

        private PackageMatchField _packageMatchField = PackageMatchField.Name;

        public PackageMatchField PackageMatchField
        {
            get { return _packageMatchField; }

            set
            {
                if (!Equals(_packageMatchField, value))
                {
                    _packageMatchField = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageMatchField)));
                }
            }
        }

        private PackageFieldMatchOption _packageFieldMatchOption = PackageFieldMatchOption.EqualsCaseInsensitive;

        public PackageFieldMatchOption PackageFieldMatchOption
        {
            get { return _packageFieldMatchOption; }

            set
            {
                if (!Equals(_packageFieldMatchOption, value))
                {
                    _packageFieldMatchOption = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageFieldMatchOption)));
                }
            }
        }

        private string _searchFailedContent;

        public string SearchFailedContent
        {
            get { return _searchFailedContent; }

            set
            {
                if (!Equals(_searchFailedContent, value))
                {
                    _searchFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchFailedContent)));
                }
            }
        }

        private ObservableCollection<SearchAppsModel> SearchAppsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetSearchPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is WinGetPage winGetPage && WinGetPageInstance is null)
            {
                WinGetPageInstance = winGetPage;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        // TODO: 未完成

        /// <summary>
        /// 下载应用
        /// </summary>
        private void OnDownloadExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is SearchAppsModel searchApps && WinGetPageInstance is not null)
            {
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private void OnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is SearchAppsModel searchApps && WinGetPageInstance is not null)
            {
            }
        }

        /// <summary>
        /// 修复应用
        /// </summary>
        private void OnRepairExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is SearchAppsModel searchApps && WinGetPageInstance is not null)
            {
            }
        }

        /// <summary>
        /// 查看版本信息
        /// </summary>

        private async void OnViewVersionInfoExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is SearchAppsModel searchApps && WinGetPageInstance is not null)
            {
                await MainWindow.Current.ShowDialogAsync(new WinGetAppsVersionDialog(WinGetOperationKind.VersionInfo, WinGetPageInstance, searchApps));
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：WinGet 搜索应用界面——挂载的事件

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        private async void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string increase && SearchAppsResultKind is SearchAppsResultKind.Successfully)
            {
                IsIncrease = Convert.ToBoolean(increase);
                SearchAppsResultKind = SearchAppsResultKind.Searching;
                List<SearchAppsModel> searchAppsList = [.. SearchAppsCollection];
                SearchAppsCollection.Clear();
                if (IsIncrease)
                {
                    searchAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                }
                else
                {
                    searchAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                }
                await Task.Delay(500);
                foreach (SearchAppsModel searchAppsItem in searchAppsList)
                {
                    SearchAppsCollection.Add(searchAppsItem);
                }
                SearchAppsResultKind = SearchAppsResultKind.Successfully;
            }
        }

        /// <summary>
        /// 更新应用搜索过滤方式
        /// </summary>
        private void OnPackageMatchFieldClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is PackageMatchField packageMatchField)
            {
                PackageMatchField = packageMatchField;
            }
        }

        /// <summary>
        /// 更新应用搜索过滤规则
        /// </summary>
        private void OnPackageFieldMatchOptionClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is PackageFieldMatchOption packageFieldMatchOption)
            {
                PackageFieldMatchOption = packageFieldMatchOption;
            }
        }

        /// <summary>
        /// 打开任务管理
        /// </summary>

        private void OnTaskManagerClicked(object sender, RoutedEventArgs args)
        {
            WinGetPageInstance?.ShowTaskManager();
        }

        /// <summary>
        /// 打开临时下载目录
        /// </summary>
        private void OnOpenTempFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
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
            });
        }

        /// <summary>
        /// 刷新搜索应用数据
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            SearchAppsResultKind = SearchAppsResultKind.Searching;
            SearchAppsCollection.Clear();

            if (string.IsNullOrEmpty(cachedSearchText))
            {
                SearchAppsResultKind = SearchAppsResultKind.Failed;
                SearchFailedContent = SearchAppsEmptyDescription;
            }
            else
            {
                await InitializeSearchAppsDataAsync();
            }
        }

        /// <summary>
        /// 配置 WinGet 数据源
        /// </summary>
        private void OnDataSourceSettingsClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.WinGetDataSource);
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private async void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                cachedSearchText = SearchText;
                SearchAppsResultKind = SearchAppsResultKind.Searching;
                SearchAppsCollection.Clear();
                await InitializeSearchAppsDataAsync();
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

        #endregion 第三部分：WinGet 搜索应用界面——挂载的事件

        #region 第四部分：WinGet 搜索应用界面——自定义事件

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
                            WinGetPageInstance.PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in WinGetPageInstance.PackageOperationCollection)
                                {
                                    if (packageOperationItem.AppID.Equals(searchApps.AppID) && packageOperationItem.PackageOperationKind is PackageOperationKind.Install)
                                    {
                                        packageOperationItem.PackageInstallProgressState = PackageInstallProgressState.Queued;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                WinGetPageInstance.PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
                // 处于下载中状态
                case PackageInstallProgressState.Downloading:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WinGetPageInstance.PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in WinGetPageInstance.PackageOperationCollection)
                                {
                                    if (packageOperationItem.AppID.Equals(searchApps.AppID) && packageOperationItem.PackageOperationKind is PackageOperationKind.Install)
                                    {
                                        packageOperationItem.PackageInstallProgressState = PackageInstallProgressState.Downloading;
                                        packageOperationItem.PackageOperationProgress = Math.Round(progress.DownloadProgress * 100, 2); packageOperationItem.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(progress.BytesDownloaded));
                                        packageOperationItem.TotalFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(progress.BytesRequired));
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                WinGetPageInstance.PackageOperationLock.Exit();
                            }
                        });

                        break;
                    }
                // 处于安装中状态
                case PackageInstallProgressState.Installing:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WinGetPageInstance.PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in WinGetPageInstance.PackageOperationCollection)
                                {
                                    if (packageOperationItem.AppID.Equals(searchApps.AppID) && packageOperationItem.PackageOperationKind is PackageOperationKind.Install)
                                    {
                                        packageOperationItem.PackageInstallProgressState = PackageInstallProgressState.Installing;
                                        packageOperationItem.PackageOperationProgress = 100;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                WinGetPageInstance.PackageOperationLock.Exit();
                            }
                        });

                        break;
                    }
                // 安装完成后等待其他操作状态
                case PackageInstallProgressState.PostInstall:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WinGetPageInstance.PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in WinGetPageInstance.PackageOperationCollection)
                                {
                                    if (packageOperationItem.AppID.Equals(searchApps.AppID) && packageOperationItem.PackageOperationKind is PackageOperationKind.Install)
                                    {
                                        packageOperationItem.PackageInstallProgressState = PackageInstallProgressState.PostInstall;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                WinGetPageInstance.PackageOperationLock.Exit();
                            }
                        });

                        break;
                    }
                // 处于安装完成状态
                case PackageInstallProgressState.Finished:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WinGetPageInstance.PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in WinGetPageInstance.PackageOperationCollection)
                                {
                                    if (packageOperationItem.AppID.Equals(searchApps.AppID) && packageOperationItem.PackageOperationKind is PackageOperationKind.Install)
                                    {
                                        packageOperationItem.PackageInstallProgressState = PackageInstallProgressState.Finished;
                                        packageOperationItem.PackageOperationProgress = 100;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                WinGetPageInstance.PackageOperationLock.Exit();
                            }
                        });

                        break;
                    }
            }
        }

        /// <summary>
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
                            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(WinGetOperationKind.SearchInstall, searchApps.AppName));

                            if (contentDialogResult is ContentDialogResult.Primary)
                            {
                                await Task.Run(() =>
                                {
                                    ShutdownHelper.Restart(ResourceService.GetLocalized("WinGet/RestartPC"), TimeSpan.FromSeconds(120));
                                });
                            }
                        });
                    }
                }
                else
                {
                    // 显示 WinGet 应用安装失败通知
                    KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                    AppNotificationBuilder appNotificationBuilder = new();
                    appNotificationBuilder.AddArgument("action", "OpenApp");
                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallFailed1"), searchApps.AppName));
                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetInstallFailed2"));
                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetInstallFailed3"));
                    AppNotificationButton installWithCommandButton = new(ResourceService.GetLocalized("Notification/InstallWithCommand"));
                    installWithCommandButton.Arguments.Add("action", winGetDataSourceName.Equals(default) ? string.Format("InstallWithCommand:{0}", searchApps.AppID) : string.Format("InstallWithCommand:{0}:{1}", searchApps.AppID, winGetDataSourceName.Key));
                    AppNotificationButton openDownloadFolderButton = new(ResourceService.GetLocalized("Notification/OpenDownloadFolder"));
                    openDownloadFolderButton.Arguments.Add("action", string.Format("OpenDownloadFolder:{0}", WinGetConfigService.DownloadFolder.Path));
                    appNotificationBuilder.AddButton(installWithCommandButton);
                    appNotificationBuilder.AddButton(openDownloadFolderButton);
                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                }

                DispatcherQueue.TryEnqueue(() =>
                {
                    // 应用安装失败，将当前任务状态修改为可安装状态
                    foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                    {
                        //if (searchAppsItem.AppID.Equals(searchApps.AppID) && searchAppsItem.IsInstalling)
                        //{
                        //    searchAppsItem.IsInstalling = false;
                        //    break;
                        //}
                    }

                    // 完成任务后从任务管理中删除任务
                    WinGetPageInstance.PackageOperationLock.Enter();
                    try
                    {
                        foreach (PackageOperationModel packageOperationItem in WinGetPageInstance.PackageOperationCollection)
                        {
                            if (packageOperationItem.AppID.Equals(searchApps.AppID) && packageOperationItem.PackageOperationKind is PackageOperationKind.Install)
                            {
                                WinGetPageInstance.PackageOperationCollection.Remove(packageOperationItem);
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                    finally
                    {
                        WinGetPageInstance.PackageOperationLock.Exit();
                    }
                });
            }
            // 安装过程已被用户取消
            else if (status is AsyncStatus.Canceled)
            {
                LogService.WriteLog(LoggingLevel.Information, "App installing operation canceled.", new Exception());

                DispatcherQueue.TryEnqueue(() =>
                {
                    // 应用安装失败，将当前任务状态修改为可安装状态
                    foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                    {
                        //if (searchAppsItem.AppID.Equals(searchApps.AppID) && searchAppsItem.IsInstalling)
                        //{
                        //    searchAppsItem.IsInstalling = false;
                        //    break;
                        //}
                    }

                    // 完成任务后从任务管理中删除任务
                    WinGetPageInstance.PackageOperationLock.Enter();
                    try
                    {
                        foreach (PackageOperationModel packageOperationItem in WinGetPageInstance.PackageOperationCollection)
                        {
                            if (packageOperationItem.AppID.Equals(searchApps.AppID) && packageOperationItem.PackageOperationKind is PackageOperationKind.Install)
                            {
                                WinGetPageInstance.PackageOperationCollection.Remove(packageOperationItem);
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                    finally
                    {
                        WinGetPageInstance.PackageOperationLock.Exit();
                    }
                });
            }
            // 安装过程发生错误
            else if (status is AsyncStatus.Error)
            {
                LogService.WriteLog(LoggingLevel.Error, "App installing failed.", result.ErrorCode);

                DispatcherQueue.TryEnqueue(() =>
                {
                    // 应用安装失败，将当前任务状态修改为可安装状态
                    foreach (SearchAppsModel searchAppsItem in SearchAppsCollection)
                    {
                        //if (searchAppsItem.AppID.Equals(searchApps.AppID) && searchAppsItem.IsInstalling)
                        //{
                        //    searchAppsItem.IsInstalling = false;
                        //    break;
                        //}
                    }

                    // 完成任务后从任务管理中删除任务
                    WinGetPageInstance.PackageOperationLock.Enter();
                    try
                    {
                        foreach (PackageOperationModel packageOperationItem in WinGetPageInstance.PackageOperationCollection)
                        {
                            if (packageOperationItem.AppID.Equals(searchApps.AppID) && packageOperationItem.PackageOperationKind is PackageOperationKind.Install)
                            {
                                WinGetPageInstance.PackageOperationCollection.Remove(packageOperationItem);
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                    finally
                    {
                        WinGetPageInstance.PackageOperationLock.Exit();
                    }
                });

                // 显示 WinGet 应用安装失败通知
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                AppNotificationBuilder appNotificationBuilder = new();
                appNotificationBuilder.AddArgument("action", "OpenApp");
                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallFailed1"), searchApps.AppName));
                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetInstallFailed2"));
                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetInstallFailed3"));
                AppNotificationButton installWithCommandButton = new(ResourceService.GetLocalized("Notification/InstallWithCommand"));
                installWithCommandButton.Arguments.Add("action", winGetDataSourceName.Equals(default) ? string.Format("InstallWithCommand:{0}", searchApps.AppID) : string.Format("InstallWithCommand:{0}:{1}", searchApps.AppID, winGetDataSourceName.Key));
                AppNotificationButton openDownloadFolderButton = new(ResourceService.GetLocalized("Notification/OpenDownloadFolder"));
                openDownloadFolderButton.Arguments.Add("action", string.Format("OpenDownloadFolder:{0}", WinGetConfigService.DownloadFolder.Path));
                appNotificationBuilder.AddButton(installWithCommandButton);
                appNotificationBuilder.AddButton(openDownloadFolderButton);
                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
            }

            result.Close();
        }

        #endregion 第四部分：WinGet 搜索应用界面——自定义事件

        /// <summary>
        /// 获取设置中选择的 WinGet 数据源
        /// </summary>

        private PackageCatalogReference GetPackageCatalogReference(PackageManager packageManager)
        {
            PackageCatalogReference packageCatalogReference = null;

            try
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                if (!winGetDataSourceName.Equals(default))
                {
                    // 使用内置源
                    if (winGetDataSourceName.Value)
                    {
                        foreach (KeyValuePair<string, PredefinedPackageCatalog> predefinedPackageCatalog in WinGetConfigService.PredefinedPackageCatalogList)
                        {
                            if (winGetDataSourceName.Key.Equals(predefinedPackageCatalog.Key))
                            {
                                packageCatalogReference = packageManager.GetPredefinedPackageCatalog(predefinedPackageCatalog.Value);
                                break;
                            }
                        }
                    }
                    // 使用自定义源
                    else
                    {
                        packageCatalogReference = packageManager.GetPackageCatalogByName(winGetDataSourceName.Key);
                    }
                }

                return packageCatalogReference;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Get package catalog reference failed", e);
                return packageCatalogReference;
            }
        }

        /// <summary>
        /// 初始化搜索应用数据
        /// </summary>
        private async Task InitializeSearchAppsDataAsync()
        {
            PackageManager packageManager = await Task.Run(() =>
            {
                return new PackageManager();
            });

            PackageCatalogReference packageCatalogReference = await Task.Run(() =>
            {
                return GetPackageCatalogReference(packageManager);
            });

            if (packageCatalogReference is not null)
            {
                (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<SearchAppsModel> searchAppsList) = await Task.Run(() =>
                {
                    return SearchAppsAsync(packageCatalogReference);
                });

                if (connectResult.Status is ConnectResultStatus.Ok)
                {
                    if (findPackagesResult.Status is FindPackagesResultStatus.Ok)
                    {
                        if (searchAppsList.Count is 0)
                        {
                            SearchAppsResultKind = SearchAppsResultKind.Failed;
                            SearchFailedContent = SearchAppsEmptyDescription;
                        }
                        else
                        {
                            foreach (SearchAppsModel searchAppsItem in searchAppsList)
                            {
                                SearchAppsCollection.Add(searchAppsItem);
                            }

                            SearchAppsResultKind = SearchAppsResultKind.Successfully;
                        }
                    }
                    else
                    {
                        SearchAppsResultKind = SearchAppsResultKind.Failed;
                        SearchFailedContent = string.Format(SearchAppsFailed, SearchFindAppsFailed, findPackagesResult.ExtendedErrorCode is not null ? findPackagesResult.ExtendedErrorCode.HResult : Unknown);
                    }
                }
                else
                {
                    SearchAppsResultKind = SearchAppsResultKind.Failed;
                    SearchFailedContent = string.Format(SearchAppsFailed, SearchCatalogReferenceFailed, findPackagesResult.ExtendedErrorCode is not null ? findPackagesResult.ExtendedErrorCode.HResult : Unknown);
                }
            }
            else
            {
                SearchAppsResultKind = SearchAppsResultKind.Failed;
                SearchFailedContent = SearchNotSelectSource;
            }
        }

        /// <summary>
        /// 搜索应用
        /// </summary>
        private async Task<(ConnectResult, FindPackagesResult, List<SearchAppsModel>)> SearchAppsAsync(PackageCatalogReference packageCatalogReference)
        {
            (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<SearchAppsModel> searchAppsList) searchAppsResult = ValueTuple.Create<ConnectResult, FindPackagesResult, List<SearchAppsModel>>(null, null, null);

            try
            {
                ConnectResult connectResult = await packageCatalogReference.ConnectAsync();
                searchAppsResult.connectResult = connectResult;

                if (connectResult is not null && connectResult.Status is ConnectResultStatus.Ok)
                {
                    FindPackagesOptions findPackagesOptions = new();

                    PackageMatchFilter packageMatchFilter = new()
                    {
                        Field = PackageMatchField,
                        Option = PackageFieldMatchOption,
                        Value = cachedSearchText
                    };

                    findPackagesOptions.Filters.Add(packageMatchFilter);

                    FindPackagesResult findPackagesResult = await connectResult.PackageCatalog.FindPackagesAsync(findPackagesOptions);
                    searchAppsResult.findPackagesResult = findPackagesResult;

                    if (findPackagesResult is not null && findPackagesResult.Status is FindPackagesResultStatus.Ok)
                    {
                        List<SearchAppsModel> searchAppsList = [];

                        int count = findPackagesResult.Matches.Count;
                        for (int index = 0; index < findPackagesResult.Matches.Count; index++)
                        {
                            MatchResult matchItem = findPackagesResult.Matches[index];

                            if (matchItem.CatalogPackage is not null && matchItem.CatalogPackage.Id is not null)
                            {
                                searchAppsList.Add(new SearchAppsModel()
                                {
                                    AppID = matchItem.CatalogPackage.Id,
                                    AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.Name) || matchItem.CatalogPackage.Name.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? Unknown : matchItem.CatalogPackage.Name,
                                    CatalogPackage = matchItem.CatalogPackage,
                                });
                            }
                        }

                        if (IsIncrease)
                        {
                            searchAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                        }
                        else
                        {
                            searchAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                        }

                        searchAppsResult.searchAppsList = searchAppsList;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Search winget app failed", e);
            }

            return searchAppsResult;
        }

        /// <summary>
        /// 获取搜索应用是否成功
        /// </summary>
        public Visibility GetSearchAppsSuccessfullyState(SearchAppsResultKind searchAppsResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? searchAppsResultKind.Equals(SearchAppsResultKind.Successfully) ? Visibility.Visible : Visibility.Collapsed : !searchAppsResultKind.Equals(SearchAppsResultKind.Successfully) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查搜索应用是否成功
        /// </summary>
        public Visibility CheckSearchAppsState(SearchAppsResultKind searchAppsResultKind, SearchAppsResultKind comparedSearchAppsResultKind)
        {
            return searchAppsResultKind.Equals(comparedSearchAppsResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取搜索框是否可进行搜索
        /// </summary>

        public bool GetSearchBoxEnabled(SearchAppsResultKind searchAppsResultKind)
        {
            return !searchAppsResultKind.Equals(SearchAppsResultKind.Searching);
        }

        /// <summary>
        /// 获取搜索框是否可进行搜索
        /// </summary>

        public bool GetSearchRefreshEnabled(SearchAppsResultKind searchAppsResultKind)
        {
            return !(searchAppsResultKind.Equals(SearchAppsResultKind.Searching) || searchAppsResultKind.Equals(SearchAppsResultKind.NotSearch));
        }
    }
}
