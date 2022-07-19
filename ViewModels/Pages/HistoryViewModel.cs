using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.History;
using GetStoreApp.Contracts.Services.Shell;
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
    public class HistoryViewModel : ObservableRecipient
    {
        private readonly IResourceService ResourceService;
        private readonly IHistoryDataService HistoryDataService;
        private readonly INavigationService NavigationService;

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

        public List<GetAppTypeModel> TypeList { get; set; }

        public List<GetAppChannelModel> ChannelList { get; set; }

        public ObservableCollection<HistoryModel> HistoryDataList { get; set; } = new ObservableCollection<HistoryModel>();

        public IAsyncRelayCommand LoadedCommand { get; set; }

        public IAsyncRelayCommand FillinCommand { get; set; }

        public IAsyncRelayCommand CopyCommand { get; set; }

        public IAsyncRelayCommand SelectCommand { get; set; }

        public IAsyncRelayCommand TimeSortCommand { get; set; }

        public IAsyncRelayCommand TypeFilterCommand { get; set; }

        public IAsyncRelayCommand ChannelFilterCommand { get; set; }

        public IAsyncRelayCommand RefreshCommand { get; set; }

        public IAsyncRelayCommand SelectAllCommand { get; set; }

        public IAsyncRelayCommand SelectNoneCommand { get; set; }

        public IAsyncRelayCommand CopySelectedCommand { get; set; }

        public IAsyncRelayCommand DeleteCommand { get; set; }

        public IAsyncRelayCommand CancelCommand { get; set; }

        public HistoryViewModel(IResourceService resourceService, IHistoryDataService historyDataService, INavigationService navigationService)
        {
            ResourceService = resourceService;
            HistoryDataService = historyDataService;
            NavigationService = navigationService;

            TypeList = ResourceService.TypeList;
            ChannelList = ResourceService.ChannelList;

            // List列表初始化，可以从数据库获得的列表中加载
            LoadedCommand = new AsyncRelayCommand(GetHistoryDataListAsync);

            FillinCommand = new AsyncRelayCommand(FillinAsync);

            CopyCommand = new AsyncRelayCommand(CopyAsync);

            SelectCommand = new AsyncRelayCommand(async () =>
            {
                await SelectNoneAsync();
                IsSelectMode = true;
            });

            TimeSortCommand = new AsyncRelayCommand<string>(async (param) =>
            {
                TimeSortOrder = Convert.ToBoolean(param);
                await GetHistoryDataListAsync();
            });

            TypeFilterCommand = new AsyncRelayCommand<string>(async (param) =>
            {
                TypeFilter = param;
                await GetHistoryDataListAsync();
            });

            ChannelFilterCommand = new AsyncRelayCommand<string>(async (param) =>
            {
                ChannelFilter = param;
                await GetHistoryDataListAsync();
            });

            RefreshCommand = new AsyncRelayCommand(GetHistoryDataListAsync);

            SelectAllCommand = new AsyncRelayCommand(async () =>
            {
                foreach (var item in HistoryDataList) item.IsSelected = true;
                await Task.CompletedTask;
            });

            SelectNoneCommand = new AsyncRelayCommand(SelectNoneAsync);

            CopySelectedCommand = new AsyncRelayCommand(CopySelectedAsync);

            DeleteCommand = new AsyncRelayCommand(DeleteAsync);

            CancelCommand = new AsyncRelayCommand(async () =>
            {
                IsSelectMode = false;
                await Task.CompletedTask;
            });

            Messenger.Register<HistoryViewModel, HistoryMessage>(this, async (historyItemViewModel, historyMessage) =>
            {
                if (historyMessage.Value) await GetHistoryDataListAsync();
            });
        }

        /// <summary>
        /// 多选模式下删除选中的条目
        /// </summary>
        private async Task DeleteAsync()
        {
            List<HistoryModel> SelectedHistoryDataList = HistoryDataList.Where(item => item.IsSelected == true).ToList();

            // 内容为空时显示空提示对话框
            if (SelectedHistoryDataList.Count == 0)
            {
                await ShowSelectEmptyPromptDialogAsync();
                return;
            };

            // 删除时显示删除确认对话框
            ContentDialogResult result = await ShowDeletePromptDialogAsync();

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
            Tuple<List<HistoryModel>, bool, bool> QueryHistoryAllData = await HistoryDataService.QueryAllHistoryDataAsync(TimeSortOrder, TypeFilter, ChannelFilter);

            // 获取数据库的原始记录数据
            List<HistoryModel> HistoryRawList = QueryHistoryAllData.Item1;

            // 数据库中的历史记录表是否为空
            IsHistoryEmpty = QueryHistoryAllData.Item2;

            // 经过筛选后历史记录是否为空
            IsHistoryEmptyAfterFilter = QueryHistoryAllData.Item3;

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
                historyRawData.IsSelected = false;
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
                await ShowSelectEmptyPromptDialogAsync();
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
            foreach (var item in HistoryDataList) item.IsSelected = false;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 单选模式下复制选中的条目
        /// </summary>
        private async Task CopyAsync()
        {
            if (SelectedHistoryItem == null)
            {
                await ShowSelectEmptyPromptDialogAsync();
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
                await ShowSelectEmptyPromptDialogAsync();
                return;
            };

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in SelectedHistoryDataList)
            {
                stringBuilder.Append(string.Format("{0}\t{1}\t{2}\n",
                    TypeList.Find(i => i.InternalName.Equals(item.HistoryType)).DisplayName,
                    ChannelList.Find(i => i.InternalName.Equals(item.HistoryChannel)).DisplayName,
                    item.HistoryLink));
            }

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            await Task.CompletedTask;
        }

        /// <summary>
        /// 选中内容为空时，显示提示对话框
        /// </summary>
        private async Task ShowSelectEmptyPromptDialogAsync()
        {
            SelectEmptyPromptDialog dialog = new SelectEmptyPromptDialog();
            dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
            await dialog.ShowAsync();
        }

        private async Task<ContentDialogResult> ShowDeletePromptDialogAsync()
        {
            DeletePromptDialog dialog = new DeletePromptDialog();
            dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
            return await dialog.ShowAsync();
        }
    }
}
