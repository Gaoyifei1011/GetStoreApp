using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Store;
using GetStoreApp.Models;
using GetStoreApp.Services.History;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.NotificationTips;
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
using System.Threading.Tasks;
using Windows.System;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.UserControls
{
    /// <summary>
    /// 商店选择器用户控件
    /// </summary>
    public partial class StoreSelectorUserControl : UserControl, INotifyPropertyChanged
    {
        private readonly string ExactSearchString = ResourceService.GetLocalized("StoreSelector/ExactSearch");
        private readonly string FastString = ResourceService.GetLocalized("StoreSelector/Fast");
        private readonly string ManifestSearchString = ResourceService.GetLocalized("StoreSelector/ManifestSearch");
        private readonly string ProductIDString = ResourceService.GetLocalized("StoreSelector/ProductID");
        private readonly string RetailString = ResourceService.GetLocalized("StoreSelector/Retail");
        private readonly string RPString = ResourceService.GetLocalized("StoreSelector/RP");
        private readonly string SampleTitleString = ResourceService.GetLocalized("StoreSelector/SampleTitle");
        private readonly string SlowString = ResourceService.GetLocalized("StoreSelector/Slow");
        private readonly string URLString = ResourceService.GetLocalized("StoreSelector/URL");
        private bool isInitialized;
        private string sampleLink;
        private StorePage storePage;

        private SelectorBarItem _selectedItem;

        public SelectorBarItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

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

        private string _queryLinksText = string.Empty;

        public string QueryLinksText
        {
            get { return _queryLinksText; }

            set
            {
                if (!string.Equals(_queryLinksText, value))
                {
                    _queryLinksText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QueryLinksText)));
                }
            }
        }

        private bool _isQueryingLinks;

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

        private bool _isQueryLinksResultVisible;

        public bool IsQueryLinksResultVisible
        {
            get { return _isQueryLinksResultVisible; }

            set
            {
                if (!Equals(_isQueryLinksResultVisible, value))
                {
                    _isQueryLinksResultVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsQueryLinksResultVisible)));
                }
            }
        }

        private ComboBoxItemModel _selectedSearchType;

        public ComboBoxItemModel SelectedSearchType
        {
            get { return _selectedSearchType; }

            set
            {
                if (!Equals(_selectedSearchType, value))
                {
                    _selectedSearchType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSearchType)));
                }
            }
        }

        private string _searchAppsText;

        public string SearchAppsText
        {
            get { return _searchAppsText; }

            set
            {
                if (!string.Equals(_searchAppsText, value))
                {
                    _searchAppsText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchAppsText)));
                }
            }
        }

        private bool _isSearchingApps;

        public bool IsSearchingApps
        {
            get { return _isSearchingApps; }

            set
            {
                if (!Equals(_isSearchingApps, value))
                {
                    _isSearchingApps = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearchingApps)));
                }
            }
        }

        private bool _isSearchAppsResultVisible;

        public bool IsSearchAppsResultVisible
        {
            get { return _isSearchAppsResultVisible; }

            set
            {
                if (!Equals(_isSearchAppsResultVisible, value))
                {
                    _isSearchAppsResultVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearchAppsResultVisible)));
                }
            }
        }

        private StoreInfoResultKind _storeInfoResultKind;

        public StoreInfoResultKind StoreInfoResultKind
        {
            get { return _storeInfoResultKind; }

            set
            {
                if (!Equals(_storeInfoResultKind, value))
                {
                    _storeInfoResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StoreInfoResultKind)));
                }
            }
        }

        private List<string> SampleLinkList { get; } = ["https://apps.microsoft.com/store/detail/9WZDNCRFJBMP", "9WZDNCRFJBMP",];

        public List<TypeModel> TypeList { get; } = [];

        public List<ChannelModel> ChannelList { get; } = [];

        private List<ComboBoxItemModel> SearchTypeList { get; } = [];

        private ObservableCollection<HistoryModel> QueryLinksHistoryCollection { get; } = [];

        private ObservableCollection<HistoryModel> SearchAppsHistoryCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public StoreSelectorUserControl()
        {
            InitializeComponent();

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

            SearchTypeList.Add(new ComboBoxItemModel() { SelectedValue = "ExactSearch", DisplayMember = ExactSearchString });
            SearchTypeList.Add(new ComboBoxItemModel() { SelectedValue = "ManifestSearch", DisplayMember = ManifestSearchString });

            SelectedType = TypeList[0];
            SelectedChannel = ChannelList[3];
            QueryLinksText = string.Empty;
            sampleLink = SampleLinkList[0];
            LinkPlaceHolderText = SampleTitleString + sampleLink;
            SelectedSearchType = SearchTypeList[0];
            SelectedItem = StoreSelectorBar.Items[0];
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制历史记录
        /// </summary>
        private async void OnCopyHistoryExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is HistoryModel history)
            {
                string copyHistory = await Task.Run(() =>
                {
                    return string.Format("[\n{0}\n{1}\n{2}\n{3}\n]\n", history.HistoryAppName, history.HistoryTypeName, history.HistoryChannelName, history.HistoryLink);
                });

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyHistory);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 删除查询链接历史记录
        /// </summary>
        private void OnQueryLinksDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is HistoryModel history)
            {
                QueryLinksHistoryCollection.Remove(history);
                HistoryStorageService.RemoveQueryLinksData(history.HistoryKey);

                if (QueryLinksHistoryCollection.Count is 0)
                {
                    QueryLinksHistoryAutoSuggestBox.IsSuggestionListOpen = false;
                }
            }
        }

        /// <summary>
        /// 删除搜索应用历史记录
        /// </summary>
        private void OnSearchAppsDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is HistoryModel history)
            {
                SearchAppsHistoryCollection.Remove(history);
                HistoryStorageService.RemoveSearchAppsData(history.HistoryKey);

                if (SearchAppsHistoryCollection.Count is 0)
                {
                    SearchAppsHistoryAutoSuggestBox.IsSuggestionListOpen = false;
                }
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：商店选择器——挂载的事件

        /// <summary>
        /// 点击选择器栏选中项发生变化时发生的事件
        /// </summary>
        private void OnSelectorBarSelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            if (!Equals(SelectedItem, sender.SelectedItem))
            {
                SelectedItem = sender.SelectedItem;
            }
        }

        /// <summary>
        /// 打开设置中的语言和区域
        /// </summary>
        private void OnLanguageAndRegionClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:regionformatting"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 了解应用具体的使用说明
        /// </summary>
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            storePage.ShowUseInstruction();
        }

        /// <summary>
        /// 关闭商店信息结果
        /// </summary>
        private void OnStoreInfoClosed(InfoBar sender, InfoBarClosedEventArgs args)
        {
            StoreInfoResultKind = StoreInfoResultKind.None;
        }

        /// <summary>
        /// 显示错误原因
        /// </summary>
        private void OnShowErrorReasonClicked(object sender, RoutedEventArgs args)
        {
            storePage.ShowUseInstruction();
        }

        #endregion 第二部分：商店选择器——挂载的事件

        #region 第三部分：查询链接控件——挂载的事件

        /// <summary>
        /// 清空查询链接输入的内容
        /// </summary>
        private void OnQueryLinksClearInputContentClicked(object sender, RoutedEventArgs args)
        {
            QueryLinksText = string.Empty;
        }

        /// <summary>
        /// 显示查询链接结果
        /// </summary>
        private void OnQueryLinksShowResultClicked(object sender, RoutedEventArgs args)
        {
            storePage.StoreControl = StoreControl.QueryLinksResult;
        }

        /// <summary>
        /// 查询链接输入框获取焦点后触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(AutoSuggestBox))]
        private void OnQueryLinksGotFocus(object sender, RoutedEventArgs args)
        {
            if (QueryLinksHistoryCollection.Count > 0 && sender is AutoSuggestBox autoSuggestBox)
            {
                autoSuggestBox.IsSuggestionListOpen = true;
            }
        }

        /// <summary>
        /// 查询链接输入框正在失去焦点时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(Button))]
        private void OnQueryLinksLosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            if (args.NewFocusedElement is Button)
            {
                args.TryCancel();
            }
        }

        /// <summary>
        /// 查询链接输入框失去焦点后触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(AutoSuggestBox))]
        private void OnQueryLinksLostFocus(object sender, RoutedEventArgs args)
        {
            if (sender is AutoSuggestBox autoSuggestBox)
            {
                autoSuggestBox.IsSuggestionListOpen = false;
            }
        }

        /// <summary>
        /// 当用户提交搜索查询时发生的事件
        /// </summary>
        private async void OnQueryLinksQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!IsQueryingLinks && args.ChosenSuggestion is null)
            {
                await QueryLinksAsync();
            }
        }

        /// <summary>
        /// 在更新可编辑控件组件的文本内容之前引发的事件
        /// </summary>
        private void OnQueryLinksSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (!IsQueryingLinks && args.SelectedItem is HistoryModel history)
            {
                SelectedType = TypeList.Find(item => string.Equals(item.InternalName, history.HistoryType, StringComparison.OrdinalIgnoreCase));
                SelectedChannel = ChannelList.Find(item => string.Equals(item.InternalName, history.HistoryChannel, StringComparison.OrdinalIgnoreCase));
                QueryLinksText = history.HistoryLink;
            }
        }

        /// <summary>
        /// 查询链接输入框内容发生改变时响应的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(AutoSuggestBox))]
        private void OnQueryLinksTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender is AutoSuggestBox autoSuggestBox)
            {
                QueryLinksText = autoSuggestBox.Text;
            }
        }

        /// <summary>
        /// 类型修改选择后修改样例文本
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnTypeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(SelectedType, comboBox.SelectedItem))
            {
                SelectedType = comboBox.SelectedItem is TypeModel type ? type : null;
                sampleLink = SampleLinkList[TypeList.FindIndex(item => string.Equals(item.InternalName, SelectedType.InternalName))];
                LinkPlaceHolderText = SampleTitleString + sampleLink;
            }
        }

        /// <summary>
        /// 通道选择修改
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnChannelSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(SelectedChannel, comboBox.SelectedItem))
            {
                SelectedChannel = comboBox.SelectedItem is ChannelModel channel ? channel : null;
            }
        }

        /// <summary>
        /// 查询链接
        /// </summary>
        private async void OnQueryLinksClicked(object sender, RoutedEventArgs args)
        {
            if (!IsQueryingLinks)
            {
                await QueryLinksAsync();
            }
        }

        #endregion 第三部分：查询链接控件——挂载的事件

        #region 第四部分：搜索应用控件——挂载的事件

        /// <summary>
        /// 选择搜索应用方式
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnSearchTypeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(SelectedSearchType, comboBox.SelectedItem))
            {
                SelectedSearchType = comboBox.SelectedItem is ComboBoxItemModel searchType ? searchType : null;
            }
        }

        /// <summary>
        /// 清空搜索应用输入的内容
        /// </summary>
        private void OnSearchAppsClearInputContentClicked(object sender, RoutedEventArgs args)
        {
            SearchAppsText = string.Empty;
        }

        /// <summary>
        /// 显示搜索应用结果
        /// </summary>
        private void OnSearchAppsShowResultClicked(object sender, RoutedEventArgs args)
        {
            storePage.StoreControl = StoreControl.SearchAppsResult;
        }

        /// <summary>
        /// 搜索应用输入框获取焦点后触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(AutoSuggestBox))]
        private void OnSearchAppsGotFocus(object sender, RoutedEventArgs args)
        {
            if (SearchAppsHistoryCollection.Count > 0 && sender is AutoSuggestBox autoSuggestBox)
            {
                autoSuggestBox.IsSuggestionListOpen = true;
            }
        }

        /// <summary>
        /// 搜索应用输入框正在失去焦点时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(Button))]
        private void OnSearchAppsLosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            if (args.NewFocusedElement is Button)
            {
                args.TryCancel();
            }
        }

        /// <summary>
        /// 搜索应用输入框失去焦点后触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(AutoSuggestBox))]
        private void OnSearchAppsLostFocus(object sender, RoutedEventArgs args)
        {
            if (sender is AutoSuggestBox autoSuggestBox)
            {
                autoSuggestBox.IsSuggestionListOpen = false;
            }
        }

        /// <summary>
        /// 当用户提交搜索查询时发生的事件
        /// </summary>
        private async void OnSearchAppsQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!IsSearchingApps && args.ChosenSuggestion is null)
            {
                await SearchAppsAsync();
            }
        }

        /// <summary>
        /// 在更新可编辑控件组件的文本内容之前引发的事件
        /// </summary>
        private void OnSearchAppsSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (!IsSearchingApps && args.SelectedItem is HistoryModel history && !string.IsNullOrEmpty(history.HistoryContent))
            {
                SearchAppsText = history.HistoryContent;
            }
        }

        /// <summary>
        /// 搜索应用文本框内容发生改变时响应的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(AutoSuggestBox))]
        private void OnSearchAppsTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender is AutoSuggestBox autoSuggestBox)
            {
                SearchAppsText = autoSuggestBox.Text;
            }
        }

        /// <summary>
        /// 搜索应用
        /// </summary>
        private async void OnSearchAppsClicked(object sender, RoutedEventArgs args)
        {
            await SearchAppsAsync();
        }

        #endregion 第四部分：搜索应用控件——挂载的事件

        /// <summary>
        /// 初始化商店选择用户控件
        /// </summary>
        public async Task InitializeStoreSelectorAsync(StorePage storePageData)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                storePage = storePageData;

                List<HistoryModel> queryLinksHistoryList = await Task.Run(HistoryStorageService.GetQueryLinksData);

                QueryLinksHistoryCollection.Clear();

                foreach (HistoryModel historyItem in queryLinksHistoryList)
                {
                    historyItem.HistoryTypeName = TypeList.Find(item => string.Equals(item.InternalName, historyItem.HistoryType, StringComparison.OrdinalIgnoreCase)) is TypeModel typeItem ? typeItem.DisplayName : string.Empty;
                    historyItem.HistoryChannelName = ChannelList.Find(item => string.Equals(item.InternalName, historyItem.HistoryChannel, StringComparison.OrdinalIgnoreCase)) is ChannelModel channelItem ? channelItem.DisplayName : string.Empty;
                    QueryLinksHistoryCollection.Add(historyItem);
                }

                HistoryStorageService.QueryLinksCleared += () =>
                {
                    DispatcherQueue.TryEnqueue(QueryLinksHistoryCollection.Clear);
                };

                HistoryStorageService.SearchAppsCleared += () =>
                {
                    DispatcherQueue.TryEnqueue(SearchAppsHistoryCollection.Clear);
                };
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public void UpdateData(List<string> dataList)
        {
            if (dataList.Count is 3)
            {
                SelectedItem = StoreSelectorBar.Items[0];
                SelectedType = Convert.ToInt32(dataList[0]) is -1 ? TypeList[0] : TypeList[Convert.ToInt32(dataList[0])];
                SelectedChannel = Convert.ToInt32(dataList[1]) is -1 ? ChannelList[3] : ChannelList[Convert.ToInt32(dataList[1])];
                QueryLinksText = dataList[2] is "PlaceHolderText" ? string.Empty : dataList[2];
            }
        }

        /// <summary>
        /// 获取选中项显示状态
        /// </summary>
        private Visibility GetSelectedItem(SelectorBarItem selectedItem, SelectorBarItem comparedSelectedItem)
        {
            return Equals(selectedItem, comparedSelectedItem) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取链接
        /// </summary>
        public async Task QueryLinksAsync()
        {
            if (!IsQueryingLinks || !IsSearchingApps)
            {
                // 设置获取数据时的相关控件状态
                IsQueryingLinks = true;
                IsQueryLinksResultVisible = false;
                QueryLinksHistoryAutoSuggestBox.IsSuggestionListOpen = false;
                QueryLinksText = string.IsNullOrEmpty(QueryLinksText) ? sampleLink : QueryLinksText;
                foreach (HistoryModel historyItem in QueryLinksHistoryCollection)
                {
                    historyItem.IsQuerying = true;
                }

                // 记录当前选定的选项和填入的内容
                int typeIndex = TypeList.FindIndex(item => string.Equals(item.InternalName, SelectedType.InternalName));
                int channelIndex = ChannelList.FindIndex(item => string.Equals(item.InternalName, SelectedChannel.InternalName));
                string link = QueryLinksText;

                // 商店接口查询方式
                if (string.Equals(QueryLinksModeService.QueryLinksMode, QueryLinksModeService.QueryLinksModeList[0]))
                {
                    (bool requestResult, bool isPackagedApp, AppInfoModel appInfoItem, List<QueryLinksResultModel> queryLinksResultList) = await Task.Run(async () =>
                    {
                        (bool requestResult, bool isPackagedApp, AppInfoModel appInfoItem, List<QueryLinksResultModel> queryLinksResultList) queryLinksResult = ValueTuple.Create<bool, bool, AppInfoModel, List<QueryLinksResultModel>>(false, false, null, null);

                        // 解析链接对应的产品 ID
                        string productId = Equals(SelectedType, TypeList[0]) ? QueryLinksHelper.ParseRequestContent(QueryLinksText) : QueryLinksText;
                        string cookie = await QueryLinksHelper.GetCookieAsync();

                        // 获取应用信息
                        (bool requestResult, AppInfoModel appInfo) appInformationResult = await QueryLinksHelper.GetAppInformationAsync(productId);
                        queryLinksResult.requestResult = appInformationResult.requestResult;
                        queryLinksResult.appInfoItem = appInformationResult.appInfo;

                        if (appInformationResult.requestResult)
                        {
                            List<QueryLinksResultModel> queryLinksResultList = [];

                            // 解析非商店应用数据
                            if (string.IsNullOrEmpty(appInformationResult.appInfo.CategoryID))
                            {
                                queryLinksResult.isPackagedApp = false;
                                queryLinksResultList.AddRange(await QueryLinksHelper.GetNonAppxPackagesAsync(productId));
                            }
                            // 解析商店应用数据
                            else
                            {
                                queryLinksResult.isPackagedApp = true;
                                string fileListXml = await QueryLinksHelper.GetFileListXmlAsync(cookie, appInformationResult.appInfo.CategoryID, ChannelList[channelIndex].InternalName);

                                if (!string.IsNullOrEmpty(fileListXml))
                                {
                                    List<QueryLinksResultModel> appxPackagesList = await QueryLinksHelper.GetAppxPackagesAsync(fileListXml, ChannelList[channelIndex].InternalName);
                                    foreach (QueryLinksResultModel appxPackage in appxPackagesList)
                                    {
                                        bool isExisted = false;
                                        foreach (QueryLinksResultModel queryLinksResultItem in queryLinksResultList)
                                        {
                                            if (string.Equals(queryLinksResultItem.FileName, appxPackage.FileName) && Equals(queryLinksResultItem.FileLink, appxPackage.FileLink) && Equals(queryLinksResultItem.FileSize, queryLinksResultItem.FileSize))
                                            {
                                                isExisted = true;
                                            }
                                        }

                                        if (!isExisted && !string.IsNullOrEmpty(appxPackage.FileLink))
                                        {
                                            queryLinksResultList.Add(appxPackage);
                                        }
                                    }
                                }
                            }

                            // 按设置选项设置的内容过滤列表
                            if (LinkFilterService.EncryptedPackageFilter)
                            {
                                queryLinksResultList.RemoveAll(item =>
                                string.Equals(Path.GetExtension(item.FileName), ".eappx", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(Path.GetExtension(item.FileName), ".emsix", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(Path.GetExtension(item.FileName), ".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(Path.GetExtension(item.FileName), ".emsixbundle", StringComparison.OrdinalIgnoreCase)
                                );
                            }

                            if (LinkFilterService.BlockMapFilter)
                            {
                                queryLinksResultList.RemoveAll(item => string.Equals(Path.GetExtension(item.FileName), ".blockmap", StringComparison.OrdinalIgnoreCase));
                            }

                            // 排序
                            queryLinksResultList.Sort((item1, item2) => item1.FileName.CompareTo(item2.FileName));
                            queryLinksResult.queryLinksResultList = queryLinksResultList;
                        }

                        return queryLinksResult;
                    });

                    IsQueryingLinks = false;
                    foreach (HistoryModel historyItem in QueryLinksHistoryCollection)
                    {
                        historyItem.IsQuerying = false;
                    }

                    if (requestResult)
                    {
                        // 获取成功
                        if (queryLinksResultList is not null && queryLinksResultList.Count > 0)
                        {
                            UpdateQueryLinksResultHistory(appInfoItem.Name, typeIndex, channelIndex, link);
                            IsQueryLinksResultVisible = true;
                            storePage.StoreControl = StoreControl.QueryLinksResult;
                            storePage.QueryLinksResult.UpdateQueryLinksResultData(appInfoItem, isPackagedApp, queryLinksResultList);
                        }
                        // 返回空数据
                        else
                        {
                            IsQueryLinksResultVisible = false;
                            storePage.StoreControl = StoreControl.StoreSelector;
                            storePage.StoreSelector.StoreInfoResultKind = StoreInfoResultKind.QueryLinksWarning;
                            storePage.QueryLinksResult.UpdateQueryLinksResultData(null, false, []);
                        }
                    }
                    else
                    {
                        // 获取错误
                        IsQueryLinksResultVisible = false;
                        storePage.StoreControl = StoreControl.StoreSelector;
                        storePage.StoreSelector.StoreInfoResultKind = StoreInfoResultKind.QueryLinksError;
                        storePage.QueryLinksResult.UpdateQueryLinksResultData(null, false, []);
                    }
                }
                // 第三方接口查询方式
                else if (string.Equals(QueryLinksModeService.QueryLinksMode, QueryLinksModeService.QueryLinksModeList[1]))
                {
                    (InfoBarSeverity requestState, bool isPackagedApp, string categoryId, List<QueryLinksResultModel> queryLinksList) = await Task.Run(async () =>
                    {
                        (InfoBarSeverity requestState, bool isPackagedApp, string categoryId, List<QueryLinksResultModel> queryLinksList) queryLinksResult = ValueTuple.Create<InfoBarSeverity, bool, string, List<QueryLinksResultModel>>(InfoBarSeverity.Error, false, null, null);

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
                            List<QueryLinksResultModel> queryLinksList = [];

                            // CategoryID 为空，非打包应用
                            if (string.IsNullOrEmpty(categoryId))
                            {
                                queryLinksResult.isPackagedApp = false;
                                List<QueryLinksResultModel> nonPackagedAppsList = HtmlParseHelper.HtmlParseNonPackagedAppLinks();
                                queryLinksList.AddRange(HtmlParseHelper.HtmlParseNonPackagedAppLinks());
                            }
                            else
                            {
                                queryLinksResult.isPackagedApp = true;
                                List<QueryLinksResultModel> packagedAppsList = HtmlParseHelper.HtmlParsePackagedAppLinks();

                                // 按设置选项设置的内容过滤列表
                                if (LinkFilterService.EncryptedPackageFilter)
                                {
                                    packagedAppsList.RemoveAll(item =>
                                    string.Equals(Path.GetExtension(item.FileName), ".eappx", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(Path.GetExtension(item.FileName), ".emsix", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(Path.GetExtension(item.FileName), ".eappxbundle", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(Path.GetExtension(item.FileName), ".emsixbundle", StringComparison.OrdinalIgnoreCase)
                                    );
                                }

                                if (LinkFilterService.BlockMapFilter)
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

                    IsQueryingLinks = false;
                    foreach (HistoryModel historyItem in QueryLinksHistoryCollection)
                    {
                        historyItem.IsQuerying = false;
                    }

                    if (requestState is InfoBarSeverity.Success)
                    {
                        UpdateQueryLinksResultHistory(categoryId, typeIndex, channelIndex, link);
                        IsQueryLinksResultVisible = true;
                        storePage.StoreControl = StoreControl.QueryLinksResult;
                        storePage.QueryLinksResult.UpdateQueryLinksResultData(null, isPackagedApp, queryLinksList);
                    }
                    else if (requestState is InfoBarSeverity.Warning)
                    {
                        IsQueryingLinks = false;
                        IsQueryLinksResultVisible = false;
                        storePage.StoreControl = StoreControl.StoreSelector;
                        storePage.StoreSelector.StoreInfoResultKind = StoreInfoResultKind.QueryLinksWarning;
                        storePage.QueryLinksResult.UpdateQueryLinksResultData(null, false, []);
                    }
                    else if (requestState is InfoBarSeverity.Error)
                    {
                        IsQueryingLinks = false;
                        IsQueryLinksResultVisible = false;
                        storePage.StoreControl = StoreControl.StoreSelector;
                        storePage.StoreSelector.StoreInfoResultKind = StoreInfoResultKind.QueryLinksError;
                        storePage.QueryLinksResult.UpdateQueryLinksResultData(null, false, []);
                    }
                }
            }
        }

        /// <summary>
        /// 搜索应用
        /// </summary>
        public async Task SearchAppsAsync()
        {
            if (!IsQueryingLinks || !IsSearchingApps)
            {
                IsSearchingApps = true;
                IsSearchAppsResultVisible = false;
                SearchAppsHistoryAutoSuggestBox.IsSuggestionListOpen = false;
                SearchAppsText = string.IsNullOrEmpty(SearchAppsText) ? "Microsoft Corporation" : SearchAppsText;
                foreach (HistoryModel historyItem in SearchAppsHistoryCollection)
                {
                    historyItem.IsQuerying = true;
                }

                (bool requestResult, List<SearchAppsResultModel> searchAppsResultList) = await Task.Run(async () =>
                {
                    if (Equals(SelectedSearchType, SearchTypeList[0]))
                    {
                        string searchText = SearchAppsText;
                        return await SearchAppsHelper.StoreExactSearchAsync(searchText);
                    }
                    else if (Equals(SelectedSearchType, SearchTypeList[1]))
                    {
                        string searchText = SearchAppsText;
                        string generatedContent = SearchAppsHelper.GenerateManifestSearchString(searchText);
                        return await SearchAppsHelper.ManifestSearchAsync(generatedContent);
                    }
                    else
                    {
                        return ValueTuple.Create<bool, List<SearchAppsResultModel>>(false, null);
                    }
                });

                IsSearchingApps = false;
                foreach (HistoryModel historyItem in SearchAppsHistoryCollection)
                {
                    historyItem.IsQuerying = false;
                }

                // 获取成功
                if (requestResult)
                {
                    // 搜索成功，有数据
                    if (searchAppsResultList.Count > 0)
                    {
                        UpdateSearchAppsHistory(SearchAppsText);
                        IsSearchAppsResultVisible = true;
                        storePage.StoreControl = StoreControl.SearchAppsResult;
                        storePage.SearchAppsResult.UpdateSearchAppsResultData(searchAppsResultList);
                    }
                    // 返回空数据
                    else
                    {
                        IsSearchAppsResultVisible = false;
                        storePage.StoreControl = StoreControl.StoreSelector;
                        storePage.StoreSelector.StoreInfoResultKind = StoreInfoResultKind.SearchAppsWarning;
                        storePage.SearchAppsResult.UpdateSearchAppsResultData([]);
                    }
                }
                else
                {
                    // 搜索失败
                    IsSearchAppsResultVisible = false;
                    storePage.StoreControl = StoreControl.StoreSelector;
                    storePage.StoreSelector.StoreInfoResultKind = StoreInfoResultKind.SearchAppsError;
                    storePage.SearchAppsResult.UpdateSearchAppsResultData([]);
                }
            }
        }

        /// <summary>
        /// 更新查询链接历史记录，包括主页历史记录内容、数据库中的内容和任务栏跳转列表中的内容
        /// </summary>
        private void UpdateQueryLinksResultHistory(string appName, int selectedType, int selectedChannel, string link)
        {
            Task.Run(() =>
            {
                // 计算时间戳
                long timeStamp = Convert.ToInt64((DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
                string historyKey = HashAlgorithmHelper.GenerateHistoryKey(TypeList[selectedType].InternalName, ChannelList[selectedChannel].InternalName, link);
                List<HistoryModel> historyList = [.. QueryLinksHistoryCollection];
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

                    // 保留前 20 项
                    if (historyList.Count > 20)
                    {
                        historyList.RemoveRange(20, historyList.Count - 20);
                    }
                    HistoryStorageService.SaveQueryLinksData(historyList);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        QueryLinksHistoryCollection.Clear();

                        foreach (HistoryModel historyItem in historyList)
                        {
                            QueryLinksHistoryCollection.Add(historyItem);
                        }
                    });
                }
                // 存在则修改原来项的时间戳，并调整顺序
                else
                {
                    HistoryModel historyItem = historyList[index];
                    historyItem.CreateTimeStamp = timeStamp;
                    historyItem.HistoryAppName = appName;
                    HistoryStorageService.UpdateQueryLinksData(historyItem);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        QueryLinksHistoryCollection.RemoveAt(index);
                        QueryLinksHistoryCollection.Insert(0, historyItem);
                    });
                }
            });
        }

        /// <summary>
        /// 更新搜索应用历史记录，包括主页历史记录内容、数据库中的内容和任务栏跳转列表中的内容
        /// </summary>
        private void UpdateSearchAppsHistory(string inputContent)
        {
            Task.Run(() =>
            {
                // 计算时间戳
                long timeStamp = Convert.ToInt64((DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
                string historyKey = HashAlgorithmHelper.GenerateHistoryKey(inputContent);
                List<HistoryModel> historyList = [.. SearchAppsHistoryCollection];
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

                    // 保留前 20 项
                    if (historyList.Count > 20)
                    {
                        historyList.RemoveRange(20, historyList.Count - 20);
                    }
                    HistoryStorageService.SaveSearchAppsData(historyList);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        SearchAppsHistoryCollection.Clear();

                        foreach (HistoryModel historyItem in historyList)
                        {
                            SearchAppsHistoryCollection.Add(historyItem);
                        }
                    });
                }
                // 存在则修改原来项的时间戳，并调整顺序
                else
                {
                    HistoryModel historyItem = historyList[index];
                    historyItem.CreateTimeStamp = timeStamp;
                    HistoryStorageService.UpdateSearchAppsData(historyItem);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        SearchAppsHistoryCollection.RemoveAt(index);
                        SearchAppsHistoryCollection.Insert(0, historyItem);
                    });
                }
            });
        }

        /// <summary>
        /// 获取显示商店信息结果
        /// </summary>
        private bool GetShowStoreInfo(StoreInfoResultKind storeInfoResultKind)
        {
            return storeInfoResultKind is not StoreInfoResultKind.None;
        }

        /// <summary>
        /// 获取商店信息结果显示状态
        /// </summary>
        private InfoBarSeverity GetStoreInfoSeverity(StoreInfoResultKind storeInfoResultKind)
        {
            if (storeInfoResultKind is StoreInfoResultKind.None)
            {
                return InfoBarSeverity.Informational;
            }
            else if (storeInfoResultKind is StoreInfoResultKind.QueryLinksWarning || storeInfoResultKind is StoreInfoResultKind.SearchAppsWarning)
            {
                return InfoBarSeverity.Warning;
            }
            else if (storeInfoResultKind is StoreInfoResultKind.QueryLinksError || storeInfoResultKind is StoreInfoResultKind.SearchAppsError)
            {
                return InfoBarSeverity.Error;
            }
            else
            {
                return InfoBarSeverity.Informational;
            }
        }

        /// <summary>
        /// 获取商店信息结果显示内容
        /// </summary>
        private Visibility GetStoreInfoResultKind(StoreInfoResultKind selectedStoreInfoResultKind, StoreInfoResultKind comparedStoreInfoResultKind)
        {
            return Equals(selectedStoreInfoResultKind, comparedStoreInfoResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
