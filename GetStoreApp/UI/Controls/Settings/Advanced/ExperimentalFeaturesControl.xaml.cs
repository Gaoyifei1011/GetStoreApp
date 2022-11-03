using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Advanced;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    public sealed partial class ExperimentalFeaturesControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public ExperimentalFeaturesViewModel ViewModel { get; } = ContainerHelper.GetInstance<ExperimentalFeaturesViewModel>();

        public ExperimentalFeaturesControl()
        {
            InitializeComponent();
        }
    }
}
