using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Appearance;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    public sealed partial class BackdropControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public BackdropViewModel ViewModel { get; } = IOCHelper.GetService<BackdropViewModel>();

        public BackdropControl()
        {
            InitializeComponent();
        }
    }
}
