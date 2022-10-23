using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings.Appearance;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Shell
{
    public sealed partial class AppTitleBar : Grid
    {
        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public AppTitleBar()
        {
            InitializeComponent();
        }
    }
}
