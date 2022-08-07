using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class RegionViewModel : ObservableRecipient
    {
        private IRegionService RegionService { get; } = IOCHelper.GetService<IRegionService>();

        private RegionModel _region;

        public RegionModel Region
        {
            get { return _region; }

            set { SetProperty(ref _region, value); }
        }

        public List<RegionModel> RegionList { get; set; }

        public IAsyncRelayCommand RegionSelectCommand { get; set; }

        public RegionViewModel()
        {
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
