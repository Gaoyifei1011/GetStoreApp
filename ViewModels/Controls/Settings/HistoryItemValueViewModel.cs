using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class HistoryItemValueViewModel : ObservableRecipient
    {
        private int _selectedHistoryItemValue = HistoryItemValueService.HistoryItemValue;

        public int SelectedHistoryItemValue
        {
            get
            {
                return _selectedHistoryItemValue;
            }

            set
            {
                SetProperty(ref _selectedHistoryItemValue, value);
                HistoryItemValueService.SetHistoryItemValue(value);
                Messenger.Send(new HistoryItemValueMessage(value));
            }
        }

        public IReadOnlyList<HistoryItemValue> HistoryItemValueList { get; } = new List<HistoryItemValue>()
            {
                new HistoryItemValue(){ HistoryItemName=LanguageService.GetResources("/Settings/HistoryItemValueMin"),HistoryItemNum=3 },
                new HistoryItemValue(){ HistoryItemName=LanguageService.GetResources("/Settings/HistoryItemValueMax"),HistoryItemNum=5 }
            };
    }
}
