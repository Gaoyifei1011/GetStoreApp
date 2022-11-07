using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.History;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.History;
using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Models.Notifications;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class HistoryLiteViewModel : ObservableRecipient
    {
        // 临界区资源访问互斥锁
        private readonly object HistoryLiteDataListLock = new object();

        private HistoryLiteNumModel HistoryLiteItem { get; set; }

        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        private IHistoryDBService HistoryDBService { get; } = ContainerHelper.GetInstance<IHistoryDBService>();

        private IHistoryLiteNumService HistoryLiteNumService { get; } = ContainerHelper.GetInstance<IHistoryLiteNumService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        public List<TypeModel> TypeList => ResourceService.TypeList;

        public List<ChannelModel> ChannelList => ResourceService.ChannelList;

        public ObservableCollection<HistoryModel> HistoryLiteDataList { get; } = new ObservableCollection<HistoryModel>();

        // 列表初始化，可以从数据库获得的列表中加载
        public IRelayCommand LoadedCommand => new RelayCommand(async () =>
        {
            await GetHistoryLiteDataListAsync();
        });

        // 查看全部
        public IRelayCommand ViewAllCommand => new RelayCommand(() =>
        {
            NavigationService.NavigateTo(typeof(HistoryViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
        });

        // 复制到剪贴板
        public IRelayCommand CopyCommand => new RelayCommand<HistoryModel>((historyItem) =>
        {
            string CopyContent = string.Format("{0}\t{1}\t{2}",
                TypeList.Find(item => item.InternalName.Equals(historyItem.HistoryType)).DisplayName,
                ChannelList.Find(item => item.InternalName.Equals(historyItem.HistoryChannel)).DisplayName,
                historyItem.HistoryLink);
            CopyPasteHelper.CopyToClipBoard(CopyContent);

            WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
            {
                NotificationArgs = InAppNotificationArgs.HistoryCopy,
                NotificationValue = new object[] { true, false }
            }));
        });

        // 填入到文本框
        public IRelayCommand FillinCommand => new RelayCommand<HistoryModel>((historyItem) =>
        {
            WeakReferenceMessenger.Default.Send(new FillinMessage(historyItem));
        });

        public HistoryLiteViewModel()
        {
            HistoryLiteItem = HistoryLiteNumService.HistoryLiteNum;

            WeakReferenceMessenger.Default.Register<HistoryLiteViewModel, HistoryMessage>(this, async (historyLiteViewModel, historyMessage) =>
            {
                if (historyMessage.Value)
                {
                    await GetHistoryLiteDataListAsync();
                }
            });

            WeakReferenceMessenger.Default.Register<HistoryLiteViewModel, HistoryLiteNumMessage>(this, async (historyLiteViewModel, historyLiteNumMessage) =>
            {
                HistoryLiteItem = historyLiteNumMessage.Value;
                await GetHistoryLiteDataListAsync();
            });

            WeakReferenceMessenger.Default.Register<HistoryLiteViewModel, WindowClosedMessage>(this, (historyLiteViewModel, windowClosedMessage) =>
            {
                if (windowClosedMessage.Value)
                {
                    WeakReferenceMessenger.Default.UnregisterAll(this);
                }
            });
        }

        /// <summary>
        /// UI加载完成时/或者是数据库数据发生变化时，从数据库中异步加载数据
        /// </summary>
        private async Task GetHistoryLiteDataListAsync()
        {
            // 获取数据库的原始记录数据
            List<HistoryModel> HistoryRawList = await HistoryDBService.QueryAsync(HistoryLiteItem.HistoryLiteNumValue);

            lock (HistoryLiteDataListLock)
            {
                HistoryLiteDataList.Clear();
            }

            lock (HistoryLiteDataListLock)
            {
                foreach (HistoryModel historyRawData in HistoryRawList)
                {
                    HistoryLiteDataList.Add(historyRawData);
                }
            }
        }
    }
}
