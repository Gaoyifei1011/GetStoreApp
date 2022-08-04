using GetStoreApp.Contracts.Services.App;
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
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<UseInstructionViewModel>();
            InitializeComponent();
        }
    }
}
