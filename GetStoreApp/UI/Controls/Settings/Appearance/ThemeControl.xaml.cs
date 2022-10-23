using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings.Appearance;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    public sealed partial class ThemeControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public ThemeViewModel ViewModel { get; } = IOCHelper.GetService<ThemeViewModel>();

        public ThemeControl()
        {
            InitializeComponent();
        }
    }
}
