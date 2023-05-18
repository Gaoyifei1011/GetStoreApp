using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.History;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Notifications;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Store
{
    /// <summary>
    /// 微软商店页面：部分历史记录用户控件视图模型
    /// </summary>
    public sealed class HistoryLiteViewModel
    {
        // 临界区资源访问互斥锁
        private readonly object HistoryLiteDataListLock = new object();

        private HistoryLiteNumModel HistoryLiteItem { get; set; }

        public List<TypeModel> TypeList { get; } = ResourceService.TypeList;

        public List<ChannelModel> ChannelList { get; } = ResourceService.ChannelList;

        // 复制到剪贴板
        public XamlUICommand CopyCommand { get; } = new XamlUICommand();

        // 填入到文本框
        public XamlUICommand FillinCommand { get; } = new XamlUICommand();

        public ObservableCollection<HistoryModel> HistoryLiteDataList { get; } = new ObservableCollection<HistoryModel>();

        /// <summary>
        /// 列表初始化，可以从数据库获得的列表中加载
        /// </summary>
        public async void OnLoaded(object sender, RoutedEventArgs args)
        {
            await GetHistoryLiteDataListAsync();
        }

        /// <summary>
        /// 查看全部
        /// </summary>
        public void OnViewAllClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(HistoryPage), AppNaviagtionArgs.History);
        }

        public HistoryLiteViewModel()
        {
            HistoryLiteItem = HistoryRecordService.HistoryLiteNum;

            CopyCommand.ExecuteRequested += (sender, args) =>
            {
                HistoryModel historyItem = args.Parameter as HistoryModel;

                if (historyItem is not null)
                {
                    string CopyContent = string.Format("{0}\t{1}\t{2}",
                        TypeList.Find(item => item.InternalName.Equals(historyItem.HistoryType)).DisplayName,
                        ChannelList.Find(item => item.InternalName.Equals(historyItem.HistoryChannel)).DisplayName,
                        historyItem.HistoryLink);
                    CopyPasteHelper.CopyToClipBoard(CopyContent);

                    new HistoryCopyNotification(true, false).Show();
                }
            };

            FillinCommand.ExecuteRequested += (sender, args) =>
            {
                HistoryModel historyItem = args.Parameter as HistoryModel;

                if (historyItem is not null)
                {
                    Messenger.Default.Send(historyItem, MessageToken.Fillin);
                }
            };

            Messenger.Default.Register<bool>(this, MessageToken.History, async (historyMessage) =>
            {
                if (historyMessage)
                {
                    await GetHistoryLiteDataListAsync();
                }
            });

            Messenger.Default.Register<HistoryLiteNumModel>(this, MessageToken.HistoryLiteNum, async (historyLiteNumMessage) =>
            {
                HistoryLiteItem = historyLiteNumMessage;
                await GetHistoryLiteDataListAsync();
            });

            Messenger.Default.Register<bool>(this, MessageToken.WindowClosed, (windowClosedMessage) =>
            {
                if (windowClosedMessage)
                {
                    Messenger.Default.Unregister(this);
                }
            });
        }

        /// <summary>
        /// UI加载完成时/或者是数据库数据发生变化时，从数据库中异步加载数据
        /// </summary>
        private async Task GetHistoryLiteDataListAsync()
        {
            // 获取数据库的原始记录数据
            List<HistoryModel> HistoryRawList = await HistoryXmlService.QueryAsync(HistoryLiteItem.HistoryLiteNumValue);

            lock (HistoryLiteDataListLock)
            {
                HistoryLiteDataList.Clear();
            }

            lock (HistoryLiteDataListLock)
            {
                HistoryRawList.ForEach(HistoryLiteDataList.Add);
            }
        }
    }
}
