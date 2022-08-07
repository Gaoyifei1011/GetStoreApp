using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class ReferenceControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public ReferenceViewModel ViewModel { get; }

        public string ReferenceDescription { get; }

        public ReferenceControl()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<ReferenceViewModel>();

            ReferenceDescription = ResourceService.GetLocalized("/About/ReferenceDescription");
            InitializeComponent();
        }
    }
}
