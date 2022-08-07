using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class SettingsPage : Page
    {
        public IResourceService ResourceService { get; }

        public SettingsViewModel ViewModel { get; }

        public SettingsPage()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<SettingsViewModel>();
            InitializeComponent();
        }
    }
}
