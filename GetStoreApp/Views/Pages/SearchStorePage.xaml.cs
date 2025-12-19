using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Store;
using GetStoreApp.Models;
using GetStoreApp.Services.History;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.System;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 搜索应用页面
    /// </summary>
    public sealed partial class SearchStorePage : Page, INotifyPropertyChanged
    {
        private readonly string InfoBarErrorString = ResourceService.GetLocalized("SearchStore/InfoBarError");
        private readonly string InfoBarGettingString = ResourceService.GetLocalized("SearchStore/InfoBarGetting");
        private readonly string InfoBarSuccessString = ResourceService.GetLocalized("SearchStore/InfoBarSuccess");
        private readonly string InfoBarWarningString = ResourceService.GetLocalized("SearchStore/InfoBarWarning");
        private readonly string SearchStoreCountInfo = ResourceService.GetLocalized("SearchStore/SearchStoreCountInfo");
        private readonly string WelcomeString = ResourceService.GetLocalized("SearchStore/Welcome");

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
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

        private bool _isSearchingStore = false;

        public bool IsSearchingStore
        {
            get { return _isSearchingStore; }

            set
            {
                if (!Equals(_isSearchingStore, value))
                {
                    _isSearchingStore = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearchingStore)));
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

        private List<InfoBarModel> SearchStoreInfoList { get; } = [];

        private ObservableCollection<HistoryModel> HistoryCollection { get; } = [];

        private ObservableCollection<SearchStoreModel> SearchStoreCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchStorePage()
        {
            InitializeComponent();
            StateInfoText = WelcomeString;

            SearchStoreInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Informational,
                Message = InfoBarGettingString,
                PrRingActValue = true,
                PrRingVisValue = true
            });
            SearchStoreInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Success,
                Message = InfoBarSuccessString,
                PrRingActValue = false,
                PrRingVisValue = false
            });
            SearchStoreInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Warning,
                Message = InfoBarWarningString,
                PrRingActValue = false,
                PrRingVisValue = false
            });
            SearchStoreInfoList.Add(new InfoBarModel
            {
                Severity = InfoBarSeverity.Error,
                Message = InfoBarErrorString,
                PrRingActValue = false,
                PrRingVisValue = false
            });

            HistoryStorageService.SearchStoreCleared += () =>
            {
                DispatcherQueue.TryEnqueue(HistoryCollection.Clear);
            };
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 点击回车键搜索应用
        /// </summary>
        protected override async void OnKeyDown(KeyRoutedEventArgs args)
        {
            base.OnKeyDown(args);

            if (args.Key is VirtualKey.Enter && !IsSearchingStore && !IsSelectMode)
            {
                await SearchStoreAsync();
            }
        }

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            List<HistoryModel> searchStoreHistoryList = await Task.Run(HistoryStorageService.GetSearchStoreData);

            HistoryCollection.Clear();
            foreach (HistoryModel historyItem in searchStoreHistoryList)
            {
                HistoryCollection.Add(historyItem);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制指定应用的链接
        /// </summary>
        private async void OnCopyLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string storeAppLink && !string.IsNullOrEmpty(storeAppLink))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(storeAppLink);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 填入到文本框
        /// </summary>
        private void OnFillInExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string historyContent && !string.IsNullOrEmpty(historyContent))
            {
                SearchText = historyContent;
            }
        }

        /// <summary>
        /// 打开指定项目的链接
        /// </summary>
        private void OnOpenLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appLink && !string.IsNullOrEmpty(appLink))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        if (Equals(AppLinkOpenModeService.AppLinkOpenMode, AppLinkOpenModeService.AppLinkOpenModeList[0]))
                        {
                            await Launcher.LaunchUriAsync(new Uri("getstoreappwebview:"), new LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
                            {
                                {"AppLink", appLink },
                            });
                        }
                        else if (Equals(AppLinkOpenModeService.AppLinkOpenMode, AppLinkOpenModeService.AppLinkOpenModeList[1]))
                        {
                            await Launcher.LaunchUriAsync(new Uri(appLink));
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                });
            }
        }

        /// <summary>
        /// 查询指定应用及其依赖的下载链接
        /// </summary>
        private void OnQueryLinksExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appLink && MainWindow.Current.GetFrameContent() is StorePage storePage && !Equals(storePage.GetCurrentPageType(), typeof(QueryLinksPage)))
            {
                storePage.NavigateTo(storePage.PageList[0], new List<string> { "0", null, appLink }, false);
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：搜索应用页面——挂载的事件

        /// <summary>
        /// 输入文本框内容发生改变时响应的事件
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            SearchText = sender.As<TextBox>().Text;
        }

        /// <summary>
        /// 搜索应用
        /// </summary>
        private async void OnSearchStoreClicked(object sender, RoutedEventArgs args)
        {
            await SearchStoreAsync();
        }

        #endregion 第三部分：搜索应用页面——挂载的事件

        /// <summary>
        /// 搜索应用
        /// </summary>
        public async Task SearchStoreAsync()
        {
            if (!IsSearchingStore)
            {
                IsSearchingStore = true;
                SearchText = string.IsNullOrEmpty(SearchText) ? "Microsoft Corporation" : SearchText;
                SetControlState(InfoBarSeverity.Informational);
                foreach (HistoryModel historyItem in HistoryCollection)
                {
                    historyItem.IsQuerying = true;
                }

                (bool requestResult, List<SearchStoreModel> searchStoreList) = await Task.Run(async () =>
                {
                    string searchText = SearchText;
                    string generatedContent = SearchStoreHelper.GenerateSearchString(searchText);
                    return await SearchStoreHelper.SearchStoreAppsAsync(generatedContent);
                });

                // 获取成功
                if (requestResult)
                {
                    // 搜索成功，有数据
                    if (searchStoreList.Count > 0)
                    {
                        IsSearchingStore = false;
                        SetControlState(InfoBarSeverity.Success);
                        ResultControlVisible = true;
                        UpdateHistory(SearchText);
                        foreach (HistoryModel historyItem in HistoryCollection)
                        {
                            historyItem.IsQuerying = false;
                        }

                        SearchStoreCollection.Clear();
                        foreach (SearchStoreModel searchStoreItem in searchStoreList)
                        {
                            SearchStoreCollection.Add(searchStoreItem);
                        }
                    }
                    // 搜索成功，没有数据
                    else
                    {
                        IsSearchingStore = false;
                        SetControlState(InfoBarSeverity.Warning);
                        ResultControlVisible = false;
                        foreach (HistoryModel historyItem in HistoryCollection)
                        {
                            historyItem.IsQuerying = false;
                        }
                    }
                }
                // 搜索失败
                else
                {
                    IsSearchingStore = false;
                    SetControlState(InfoBarSeverity.Error);
                    ResultControlVisible = false;
                    foreach (HistoryModel historyItem in HistoryCollection)
                    {
                        historyItem.IsQuerying = false;
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

            ResultSeverity = SearchStoreInfoList[state].Severity;
            StateInfoText = SearchStoreInfoList[state].Message;
            IsRingActive = SearchStoreInfoList[state].PrRingActValue;
        }

        /// <summary>
        /// 更新历史记录，包括主页历史记录内容、数据库中的内容和任务栏跳转列表中的内容
        /// </summary>
        private void UpdateHistory(string inputContent)
        {
            Task.Run(() =>
            {
                // 计算时间戳
                long timeStamp = Convert.ToInt64((DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
                string historyKey = HashAlgorithmHelper.GenerateHistoryKey(inputContent);

                List<HistoryModel> historyList = [.. HistoryCollection];

                int index = historyList.FindIndex(item => string.Equals(item.HistoryKey, historyKey, StringComparison.OrdinalIgnoreCase));

                // 不存在直接添加
                if (index is -1)
                {
                    HistoryModel history = new()
                    {
                        CreateTimeStamp = timeStamp,
                        HistoryKey = historyKey,
                        HistoryContent = inputContent
                    };

                    historyList.Insert(0, history);
                    if (historyList.Count is 4)
                    {
                        historyList.RemoveAt(historyList.Count - 1);
                    }
                    HistoryStorageService.SaveSearchStoreData(historyList);

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
                    historyList.RemoveAt(index);
                    historyList.Insert(0, historyItem);
                    HistoryStorageService.SaveSearchStoreData(historyList);

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
        private bool CheckSearchStoreState(bool isSearchingStore, bool isSelectMode)
        {
            return !(isSearchingStore || isSelectMode);
        }
    }
}
