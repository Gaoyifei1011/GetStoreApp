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
    public class RegionViewModel : ObservableRecipient
    {
        private readonly IRegionService _regionService;

        private string _region;

        public string Region
        {
            get { return _region; }

            set { SetProperty(ref _region, value); }
        }

        public List<RegionModel> RegionList { get; set; }

        public IAsyncRelayCommand RegionSelectCommand { get; set; }

        public RegionViewModel(IRegionService regionService)
        {
            _regionService = regionService;

            RegionList = _regionService.RegionList;
            Region = _regionService.AppRegion;

            RegionSelectCommand = new AsyncRelayCommand(async () =>
            {
                await _regionService.SetRegionAsync(Region);
                Messenger.Send(new RegionMessage(Region));
            });
        }
    }
}
