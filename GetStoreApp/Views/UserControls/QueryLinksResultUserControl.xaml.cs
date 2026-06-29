using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.UserControls
{
    /// <summary>
    /// 查询链接结果用户控件
    /// </summary>
    public sealed partial class QueryLinksResultUserControl : UserControl, INotifyPropertyChanged
    {
        private readonly string QueriedAppDescriptionString = ResourceService.GetLocalized("QueryLinksResult/QueriedAppDescription");
        private readonly string QueriedAppNameString = ResourceService.GetLocalized("QueryLinksResult/QueriedAppName");
        private readonly string QueriedAppPublisherString = ResourceService.GetLocalized("QueryLinksResult/QueriedAppPublisher");
        private readonly string QueryLinksResultCountInfoString = ResourceService.GetLocalized("QueryLinksResult/QueryLinksResultCountInfo");
        private readonly Lock queryLinksResultLock = new();
        private bool isInitialized;
        private StorePage storePage;

        private AppInfoModel _appInfo = new();

        public AppInfoModel AppInfo
        {
            get { return _appInfo; }

            set
            {
                if (!Equals(_appInfo, value))
                {
                    _appInfo = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppInfo)));
                }
            }
        }

        private bool _isAppInfoVisible;

        public bool IsAppInfoVisible
        {
            get { return _isAppInfoVisible; }

            set
            {
                if (!Equals(_isAppInfoVisible, value))
                {
                    _isAppInfoVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAppInfoVisible)));
                }
            }
        }

        private bool _isPackagedApp;

        public bool IsPackagedApp
        {
            get { return _isPackagedApp; }

            set
            {
                if (!Equals(_isPackagedApp, value))
                {
                    _isPackagedApp = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPackagedApp)));
                }
            }
        }

        private bool _isSelectMode;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                if (!Equals(_isSelectMode, value))
                {
                    _isSelectMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectMode)));
                }
            }
        }

        private ObservableCollection<QueryLinksResultModel> QueryLinksResultCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public QueryLinksResultUserControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 根据设置存储的文件链接操作方式操作获取到的文件链接
        /// </summary>
        private async void OnDownloadExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is QueryLinksResultModel queryLinksResult)
            {
                string downloadFolder = string.Empty;

                // 手动设置下载目录
                if (DownloadOptionsService.ManualSetDownloadFolder)
                {
                    try
                    {
                        FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                        {
                            SuggestedStartFolder = DownloadOptionsService.DownloadFolder
                        };

                        if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                        {
                            downloadFolder = pickFolderResult.Path;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksResultUserControl), nameof(OnDownloadSelectedClicked), 1, e);
                        downloadFolder = DownloadOptionsService.DownloadFolder;
                    }
                }
                else
                {
                    downloadFolder = DownloadOptionsService.DownloadFolder;
                }

                if (!string.IsNullOrEmpty(downloadFolder))
                {
                    bool isDownloadSuccessfully = await Task.Run(() =>
                    {
                        bool isDownloadSuccessfully = false;
                        List<DownloadSchedulerModel> downloadSchedulerList = [];
                        DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();

                        try
                        {
                            downloadSchedulerList.AddRange(DownloadSchedulerService.DownloadSchedulerList);

                            if (!DownloadSchedulerService.IsDownloadingPageInitialized)
                            {
                                downloadSchedulerList.AddRange(DownloadSchedulerService.DownloadFailedList);
                            }

                            downloadSchedulerList.AddRange(DownloadStorageService.GetDownloadData());
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksResultUserControl), nameof(OnDownloadExecuteRequested), 1, e);
                        }
                        finally
                        {
                            DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
                        }

                        string downloadFilePath = Path.Combine(downloadFolder, queryLinksResult.FileName);

                        try
                        {
                            // 检查下载目录是否存在
                            if (!Directory.Exists(downloadFolder))
                            {
                                Directory.CreateDirectory(downloadFolder);
                            }

                            // 检查是否已有重复文件，如果有，保存文件名重复，追加(1)(2)......
                            if (File.Exists(downloadFilePath))
                            {
                                downloadFilePath = RenameDuplicatedFile(downloadFilePath);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksResultUserControl), nameof(OnDownloadExecuteRequested), 2, e);
                        }

                        DownloadSchedulerService.CreateDownload(queryLinksResult.FileLink, downloadFilePath);
                        isDownloadSuccessfully = true;
                        return isDownloadSuccessfully;
                    });

                    // 显示下载任务创建成功消息
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DownloadCreate, isDownloadSuccessfully));
                }
            }
        }

        /// <summary>
        /// 打开指定项目的链接
        /// </summary>
        private void OnOpenLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string fileLink && !string.IsNullOrEmpty(fileLink))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri(fileLink));
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                });
            }
        }

        /// <summary>
        /// 复制指定项目的链接
        /// </summary>
        private async void OnCopyLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string fileLink && !string.IsNullOrEmpty(fileLink))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(fileLink);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制指定项目的信息
        /// </summary>
        private async void OnCopyInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is QueryLinksResultModel queryLinksResult)
            {
                string copyInformation = await Task.Run(() =>
                {
                    return string.Format("[\n{0}\n{1}\n{2}\n]\n", queryLinksResult.FileName, queryLinksResult.FileLink, queryLinksResult.FileSize);
                });

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyInformation);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：查询链接结果用户控件——挂载的事件

        /// <summary>
        /// 返回主页
        /// </summary>
        private void OnBackToHomePageClicked(object sender, RoutedEventArgs args)
        {
            storePage.StoreControl = StoreControl.StoreSelector;
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private async void OnCopyQueriedAppInfoClicked(object sender, RoutedEventArgs args)
        {
            string appInformationCopyString = await Task.Run(() =>
            {
                List<string> appInformationCopyStringList = [];
                appInformationCopyStringList.Add(QueriedAppNameString + AppInfo.Name);
                appInformationCopyStringList.Add(QueriedAppPublisherString + AppInfo.Publisher);
                appInformationCopyStringList.Add(QueriedAppDescriptionString);
                appInformationCopyStringList.Add(AppInfo.Description);

                return string.Join(Environment.NewLine, appInformationCopyStringList);
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(appInformationCopyString);
            await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
        }

        /// <summary>
        /// 查看应用更多信息
        /// </summary>
        private void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (Equals(AppLinkOpenModeService.AppLinkOpenMode, AppLinkOpenModeService.AppLinkOpenModeList[0]))
                    {
                        await Launcher.LaunchUriAsync(new Uri("getstoreappwebview:"), new LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
                            {
                                {"AppLink", string.Format("https://apps.microsoft.com/store/detail/{0}", AppInfo.ProductID) },
                            });
                    }
                    else if (Equals(AppLinkOpenModeService.AppLinkOpenMode, AppLinkOpenModeService.AppLinkOpenModeList[1]))
                    {
                        await Launcher.LaunchUriAsync(new Uri(string.Format("https://apps.microsoft.com/store/detail/{0}", AppInfo.ProductID)));
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        private void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            queryLinksResultLock.Enter();

            try
            {
                foreach (QueryLinksResultModel queryLinksResultItem in QueryLinksResultCollection)
                {
                    queryLinksResultItem.IsSelectMode = true;
                    queryLinksResultItem.IsSelected = false;
                }

                IsSelectMode = true;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                queryLinksResultLock.Exit();
            }
        }

        /// <summary>
        /// 全选 / 全部不选
        /// </summary>
        private void OnSelectHeaderClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            if (IsSelectMode)
            {
                queryLinksResultLock.Enter();

                try
                {
                    bool isAllSelected = true;

                    foreach (QueryLinksResultModel queryLinksResultItem in QueryLinksResultCollection)
                    {
                        if (!queryLinksResultItem.IsSelected)
                        {
                            isAllSelected = false;
                            break;
                        }
                    }

                    if (!isAllSelected)
                    {
                        foreach (QueryLinksResultModel queryLinksResultItem in QueryLinksResultCollection)
                        {
                            queryLinksResultItem.IsSelected = true;
                        }
                    }
                    else
                    {
                        foreach (QueryLinksResultModel queryLinksResultItem in QueryLinksResultCollection)
                        {
                            queryLinksResultItem.IsSelected = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    queryLinksResultLock.Exit();
                }
            }
        }

        /// <summary>
        /// 复制选定项目的内容
        /// </summary>
        private async void OnCopySelectedClicked(object sender, RoutedEventArgs args)
        {
            List<QueryLinksResultModel> selectedQueryLinksResultList = await Task.Run(() =>
            {
                List<QueryLinksResultModel> selectedQueryLinksResultList = [];
                queryLinksResultLock.Enter();

                try
                {
                    foreach (QueryLinksResultModel queryLinksResultItem in QueryLinksResultCollection)
                    {
                        if (queryLinksResultItem.IsSelected)
                        {
                            selectedQueryLinksResultList.Add(queryLinksResultItem);
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    queryLinksResultLock.Exit();
                }

                return selectedQueryLinksResultList;
            });

            // 内容为空时显示空提示对话框
            if (selectedQueryLinksResultList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectEmpty));
                return;
            }
            else
            {
                string queryLinksResultCopyString = await Task.Run(() =>
                {
                    List<string> queryLinksCopyStringList = [];

                    foreach (QueryLinksResultModel queryLinksResultItem in selectedQueryLinksResultList)
                    {
                        queryLinksCopyStringList.Add(string.Format("[\n{0}\n{1}\n{2}\n]", queryLinksResultItem.FileName, queryLinksResultItem.FileLink, queryLinksResultItem.FileSize));
                    }

                    return string.Join(Environment.NewLine, queryLinksCopyStringList);
                });

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(queryLinksResultCopyString);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制选定项目的链接
        /// </summary>
        private async void OnCopySelectedLinkClicked(object sender, RoutedEventArgs args)
        {
            List<QueryLinksResultModel> selectedQueryLinksResultList = await Task.Run(() =>
            {
                List<QueryLinksResultModel> selectedQueryLinksResultList = [];
                queryLinksResultLock.Enter();

                try
                {
                    foreach (QueryLinksResultModel queryLinksResultItem in QueryLinksResultCollection)
                    {
                        if (queryLinksResultItem.IsSelected)
                        {
                            selectedQueryLinksResultList.Add(queryLinksResultItem);
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    queryLinksResultLock.Exit();
                }

                return selectedQueryLinksResultList;
            });

            // 内容为空时显示空提示对话框
            if (selectedQueryLinksResultList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectEmpty));
                return;
            }
            else
            {
                string queryLinksResultCopyString = await Task.Run(() =>
                {
                    List<string> queryLinksResultCopyStringList = [];

                    foreach (QueryLinksResultModel queryLinksResultItem in selectedQueryLinksResultList)
                    {
                        queryLinksResultCopyStringList.Add(queryLinksResultItem.FileLink);
                    }

                    return string.Join(Environment.NewLine, queryLinksResultCopyStringList);
                });

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(queryLinksResultCopyString);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 下载选定项目
        /// </summary>
        private async void OnDownloadSelectedClicked(object sender, RoutedEventArgs args)
        {
            IsSelectMode = false;
            queryLinksResultLock.Enter();

            try
            {
                foreach (QueryLinksResultModel queryLinksResultItem in QueryLinksResultCollection)
                {
                    queryLinksResultItem.IsSelectMode = false;
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                queryLinksResultLock.Exit();
            }

            string downloadFolder = string.Empty;

            // 手动设置下载目录
            if (DownloadOptionsService.ManualSetDownloadFolder)
            {
                try
                {
                    FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                    {
                        SuggestedStartFolder = DownloadOptionsService.DownloadFolder
                    };

                    if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                    {
                        downloadFolder = pickFolderResult.Path;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksResultUserControl), nameof(OnDownloadSelectedClicked), 1, e);
                    downloadFolder = DownloadOptionsService.DownloadFolder;
                }
            }
            else
            {
                downloadFolder = DownloadOptionsService.DownloadFolder;
            }

            if (!string.IsNullOrEmpty(downloadFolder))
            {
                List<QueryLinksResultModel> selectedQueryLinksResultList = await Task.Run(() =>
                {
                    List<QueryLinksResultModel> selectedQueryLinksResultList = [];
                    queryLinksResultLock.Enter();

                    try
                    {
                        foreach (QueryLinksResultModel queryLinksResultItem in QueryLinksResultCollection)
                        {
                            if (queryLinksResultItem.IsSelected)
                            {
                                selectedQueryLinksResultList.Add(queryLinksResultItem);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                    finally
                    {
                        queryLinksResultLock.Exit();
                    }

                    return selectedQueryLinksResultList;
                });

                // 内容为空时显示空提示对话框
                if (selectedQueryLinksResultList.Count is 0)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectEmpty));
                    return;
                }
                else
                {
                    bool isDownloadSuccessfully = false;

                    List<DownloadSchedulerModel> downloadSchedulerList = await Task.Run(() =>
                    {
                        List<DownloadSchedulerModel> downloadSchedulerList = [];
                        DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();

                        try
                        {
                            downloadSchedulerList.AddRange(DownloadSchedulerService.DownloadSchedulerList);

                            if (!DownloadSchedulerService.IsDownloadingPageInitialized)
                            {
                                downloadSchedulerList.AddRange(DownloadSchedulerService.DownloadFailedList);
                            }

                            downloadSchedulerList.AddRange(DownloadStorageService.GetDownloadData());
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksResultUserControl), nameof(OnDownloadSelectedClicked), 2, e);
                        }
                        finally
                        {
                            DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
                        }

                        foreach (QueryLinksResultModel queryLinksResultItem in selectedQueryLinksResultList)
                        {
                            string downloadFilePath = Path.Combine(downloadFolder, queryLinksResultItem.FileName);

                            try
                            {
                                // 检查下载目录是否存在
                                if (!Directory.Exists(downloadFolder))
                                {
                                    Directory.CreateDirectory(downloadFolder);
                                }

                                // 检查是否已有重复文件，如果有，保存文件名重复，追加(1)(2)......
                                if (File.Exists(downloadFilePath))
                                {
                                    downloadFilePath = RenameDuplicatedFile(downloadFilePath);
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksResultUserControl), nameof(OnDownloadSelectedClicked), 3, e);
                                continue;
                            }

                            DownloadSchedulerService.CreateDownload(queryLinksResultItem.FileLink, downloadFilePath);

                            if (!isDownloadSuccessfully)
                            {
                                isDownloadSuccessfully = true;
                            }
                        }

                        return downloadSchedulerList;
                    });

                    // 显示下载任务创建成功消息
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.DownloadCreate, isDownloadSuccessfully));
                }
            }
        }

        /// <summary>
        /// 退出多选模式
        /// </summary>
        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            queryLinksResultLock.Enter();

            try
            {
                IsSelectMode = false;

                foreach (QueryLinksResultModel queryLinksResultItem in QueryLinksResultCollection)
                {
                    queryLinksResultItem.IsSelectMode = false;
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                queryLinksResultLock.Exit();
            }
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        private void OnItemClick(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is QueryLinksResultModel queryLinksResult)
            {
                queryLinksResultLock.Enter();

                try
                {
                    int ClickedIndex = QueryLinksResultCollection.IndexOf(queryLinksResult);
                    QueryLinksResultCollection[ClickedIndex].IsSelected = !QueryLinksResultCollection[ClickedIndex].IsSelected;
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    queryLinksResultLock.Exit();
                }
            }
        }

        #endregion 第二部分：查询链接结果用户控件——挂载的事件

        /// <summary>
        /// 初始化查询链接结果用户控件
        /// </summary>
        public void InitializeQueryLinksResult(StorePage storePageData)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                storePage = storePageData;
            }
        }

        /// <summary>
        /// 更新查询链接结果
        /// </summary>
        public void UpdateQueryLinksResultData(AppInfoModel appInfo, bool isPackagedApp, List<QueryLinksResultModel> queryLinksList)
        {
            IsAppInfoVisible = false;
            IsPackagedApp = isPackagedApp;
            AppInfo = null;
            QueryLinksResultCollection.Clear();

            if (appInfo is not null)
            {
                IsAppInfoVisible = true;
                AppInfo = appInfo;
            }
            else
            {
                IsAppInfoVisible = false;
            }

            queryLinksResultLock.Enter();

            try
            {
                foreach (QueryLinksResultModel queryLinksResultItem in queryLinksList)
                {
                    QueryLinksResultCollection.Add(queryLinksResultItem);
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                queryLinksResultLock.Exit();
            }
        }

        /// <summary>
        /// 重命名重复文件
        /// </summary>
        private string RenameDuplicatedFile(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            string filename = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            int counter = 1;
            string newFilename = string.Format("{0}{1}", filename, extension);
            string newFullPath = Path.Combine(directory, newFilename);
            while (File.Exists(newFullPath))
            {
                newFilename = string.Format("{0}({1}){2}", filename, counter, extension);
                newFullPath = Path.Combine(directory, newFilename);
                counter++;
            }
            filePath = newFullPath;
            return filePath;
        }
    }
}
