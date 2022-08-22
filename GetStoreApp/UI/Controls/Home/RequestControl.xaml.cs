using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class RequestControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public RequestViewModel ViewModel { get; } = IOCHelper.GetService<RequestViewModel>();

        public List<GetAppTypeModel> TypeList => ResourceService.TypeList;

        public List<GetAppChannelModel> ChannelList => ResourceService.ChannelList;

        public RequestControl()
        {
            InitializeComponent();
        }

        public string GetSelectedTypeName(int index)
        {
            return TypeList[index].DisplayName;
        }

        public string GetSelectedChannelName(int index)
        {
            return ChannelList[index].DisplayName;
        }
    }
}
