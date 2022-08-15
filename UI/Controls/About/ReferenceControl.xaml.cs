using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class ReferenceControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public ReferenceViewModel ViewModel { get; } = IOCHelper.GetService<ReferenceViewModel>();

        public string ReferenceDescription { get; }

        public ReferenceControl()
        {
            ReferenceDescription = ResourceService.GetLocalized("/About/ReferenceDescription");
            InitializeComponent();
        }
    }
}
