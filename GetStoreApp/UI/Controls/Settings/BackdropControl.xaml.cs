using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
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
