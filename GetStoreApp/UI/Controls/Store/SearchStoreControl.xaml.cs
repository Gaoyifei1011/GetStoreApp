using GetStoreApp.Helpers.Controls.Store;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
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
        private readonly object HistoryLock = new object();
        private readonly object SearchStoreLock = new object();

        private string SearchStoreCountInfo = ResourceService.GetLocalized("Store/SearchStoreCountInfo");

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

        private InfoBarSeverity _infoSeverity = InfoBarSeverity.Informational;

        public InfoBarSeverity InfoBarSeverity
        {
            get { return _infoSeverity; }

            set
            {
                _infoSeverity = value;
                OnPropertyChanged();
            }
        }

        private string _stateInfoText = ResourceService.GetLocalized("Store/StatusInfoWelcome");

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
                StorePage storePage = NavigationService.NavigationFrame.Content as StorePage;
                if (storePage is not null)
                {
                    SearchText = historyItem.HistoryAppName;
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
                StorePage storePage = NavigationService.NavigationFrame.Content as StorePage;
                if (storePage is not null)
                {
                    storePage.QueryLinks.SelectedType = storePage.QueryLinks.TypeList[0];
                    storePage.QueryLinks.LinkText = appLink;
                    storePage.SelectedIndex = 0;
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

        /// <summary>
        /// 显示复制选项
        /// </summary>
        private void OnCopyOptionsClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as MenuFlyoutItem);
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
        /// 搜索应用
        /// </summary>
        public void SearchStore()
        {
            SearchText = string.IsNullOrEmpty(SearchText) ? "Microsoft Corporation" : SearchText;
            IsSeachingStore = true;
            // 设置InfoBar控件状态

            Task.Run(async () =>
            {
                string searchText = SearchText;
                string generatedContent = SearchStoreHelper.GenerateSearchString(searchText);

                Tuple<bool, List<SearchStoreModel>> searchStoreResult = await SearchStoreHelper.SerachStoreAppsAsync(searchText);

                if (searchStoreResult.Item1)
                {
                    IsSeachingStore = false;

                    if (searchStoreResult.Item2.Count is 0)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            // 设置InfoBar控件的状态
                            ResultControlVisable = false;
                        });
                    }
                    else
                    {
                        // 设置InfoBar控件的状态
                        ResultControlVisable = true;
                    }
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        IsSeachingStore = false;
                        // 设置InfoBar控件的状态
                        ResultControlVisable = false;
                    });
                }
            });
        }
    }
}
