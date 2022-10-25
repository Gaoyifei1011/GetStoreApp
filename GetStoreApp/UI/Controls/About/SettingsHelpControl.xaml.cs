using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class SettingsHelpControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public SettingsHelpViewModel ViewModel { get; } = IOCHelper.GetService<SettingsHelpViewModel>();

        public SettingsHelpControl()
        {
            InitializeComponent();
        }
    }
}
