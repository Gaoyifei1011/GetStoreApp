using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class RequestControl : UserControl
    {
        public List<TypeModel> TypeList => ResourceService.TypeList;

        public List<ChannelModel> ChannelList => ResourceService.ChannelList;

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
