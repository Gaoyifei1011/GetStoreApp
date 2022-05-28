using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using GetStoreApp.ViewModels.Pages;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class HistoryItemValueViewModel : ObservableRecipient
    {
        // 主页“历史记录”展示条目设置
        private int _selectedHistoryItemValue;

        public int SelectedHistoryItemValue
        {
            get
            {
                _selectedHistoryItemValue = HistoryItemValueService.HistoryItemValue;
                return _selectedHistoryItemValue;
            }

            set
            {
                SetProperty(ref _selectedHistoryItemValue, value);
                HistoryItemValueService.SetHistoryItemValue(value);
                Messenger.Send(new HistoryItemValueMessage(value));
            }
        }

        public List<HistoryItemValueModel> HistoryItemValueList = SettingsViewModel.HistoryItemValueList;
    }
}