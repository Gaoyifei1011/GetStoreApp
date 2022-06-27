using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Messages;
using GetStoreApp.Services.History;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class ClearRecordViewModel : ObservableRecipient
    {
        public bool _clearState = false;

        public bool ClearState
        {
            get { return _clearState; }

            set { SetProperty(ref _clearState, value); }
        }

        public bool _clearTextVisValue = false;

        public bool ClearTextVisValue
        {
            get { return _clearTextVisValue; }

            set { SetProperty(ref _clearTextVisValue, value); }
        }

        public IAsyncRelayCommand ClearRecordCommand { get; set; }

        public ClearRecordViewModel()
        {
            ClearRecordCommand = new AsyncRelayCommand(ClearRecordAsync);
        }

        private async Task ClearRecordAsync()
        {
            ClearTextVisValue = false;
            bool result = await HistoryDataService.ClearHistoryDataAsync();

            if (result) ClearState = true;
            else ClearState = false;

            ClearTextVisValue = true;

            Messenger.Send(new HistoryMessage(true));
        }
    }
}
