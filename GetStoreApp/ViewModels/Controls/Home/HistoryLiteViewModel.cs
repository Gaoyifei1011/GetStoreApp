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
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class HistoryLiteViewModel : ObservableRecipient
    {
        private HistoryLiteNumModel HistoryLiteItem { get; set; }

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private IHistoryDBService HistoryDBService { get; } = IOCHelper.GetService<IHistoryDBService>();

        private IHistoryLiteNumService HistoryLiteNumService { get; } = IOCHelper.GetService<IHistoryLiteNumService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public List<GetAppTypeModel> TypeList => ResourceService.TypeList;

        public List<GetAppChannelModel> ChannelList => ResourceService.ChannelList;

        public ObservableCollection<HistoryModel> HistoryLiteDataList { get; } = new ObservableCollection<HistoryModel>();

        // List列表初始化，可以从数据库获得的列表中加载
        public IAsyncRelayCommand LoadedCommand => new AsyncRelayCommand(GetHistoryLiteDataListAsync);

        public IAsyncRelayCommand ViewAllCommand => new AsyncRelayCommand(async () =>
        {
            NavigationService.NavigateTo(typeof(HistoryViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand CopyCommand => new AsyncRelayCommand<HistoryModel>(async (param) =>
        {
            await CopyAsync(param);
        });

        public IAsyncRelayCommand FillinCommand => new AsyncRelayCommand<HistoryModel>(async (param) =>
        {
            await FillinAsync(param);
        });

        public HistoryLiteViewModel()
        {
            HistoryLiteItem = HistoryLiteNumService.HistoryLiteNum;

            Messenger.Register<HistoryLiteViewModel, HistoryMessage>(this, async (historyItemViewModel, historyMessage) =>
            {
                if (historyMessage.Value)
                {
                    await GetHistoryLiteDataListAsync();
                }
            });

            Messenger.Register<HistoryLiteViewModel, HistoryItemValueMessage>(this, async (historyItemViewModel, historyItemValueMessage) =>
            {
                HistoryLiteItem = historyItemValueMessage.Value;
                await GetHistoryLiteDataListAsync();
            });
        }

        /// <summary>
        /// UI加载完成时/或者是数据库数据发生变化时，从数据库中异步加载数据
        /// </summary>
        private async Task GetHistoryLiteDataListAsync()
        {
            // 获取数据库的原始记录数据
            List<HistoryModel> HistoryRawList = await HistoryDBService.QueryHistoryDataAsync(HistoryLiteItem.HistoryLiteNumValue);

            // 更新UI上面的数据
            UpdateList(HistoryRawList);
        }

        /// <summary>
        /// 更新UI上面的数据
        /// </summary>
        private void UpdateList(List<HistoryModel> historyRawList)
        {
            HistoryLiteDataList.Clear();

            foreach (HistoryModel historyRawData in historyRawList)
            {
                HistoryLiteDataList.Add(historyRawData);
            }
        }

        /// <summary>
        /// 将选中的历史记录条目填入到选择控件中，然后点击“获取链接”即可获取
        /// </summary>
        private async Task FillinAsync(HistoryModel historyItem)
        {
            Messenger.Send(new FillinMessage(historyItem));
            await Task.CompletedTask;
        }

        /// <summary>
        /// 复制到剪贴板
        /// </summary>
        private async Task CopyAsync(HistoryModel historyItem)
        {
            string CopyContent = string.Format("{0}\t{1}\t{2}",
                TypeList.Find(item => item.InternalName.Equals(historyItem.HistoryType)).DisplayName,
                ChannelList.Find(item => item.InternalName.Equals(historyItem.HistoryChannel)).DisplayName,
                historyItem.HistoryLink);
            CopyPasteHelper.CopyToClipBoard(CopyContent);

            await Task.CompletedTask;
        }
    }
}
