using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.Settings.Common;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public class HistoryLiteConfigViewModel : ObservableRecipient
    {
        private IHistoryLiteNumService HistoryLiteNumService { get; } = IOCHelper.GetService<IHistoryLiteNumService>();

        public List<HistoryLiteNumModel> HistoryLiteNumList => HistoryLiteNumService.HistoryLiteNumList;

        private HistoryLiteNumModel _historyLiteItem;

        public HistoryLiteNumModel HistoryLiteItem
        {
            get { return _historyLiteItem; }

            set { SetProperty(ref _historyLiteItem, value); }
        }

        // 主页面“历史记录”显示数目修改
        public IRelayCommand HistoryLiteItemSelectCommand => new RelayCommand(async () =>
        {
            await HistoryLiteNumService.SetHistoryLiteNumAsync(HistoryLiteItem);
            WeakReferenceMessenger.Default.Send(new HistoryLiteNumMessage(HistoryLiteItem));
        });

        public HistoryLiteConfigViewModel()
        {
            HistoryLiteItem = HistoryLiteNumService.HistoryLiteNum;
        }
    }
}
