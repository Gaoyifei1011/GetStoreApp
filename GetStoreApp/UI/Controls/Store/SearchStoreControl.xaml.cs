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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.System;

namespace GetStoreApp.UI.Controls.Store
{
    /// <summary>
    /// 搜索应用控件
    /// </summary>
    public sealed partial class SearchStoreControl : StackPanel, INotifyPropertyChanged
    {
        private readonly object historyLock = new object();
        private readonly object searchStoreLock = new object();

        private string SearchStoreCountInfo { get; } = ResourceService.GetLocalized("Store/SearchStoreCountInfo");

        private bool _isSearchingStore = false;

        public bool IsSeachingStore
        {
            get { return _isSearchingStore; }

            set
            {
                _isSearchingStore = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                _searchText = value;
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

        private List<InfoBarModel> SearchStoreInfoList { get; } = ResourceService.SearchStoreInfoList;

        private ObservableCollection<HistoryModel> HistoryCollection { get; } = new ObservableCollection<HistoryModel>();

        private ObservableCollection<SearchStoreModel> SearchStoreCollection { get; } = new ObservableCollection<SearchStoreModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchStoreControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

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
                    SearchText = historyItem.HistoryContent;
                }
            }
        }

        /// <summary>
        /// 打开指定项目的链接
        /// </summary>
        private async void OnOpenLinkExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string appLink = args.Parameter as string;

            if (appLink is not null)
            {
                await Launcher.LaunchUriAsync(new Uri(appLink));
            }
        }

        /// <summary>
        /// 查询指定应用及其依赖的下载链接
        /// </summary>
        private void OnQueryLinksExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string appLink = args.Parameter as string;

            if (appLink is not null)
            {
                StorePage storePage = MainWindow.Current.GetFrameContent() as StorePage;
                if (storePage is not null)
                {
                    storePage.QueryLinks.SelectedType = storePage.QueryLinks.TypeList[0];
                    storePage.QueryLinks.LinkText = appLink;
                    storePage.StoreSelectorBar.SelectedItem = storePage.StoreSelectorBar.Items[0];
                }
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
        /// 搜索应用
        /// </summary>
        private void OnSearchStoreClicked(object sender, RoutedEventArgs args)
        {
            SearchStore();
        }

        #endregion 第二部分：搜索应用控件——挂载的事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 从本地数据存储中加载搜索应用历史记录数据
        /// </summary>
        public void GetSearchStoreHistoryData()
        {
            Task.Run(() =>
            {
                List<HistoryModel> searchStoreHistoryList = HistoryService.GetSearchStoreData();

                DispatcherQueue.TryEnqueue(() =>
                {
                    lock (historyLock)
                    {
                        HistoryCollection.Clear();
                        Task.Delay(10);
                        foreach (HistoryModel historyItem in searchStoreHistoryList)
                        {
                            HistoryCollection.Add(historyItem);
                        }
                    }
                });
            });
        }

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

                            lock (searchStoreLock)
                            {
                                SearchStoreCollection.Clear();
                                foreach (SearchStoreModel searchStoreItem in searchStoreResult.Item2)
                                {
                                    SearchStoreCollection.Add(searchStoreItem);
                                }
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
                        HistoryContent = inputContent
                    };

                    historyList.Insert(0, historyItem);
                    HistoryService.SaveSearchStoreData(historyList);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (historyLock)
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
                    HistoryService.SaveSearchStoreData(historyList);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (historyLock)
                        {
                            HistoryCollection.RemoveAt(index);
                            HistoryCollection.Insert(0, historyItem);
                        }
                    });
                }
            });
        }
    }
}
