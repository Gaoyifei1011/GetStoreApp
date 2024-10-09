using GetStoreApp.Helpers.Controls.Store;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.UI.Controls.Store
{
    /// <summary>
    /// 搜索应用控件
    /// </summary>
    public sealed partial class SearchStoreControl : StackPanel, INotifyPropertyChanged
    {
        private string SearchStoreCountInfo { get; } = ResourceService.GetLocalized("Store/SearchStoreCountInfo");

        private bool _isSearchingStore;

        public bool IsSeachingStore
        {
            get { return _isSearchingStore; }

            set
            {
                if (!Equals(_isSearchingStore, value))
                {
                    _isSearchingStore = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSeachingStore)));
                }
            }
        }

        private string _searchText;

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

        private List<InfoBarModel> SearchStoreInfoList { get; } = ResourceService.SearchStoreInfoList;

        private ObservableCollection<HistoryModel> HistoryCollection { get; } = [];

        private ObservableCollection<SearchStoreModel> SearchStoreCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchStoreControl()
        {
            InitializeComponent();

            HistoryStorageService.SearchStoreCleared += () =>
            {
                DispatcherQueue.TryEnqueue(HistoryCollection.Clear);
            };

            Task.Run(() =>
            {
                List<HistoryModel> searchStoreHistoryList = HistoryStorageService.GetSearchStoreData();

                DispatcherQueue.TryEnqueue(() =>
                {
                    HistoryCollection.Clear();
                    Task.Delay(10);
                    foreach (HistoryModel historyItem in searchStoreHistoryList)
                    {
                        HistoryCollection.Add(historyItem);
                    }
                });
            });
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 填入到文本框
        /// </summary>
        private void OnFillinExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string historyContent && !string.IsNullOrEmpty(historyContent) && MainWindow.Current.GetFrameContent() is StorePage)
            {
                SearchText = historyContent;
            }
        }

        /// <summary>
        /// 打开指定项目的链接
        /// </summary>
        private async void OnOpenLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appLink && !string.IsNullOrEmpty(appLink))
            {
                await Launcher.LaunchUriAsync(new Uri(appLink));
            }
        }

        /// <summary>
        /// 查询指定应用及其依赖的下载链接
        /// </summary>
        private void OnQueryLinksExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appLink && !string.IsNullOrEmpty(appLink) && MainWindow.Current.GetFrameContent() is StorePage storePage)
            {
                storePage.QueryLinks.SelectedType = storePage.QueryLinks.TypeList[0];
                storePage.QueryLinks.LinkText = appLink;
                storePage.StoreSelectorBar.SelectedItem = storePage.StoreSelectorBar.Items[0];
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：搜索应用控件——挂载的事件

        /// <summary>
        /// 输入文本框内容发生改变时响应的事件
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            SearchText = (sender as TextBox).Text;
        }

        /// <summary>
        /// 点击回车键搜索应用
        /// </summary>
        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Enter)
            {
                SearchStore();
            }
        }

        /// <summary>
        /// 搜索应用
        /// </summary>
        private void OnSearchStoreClicked(object sender, RoutedEventArgs args)
        {
            SearchStore();
        }

        #endregion 第二部分：搜索应用控件——挂载的事件

        /// <summary>
        /// 搜索应用
        /// </summary>
        public void SearchStore()
        {
            SearchText = string.IsNullOrEmpty(SearchText) ? "Microsoft Corporation" : SearchText;
            IsSeachingStore = true;
            SetControlState(InfoBarSeverity.Informational);

            Task.Run(async () =>
            {
                string searchText = SearchText;
                string generatedContent = SearchStoreHelper.GenerateSearchString(searchText);
                Tuple<bool, List<SearchStoreModel>> searchStoreResult = await SearchStoreHelper.SerachStoreAppsAsync(generatedContent);

                // 获取成功
                if (searchStoreResult.Item1)
                {
                    // 搜索成功，有数据
                    if (searchStoreResult.Item2.Count > 0)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            IsSeachingStore = false;
                            SetControlState(InfoBarSeverity.Success);
                            ResultControlVisable = true;
                            UpdateHistory(searchText);

                            SearchStoreCollection.Clear();
                            foreach (SearchStoreModel searchStoreItem in searchStoreResult.Item2)
                            {
                                SearchStoreCollection.Add(searchStoreItem);
                            }
                        });
                    }
                    // 搜索成功，没有数据
                    else
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            IsSeachingStore = false;
                            SetControlState(InfoBarSeverity.Warning);
                            ResultControlVisable = false;
                        });
                    }
                }
                // 搜索失败
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        IsSeachingStore = false;
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
                long timeStamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
                string historyKey = HashAlgorithmHelper.GenerateHistoryKey(inputContent);

                List<HistoryModel> historyList = [.. HistoryCollection];

                int index = historyList.FindIndex(item => item.HistoryKey.Equals(historyKey, StringComparison.OrdinalIgnoreCase));

                // 不存在直接添加
                if (index is -1)
                {
                    HistoryModel historyItem = new()
                    {
                        CreateTimeStamp = timeStamp,
                        HistoryKey = historyKey,
                        HistoryContent = inputContent
                    };

                    historyList.Insert(0, historyItem);
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

                        HistoryCollection.Insert(0, historyItem);
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
    }
}
