using GetStoreApp.Contracts.Services.App;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class CloudflareValidationDialog : ContentDialog
    {
        public IResourceService ResourceService { get; }

        public CloudflareValidationDialog()
        {
            ResourceService = App.GetService<IResourceService>();
            this.InitializeComponent();
        }
    }
}
