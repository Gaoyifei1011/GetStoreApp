using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Appearance;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    public sealed partial class TopMostControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public TopMostViewModel ViewModel { get; } = IOCHelper.GetService<TopMostViewModel>();

        public TopMostControl()
        {
            InitializeComponent();
        }
    }
}
