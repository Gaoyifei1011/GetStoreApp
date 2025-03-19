using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Controls.Store;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
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
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.UI.Controls.Store
{
    /// <summary>
    /// 查找链接控件
    /// </summary>
    [GeneratedBindableCustomProperty]
    public sealed partial class QueryLinksControl : StackPanel, INotifyPropertyChanged
    {
        private readonly Lock queryLinksLock = new();

        private bool isInitialized;
        private string sampleLink;
        private readonly string sampleTitle = ResourceService.GetLocalized("Store/SampleTitle");

        private string QueryLinksCountInfo { get; } = ResourceService.GetLocalized("Store/QueryLinksCountInfo");

        private TypeModel _selectedType;

        public TypeModel SelectedType
        {
            get { return _selectedType; }

            set
            {
                if (!Equals(_selectedType, value))
                {
                    _selectedType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
                }
            }
        }

        private ChannelModel _selectedChannel;

        public ChannelModel SelectedChannel
        {
            get { return _selectedChannel; }

            set
            {
                if (!Equals(_selectedChannel, value))
                {
                    _selectedChannel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedChannel)));
                }
            }
        }

        private string _linkPlaceHolderText = string.Empty;

        public string LinkPlaceHolderText
        {
            get { return _linkPlaceHolderText; }

            set
            {
                if (!Equals(_linkPlaceHolderText, value))
                {
                    _linkPlaceHolderText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinkPlaceHolderText)));
                }
            }
        }

        private string _linkText = string.Empty;

        public string LinkText
        {
            get { return _linkText; }

            set
            {
                if (!Equals(_linkText, value))
                {
                    _linkText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinkText)));
                }
            }
        }

        private bool _isNotQueryingLinks = true;

        public bool IsNotQueryingLinks
        {
            get { return _isNotQueryingLinks; }

            set
            {
                if (!Equals(_isNotQueryingLinks, value))
                {
                    _isNotQueryingLinks = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotQueryingLinks)));
                }
            }
        }

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

        private InfoBarSeverity _resultSeverity = InfoBarSeverity.Informational;

        public InfoBarSeverity ResultSeverity
        {
            get { return _resultSeverity; }

            set
            {
                if (!Equals(_resultSeverity, value))
                {
                    _resultSeverity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultSeverity)));
                }
            }
        }

        private string _stateInfoText = ResourceService.GetLocalized("Store/Welcome");

        public string StateInfoText
        {
            get { return _stateInfoText; }

            set
            {
                if (!Equals(_stateInfoText, value))
                {
                    _stateInfoText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StateInfoText)));
                }
            }
        }

        private bool _isRingActive;

        public bool IsRingActive
        {
            get { return _isRingActive; }

            set
            {
                if (!Equals(_isRingActive, value))
                {
                    _isRingActive = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRingActive)));
                }
            }
        }

        private bool _resultCotnrolVisable;

        public bool ResultControlVisable
        {
            get { return _resultCotnrolVisable; }

            set
            {
                if (!Equals(_resultCotnrolVisable, value))
                {
                    _resultCotnrolVisable = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultControlVisable)));
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

        private static readonly List<string> SampleLinkList = ["https://apps.microsoft.com/store/detail/9WZDNCRFJBMP", "9WZDNCRFJBMP",];

        private readonly List<InfoBarModel> QueryLinksInfoList = ResourceService.QueryLinksInfoList;

        public List<TypeModel> TypeList { get; } = ResourceService.TypeList;

        public List<ChannelModel> ChannelList { get; } = ResourceService.ChannelList;

        private ObservableCollection<HistoryModel> HistoryCollection { get; } = [];

        private ObservableCollection<QueryLinksModel> QueryLinksCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public QueryLinksControl()
        {
            InitializeComponent();

            SelectedType = TypeList[0];
            SelectedChannel = ChannelList[3];
            LinkText = string.Empty;
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制到剪贴板
        /// </summary>
        private async void OnCopyExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is HistoryModel historyItem)
            {
                string copyContent = string.Join('\t', new object[] { historyItem.HistoryAppName, historyItem.HistoryType.Value, historyItem.HistoryChannel.Value, historyItem.HistoryLink });
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyContent);

                await TeachingTipHelper.ShowAsync(new MainDataCopyTip(DataCopyKind.History, copyResult, false));
            }
        }

        /// <summary>
        /// 填入到文本框
        /// </summary>
        private void OnFillinExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is HistoryModel historyItem && MainWindow.Current.GetFrameContent() is StorePage storePage)
            {
                SelectedType = TypeList.Find(item => item.InternalName.Equals(historyItem.HistoryType.Key, StringComparison.OrdinalIgnoreCase));
                SelectedChannel = ChannelList.Find(item => item.InternalName.Equals(historyItem.HistoryChannel.Key, StringComparison.OrdinalIgnoreCase));
                LinkText = historyItem.HistoryLink;
            }
        }

        /// <summary>
        /// 根据设置存储的文件链接操作方式操作获取到的文件链接
        /// </summary>
        private async void OnDownloadExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is QueryLinksModel queryLinksItem)
            {
                bool isDownloadSuccessfully = false;
                await Task.Run(() =>
                {
                    List<DownloadSchedulerModel> downloadInfoList = [];
                    DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();

                    try
                    {
                        foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerService.GetDownloadSchedulerList())
                        {
                            downloadInfoList.Add(downloadSchedulerItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Query download scheduler list failed", e);
                    }
                    finally
                    {
                        DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
                    }

                    DownloadStorageService.DownloadStorageSemaphoreSlim?.Wait();

                    try
                    {
                        foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadStorageService.GetDownloadData())
                        {
                            downloadInfoList.Add(downloadSchedulerItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Query download storage data failed", e);
                    }
                    finally
                    {
                        DownloadStorageService.DownloadStorageSemaphoreSlim?.Release();
                    }

                    bool isExisted = false;
                    string downloadFilePath = Path.Combine(DownloadOptionsService.DownloadFolder.Path, queryLinksItem.FileName);

                    // 检查下载文件信息是否已存在
                    foreach (DownloadSchedulerModel downloadSchedulerItem in downloadInfoList)
                    {
                        if (queryLinksItem.FileName.Equals(downloadSchedulerItem.FileName, StringComparison.OrdinalIgnoreCase) && downloadFilePath.Equals(downloadSchedulerItem.FilePath, StringComparison.OrdinalIgnoreCase))
                        {
                            isExisted = true;
                            break;
                        }
                    }

                    if (!isExisted)
                    {
                        try
                        {
                            // 检查下载目录是否存在
                            if (!Directory.Exists(DownloadOptionsService.DownloadFolder.Path))
                            {
                                Directory.CreateDirectory(DownloadOptionsService.DownloadFolder.Path);
                            }

                            // 检查是否已有重复文件，如果有，直接删除
                            if (File.Exists(downloadFilePath))
                            {
                                File.Delete(downloadFilePath);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, "Delete existed file failed", e);
                        }

                        DownloadSchedulerService.CreateDownload(queryLinksItem.FileLink, downloadFilePath);
                        isDownloadSuccessfully = true;
                    }
                });

                // 显示下载任务创建成功消息
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.DownloadCreate, isDownloadSuccessfully));
            }
        }

        /// <summary>
        /// 打开指定项目的链接
        /// </summary>
        private async void OnOpenLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string fileLink && !string.IsNullOrEmpty(fileLink))
            {
                await Launcher.LaunchUriAsync(new Uri(fileLink));
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
                await TeachingTipHelper.ShowAsync(new MainDataCopyTip(DataCopyKind.ResultLink, copyResult, false));
            }
        }

        /// <summary>
        /// 复制指定项目的信息
        /// </summary>
        private async void OnCopyInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is QueryLinksModel queryLinksItem)
            {
                string copyInformation = string.Format("[\n{0}\n{1}\n{2}\n]\n",
                    queryLinksItem.FileName,
                    queryLinksItem.FileLink,
                    queryLinksItem.FileSize
                    );

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyInformation);
                await TeachingTipHelper.ShowAsync(new MainDataCopyTip(DataCopyKind.ResultInformation, copyResult, false));
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：查找链接控件——挂载的事件

        /// <summary>
        /// 查询链接控件初始化完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                sampleLink = SampleLinkList[0];
                LinkPlaceHolderText = sampleTitle + sampleLink;

                HistoryStorageService.QueryLinksCleared += () =>
                {
                    DispatcherQueue.TryEnqueue(HistoryCollection.Clear);
                };

                List<HistoryModel> queryLinksHistoryList = await Task.Run(HistoryStorageService.GetQueryLinksData);

                HistoryCollection.Clear();

                foreach (HistoryModel historyItem in queryLinksHistoryList)
                {
                    HistoryCollection.Add(historyItem);
                }
            }
        }

        /// <summary>
        /// 输入文本框内容发生改变时响应的事件
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            LinkText = (sender as TextBox).Text;
        }

        /// <summary>
        /// 点击回车键搜索应用
        /// </summary>
        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Enter)
            {
                QueryLinks();
            }
        }

        /// <summary>
        /// 类型修改选择后修改样例文本
        /// </summary>
        private void OnTypeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                SelectedType = TypeList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
                sampleLink = SampleLinkList[TypeList.FindIndex(item => item.InternalName == SelectedType.InternalName)];
                LinkPlaceHolderText = sampleTitle + sampleLink;

                LinkText = string.Empty;
            }
        }

        /// <summary>
        /// 通道选择修改
        /// </summary>
        private void OnChannelSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is not null)
            {
                SelectedChannel = ChannelList[Convert.ToInt32(radioMenuFlyoutItem.Tag)];
            }
        }

        /// <summary>
        /// 查询链接
        /// </summary>
        private void OnQueryLinksClicked(object sender, RoutedEventArgs args)
        {
            QueryLinks();
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private async void OnCopyQueryedAppInfoClicked(object sender, RoutedEventArgs args)
        {
            List<string> appInformationCopyStringList = [];
            appInformationCopyStringList.Add(ResourceService.GetLocalized("Store/QueryedAppName") + AppInfo.Name);
            appInformationCopyStringList.Add(ResourceService.GetLocalized("Store/QueryedAppPublisher") + AppInfo.Publisher);
            appInformationCopyStringList.Add(ResourceService.GetLocalized("Store/QueryedAppDescription"));
            appInformationCopyStringList.Add(AppInfo.Description);

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, appInformationCopyStringList));
            await TeachingTipHelper.ShowAsync(new MainDataCopyTip(DataCopyKind.AppInformation, copyResult));
        }

        /// <summary>
        /// 查看应用更多信息
        /// </summary>
        private async void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri(string.Format("https://apps.microsoft.com/store/detail/{0}", AppInfo.ProductID)));
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        private void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            queryLinksLock.Enter();

            try
            {
                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    queryLinksItem.IsSelectMode = true;
                    queryLinksItem.IsSelected = false;
                }

                IsSelectMode = true;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                queryLinksLock.Exit();
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            queryLinksLock.Enter();

            try
            {
                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    queryLinksItem.IsSelected = true;
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                queryLinksLock.Exit();
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            queryLinksLock.Enter();

            try
            {
                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    queryLinksItem.IsSelected = false;
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                queryLinksLock.Exit();
            }
        }

        /// <summary>
        /// 复制选定项目的内容
        /// </summary>
        private async void OnCopySelectedClicked(object sender, RoutedEventArgs args)
        {
            List<QueryLinksModel> selectedQueryLinksList = [];

            await Task.Run(() =>
            {
                queryLinksLock.Enter();

                try
                {
                    foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                    {
                        if (queryLinksItem.IsSelected)
                        {
                            selectedQueryLinksList.Add(queryLinksItem);
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    queryLinksLock.Exit();
                }
            });

            // 内容为空时显示空提示对话框
            if (selectedQueryLinksList.Count is 0)
            {
                await MainWindow.Current.ShowDialogAsync(new SelectEmptyPromptDialog());
                return;
            }
            else
            {
                List<string> queryLinksCopyStringList = [];

                await Task.Run(() =>
                {
                    foreach (QueryLinksModel queryLinksItem in selectedQueryLinksList)
                    {
                        queryLinksCopyStringList.Add(string.Format("[\n{0}\n{1}\n{2}\n]", queryLinksItem.FileName, queryLinksItem.FileLink, queryLinksItem.FileSize));
                    }
                });

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, queryLinksCopyStringList));
                await TeachingTipHelper.ShowAsync(new MainDataCopyTip(DataCopyKind.ResultInformation, copyResult, true, selectedQueryLinksList.Count));
            }
        }

        /// <summary>
        /// 复制选定项目的链接
        /// </summary>
        private async void OnCopySelectedLinkClicked(object sender, RoutedEventArgs args)
        {
            List<QueryLinksModel> selectedQueryLinksList = [];

            await Task.Run(() =>
            {
                queryLinksLock.Enter();

                try
                {
                    foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                    {
                        if (queryLinksItem.IsSelected)
                        {
                            selectedQueryLinksList.Add(queryLinksItem);
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    queryLinksLock.Exit();
                }
            });

            // 内容为空时显示空提示对话框
            if (selectedQueryLinksList.Count is 0)
            {
                await MainWindow.Current.ShowDialogAsync(new SelectEmptyPromptDialog());
                return;
            }
            else
            {
                List<string> queryLinksCopyStringList = [];

                await Task.Run(() =>
                {
                    foreach (QueryLinksModel queryLinksItem in selectedQueryLinksList)
                    {
                        queryLinksCopyStringList.Add(queryLinksItem.FileLink);
                    }
                });

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, queryLinksCopyStringList));
                await TeachingTipHelper.ShowAsync(new MainDataCopyTip(DataCopyKind.ResultLink, copyResult, true, selectedQueryLinksList.Count));
            }
        }

        /// <summary>
        /// 下载选定项目
        /// </summary>
        private async void OnDownloadSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<QueryLinksModel> selectedQueryLinksList = [];

            await Task.Run(() =>
            {
                queryLinksLock.Enter();

                try
                {
                    foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                    {
                        if (queryLinksItem.IsSelected)
                        {
                            selectedQueryLinksList.Add(queryLinksItem);
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    queryLinksLock.Exit();
                }
            });

            // 内容为空时显示空提示对话框
            if (selectedQueryLinksList.Count is 0)
            {
                await MainWindow.Current.ShowDialogAsync(new SelectEmptyPromptDialog());
                return;
            }
            else
            {
                bool isDownloadSuccessfully = false;
                List<DownloadSchedulerModel> downloadInfoList = [];

                await Task.Run(() =>
                {
                    DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();

                    try
                    {
                        foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerService.GetDownloadSchedulerList())
                        {
                            downloadInfoList.Add(downloadSchedulerItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Query download scheduler list failed", e);
                    }
                    finally
                    {
                        DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
                    }

                    DownloadStorageService.DownloadStorageSemaphoreSlim?.Wait();

                    try
                    {
                        foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadStorageService.GetDownloadData())
                        {
                            downloadInfoList.Add(downloadSchedulerItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Query download storage data failed", e);
                    }
                    finally
                    {
                        DownloadStorageService.DownloadStorageSemaphoreSlim?.Release();
                    }

                    foreach (QueryLinksModel queryLinksItem in selectedQueryLinksList)
                    {
                        bool isExisted = false;
                        string downloadFilePath = Path.Combine(DownloadOptionsService.DownloadFolder.Path, queryLinksItem.FileName);

                        // 检查下载文件信息是否已存在
                        foreach (DownloadSchedulerModel downloadSchedulerItem in downloadInfoList)
                        {
                            if (queryLinksItem.FileName.Equals(downloadSchedulerItem.FileName, StringComparison.OrdinalIgnoreCase) && downloadFilePath.Equals(downloadSchedulerItem.FilePath, StringComparison.OrdinalIgnoreCase))
                            {
                                isExisted = true;
                                break;
                            }
                        }

                        if (!isExisted)
                        {
                            try
                            {
                                // 检查下载目录是否存在
                                if (!Directory.Exists(DownloadOptionsService.DownloadFolder.Path))
                                {
                                    Directory.CreateDirectory(DownloadOptionsService.DownloadFolder.Path);
                                }

                                // 检查是否已有重复文件，如果有，直接删除
                                if (File.Exists(downloadFilePath))
                                {
                                    File.Delete(downloadFilePath);
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Delete existed file failed", e);
                                continue;
                            }

                            DownloadSchedulerService.CreateDownload(queryLinksItem.FileLink, downloadFilePath);
                            isDownloadSuccessfully = true;
                        }
                    }
                });

                // 显示下载任务创建成功消息
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.DownloadCreate, isDownloadSuccessfully));
                IsSelectMode = false;
                queryLinksLock.Enter();

                try
                {
                    foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                    {
                        queryLinksItem.IsSelectMode = false;
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    queryLinksLock.Exit();
                }
            }
        }

        /// <summary>
        /// 退出多选模式
        /// </summary>
        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            queryLinksLock.Enter();

            try
            {
                IsSelectMode = false;

                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    queryLinksItem.IsSelectMode = false;
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                queryLinksLock.Exit();
            }
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        private void OnItemInvoked(object sender, ItemsViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is QueryLinksModel queryLinksItem)
            {
                queryLinksLock.Enter();

                try
                {
                    int ClickedIndex = QueryLinksCollection.IndexOf(queryLinksItem);
                    QueryLinksCollection[ClickedIndex].IsSelected = !QueryLinksCollection[ClickedIndex].IsSelected;
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    queryLinksLock.Exit();
                }
            }
        }

        #endregion 第二部分：查找链接控件——挂载的事件

        /// <summary>
        /// 获取链接
        /// </summary>
        public void QueryLinks()
        {
            // 设置获取数据时的相关控件状态
            LinkText = string.IsNullOrEmpty(LinkText) ? sampleLink : LinkText;
            IsNotQueryingLinks = false;
            SetControlState(InfoBarSeverity.Informational);

            // 商店接口查询方式
            if (QueryLinksModeService.QueryLinksMode.Equals(QueryLinksModeService.QueryLinksModeList[0]))
            {
                Task.Run(async () =>
                {
                    List<QueryLinksModel> queryLinksList = [];

                    // 记录当前选定的选项和填入的内容
                    int typeIndex = TypeList.FindIndex(item => item.InternalName == SelectedType.InternalName);
                    int channelIndex = ChannelList.FindIndex(item => item.InternalName == SelectedChannel.InternalName);
                    string link = LinkText;

                    // 解析链接对应的产品 ID
                    string productId = SelectedType.Equals(TypeList[0]) ? QueryLinksHelper.ParseRequestContent(LinkText) : LinkText;

                    string cookie = await QueryLinksHelper.GetCookieAsync();

                    // 获取应用信息
                    Tuple<bool, AppInfoModel> appInformationResult = await QueryLinksHelper.GetAppInformationAsync(productId);

                    if (appInformationResult.Item1)
                    {
                        // 解析非商店应用数据
                        if (string.IsNullOrEmpty(appInformationResult.Item2.CategoryID))
                        {
                            List<QueryLinksModel> nonAppxPackagesList = await QueryLinksHelper.GetNonAppxPackagesAsync(productId);
                            foreach (QueryLinksModel nonAppxPackage in nonAppxPackagesList)
                            {
                                queryLinksList.Add(nonAppxPackage);
                            }
                        }
                        // 解析商店应用数据
                        else
                        {
                            string fileListXml = await QueryLinksHelper.GetFileListXmlAsync(cookie, appInformationResult.Item2.CategoryID, ChannelList[channelIndex].InternalName);

                            if (!string.IsNullOrEmpty(fileListXml))
                            {
                                List<QueryLinksModel> appxPackagesList = QueryLinksHelper.GetAppxPackages(fileListXml, ChannelList[channelIndex].InternalName);
                                foreach (QueryLinksModel appxPackage in appxPackagesList)
                                {
                                    bool isExisted = false;
                                    foreach (QueryLinksModel queryLinksItem in queryLinksList)
                                    {
                                        if (queryLinksItem.FileName.Equals(appxPackage.FileName) && queryLinksItem.FileLink.Equals(appxPackage.FileLink) && queryLinksItem.FileSize.Equals(queryLinksItem.FileSize))
                                        {
                                            isExisted = true;
                                        }
                                    }

                                    if (!isExisted)
                                    {
                                        queryLinksList.Add(appxPackage);
                                    }
                                }
                            }
                        }

                        // 按设置选项设置的内容过滤列表
                        if (LinkFilterService.EncryptedPackageFilterValue)
                        {
                            queryLinksList.RemoveAll(item =>
                            item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                            item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                            item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                            item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                            );
                        }

                        if (LinkFilterService.BlockMapFilterValue)
                        {
                            queryLinksList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
                        }

                        queryLinksList.Sort((item1, item2) => item1.FileName.CompareTo(item2.FileName));

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            IsNotQueryingLinks = true;

                            if (queryLinksList.Count > 0)
                            {
                                UpdateHistory(appInformationResult.Item2.Name, typeIndex, channelIndex, link);
                                SetControlState(InfoBarSeverity.Success);
                                ResultControlVisable = true;
                                IsAppInfoVisible = true;
                                IsPackagedApp = !string.IsNullOrEmpty(appInformationResult.Item2.CategoryID);

                                AppInfo.Name = appInformationResult.Item2.Name;
                                AppInfo.Publisher = appInformationResult.Item2.Publisher;
                                AppInfo.Description = appInformationResult.Item2.Description;
                                AppInfo.CategoryID = appInformationResult.Item2.CategoryID;
                                AppInfo.ProductID = appInformationResult.Item2.ProductID;

                                queryLinksLock.Enter();

                                try
                                {
                                    QueryLinksCollection.Clear();
                                    foreach (QueryLinksModel resultItem in queryLinksList)
                                    {
                                        QueryLinksCollection.Add(resultItem);
                                        Task.Delay(1);
                                    }
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                                finally
                                {
                                    queryLinksLock.Exit();
                                }
                            }
                            else
                            {
                                SetControlState(InfoBarSeverity.Warning);
                                ResultControlVisable = false;
                                IsAppInfoVisible = false;
                            }
                        });
                    }
                    else
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            IsNotQueryingLinks = true;
                            SetControlState(InfoBarSeverity.Error);
                            ResultControlVisable = false;
                        });
                    }
                });
            }

            // 第三方接口查询方式
            else if (QueryLinksModeService.QueryLinksMode.Equals(QueryLinksModeService.QueryLinksModeList[1]))
            {
                Task.Run(async () =>
                {
                    // 记录当前选定的选项和填入的内容
                    int typeIndex = TypeList.FindIndex(item => item.InternalName == SelectedType.InternalName);
                    int channelIndex = ChannelList.FindIndex(item => item.InternalName == SelectedChannel.InternalName);
                    string link = LinkText;

                    // 生成请求的内容
                    string generateContent = HtmlRequestHelper.GenerateRequestContent(SelectedType.InternalName, link, SelectedChannel.InternalName);

                    // 获取网页反馈回的原始数据
                    RequestModel httpRequestData = await HtmlRequestHelper.HttpRequestAsync(generateContent);

                    // 检查服务器返回获取的状态
                    InfoBarSeverity requestState = HtmlRequestHelper.CheckRequestState(httpRequestData);

                    if (requestState is InfoBarSeverity.Success)
                    {
                        HtmlParseHelper.InitializeParseData(httpRequestData);
                        string categoryId = HtmlParseHelper.HtmlParseCID().ToUpper();
                        List<QueryLinksModel> queryLinksList = [];

                        // CategoryID 为空，非打包应用
                        if (string.IsNullOrEmpty(categoryId))
                        {
                            List<QueryLinksModel> nonPackagedAppsList = HtmlParseHelper.HtmlParseNonPackagedAppLinks();

                            foreach (QueryLinksModel queryLinksItem in nonPackagedAppsList)
                            {
                                queryLinksList.Add(queryLinksItem);
                            }
                        }
                        else
                        {
                            List<QueryLinksModel> packagedAppsList = HtmlParseHelper.HtmlParsePackagedAppLinks();

                            // 按设置选项设置的内容过滤列表
                            if (LinkFilterService.EncryptedPackageFilterValue)
                            {
                                queryLinksList.RemoveAll(item =>
                                item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                                item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                                item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                                item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                                );
                            }

                            if (LinkFilterService.BlockMapFilterValue)
                            {
                                queryLinksList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
                            }

                            foreach (QueryLinksModel queryLinksItem in packagedAppsList)
                            {
                                queryLinksList.Add(queryLinksItem);
                            }
                        }

                        queryLinksList.Sort((item1, item2) => item1.FileName.CompareTo(item2.FileName));

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            IsNotQueryingLinks = true;
                            IsAppInfoVisible = false;
                            IsPackagedApp = !string.IsNullOrEmpty(categoryId);

                            UpdateHistory(categoryId, typeIndex, channelIndex, link);
                            SetControlState(InfoBarSeverity.Success);
                            ResultControlVisable = true;

                            queryLinksLock.Enter();

                            try
                            {
                                QueryLinksCollection.Clear();
                                foreach (QueryLinksModel resultItem in queryLinksList)
                                {
                                    QueryLinksCollection.Add(resultItem);
                                    Task.Delay(1);
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                queryLinksLock.Exit();
                            }
                        });
                    }
                    else if (requestState is InfoBarSeverity.Warning)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            IsNotQueryingLinks = true;
                            SetControlState(InfoBarSeverity.Warning);
                            ResultControlVisable = false;
                            IsAppInfoVisible = false;
                        });
                    }
                    else if (requestState is InfoBarSeverity.Error)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            IsNotQueryingLinks = true;
                            SetControlState(InfoBarSeverity.Error);
                            ResultControlVisable = false;
                            IsAppInfoVisible = false;
                        });
                    }
                });
            }
        }

        /// <summary>
        /// 设置控件的状态
        /// </summary>
        private void SetControlState(InfoBarSeverity severity)
        {
            int state = Convert.ToInt32(severity);

            ResultSeverity = QueryLinksInfoList[state].Severity;
            StateInfoText = QueryLinksInfoList[state].Message;
            IsRingActive = QueryLinksInfoList[state].PrRingActValue;
        }

        /// <summary>
        /// 更新历史记录，包括主页历史记录内容、数据库中的内容和任务栏跳转列表中的内容
        /// </summary>
        private void UpdateHistory(string appName, int selectedType, int selectedChannel, string link)
        {
            Task.Run(() =>
            {
                // 计算时间戳
                long timeStamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
                string historyKey = HashAlgorithmHelper.GenerateHistoryKey(TypeList[selectedType].InternalName, ChannelList[selectedChannel].InternalName, link);

                List<HistoryModel> historyList = [.. HistoryCollection];

                int index = historyList.FindIndex(item => item.HistoryKey.Equals(historyKey, StringComparison.OrdinalIgnoreCase));

                // 不存在直接添加
                if (index is -1)
                {
                    HistoryModel historyItem = new()
                    {
                        CreateTimeStamp = timeStamp,
                        HistoryKey = historyKey,
                        HistoryAppName = appName,
                        HistoryType = new KeyValuePair<string, string>(TypeList[selectedType].InternalName, TypeList[selectedType].DisplayName),
                        HistoryChannel = new KeyValuePair<string, string>(ChannelList[selectedChannel].InternalName, ChannelList[selectedChannel].DisplayName),
                        HistoryLink = link
                    };

                    historyList.Insert(0, historyItem);
                    if (historyList.Count is 4)
                    {
                        historyList.RemoveAt(historyList.Count - 1);
                    }
                    HistoryStorageService.SaveQueryLinksData(historyList);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        if (HistoryCollection.Count is 3)
                        {
                            HistoryCollection.RemoveAt(HistoryCollection.Count - 1);
                        }

                        HistoryCollection.Insert(0, historyItem);
                    });
                }
                // 存在则修改原来项的时间戳，并调整顺序
                else
                {
                    HistoryModel historyItem = historyList[index];
                    historyItem.CreateTimeStamp = timeStamp;
                    historyItem.HistoryAppName = appName;
                    historyList.RemoveAt(index);
                    historyList.Insert(0, historyItem);
                    HistoryStorageService.SaveQueryLinksData(historyList);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        HistoryCollection.RemoveAt(index);
                        HistoryCollection.Insert(0, historyItem);
                    });
                }
            });
        }
    }
}
