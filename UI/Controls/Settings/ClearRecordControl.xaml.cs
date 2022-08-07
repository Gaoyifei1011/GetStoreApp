using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class ClearRecordControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public ClearRecordViewModel ViewModel { get; }

        public ClearRecordControl()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<ClearRecordViewModel>();
            InitializeComponent();
        }

        public string LocalizedClearState(bool clearState)
        {
            if (clearState) return ResourceService.GetLocalized("/Settings/ClearSuccessfully");
            else return ResourceService.GetLocalized("/Settings/ClearFailed");
        }
    }
}
