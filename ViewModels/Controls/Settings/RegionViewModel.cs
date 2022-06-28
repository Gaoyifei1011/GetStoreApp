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
    public class RegionViewModel : ObservableRecipient
    {
        private string _selectedRegion = RegionService.RegionCodeName;

        public string SelectedRegion
        {
            get { return _selectedRegion; }

            set { SetProperty(ref _selectedRegion,value); }
        }

        public IAsyncRelayCommand RegionSelectCommand { get; set; }

        public IReadOnlyList<GeographicalLocationModel> RegionList { get; } = RegionService.AppGlobalLocations;

        public RegionViewModel()
        {
            RegionSelectCommand = new AsyncRelayCommand(RegionSelectAsync);
        }

        public async Task RegionSelectAsync()
        {
            RegionService.SetRegion(SelectedRegion);
            Messenger.Send(new RegionMessage(SelectedRegion));
            await Task.CompletedTask;
        }
    }
}
