using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Models;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class RequestControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public RequestViewModel ViewModel { get; }

        public List<GetAppTypeModel> TypeList { get; set; }

        public List<GetAppChannelModel> ChannelList { get; set; }

        public RequestControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<RequestViewModel>();

            TypeList = ResourceService.TypeList;
            ChannelList = ResourceService.ChannelList;

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
