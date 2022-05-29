using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using GetStoreApp.ViewModels.Pages;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class RegionViewModel : ObservableRecipient
    {
        // 区域设置
        private string _selectedRegion = RegionService.RegionCodeName;

        public string SelectedRegion
        {
            get { return _selectedRegion; }

            set
            {
                Messenger.Send(new RegionMessage(value));
                RegionService.SetRegion(value);
            }
        }

        // 区域列表
        public List<GeographicalLocation> RegionList = SettingsViewModel.RegionList;
    }
}