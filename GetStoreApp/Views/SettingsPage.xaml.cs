using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class SettingsPage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public SettingsViewModel ViewModel { get; } = IOCHelper.GetService<SettingsViewModel>();

        public SettingsPage()
        {
            InitializeComponent();
        }
    }
}
