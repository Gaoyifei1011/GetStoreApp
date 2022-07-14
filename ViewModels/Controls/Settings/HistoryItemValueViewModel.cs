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
        private readonly IHistoryItemValueService HistoryItemValueService;

        private int _historyItemValue;

        public int HistoryItemValue
        {
            get { return _historyItemValue; }

            set { SetProperty(ref _historyItemValue, value); }
        }

        public List<HistoryItemValueModel> HistoryItemValueList { get; set; }

        public IAsyncRelayCommand HistoryItemSelectCommand { get; set; }

        public HistoryItemValueViewModel(IHistoryItemValueService historyItemValueService)
        {
            HistoryItemValueService = historyItemValueService;

            HistoryItemValueList = HistoryItemValueService.HistoryItemValueList;
            HistoryItemValue = HistoryItemValueService.HistoryItemValue;

            HistoryItemSelectCommand = new AsyncRelayCommand(async () =>
            {
                await HistoryItemValueService.SetHistoryItemValueAsync(HistoryItemValue);
                Messenger.Send(new HistoryItemValueMessage(HistoryItemValue));
            });
        }
    }
}
