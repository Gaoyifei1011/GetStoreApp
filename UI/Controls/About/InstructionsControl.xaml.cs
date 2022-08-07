using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class InstructionsControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public InstructionsViewModel ViewModel { get; }

        public InstructionsControl()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<InstructionsViewModel>();
            InitializeComponent();
        }
    }
}
