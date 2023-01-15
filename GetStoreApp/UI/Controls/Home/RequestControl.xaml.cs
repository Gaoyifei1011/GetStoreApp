using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.UI.Controls.Home
{
    /// <summary>
    /// 主页面：请求用户控件视图
    /// </summary>
    public sealed partial class RequestControl : UserControl
    {
        public List<TypeModel> TypeList { get; } = ResourceService.TypeList;

        public List<ChannelModel> ChannelList { get; } = ResourceService.ChannelList;

        public RequestControl()
        {
            InitializeComponent();
        }

        public bool IsTypeItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        public bool IsChannelItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }
    }
}
