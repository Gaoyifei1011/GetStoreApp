using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Experiment;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Experiment
{
    public sealed partial class OpenConfigFIleControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public OpenConfigFileViewModel ViewModel { get; } = ContainerHelper.GetInstance<OpenConfigFileViewModel>();

        public OpenConfigFIleControl()
        {
            InitializeComponent();
        }
    }
}
