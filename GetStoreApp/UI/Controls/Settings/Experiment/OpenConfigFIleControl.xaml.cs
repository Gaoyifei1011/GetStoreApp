using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings.Experiment;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Experiment
{
    public sealed partial class OpenConfigFIleControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public OpenConfigFileViewModel ViewModel { get; } = IOCHelper.GetService<OpenConfigFileViewModel>();

        public OpenConfigFIleControl()
        {
            InitializeComponent();
        }
    }
}
