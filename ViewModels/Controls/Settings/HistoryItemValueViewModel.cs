using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class HistoryItemValueViewModel : ObservableRecipient
    {
        private int _selectedHistoryItemValue = HistoryItemValueService.HistoryItemValue;

        public int SelectedHistoryItemValue
        {
            get { return _selectedHistoryItemValue; }

            set { SetProperty(ref _selectedHistoryItemValue, value); }
        }

        public IAsyncRelayCommand HistoryItemSelectCommand { get; set; }

        public IReadOnlyList<HistoryItemSetModel> HistoryItemValueList { get; } = new List<HistoryItemSetModel>
            {
                new HistoryItemSetModel{ HistoryItemName=LanguageService.GetResources("/Settings/HistoryItemValueMin"),HistoryItemNum=3 },
                new HistoryItemSetModel{ HistoryItemName=LanguageService.GetResources("/Settings/HistoryItemValueMax"),HistoryItemNum=5 }
            };

        public HistoryItemValueViewModel()
        {
            HistoryItemSelectCommand = new AsyncRelayCommand(HistoryItemSelectAsync);
        }

        /// <summary>
        /// 设置主页面历史记录显示条目的最大数量
        /// </summary>
        private async Task HistoryItemSelectAsync()
        {
            HistoryItemValueService.SetHistoryItemValue(SelectedHistoryItemValue);
            Messenger.Send(new HistoryItemValueMessage(SelectedHistoryItemValue));
            await Task.CompletedTask;
        }
    }
}
