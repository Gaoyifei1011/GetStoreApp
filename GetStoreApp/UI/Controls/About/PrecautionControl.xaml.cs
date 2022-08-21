using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class PrecautionControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public PrecautionViewModel ViewModel { get; } = IOCHelper.GetService<PrecautionViewModel>();

        public PrecautionControl()
        {
            InitializeComponent();
        }
    }
}
