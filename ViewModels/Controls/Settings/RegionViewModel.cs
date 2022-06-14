using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class RegionViewModel : ObservableRecipient
    {
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

        public IReadOnlyList<GeographicalLocationModel> RegionList { get; } = RegionService.AppGlobalLocations;
    }
}
