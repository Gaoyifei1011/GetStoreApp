using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class HistoryItemValueViewModel : ObservableRecipient
    {
        private IHistoryLiteNumService HistoryItemValueService { get; } = IOCHelper.GetService<IHistoryLiteNumService>();

        public List<HistoryLiteNumModel> HistoryItemValueList => HistoryItemValueService.HistoryLiteNumList;

        private HistoryLiteNumModel _historyItem;

        public HistoryLiteNumModel HistoryItem
        {
            get { return _historyItem; }

            set { SetProperty(ref _historyItem, value); }
        }

        public IAsyncRelayCommand HistoryItemSelectCommand => new AsyncRelayCommand(async () =>
        {
            await HistoryItemValueService.SetHistoryLiteNumAsync(HistoryItem);
            Messenger.Send(new HistoryItemValueMessage(HistoryItem));
        });

        public HistoryItemValueViewModel()
        {
            HistoryItem = HistoryItemValueService.HistoryLiteNum;
        }
    }
}
