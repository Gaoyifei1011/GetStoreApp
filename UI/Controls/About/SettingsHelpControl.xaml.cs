using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class SettingsHelpControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public SettingsHelpViewModel ViewModel { get; }

        public SettingsHelpControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<SettingsHelpViewModel>();
            InitializeComponent();
        }
    }
}
