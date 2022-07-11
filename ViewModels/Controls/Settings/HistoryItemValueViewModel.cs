using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class HistoryItemValueViewModel : ObservableRecipient
    {
        private readonly IHistoryItemValueService _historyItemValueService;

        private string _historyItemValue;

        public string HistoryItemValue
        {
            get { return _historyItemValue; }

            set { SetProperty(ref _historyItemValue, value); }
        }

        public List<HistoryItemValueModel> HistoryItemValueList { get; set; }

        public IAsyncRelayCommand HistoryItemSelectCommand { get; set; }

        public HistoryItemValueViewModel(IHistoryItemValueService historyItemValueService)
        {
            _historyItemValueService = historyItemValueService;

            HistoryItemValueList = _historyItemValueService.HistoryItemValueList;
            HistoryItemValue = _historyItemValueService.HistoryItemValue;

            HistoryItemSelectCommand = new AsyncRelayCommand(async () =>
            {
                await _historyItemValueService.SetHistoryItemValueAsync(HistoryItemValue);
                Messenger.Send(new HistoryItemValueMessage(HistoryItemValue));
            });
        }
    }
}
