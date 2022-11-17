using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class SettingsHelpControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public SettingsHelpViewModel ViewModel { get; } = ContainerHelper.GetInstance<SettingsHelpViewModel>();

        public SettingsHelpControl()
        {
            InitializeComponent();
        }
    }
}
