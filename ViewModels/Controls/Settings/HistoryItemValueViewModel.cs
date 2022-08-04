using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class HistoryItemValueViewModel : ObservableRecipient
    {
        private IHistoryItemValueService HistoryItemValueService { get; } = App.GetService<IHistoryItemValueService>();

        private HistoryItemValueModel _historyItem;

        public HistoryItemValueModel HistoryItem
        {
            get { return _historyItem; }

            set { SetProperty(ref _historyItem, value); }
        }

        public List<HistoryItemValueModel> HistoryItemValueList { get; set; }

        public IAsyncRelayCommand HistoryItemSelectCommand { get; set; }

        public HistoryItemValueViewModel()
        {
            HistoryItemValueList = HistoryItemValueService.HistoryItemValueList;
            HistoryItem = HistoryItemValueService.HistoryItem;

            HistoryItemSelectCommand = new AsyncRelayCommand(async () =>
            {
                await HistoryItemValueService.SetHistoryItemValueAsync(HistoryItem);
                Messenger.Send(new HistoryItemValueMessage(HistoryItem));
            });
        }
    }
}
