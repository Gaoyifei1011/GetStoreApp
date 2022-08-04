using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Models;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class BackdropControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public BackdropViewModel ViewModel { get; }

        public BackdropControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<BackdropViewModel>();
            InitializeComponent();
        }

        public string GetSelectedBackdropName(BackdropModel backdrop)
        {
            return backdrop.DisplayName;
        }
    }
}
