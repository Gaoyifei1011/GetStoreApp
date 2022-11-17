using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Appearance;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    public sealed partial class ThemeControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public ThemeViewModel ViewModel { get; } = ContainerHelper.GetInstance<ThemeViewModel>();

        public ThemeControl()
        {
            InitializeComponent();
        }
    }
}
