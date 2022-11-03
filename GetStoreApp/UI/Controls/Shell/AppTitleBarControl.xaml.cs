using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Shell
{
    public sealed partial class AppTitleBarControl : Grid
    {
        public IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public AppTitleBarControl()
        {
            InitializeComponent();
        }
    }
}
