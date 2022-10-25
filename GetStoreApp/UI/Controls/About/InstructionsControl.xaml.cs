using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class InstructionsControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public InstructionsViewModel ViewModel { get; } = IOCHelper.GetService<InstructionsViewModel>();

        public InstructionsControl()
        {
            InitializeComponent();
        }
    }
}
