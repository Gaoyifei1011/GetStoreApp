using GetStoreApp.Contracts.Services.App;
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
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<InstructionsViewModel>();
            this.InitializeComponent();
        }
    }
}
