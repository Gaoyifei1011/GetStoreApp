using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.History;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class HistoryItemViewModel : ObservableRecipient
    {
        private HistoryItemValueModel HistoryItem { get; set; }

        private IResourceService ResourceService { get; } = App.GetService<IResourceService>();

        private IHistoryDataService HistoryDataService { get; } = App.GetService<IHistoryDataService>();

        private IHistoryItemValueService HistoryItemValueService { get; } = App.GetService<IHistoryItemValueService>();

        private INavigationService NavigationService { get; } = App.GetService<INavigationService>();

        private HistoryModel _selectedHistoryItem;

        public HistoryModel SelectedHistoryItem
        {
            get { return _selectedHistoryItem; }

            set { SetProperty(ref _selectedHistoryItem, value); }
        }

        public List<GetAppTypeModel> TypeList { get; set; }

        public List<GetAppChannelModel> ChannelList { get; set; }

        public ObservableCollection<HistoryModel> HistoryItemDataList { get; set; } = new ObservableCollection<HistoryModel>();

        public IAsyncRelayCommand LoadedCommand { get; set; }

        public IAsyncRelayCommand ViewAllCommand { get; set; }

        public IAsyncRelayCommand CopyCommand { get; set; }

        public IAsyncRelayCommand FillinCommand { get; set; }

        public HistoryItemViewModel()
        {
            TypeList = ResourceService.TypeList;
            ChannelList = ResourceService.ChannelList;

            HistoryItem = HistoryItemValueService.HistoryItem;

            // List列表初始化，可以从数据库获得的列表中加载
            LoadedCommand = new AsyncRelayCommand(GetHistoryItemDataListAsync);

            ViewAllCommand = new AsyncRelayCommand(ViewAllAsync);

            FillinCommand = new AsyncRelayCommand(FillinAsync);

            CopyCommand = new AsyncRelayCommand(CopyAsync);

            Messenger.Register<HistoryItemViewModel, HistoryMessage>(this, async (historyItemViewModel, historyMessage) =>
            {
                if (historyMessage.Value) await GetHistoryItemDataListAsync();
            });

            Messenger.Register<HistoryItemViewModel, HistoryItemValueMessage>(this, async (historyItemViewModel, historyItemValueMessage) =>
            {
                HistoryItem = historyItemValueMessage.Value;
                await GetHistoryItemDataListAsync();
            });
        }

        /// <summary>
        /// UI加载完成时/或者是数据库数据发生变化时，从数据库中异步加载数据
        /// </summary>
        private async Task GetHistoryItemDataListAsync()
        {
            // 获取数据库的原始记录数据
            List<HistoryModel> HistoryRawList = await HistoryDataService.QueryHistoryDataAsync(HistoryItem.HistoryItemValue);

            // 更新UI上面的数据
            UpdateList(HistoryRawList);
        }

        /// <summary>
        /// 更新UI上面的数据
        /// </summary>
        private void UpdateList(List<HistoryModel> historyRawList)
        {
            HistoryItemDataList.Clear();

            foreach (HistoryModel historyRawData in historyRawList) HistoryItemDataList.Add(historyRawData);
        }

        private async Task ViewAllAsync()
        {
            NavigationService.NavigateTo(typeof(HistoryViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
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
            await Task.CompletedTask;
        }

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
        /// 选中内容为空时，显示提示对话框
        /// </summary>
        private async Task ShowSelectEmptyPromptDialogAsync()
        {
            SelectEmptyPromptDialog dialog = new SelectEmptyPromptDialog { XamlRoot = App.MainWindow.Content.XamlRoot };
            await dialog.ShowAsync();
        }
    }
}
