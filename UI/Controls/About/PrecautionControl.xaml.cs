using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class PrecautionControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public PrecautionViewModel ViewModel { get; }

        public PrecautionControl()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<PrecautionViewModel>();
            InitializeComponent();
        }
    }
}
