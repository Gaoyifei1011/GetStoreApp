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
        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private IHistoryDataService HistoryDataService { get; } = IOCHelper.GetService<IHistoryDataService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

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

        private HistoryModel _selectedHistoryItem;

        public HistoryModel SelectedHistoryItem
        {
            get { return _selectedHistoryItem; }

            set { SetProperty(ref _selectedHistoryItem, value); }
        }

        public List<GetAppTypeModel> TypeList => ResourceService.TypeList;

        public List<GetAppChannelModel> ChannelList => ResourceService.ChannelList;

        public ObservableCollection<HistoryModel> HistoryDataList { get; } = new ObservableCollection<HistoryModel>();

        public IAsyncRelayCommand FillinCommand => new AsyncRelayCommand(FillinAsync);

        public IAsyncRelayCommand CopyCommand => new AsyncRelayCommand(CopyAsync);

        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            await SelectNoneAsync();
            IsSelectMode = true;
        });

        public IAsyncRelayCommand TimeSortCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            TimeSortOrder = Convert.ToBoolean(param);
            await GetHistoryDataListAsync();
        });

        public IAsyncRelayCommand TypeFilterCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            TypeFilter = param;
            await GetHistoryDataListAsync();
        });

        public IAsyncRelayCommand ChannelFilterCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            ChannelFilter = param;
            await GetHistoryDataListAsync();
        });

        public IAsyncRelayCommand RefreshCommand => new AsyncRelayCommand(GetHistoryDataListAsync);

        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            foreach (HistoryModel historyItem in HistoryDataList)
            {
                historyItem.IsSelected = true;
            }
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(SelectNoneAsync);

        public IAsyncRelayCommand CopySelectedCommand => new AsyncRelayCommand(CopySelectedAsync);

        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand(DeleteAsync);

        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        public HistoryViewModel()
        {
            Messenger.Register<HistoryViewModel, HistoryMessage>(this, async (historyItemViewModel, historyMessage) =>
            {
                if (historyMessage.Value)
                {
                    await GetHistoryDataListAsync();
                }
            });
        }

        // 导航到历史记录页面时，历史记录数据列表初始化，从数据库中存储的列表中加载
        public async void OnNavigatedTo(object parameter)
        {
            await GetHistoryDataListAsync();
        }

        public void OnNavigatedFrom()
        { }

        /// <summary>
        /// 多选模式下删除选中的条目
        /// </summary>
        private async Task DeleteAsync()
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

                await HistoryDataService.DeleteHistoryDataAsync(SelectedHistoryDataList);

                await GetHistoryDataListAsync();

                Messenger.Send(new HistoryMessage(true));
            }
        }

        /// <summary>
        /// 从数据库中加载数据
        /// </summary>
        private async Task GetHistoryDataListAsync()
        {
            Tuple<List<HistoryModel>, bool, bool> HistoryAllData = await HistoryDataService.QueryAllHistoryDataAsync(TimeSortOrder, TypeFilter, ChannelFilter);

            // 获取数据库的原始记录数据
            List<HistoryModel> HistoryRawList = HistoryAllData.Item1;
            // 数据库中的历史记录表是否为空
            IsHistoryEmpty = HistoryAllData.Item2;

            // 经过筛选后历史记录是否为空
            IsHistoryEmptyAfterFilter = HistoryAllData.Item3;

            //Todo: need to debug why observablecollection clear cause object reference not set to an instance of an object.
            try
            {
                // 更新UI上面的数据
                ConvertRawListToDisplayList(ref HistoryRawList);
            }
            catch (Exception)
            {
                ConvertRawListToDisplayList(ref HistoryRawList);
            }
        }

        /// <summary>
        /// 将原始数据转换为在UI界面上呈现出来的数据
        /// </summary>
        private void ConvertRawListToDisplayList(ref List<HistoryModel> historyRawList)
        {
            HistoryDataList.Clear();

            foreach (HistoryModel historyRawData in historyRawList)
            {
                HistoryDataList.Add(historyRawData);
            }
        }

        /// <summary>
        /// 将选中的历史记录条目填入到选择控件中，然后点击“获取链接”即可获取
        /// </summary>
        private async Task FillinAsync()
        {
            if (SelectedHistoryItem == null)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            Messenger.Send(new FillinMessage(SelectedHistoryItem));
            NavigationService.NavigateTo(typeof(HomeViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
        }

        /// <summary>
        /// 点击全部不选按钮时，让复选框的历史记录全部不选
        /// </summary>
        private async Task SelectNoneAsync()
        {
            foreach (HistoryModel historyItem in HistoryDataList)
            {
                historyItem.IsSelected = false;
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 单选模式下复制选中的条目
        /// </summary>
        private async Task CopyAsync()
        {
            if (SelectedHistoryItem == null)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            };

            string CopyContent = string.Format("{0}\t{1}\t{2}",
                TypeList.Find(item => item.InternalName.Equals(SelectedHistoryItem.HistoryType)).DisplayName,
                ChannelList.Find(item => item.InternalName.Equals(SelectedHistoryItem.HistoryChannel)).DisplayName,
                SelectedHistoryItem.HistoryLink);
            CopyPasteHelper.CopyToClipBoard(CopyContent);

            await Task.CompletedTask;
        }

        /// <summary>
        /// 多选模式下复制选中的条目
        /// </summary>
        private async Task CopySelectedAsync()
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
        }
    }
}
