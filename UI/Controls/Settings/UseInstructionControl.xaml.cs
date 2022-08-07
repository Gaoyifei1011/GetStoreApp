using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class UseInstructionControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public UseInstructionViewModel ViewModel { get; }

        public UseInstructionControl()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<UseInstructionViewModel>();
            InitializeComponent();
        }
    }
}
