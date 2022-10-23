using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings.Advanced;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    public sealed partial class ExperimentalFeaturesControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public ExperimentalFeaturesViewModel ViewModel { get; } = IOCHelper.GetService<ExperimentalFeaturesViewModel>();

        public ExperimentalFeaturesControl()
        {
            InitializeComponent();
        }
    }
}
