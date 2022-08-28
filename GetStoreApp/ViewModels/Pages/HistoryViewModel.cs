using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.History;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Contracts.ViewModels;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Pages
{
    public class HistoryViewModel : ObservableRecipient, INavigationAware
    {
        // 临界区资源访问互斥锁
        private readonly object HistoryDataListLock = new object();

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private IHistoryDBService HistoryDBService { get; } = IOCHelper.GetService<IHistoryDBService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public List<GetAppTypeModel> TypeList => ResourceService.TypeList;

        public List<GetAppChannelModel> ChannelList => ResourceService.ChannelList;

        public ObservableCollection<HistoryModel> HistoryDataList { get; } = new ObservableCollection<HistoryModel>();

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set { SetProperty(ref _isSelectMode, value); }
        }

        private bool _isHistoryEmpty = true;

        public bool IsHistoryEmpty
        {
            get { return _isHistoryEmpty; }

            set { SetProperty(ref _isHistoryEmpty, value); }
        }

        private bool _isHistoryEmptyAfterFilter = true;

        public bool IsHistoryEmptyAfterFilter
        {
            get { return _isHistoryEmptyAfterFilter; }

            set { SetProperty(ref _isHistoryEmptyAfterFilter, value); }
        }

        /// <summary>
        /// 时间排序的规则，false为递减排序，true为递增排序
        /// </summary>
        private bool _timeSortOrder = false;

        public bool TimeSortOrder
        {
            get { return _timeSortOrder; }

            set { SetProperty(ref _timeSortOrder, value); }
        }

        private string _typeFilter = "None";

        public string TypeFilter
        {
            get { return _typeFilter; }

            set { SetProperty(ref _typeFilter, value); }
        }

        private string _channelFilter = "None";

        public string ChannelFilter
        {
            get { return _channelFilter; }

            set { SetProperty(ref _channelFilter, value); }
        }

        // 进入多选模式
        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            // 保证线程安全
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyItem in HistoryDataList)
                {
                    historyItem.IsSelected = false;
                }
            }

            IsSelectMode = true;
            await Task.CompletedTask;
        });

        // 按时间进行排序
        public IAsyncRelayCommand TimeSortCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            TimeSortOrder = Convert.ToBoolean(param);
            await GetHistoryDataListAsync();
        });

        // 按类型进行过滤
        public IAsyncRelayCommand TypeFilterCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            TypeFilter = param;
            await GetHistoryDataListAsync();
        });

        // 按通道进行过滤
        public IAsyncRelayCommand ChannelFilterCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            ChannelFilter = param;
            await GetHistoryDataListAsync();
        });

        // 刷新数据
        public IAsyncRelayCommand RefreshCommand => new AsyncRelayCommand(GetHistoryDataListAsync);

        // 全选
        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            // 保证线程安全
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyItem in HistoryDataList)
                {
                    historyItem.IsSelected = true;
                }
            }

            await Task.CompletedTask;
        });

        // 全部不选
        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(async () =>
        {
            // 保证线程安全
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyItem in HistoryDataList)
                {
                    historyItem.IsSelected = false;
                }
            }

            await Task.CompletedTask;
        });

        // 复制选定项目的内容
        public IAsyncRelayCommand CopySelectedCommand => new AsyncRelayCommand(async () =>
        {
            List<HistoryModel> SelectedHistoryDataList = HistoryDataList.Where(item => item.IsSelected == true).ToList();

            if (SelectedHistoryDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            StringBuilder stringBuilder = new StringBuilder();

            foreach (HistoryModel selectedHistoryData in SelectedHistoryDataList)
            {
                stringBuilder.Append(string.Format("{0}\t{1}\t{2}\n",
                    TypeList.Find(i => i.InternalName.Equals(selectedHistoryData.HistoryType)).DisplayName,
                    ChannelList.Find(i => i.InternalName.Equals(selectedHistoryData.HistoryChannel)).DisplayName,
                    selectedHistoryData.HistoryLink));
            }

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            await Task.CompletedTask;
        });

        // 删除选定的项目
        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand(async () =>
        {
            List<HistoryModel> SelectedHistoryDataList = HistoryDataList.Where(item => item.IsSelected == true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedHistoryDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog().ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                bool DeleteResult = await HistoryDBService.DeleteHistoryDataAsync(SelectedHistoryDataList);

                if (DeleteResult)
                {
                    // 确保线程安全
                    lock (HistoryDataListLock)
                    {
                        foreach (HistoryModel historyItem in SelectedHistoryDataList)
                        {
                            HistoryDataList.Remove(historyItem);
                        }
                    }
                }

                if (TypeFilter == "None" || ChannelFilter == "None")
                {
                    IsHistoryEmpty = true;
                    IsHistoryEmptyAfterFilter = false;
                }
                else
                {
                    IsHistoryEmpty = false;
                    IsHistoryEmptyAfterFilter = true;
                }

                Messenger.Send(new HistoryMessage(true));
            }
        });

        // 退出多选模式
        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        // 在多选模式下点击项目选择相应的条目
        public IAsyncRelayCommand ItemClickCommand => new AsyncRelayCommand<ItemClickEventArgs>(async (param) =>
        {
            HistoryModel historyItem = (HistoryModel)param.ClickedItem;
            int ClickedIndex = HistoryDataList.IndexOf(historyItem);

            lock (HistoryDataListLock)
            {
                HistoryDataList[ClickedIndex].IsSelected = !HistoryDataList[ClickedIndex].IsSelected;
            }

            await Task.CompletedTask;
        });

        // 填入指定项目的内容
        public IAsyncRelayCommand FillinCommand => new AsyncRelayCommand<HistoryModel>(async (param) =>
        {
            Messenger.Send(new FillinMessage(param));
            NavigationService.NavigateTo(typeof(HomeViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
        });

        // 复制指定项目的内容
        public IAsyncRelayCommand CopyCommand => new AsyncRelayCommand<HistoryModel>(async (param) =>
        {
            string CopyContent = string.Format("{0}\t{1}\t{2}",
                TypeList.Find(item => item.InternalName.Equals(param.HistoryType)).DisplayName,
                ChannelList.Find(item => item.InternalName.Equals(param.HistoryChannel)).DisplayName,
                param.HistoryLink);
            CopyPasteHelper.CopyToClipBoard(CopyContent);

            await Task.CompletedTask;
        });

        // 导航到历史记录页面时，历史记录数据列表初始化，从数据库中存储的列表中加载
        public async void OnNavigatedTo(object parameter)
        {
            await GetHistoryDataListAsync();
        }

        public void OnNavigatedFrom()
        { }

        /// <summary>
        /// 从数据库中加载数据
        /// </summary>
        private async Task GetHistoryDataListAsync()
        {
            Tuple<List<HistoryModel>, bool, bool> HistoryAllData = await HistoryDBService.QueryAllHistoryDataAsync(TimeSortOrder, TypeFilter, ChannelFilter);

            // 获取数据库的原始记录数据
            List<HistoryModel> HistoryRawList = HistoryAllData.Item1;
            // 数据库中的历史记录表是否为空
            IsHistoryEmpty = HistoryAllData.Item2;

            // 经过筛选后历史记录是否为空
            IsHistoryEmptyAfterFilter = HistoryAllData.Item3;

            // 保证线程安全
            lock (HistoryDataListLock)
            {
                HistoryDataList.Clear();
            }

            // 保证线程安全
            lock (HistoryDataListLock)
            {
                foreach (HistoryModel historyRawData in HistoryRawList)
                {
                    HistoryDataList.Add(historyRawData);
                }
            }
        }
    }
}
