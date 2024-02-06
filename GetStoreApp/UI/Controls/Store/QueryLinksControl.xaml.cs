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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;

namespace GetStoreApp.UI.Controls.Store
{
    /// <summary>
    /// 查找链接控件
    /// </summary>
    public sealed partial class QueryLinksControl : StackPanel, INotifyPropertyChanged
    {
        private readonly object HistoryLock = new object();
        private readonly object QueryLinksLock = new object();

        private string SampleLink;
        private string SampleTitle = ResourceService.GetLocalized("Store/SampleTitle");
        private string QueryLinksCountInfo = ResourceService.GetLocalized("Store/QueryLinksCountInfo");

        private TypeModel _selectedType;

        public TypeModel SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                OnPropertyChanged();
            }
        }

        private ChannelModel _selectedChannel;

        public ChannelModel SelectedChannel
        {
            get { return _selectedChannel; }

            set
            {
                _selectedChannel = value;
                OnPropertyChanged();
            }
        }

        private string _linkPlaceHolderText;

        public string LinkPlaceHolderText
        {
            get { return _linkPlaceHolderText; }

            set
            {
                _linkPlaceHolderText = value;
                OnPropertyChanged();
            }
        }

        private string _linkText;

        public string LinkText
        {
            get { return _linkText; }

            set
            {
                _linkText = value;
                OnPropertyChanged();
            }
        }

        private bool _isQueryingLinks = false;

        public bool IsQueryingLinks
        {
            get { return _isQueryingLinks; }

            set
            {
                _isQueryingLinks = value;
                OnPropertyChanged();
            }
        }

        private AppInfoModel _appInfo = new AppInfoModel();

        public AppInfoModel AppInfo
        {
            get { return _appInfo; }

            set
            {
                _appInfo = value;
                OnPropertyChanged();
            }
        }

        private InfoBarSeverity _resultSeverity = InfoBarSeverity.Informational;

        public InfoBarSeverity ResultSeverity
        {
            get { return _resultSeverity; }

            set
            {
                _resultSeverity = value;
                OnPropertyChanged();
            }
        }

        private string _stateInfoText = ResourceService.GetLocalized("Store/Welcome");

        public string StateInfoText
        {
            get { return _stateInfoText; }

            set
            {
                _stateInfoText = value;
                OnPropertyChanged();
            }
        }

        private bool _isRingActive = false;

        public bool IsRingActive
        {
            get { return _isRingActive; }

            set
            {
                _isRingActive = value;
                OnPropertyChanged();
            }
        }

        private bool _resultCotnrolVisable = false;

        public bool ResultControlVisable
        {
            get { return _resultCotnrolVisable; }

            set
            {
                _resultCotnrolVisable = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                _isSelectMode = value;
                OnPropertyChanged();
            }
        }

        private static List<string> SampleLinkList = new List<string>
        {
            "https://www.microsoft.com/store/productId/9WZDNCRFJBMP",
            "9WZDNCRFJBMP",
        };

        private List<InfoBarModel> QueryLinksInfoList = ResourceService.QueryLinksInfoList;

        public List<TypeModel> TypeList { get; } = ResourceService.TypeList;

        public List<ChannelModel> ChannelList { get; } = ResourceService.ChannelList;

        private ObservableCollection<HistoryModel> HistoryCollection { get; } = new ObservableCollection<HistoryModel>();

        private ObservableCollection<QueryLinksModel> QueryLinksCollection { get; } = new ObservableCollection<QueryLinksModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public QueryLinksControl()
        {
            InitializeComponent();

            SelectedType = Convert.ToInt32(DesktopLaunchService.LaunchArgs["TypeName"]) is -1 ? TypeList[0] : TypeList[Convert.ToInt32(DesktopLaunchService.LaunchArgs["TypeName"])];
            SelectedChannel = Convert.ToInt32(DesktopLaunchService.LaunchArgs["ChannelName"]) is -1 ? ChannelList[3] : ChannelList[Convert.ToInt32(DesktopLaunchService.LaunchArgs["ChannelName"])];
            LinkText = DesktopLaunchService.LaunchArgs["Link"] is null ? string.Empty : (string)DesktopLaunchService.LaunchArgs["Link"];

            SampleLink = SampleLinkList[0];
            LinkPlaceHolderText = SampleTitle + SampleLink;
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制到剪贴板
        /// </summary>
        private void OnCopyExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            HistoryModel historyItem = args.Parameter as HistoryModel;

            if (historyItem is not null)
            {
                string copyContent = string.Format("{0}\t{1}\t{2}\t{3}",
                    historyItem.HistoryAppName,
                    TypeList.Find(item => item.InternalName.Equals(historyItem.HistoryType)).DisplayName,
                    ChannelList.Find(item => item.InternalName.Equals(historyItem.HistoryChannel)).DisplayName,
                    historyItem.HistoryLink);
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyContent);

                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.History, copyResult, false));
            }
        }

        /// <summary>
        /// 填入到文本框
        /// </summary>
        private void OnFillinExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            HistoryModel historyItem = args.Parameter as HistoryModel;

            if (historyItem is not null)
            {
                StorePage storePage = MainWindow.Current.GetFrameContent() as StorePage;
                if (storePage is not null)
                {
                    SelectedType = TypeList.Find(item => item.InternalName.Equals(historyItem.HistoryType));
                    SelectedChannel = ChannelList.Find(item => item.InternalName.Equals(historyItem.HistoryChannel));
                    LinkText = historyItem.HistoryLink;
                }
            }
        }

        /// <summary>
        /// 根据设置存储的文件链接操作方式操作获取到的文件链接
        /// </summary>
        private void OnDownloadExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            QueryLinksModel queryLinksItem = args.Parameter as QueryLinksModel;
            if (queryLinksItem is not null)
            {
                // 查看是否开启了网络监控服务
                if (NetWorkMonitorService.NetWorkMonitorValue)
                {
                    // 网络处于未连接状态，不再进行下载，显示通知
                    if (!NetWorkHelper.IsNetworkConnected(out bool checkFailed))
                    {
                        if (!checkFailed)
                        {
                            TeachingTipHelper.Show(new NetWorkErrorTip());
                            return;
                        }
                    }
                }

                Task.Run(async () =>
                {
                    AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                    // 使用应用内提供的下载方式
                    if (DownloadOptionsService.DownloadMode.Value == DownloadOptionsService.DownloadModeList[0].Value)
                    {
                        string DownloadFilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, queryLinksItem.FileName);

                        BackgroundModel backgroundItem = new BackgroundModel
                        {
                            DownloadKey = HashAlgorithmHelper.GenerateDownloadKey(queryLinksItem.FileName, DownloadFilePath),
                            FileName = queryLinksItem.FileName,
                            FileLink = queryLinksItem.FileLink,
                            FilePath = DownloadFilePath,
                            TotalSize = 0,
                            DownloadFlag = 1
                        };

                        // 检查是否存在相同的任务记录
                        DuplicatedDataKind CheckResult = await DownloadXmlService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                        switch (CheckResult)
                        {
                            case DuplicatedDataKind.None:
                                {
                                    bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        TeachingTipHelper.Show(new DownloadCreateTip(AddResult));
                                    });

                                    break;
                                }

                            case DuplicatedDataKind.Unfinished:
                                {
                                    ContentDialogResult result = ContentDialogResult.None;

                                    DispatcherQueue.TryEnqueue(async () =>
                                    {
                                        result = await ContentDialogHelper.ShowAsync(new DownloadNotifyDialog(DuplicatedDataKind.Unfinished), this);
                                        autoResetEvent.Set();
                                    });

                                    autoResetEvent.WaitOne();

                                    if (result is ContentDialogResult.Primary)
                                    {
                                        try
                                        {
                                            if (File.Exists(backgroundItem.FilePath))
                                            {
                                                File.Delete(backgroundItem.FilePath);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(LoggingLevel.Warning, "Delete duplicated unfinished downloaded file failed.", e);
                                        }
                                        finally
                                        {
                                            bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                            DispatcherQueue.TryEnqueue(() =>
                                            {
                                                TeachingTipHelper.Show(new DownloadCreateTip(AddResult));
                                            });
                                        }
                                    }
                                    else if (result is ContentDialogResult.Secondary)
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            MainWindow.Current.NavigateTo(typeof(DownloadPage));
                                        });
                                    }
                                    break;
                                }

                            case DuplicatedDataKind.Completed:
                                {
                                    ContentDialogResult result = ContentDialogResult.None;

                                    DispatcherQueue.TryEnqueue(async () =>
                                    {
                                        result = await ContentDialogHelper.ShowAsync(new DownloadNotifyDialog(DuplicatedDataKind.Completed), this);
                                        autoResetEvent.Set();
                                    });

                                    autoResetEvent.WaitOne();

                                    if (result is ContentDialogResult.Primary)
                                    {
                                        try
                                        {
                                            if (File.Exists(backgroundItem.FilePath))
                                            {
                                                File.Delete(backgroundItem.FilePath);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(LoggingLevel.Warning, "Delete duplicated completed downloaded file failed.", e);
                                        }
                                        finally
                                        {
                                            bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                            DispatcherQueue.TryEnqueue(() =>
                                            {
                                                TeachingTipHelper.Show(new DownloadCreateTip(AddResult));
                                            });
                                        }
                                    }
                                    else if (result is ContentDialogResult.Secondary)
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            MainWindow.Current.NavigateTo(typeof(DownloadPage));
                                        });
                                    }
                                    break;
                                }
                        }
                    }

                    // 使用浏览器下载
                    else if (DownloadOptionsService.DownloadMode.Value == DownloadOptionsService.DownloadModeList[1].Value)
                    {
                        await Launcher.LaunchUriAsync(new Uri(queryLinksItem.FileLink));
                    }

                    autoResetEvent.Dispose();
                });
            }
        }

        /// <summary>
        /// 打开指定项目的链接
        /// </summary>
        private async void OnOpenLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string fileLink = args.Parameter as string;

            if (fileLink is not null)
            {
                await Launcher.LaunchUriAsync(new Uri(fileLink));
            }
        }

        /// <summary>
        /// 复制指定项目的链接
        /// </summary>
        private void OnCopyLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string fileLink = args.Parameter as string;

            if (fileLink is not null)
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(fileLink);
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.ResultLink, copyResult, false));
            }
        }

        /// <summary>
        /// 复制指定项目的信息
        /// </summary>
        private void OnCopyInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            QueryLinksModel queryLinksItem = args.Parameter as QueryLinksModel;
            if (queryLinksItem is not null)
            {
                string copyInformation = string.Format("[\n{0}\n{1}\n{2}\n]\n",
                    queryLinksItem.FileName,
                    queryLinksItem.FileLink,
                    queryLinksItem.FileSize
                    );

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyInformation);
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.ResultInformation, copyResult, false));
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：查找链接控件——挂载的事件

        /// <summary>
        /// 输入文本框内容发生改变时响应的事件
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            LinkText = (sender as TextBox).Text;
        }

        /// <summary>
        /// 类型修改选择后修改样例文本
        /// </summary>
        private void OnTypeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedType = TypeList[Convert.ToInt32(item.Tag)];
                SampleLink = SampleLinkList[TypeList.FindIndex(item => item.InternalName == SelectedType.InternalName)];
                LinkPlaceHolderText = SampleTitle + SampleLink;

                LinkText = string.Empty;
            }
        }

        /// <summary>
        /// 通道选择修改
        /// </summary>
        private void OnChannelSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                SelectedChannel = ChannelList[Convert.ToInt32(item.Tag)];
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
        private void OnCopyQueryedAppInfoClicked(object sender, RoutedEventArgs args)
        {
            StringBuilder appInformationBuilder = new StringBuilder();
            appInformationBuilder.Append(ResourceService.GetLocalized("Store/AppName"));
            appInformationBuilder.AppendLine(AppInfo.Name);
            appInformationBuilder.Append(ResourceService.GetLocalized("Store/AppPublisher"));
            appInformationBuilder.AppendLine(AppInfo.Publisher);
            appInformationBuilder.AppendLine(ResourceService.GetLocalized("Store/AppDescription"));
            appInformationBuilder.AppendLine(AppInfo.Description);

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(appInformationBuilder.ToString());
            TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.AppInformation, copyResult));
        }

        /// <summary>
        /// 查看应用更多信息
        /// </summary>
        private async void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri(string.Format("https://www.microsoft.com/store/productId/{0}", AppInfo.ProductID)));
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        private void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            lock (QueryLinksLock)
            {
                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    queryLinksItem.IsSelectMode = true;
                    queryLinksItem.IsSelected = false;
                }

                IsSelectMode = true;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            lock (QueryLinksLock)
            {
                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    queryLinksItem.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            lock (QueryLinksLock)
            {
                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    queryLinksItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// 复制选定项目的内容
        /// </summary>
        private void OnCopySelectedClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                List<QueryLinksModel> selectedQueryLinksList = new List<QueryLinksModel>();

                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    if (queryLinksItem.IsSelected is true)
                    {
                        selectedQueryLinksList.Add(queryLinksItem);
                    }
                }

                // 内容为空时显示空提示对话框
                if (selectedQueryLinksList.Count is 0)
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                    });
                    return;
                };

                StringBuilder stringBuilder = new StringBuilder();

                foreach (QueryLinksModel queryLinksItem in selectedQueryLinksList)
                {
                    stringBuilder.AppendLine(string.Format("[\n{0}\n{1}\n{2}\n]",
                        queryLinksItem.FileName,
                        queryLinksItem.FileLink,
                        queryLinksItem.FileSize)
                        );
                }

                DispatcherQueue.TryEnqueue(() =>
                {
                    bool copyResult = CopyPasteHelper.CopyTextToClipBoard(stringBuilder.ToString());
                    TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.ResultInformation, copyResult, true, selectedQueryLinksList.Count));
                });
            });
        }

        /// <summary>
        /// 复制选定项目的链接
        /// </summary>
        private void OnCopySelectedLinkClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                List<QueryLinksModel> selectedQueryLinksList = new List<QueryLinksModel>();

                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    if (queryLinksItem.IsSelected is true)
                    {
                        selectedQueryLinksList.Add(queryLinksItem);
                    }
                }

                // 内容为空时显示空提示对话框
                if (selectedQueryLinksList.Count is 0)
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                    });
                    return;
                };

                StringBuilder stringBuilder = new StringBuilder();

                foreach (QueryLinksModel queryLinksItem in selectedQueryLinksList)
                {
                    stringBuilder.AppendLine(string.Format("{0}", queryLinksItem.FileLink));
                }

                DispatcherQueue.TryEnqueue(() =>
                {
                    bool copyResult = CopyPasteHelper.CopyTextToClipBoard(stringBuilder.ToString());
                    TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.ResultLink, copyResult, true, selectedQueryLinksList.Count));
                });
            });
        }

        /// <summary>
        /// 下载选定项目
        /// </summary>
        private void OnDownloadSelectedClicked(object sender, RoutedEventArgs args)
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                // 网络处于未连接状态，不再进行下载，显示通知
                if (!NetWorkHelper.IsNetworkConnected(out bool checkFailed))
                {
                    if (!checkFailed)
                    {
                        TeachingTipHelper.Show(new NetWorkErrorTip());
                        return;
                    }
                }
            }

            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            Task.Run(async () =>
            {
                List<QueryLinksModel> selectedQueryLinksList = new List<QueryLinksModel>();

                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    if (queryLinksItem.IsSelected is true)
                    {
                        selectedQueryLinksList.Add(queryLinksItem);
                    }
                }

                // 内容为空时显示空提示对话框
                if (selectedQueryLinksList.Count is 0)
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                    });

                    return;
                };

                // 使用应用内提供的下载方式
                if (DownloadOptionsService.DownloadMode.Value == DownloadOptionsService.DownloadModeList[0].Value)
                {
                    List<BackgroundModel> duplicatedList = new List<BackgroundModel>();

                    bool IsDownloadSuccessfully = false;

                    foreach (QueryLinksModel queryLinksItem in selectedQueryLinksList)
                    {
                        string DownloadFilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, queryLinksItem.FileName);

                        BackgroundModel backgroundItem = new BackgroundModel
                        {
                            DownloadKey = HashAlgorithmHelper.GenerateDownloadKey(queryLinksItem.FileName, DownloadFilePath),
                            FileName = queryLinksItem.FileName,
                            FileLink = queryLinksItem.FileLink,
                            FilePath = DownloadFilePath,
                            TotalSize = 0,
                            DownloadFlag = 1
                        };

                        DuplicatedDataKind CheckResult = await DownloadXmlService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                        if (CheckResult is DuplicatedDataKind.None)
                        {
                            await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                            IsDownloadSuccessfully = true;
                        }
                        else
                        {
                            duplicatedList.Add(backgroundItem);
                        }
                    }

                    if (duplicatedList.Count > 0)
                    {
                        ContentDialogResult result = ContentDialogResult.None;

                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            result = await ContentDialogHelper.ShowAsync(new DownloadNotifyDialog(DuplicatedDataKind.MultiRecord), this);
                            autoResetEvent.Set();
                        });

                        autoResetEvent.WaitOne();
                        autoResetEvent.Dispose();

                        if (result is ContentDialogResult.Primary)
                        {
                            foreach (BackgroundModel backgroundItem in duplicatedList)
                            {
                                try
                                {
                                    if (File.Exists(backgroundItem.FilePath))
                                    {
                                        File.Delete(backgroundItem.FilePath);
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Warning, "Delete duplicated downloaded file failed.", e);
                                }
                                finally
                                {
                                    await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                    IsDownloadSuccessfully = true;
                                }
                            }
                        }
                        else if (result is ContentDialogResult.Secondary)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                MainWindow.Current.NavigateTo(typeof(DownloadPage));
                            });
                        }
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        // 显示下载任务创建成功消息
                        TeachingTipHelper.Show(new DownloadCreateTip(IsDownloadSuccessfully));

                        foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                        {
                            queryLinksItem.IsSelectMode = false;
                        }
                        IsSelectMode = false;
                    });
                }

                // 使用浏览器下载
                else if (DownloadOptionsService.DownloadMode.Value == DownloadOptionsService.DownloadModeList[1].Value)
                {
                    foreach (QueryLinksModel queryLinksItem in selectedQueryLinksList)
                    {
                        await Launcher.LaunchUriAsync(new Uri(queryLinksItem.FileLink));
                    }
                }
            });
        }

        /// <summary>
        /// 退出多选模式
        /// </summary>
        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            lock (QueryLinksLock)
            {
                IsSelectMode = false;
                foreach (QueryLinksModel queryLinksItem in QueryLinksCollection)
                {
                    queryLinksItem.IsSelectMode = false;
                }
            }
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        private void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            QueryLinksModel queryLinksItem = args.ClickedItem as QueryLinksModel;

            if (queryLinksItem is not null)
            {
                lock (QueryLinksLock)
                {
                    int ClickedIndex = QueryLinksCollection.IndexOf(queryLinksItem);
                    QueryLinksCollection[ClickedIndex].IsSelected = !QueryLinksCollection[ClickedIndex].IsSelected;
                }
            }
        }

        #endregion 第二部分：查找链接控件——挂载的事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 本地化应用类型字符串
        /// </summary>
        private string LocalizeQueryedAppType(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return string.Format(ResourceService.GetLocalized("Store/QueryedAppType"), ResourceService.GetLocalized("Store/NonPackagedApp"));
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("Store/QueryedAppType"), ResourceService.GetLocalized("Store/PackagedApp"));
            }
        }

        /// <summary>
        /// 从本地数据存储中加载查询链接历史记录数据
        /// </summary>
        public void GetQueryLinksHistoryData()
        {
            Task.Run(() =>
            {
                List<HistoryModel> queryLinksHistoryList = HistoryService.GetQueryLinksData();

                DispatcherQueue.TryEnqueue(() =>
                {
                    lock (HistoryLock)
                    {
                        HistoryCollection.Clear();
                        Task.Delay(10);
                        foreach (HistoryModel historyItem in queryLinksHistoryList)
                        {
                            HistoryCollection.Add(historyItem);
                        }
                    }
                });
            });
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        public void QueryLinks()
        {
            // 设置获取数据时的相关控件状态
            LinkText = string.IsNullOrEmpty(LinkText) ? SampleLink : LinkText;
            IsQueryingLinks = true;
            SetControlState(InfoBarSeverity.Informational);

            Task.Run(async () =>
            {
                List<QueryLinksModel> queryLinksList = new List<QueryLinksModel>();

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
                                queryLinksList.Add(appxPackage);
                            }
                        }
                    }

                    ResultListFilter(ref queryLinksList);
                    queryLinksList.Sort((item1, item2) => item1.FileName.CompareTo(item2.FileName));

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        IsQueryingLinks = false;

                        if (queryLinksList.Count > 0)
                        {
                            UpdateHistory(appInformationResult.Item2.Name, typeIndex, channelIndex, link);
                            SetControlState(InfoBarSeverity.Success);
                            ResultControlVisable = true;

                            AppInfo.Name = appInformationResult.Item2.Name;
                            AppInfo.Publisher = appInformationResult.Item2.Publisher;
                            AppInfo.Description = appInformationResult.Item2.Description;
                            AppInfo.CategoryID = appInformationResult.Item2.CategoryID;
                            AppInfo.ProductID = appInformationResult.Item2.ProductID;

                            lock (QueryLinksLock)
                            {
                                QueryLinksCollection.Clear();
                                foreach (QueryLinksModel resultItem in queryLinksList)
                                {
                                    QueryLinksCollection.Add(resultItem);
                                    Task.Delay(1);
                                }
                            }
                        }
                        else
                        {
                            SetControlState(InfoBarSeverity.Warning);
                            ResultControlVisable = false;
                        }
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        IsQueryingLinks = false;
                        SetControlState(InfoBarSeverity.Error);
                        ResultControlVisable = false;
                    });
                }
            });
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
                long timeStamp = GenerateTimeStamp();
                string historyKey = HashAlgorithmHelper.GenerateHistoryKey(TypeList[selectedType].InternalName, ChannelList[selectedChannel].InternalName, link);

                List<HistoryModel> historyList = new List<HistoryModel>();
                foreach (HistoryModel historyItem in HistoryCollection)
                {
                    historyList.Add(historyItem);
                }

                int index = historyList.FindIndex(item => item.HistoryKey.Equals(historyKey, StringComparison.OrdinalIgnoreCase));

                // 不存在直接添加
                if (index is -1)
                {
                    HistoryModel historyItem = new HistoryModel()
                    {
                        CreateTimeStamp = timeStamp,
                        HistoryKey = historyKey,
                        HistoryAppName = appName,
                        HistoryType = TypeList[selectedType].InternalName,
                        HistoryChannel = ChannelList[selectedChannel].InternalName,
                        HistoryLink = link
                    };

                    historyList.Insert(0, historyItem);
                    HistoryService.SaveQueryLinksData(historyList);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (HistoryLock)
                        {
                            if (HistoryCollection.Count is 3)
                            {
                                HistoryCollection.RemoveAt(HistoryCollection.Count - 1);
                            }

                            HistoryCollection.Insert(0, historyItem);
                        }
                    });
                }
                // 存在则修改原来项的时间戳，并调整顺序
                else
                {
                    HistoryModel historyItem = historyList[index];
                    historyItem.CreateTimeStamp = timeStamp;
                    historyList.RemoveAt(index);
                    historyList.Insert(0, historyItem);
                    HistoryService.SaveQueryLinksData(historyList);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (HistoryLock)
                        {
                            HistoryCollection.RemoveAt(index);
                            HistoryCollection.Insert(0, historyItem);
                        }
                    });
                }
            });
        }

        /// <summary>
        /// 按设置选项设置的内容过滤列表
        /// </summary>
        private void ResultListFilter(ref List<QueryLinksModel> resultDataList)
        {
            if (LinkFilterService.EncryptedPackageFilterValue)
            {
                resultDataList.RemoveAll(item =>
                item.FileName.EndsWith(".eappx", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsix", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                item.FileName.EndsWith(".emsixbundle", StringComparison.OrdinalIgnoreCase)
                );
            }

            if (LinkFilterService.BlockMapFilterValue)
            {
                resultDataList.RemoveAll(item => item.FileName.EndsWith("blockmap", StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 生成时间戳
        /// </summary>
        private long GenerateTimeStamp()
        {
            TimeSpan TimeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(TimeSpan.TotalSeconds);
        }
    }
}
