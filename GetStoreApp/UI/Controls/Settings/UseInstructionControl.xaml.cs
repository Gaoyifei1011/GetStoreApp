using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class UseInstructionControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public UseInstructionViewModel ViewModel { get; } = IOCHelper.GetService<UseInstructionViewModel>();

        public UseInstructionControl()
        {
            InitializeComponent();
        }
    }
}
