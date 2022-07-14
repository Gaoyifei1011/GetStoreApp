using GetStoreApp.Contracts.Services.App;
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
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<ClearRecordViewModel>();
            this.InitializeComponent();
        }

        public string LocalizedClearState(bool clearState)
        {
            if (clearState) return ResourceService.GetLocalized("/Settings/ClearSuccessfully");
            else return ResourceService.GetLocalized("/Settings/ClearFailed");
        }
    }
}
