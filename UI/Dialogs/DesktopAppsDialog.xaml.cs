using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class DesktopAppsDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public DesktopAppsDialog()
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            InitializeComponent();
        }
    }
}
