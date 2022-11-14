using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Window;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Window
{
    public sealed partial class AppTitleBarControl : Grid
    {
        public IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public AppTitleBarViewModel ViewModel { get; } = ContainerHelper.GetInstance<AppTitleBarViewModel>();

        public AppTitleBarControl()
        {
            InitializeComponent();
        }
    }
}
