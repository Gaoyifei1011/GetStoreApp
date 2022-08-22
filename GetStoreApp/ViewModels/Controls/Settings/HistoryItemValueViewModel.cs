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
        private IHistoryItemValueService HistoryItemValueService { get; } = IOCHelper.GetService<IHistoryItemValueService>();

        public List<HistoryItemValueModel> HistoryItemValueList => HistoryItemValueService.HistoryItemValueList;

        private HistoryItemValueModel _historyItem;

        public HistoryItemValueModel HistoryItem
        {
            get { return _historyItem; }

            set { SetProperty(ref _historyItem, value); }
        }

        public IAsyncRelayCommand HistoryItemSelectCommand => new AsyncRelayCommand(async () =>
        {
            await HistoryItemValueService.SetHistoryItemValueAsync(HistoryItem);
            Messenger.Send(new HistoryItemValueMessage(HistoryItem));
        });

        public HistoryItemValueViewModel()
        {
            HistoryItem = HistoryItemValueService.HistoryItem;
        }
    }
}
