using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Store;
using GetStoreApp.Models;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.History;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
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
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 查找链接页面
    /// </summary>

    public sealed partial class QueryLinksPage : Page, INotifyPropertyChanged
    {
        private readonly string FastString = ResourceService.GetLocalized("QueryLinks/Fast");
        private readonly string QueriedAppDescriptionString = ResourceService.GetLocalized("QueryLinks/QueriedAppDescription");
        private readonly string QueriedAppNameString = ResourceService.GetLocalized("QueryLinks/QueriedAppName");
        private readonly string QueriedAppPublisherString = ResourceService.GetLocalized("QueryLinks/QueriedAppPublisher");
        private readonly string QueryLinksCountInfoString = ResourceService.GetLocalized("QueryLinks/QueryLinksCountInfo");
        private readonly string InfoBarErrorString = ResourceService.GetLocalized("QueryLinks/InfoBarError");
        private readonly string InfoBarGettingString = ResourceService.GetLocalized("QueryLinks/InfoBarGetting");
        private readonly string InfoBarSuccessString = ResourceService.GetLocalized("QueryLinks/InfoBarSuccess");
        private readonly string InfoBarWarningString = ResourceService.GetLocalized("QueryLinks/InfoBarWarning");
        private readonly string ProductIDString = ResourceService.GetLocalized("QueryLinks/ProductID");
        private readonly string RetailString = ResourceService.GetLocalized("QueryLinks/Retail");
        private readonly string RPString = ResourceService.GetLocalized("QueryLinks/RP");
        private readonly string SampleTitleString = ResourceService.GetLocalized("QueryLinks/SampleTitle");
        private readonly string SlowString = ResourceService.GetLocalized("QueryLinks/Slow");
        private readonly string URLString = ResourceService.GetLocalized("QueryLinks/URL");
        private readonly string WelcomeString = ResourceService.GetLocalized("QueryLinks/Welcome");
        private readonly Lock queryLinksLock = new();

        private bool isInitialized;
        private string sampleLink;

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
                if (!string.Equals(_linkPlaceHolderText, value))
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
                if (!string.Equals(_linkText, value))
                {
                    _linkText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinkText)));
                }
            }
        }

        private bool _isQueryingLinks = false;

        public bool IsQueryingLinks
        {
            get { return _isQueryingLinks; }

            set
            {
                if (!Equals(_isQueryingLinks, value))
                {
                    _isQueryingLinks = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsQueryingLinks)));
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

        private string _stateInfoText;

        public string StateInfoText
        {
            get { return _stateInfoText; }

            set
            {
                if (!string.Equals(_stateInfoText, value))
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

        private bool _resultControlVisible;

        public bool ResultControlVisible
        {
            get { return _resultControlVisible; }

            set
            {
                if (!Equals(_resultControlVisible, value))
                {
                    _resultControlVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultControlVisible)));
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

        private List<string> SampleLinkList { get; } = ["https://apps.microsoft.com/store/detail/9WZDNCRFJBMP", "9WZDNCRFJBMP",];

        public List<TypeModel> TypeList { get; } = [];

        public List<ChannelModel> ChannelList { get; } = [];

        private List<InfoBarModel> InfoBarList { get; } = [];

        private ObservableCollection<HistoryModel> HistoryCollection { get; } = [];

        private ObservableCollection<QueryLinksModel> QueryLinksCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public QueryLinksPage()
        {
            InitializeComponent();
            StateInfoText = WelcomeString;

            TypeList.Add(new TypeModel
            {
                DisplayName = URLString,
                InternalName = "url",
                ShortName = "url"
            });
            TypeList.Add(new TypeModel
            {
                DisplayName = ProductIDString,
                InternalName = "ProductId",
                ShortName = "pid"
            });

            ChannelList.Add(new ChannelModel
            {
                DisplayName = FastString,
                InternalName = "WIF",
                ShortName = "wif"
            });
            ChannelList.Add(new ChannelModel
            {
                DisplayName = SlowString,
                InternalName = "WIS",
                ShortName = "wis"
            });
            ChannelList.Add(new ChannelModel
            {
                DisplayName = RPString,
                InternalName = "RP",
                ShortName = "rp"
            });
            ChannelList.Add(new ChannelModel
            {
                DisplayName = RetailString,
                InternalName = "Retail",
                ShortName = "rt"
            });

            SelectedType = TypeList[0];
            SelectedChannel = ChannelList[3];
            LinkText = string.Empty;

            InfoBarList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Informational,
                Message = InfoBarGettingString,
                PrRingActValue = true,
                PrRingVisValue = true
            });
            InfoBarList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Success,
                Message = InfoBarSuccessString,
                PrRingActValue = false,
                PrRingVisValue = false
            });
            InfoBarList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Warning,
                Message = InfoBarWarningString,
                PrRingActValue = false,
                PrRingVisValue = false
            });
            InfoBarList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Error,
                Message = InfoBarErrorString,
                PrRingActValue = false,
                PrRingVisValue = false
            });
        }

        /// <summary>
        /// 点击回车键搜索应用
        /// </summary>
        protected override async void OnKeyDown(KeyRoutedEventArgs args)
        {
            base.OnKeyDown(args);

            if (args.Key is VirtualKey.Enter && !IsQueryingLinks && !IsSelectMode)
            {
                await QueryLinksAsync();
            }
        }

        #region 第一部分：重载父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                sampleLink = SampleLinkList[0];
                LinkPlaceHolderText = SampleTitleString + sampleLink;

                HistoryStorageService.QueryLinksCleared += () =>
                {
                    DispatcherQueue.TryEnqueue(HistoryCollection.Clear);
                };

                List<HistoryModel> queryLinksHistoryList = await Task.Run(HistoryStorageService.GetQueryLinksData);

                HistoryCollection.Clear();

                foreach (HistoryModel historyItem in queryLinksHistoryList)
                {
                    historyItem.HistoryTypeName = TypeList.Find(item => string.Equals(item.InternalName, historyItem.HistoryType, StringComparison.OrdinalIgnoreCase)) is TypeModel typeItem ? typeItem.DisplayName : string.Empty;
                    historyItem.HistoryChannelName = ChannelList.Find(item => string.Equals(item.InternalName, historyItem.HistoryChannel, StringComparison.OrdinalIgnoreCase)) is ChannelModel channelItem ? channelItem.DisplayName : string.Empty;
                    HistoryCollection.Add(historyItem);
                }
            }

            if (args.Parameter is List<string> dataList && dataList.Count is 3)
            {
                SelectedType = Convert.ToInt32(dataList[0]) is -1 ? TypeList[0] : TypeList[Convert.ToInt32(dataList[0])];
                SelectedChannel = Convert.ToInt32(dataList[1]) is -1 ? ChannelList[3] : ChannelList[Convert.ToInt32(dataList[1])];
                LinkText = dataList[2] is "PlaceHolderText" ? string.Empty : dataList[2];
            }
        }

        #endregion 第一部分：重载父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 填入到文本框
        /// </summary>
        private void OnFillInExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is HistoryModel history && MainWindow.Current.GetFrameContent() is StorePage storePage)
            {
                SelectedType = TypeList.Find(item => string.Equals(item.InternalName, history.HistoryType, StringComparison.OrdinalIgnoreCase));
                SelectedChannel = ChannelList.Find(item => string.Equals(item.InternalName, history.HistoryChannel, StringComparison.OrdinalIgnoreCase));
                LinkText = history.HistoryLink;
            }
        }

        /// <summary>
        /// 根据设置存储的文件链接操作方式操作获取到的文件链接
        /// </summary>
        private async void OnDownloadExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is QueryLinksModel queryLinks)
            {
                string downloadFolder = string.Empty;

                // 手动设置下载目录
                if (DownloadOptionsService.ManualSetDownloadFolder)
                {
                    try
                    {
                        FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                        {
                            SuggestedStartLocation = PickerLocationId.Downloads
                        };

                        if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                        {
                            downloadFolder = pickFolderResult.Path;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksPage), nameof(OnDownloadSelectedClicked), 1, e);
                        downloadFolder = DownloadOptionsService.DownloadFolder;
                    }
                }
                else
                {
                    downloadFolder = DownloadOptionsService.DownloadFolder;
                }

                if (!string.IsNullOrEmpty(downloadFolder))
                {
                    bool isDownloadSuccessfully = false;
                    await Task.Run(() =>
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
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksPage), nameof(OnDownloadExecuteRequested), 1, e);
                        }
                        finally
                        {
                            DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
                        }

                        string downloadFilePath = Path.Combine(downloadFolder, queryLinks.FileName);

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
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksPage), nameof(OnDownloadExecuteRequested), 2, e);
                        }

                        DownloadSchedulerService.CreateDownload(queryLinks.FileLink, downloadFilePath);
                        isDownloadSuccessfully = true;
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
            if (args.Parameter is QueryLinksModel queryLinks)
            {
                string copyInformation = await Task.Run(() =>
                {
                    return string.Format("[\n{0}\n{1}\n{2}\n]\n", queryLinks.FileName, queryLinks.FileLink, queryLinks.FileSize);
                });

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyInformation);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：查找链接页面——挂载的事件

        /// <summary>
        /// 输入文本框内容发生改变时响应的事件
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            LinkText = sender.As<TextBox>().Text;
        }

        /// <summary>
        /// 类型修改选择后修改样例文本
        /// </summary>
        private void OnTypeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<RadioMenuFlyoutItem>().Tag is TypeModel type)
            {
                SelectedType = type;
                sampleLink = SampleLinkList[TypeList.FindIndex(item => string.Equals(item.InternalName, SelectedType.InternalName))];
                LinkPlaceHolderText = SampleTitleString + sampleLink;
                LinkText = string.Empty;
            }
        }

        /// <summary>
        /// 通道选择修改
        /// </summary>
        private void OnChannelSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<RadioMenuFlyoutItem>().Tag is ChannelModel channel)
            {
                SelectedChannel = channel;
            }
        }

        /// <summary>
        /// 查询链接
        /// </summary>
        private async void OnQueryLinksClicked(object sender, RoutedEventArgs args)
        {
            await QueryLinksAsync();
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
        /// 全选 / 全部不选
        /// </summary>
        private void OnSelectHeaderClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            if (IsSelectMode)
            {
                queryLinksLock.Enter();

                try
                {
                    bool isAllSelected = true;

                    foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                    {
                        if (!queryLinksItem.IsSelected)
                        {
                            isAllSelected = false;
                            break;
                        }
                    }

                    if (!isAllSelected)
                    {
                        foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                        {
                            queryLinksItem.IsSelected = true;
                        }
                    }
                    else
                    {
                        foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                        {
                            queryLinksItem.IsSelected = false;
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
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectEmpty));
                return;
            }
            else
            {
                string queryLinksCopyString = await Task.Run(() =>
                {
                    List<string> queryLinksCopyStringList = [];

                    foreach (QueryLinksModel queryLinksItem in selectedQueryLinksList)
                    {
                        queryLinksCopyStringList.Add(string.Format("[\n{0}\n{1}\n{2}\n]", queryLinksItem.FileName, queryLinksItem.FileLink, queryLinksItem.FileSize));
                    }

                    return string.Join(Environment.NewLine, queryLinksCopyStringList);
                });

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(queryLinksCopyString);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制选定项目的链接
        /// </summary>
        private async void OnCopySelectedLinkClicked(object sender, RoutedEventArgs args)
        {
            List<QueryLinksModel> selectedQueryLinksList = await Task.Run(() =>
            {
                List<QueryLinksModel> selectedQueryLinksList = [];
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

                return selectedQueryLinksList;
            });

            // 内容为空时显示空提示对话框
            if (selectedQueryLinksList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectEmpty));
                return;
            }
            else
            {
                string queryLinksCopyString = await Task.Run(() =>
                {
                    List<string> queryLinksCopyStringList = [];

                    foreach (QueryLinksModel queryLinksItem in selectedQueryLinksList)
                    {
                        queryLinksCopyStringList.Add(queryLinksItem.FileLink);
                    }

                    return string.Join(Environment.NewLine, queryLinksCopyStringList);
                });

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(queryLinksCopyString);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 下载选定项目
        /// </summary>
        private async void OnDownloadSelectedClicked(object sender, RoutedEventArgs args)
        {
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

            string downloadFolder = string.Empty;

            // 手动设置下载目录
            if (DownloadOptionsService.ManualSetDownloadFolder)
            {
                try
                {
                    FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                    {
                        SuggestedStartLocation = PickerLocationId.Downloads
                    };

                    if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                    {
                        downloadFolder = pickFolderResult.Path;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksPage), nameof(OnDownloadSelectedClicked), 1, e);
                    downloadFolder = DownloadOptionsService.DownloadFolder;
                }
            }
            else
            {
                downloadFolder = DownloadOptionsService.DownloadFolder;
            }

            if (!string.IsNullOrEmpty(downloadFolder))
            {
                List<QueryLinksModel> selectedQueryLinksList = await Task.Run(() =>
                {
                    List<QueryLinksModel> selectedQueryLinksList = [];
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

                    return selectedQueryLinksList;
                });

                // 内容为空时显示空提示对话框
                if (selectedQueryLinksList.Count is 0)
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
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksPage), nameof(OnDownloadSelectedClicked), 2, e);
                        }
                        finally
                        {
                            DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
                        }

                        foreach (QueryLinksModel queryLinksItem in selectedQueryLinksList)
                        {
                            string downloadFilePath = Path.Combine(downloadFolder, queryLinksItem.FileName);

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
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(QueryLinksPage), nameof(OnDownloadSelectedClicked), 3, e);
                                continue;
                            }

                            DownloadSchedulerService.CreateDownload(queryLinksItem.FileLink, downloadFilePath);

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
        private void OnItemClick(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is QueryLinksModel queryLinks)
            {
                queryLinksLock.Enter();

                try
                {
                    int ClickedIndex = QueryLinksCollection.IndexOf(queryLinks);
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

        #endregion 第三部分：查找链接页面——挂载的事件

        /// <summary>
        /// 获取链接
        /// </summary>
        public async Task QueryLinksAsync()
        {
            if (!IsQueryingLinks)
            {
                // 设置获取数据时的相关控件状态
                IsQueryingLinks = true;
                LinkText = string.IsNullOrEmpty(LinkText) ? sampleLink : LinkText;
                SetControlState(InfoBarSeverity.Informational);
                foreach (HistoryModel historyItem in HistoryCollection)
                {
                    historyItem.IsQuerying = true;
                }

                // 记录当前选定的选项和填入的内容
                int typeIndex = TypeList.FindIndex(item => string.Equals(item.InternalName, SelectedType.InternalName));
                int channelIndex = ChannelList.FindIndex(item => string.Equals(item.InternalName, SelectedChannel.InternalName));
                string link = LinkText;

                // 商店接口查询方式
                if (string.Equals(QueryLinksModeService.QueryLinksMode, QueryLinksModeService.QueryLinksModeList[0]))
                {
                    (bool requestResult, bool isPackagedApp, AppInfoModel appInfoItem, List<QueryLinksModel> queryLinksList) = await Task.Run(async () =>
                    {
                        (bool requestResult, bool isPackagedApp, AppInfoModel appInfoItem, List<QueryLinksModel> queryLinksList) queryLinksResult = ValueTuple.Create<bool, bool, AppInfoModel, List<QueryLinksModel>>(false, false, null, null);

                        // 解析链接对应的产品 ID
                        string productId = Equals(SelectedType, TypeList[0]) ? QueryLinksHelper.ParseRequestContent(LinkText) : LinkText;
                        string cookie = await QueryLinksHelper.GetCookieAsync();

                        // 获取应用信息
                        (bool requestResult, AppInfoModel appInfo) appInformationResult = await QueryLinksHelper.GetAppInformationAsync(productId);
                        queryLinksResult.requestResult = appInformationResult.requestResult;
                        queryLinksResult.appInfoItem = appInformationResult.appInfo;

                        if (appInformationResult.requestResult)
                        {
                            List<QueryLinksModel> queryLinksList = [];

                            // 解析非商店应用数据
                            if (string.IsNullOrEmpty(appInformationResult.appInfo.CategoryID))
                            {
                                queryLinksResult.isPackagedApp = false;
                                queryLinksList.AddRange(await QueryLinksHelper.GetNonAppxPackagesAsync(productId));
                            }
                            // 解析商店应用数据
                            else
                            {
                                queryLinksResult.isPackagedApp = true;
                                string fileListXml = await QueryLinksHelper.GetFileListXmlAsync(cookie, appInformationResult.appInfo.CategoryID, ChannelList[channelIndex].InternalName);

                                if (!string.IsNullOrEmpty(fileListXml))
                                {
                                    List<QueryLinksModel> appxPackagesList = await QueryLinksHelper.GetAppxPackagesAsync(fileListXml, ChannelList[channelIndex].InternalName);
                                    foreach (QueryLinksModel appxPackage in appxPackagesList)
                                    {
                                        bool isExisted = false;
                                        foreach (QueryLinksModel queryLinksItem in queryLinksList)
                                        {
                                            if (string.Equals(queryLinksItem.FileName, appxPackage.FileName) && Equals(queryLinksItem.FileLink, appxPackage.FileLink) && Equals(queryLinksItem.FileSize, queryLinksItem.FileSize))
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
                                string.Equals(Path.GetExtension(item.FileName), ".eappx", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(Path.GetExtension(item.FileName), ".emsix", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(Path.GetExtension(item.FileName), ".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(Path.GetExtension(item.FileName), ".emsixbundle", StringComparison.OrdinalIgnoreCase)
                                );
                            }

                            if (LinkFilterService.BlockMapFilterValue)
                            {
                                queryLinksList.RemoveAll(item => string.Equals(Path.GetExtension(item.FileName), ".blockmap", StringComparison.OrdinalIgnoreCase));
                            }

                            // 排序
                            queryLinksList.Sort((item1, item2) => item1.FileName.CompareTo(item2.FileName));
                            queryLinksResult.queryLinksList = queryLinksList;
                        }

                        return queryLinksResult;
                    });

                    if (requestResult)
                    {
                        IsQueryingLinks = false;
                        foreach (HistoryModel historyItem in HistoryCollection)
                        {
                            historyItem.IsQuerying = false;
                        }

                        if (queryLinksList is not null && queryLinksList.Count > 0)
                        {
                            UpdateHistory(appInfoItem.Name, typeIndex, channelIndex, link);
                            SetControlState(InfoBarSeverity.Success);
                            ResultControlVisible = true;
                            IsAppInfoVisible = true;
                            IsPackagedApp = isPackagedApp;

                            AppInfo.Name = appInfoItem.Name;
                            AppInfo.Publisher = appInfoItem.Publisher;
                            AppInfo.Description = appInfoItem.Description;
                            AppInfo.CategoryID = appInfoItem.CategoryID;
                            AppInfo.ProductID = appInfoItem.ProductID;

                            queryLinksLock.Enter();

                            try
                            {
                                QueryLinksCollection.Clear();
                                foreach (QueryLinksModel resultItem in queryLinksList)
                                {
                                    QueryLinksCollection.Add(resultItem);
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
                            ResultControlVisible = false;
                            IsAppInfoVisible = false;
                        }
                    }
                    else
                    {
                        IsQueryingLinks = false;
                        SetControlState(InfoBarSeverity.Error);
                        ResultControlVisible = false;
                        foreach (HistoryModel historyItem in HistoryCollection)
                        {
                            historyItem.IsQuerying = false;
                        }
                    }
                }

                // 第三方接口查询方式
                else if (string.Equals(QueryLinksModeService.QueryLinksMode, QueryLinksModeService.QueryLinksModeList[1]))
                {
                    (InfoBarSeverity requestState, bool isPackagedApp, string categoryId, List<QueryLinksModel> queryLinksList) = await Task.Run(async () =>
                    {
                        (InfoBarSeverity requestState, bool isPackagedApp, string categoryId, List<QueryLinksModel> queryLinksList) queryLinksResult = ValueTuple.Create<InfoBarSeverity, bool, string, List<QueryLinksModel>>(InfoBarSeverity.Error, false, null, null);

                        // 生成请求的内容
                        string generateContent = HtmlRequestHelper.GenerateRequestContent(SelectedType.InternalName, link, SelectedChannel.InternalName);

                        // 获取网页反馈回的原始数据
                        RequestModel httpRequestData = await HtmlRequestHelper.HttpRequestAsync(generateContent);

                        // 检查服务器返回获取的状态
                        InfoBarSeverity requestState = HtmlRequestHelper.CheckRequestState(httpRequestData);
                        queryLinksResult.requestState = requestState;

                        if (requestState is InfoBarSeverity.Success)
                        {
                            HtmlParseHelper.InitializeParseData(httpRequestData);
                            string categoryId = HtmlParseHelper.HtmlParseCID().ToUpperInvariant();
                            queryLinksResult.categoryId = categoryId;
                            List<QueryLinksModel> queryLinksList = [];

                            // CategoryID 为空，非打包应用
                            if (string.IsNullOrEmpty(categoryId))
                            {
                                queryLinksResult.isPackagedApp = false;
                                List<QueryLinksModel> nonPackagedAppsList = HtmlParseHelper.HtmlParseNonPackagedAppLinks();
                                queryLinksList.AddRange(HtmlParseHelper.HtmlParseNonPackagedAppLinks());
                            }
                            else
                            {
                                queryLinksResult.isPackagedApp = true;
                                List<QueryLinksModel> packagedAppsList = HtmlParseHelper.HtmlParsePackagedAppLinks();

                                // 按设置选项设置的内容过滤列表
                                if (LinkFilterService.EncryptedPackageFilterValue)
                                {
                                    packagedAppsList.RemoveAll(item =>
                                    string.Equals(Path.GetExtension(item.FileName), ".eappx", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(Path.GetExtension(item.FileName), ".emsix", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(Path.GetExtension(item.FileName), ".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(Path.GetExtension(item.FileName), ".emsixbundle", StringComparison.OrdinalIgnoreCase)
                                    );
                                }

                                if (LinkFilterService.BlockMapFilterValue)
                                {
                                    packagedAppsList.RemoveAll(item => string.Equals(Path.GetExtension(item.FileName), ".blockmap", StringComparison.OrdinalIgnoreCase));
                                }

                                queryLinksList.AddRange(packagedAppsList);
                            }

                            // 排序
                            queryLinksList.Sort((item1, item2) => item1.FileName.CompareTo(item2.FileName));
                            queryLinksResult.queryLinksList = queryLinksList;
                        }

                        return queryLinksResult;
                    });

                    if (requestState is InfoBarSeverity.Success)
                    {
                        IsQueryingLinks = false;
                        IsAppInfoVisible = false;
                        IsPackagedApp = isPackagedApp;
                        foreach (HistoryModel historyItem in HistoryCollection)
                        {
                            historyItem.IsQuerying = false;
                        }

                        UpdateHistory(categoryId, typeIndex, channelIndex, link);
                        SetControlState(requestState);
                        ResultControlVisible = true;

                        queryLinksLock.Enter();

                        try
                        {
                            QueryLinksCollection.Clear();

                            if (queryLinksList is not null && queryLinksList.Count > 0)
                            {
                                foreach (QueryLinksModel queryLinksItem in queryLinksList)
                                {
                                    QueryLinksCollection.Add(queryLinksItem);
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
                    }
                    else if (requestState is InfoBarSeverity.Warning)
                    {
                        IsQueryingLinks = false;
                        SetControlState(requestState);
                        ResultControlVisible = false;
                        IsAppInfoVisible = false;
                        foreach (HistoryModel historyItem in HistoryCollection)
                        {
                            historyItem.IsQuerying = false;
                        }
                    }
                    else if (requestState is InfoBarSeverity.Error)
                    {
                        IsQueryingLinks = false;
                        SetControlState(requestState);
                        ResultControlVisible = false;
                        IsAppInfoVisible = false;
                        foreach (HistoryModel historyItem in HistoryCollection)
                        {
                            historyItem.IsQuerying = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置控件的状态
        /// </summary>
        private void SetControlState(InfoBarSeverity severity)
        {
            int state = Convert.ToInt32(severity);

            ResultSeverity = InfoBarList[state].Severity;
            StateInfoText = InfoBarList[state].Message;
            IsRingActive = InfoBarList[state].PrRingActValue;
        }

        /// <summary>
        /// 更新历史记录，包括主页历史记录内容、数据库中的内容和任务栏跳转列表中的内容
        /// </summary>
        private void UpdateHistory(string appName, int selectedType, int selectedChannel, string link)
        {
            Task.Run(() =>
            {
                // 计算时间戳
                long timeStamp = Convert.ToInt64((DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
                string historyKey = HashAlgorithmHelper.GenerateHistoryKey(TypeList[selectedType].InternalName, ChannelList[selectedChannel].InternalName, link);
                List<HistoryModel> historyList = [.. HistoryCollection];

                int index = historyList.FindIndex(item => string.Equals(item.HistoryKey, historyKey, StringComparison.OrdinalIgnoreCase));

                // 不存在直接添加
                if (index is -1)
                {
                    HistoryModel history = new()
                    {
                        CreateTimeStamp = timeStamp,
                        HistoryKey = historyKey,
                        HistoryAppName = appName,
                        HistoryType = TypeList[selectedType].InternalName,
                        HistoryTypeName = TypeList[selectedType].DisplayName,
                        HistoryChannel = ChannelList[selectedChannel].InternalName,
                        HistoryChannelName = ChannelList[selectedChannel].DisplayName,
                        HistoryLink = link,
                    };

                    historyList.Insert(0, history);
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

                        HistoryCollection.Insert(0, history);
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

        /// <summary>
        /// 检查查询链接按钮可用状态
        /// </summary>
        private bool CheckQueryLinksState(bool isQueryingLinks, bool isSelectMode)
        {
            return !(isQueryingLinks || isSelectMode);
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
