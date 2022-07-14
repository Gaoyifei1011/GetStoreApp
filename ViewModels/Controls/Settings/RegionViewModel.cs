using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class RegionViewModel : ObservableRecipient
    {
        private readonly IRegionService RegionService;

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
            RegionService = regionService;

            RegionList = RegionService.RegionList;
            Region = RegionService.AppRegion;

            RegionSelectCommand = new AsyncRelayCommand(async () =>
            {
                await RegionService.SetRegionAsync(Region);
                Messenger.Send(new RegionMessage(Region));
            });
        }
    }
}
