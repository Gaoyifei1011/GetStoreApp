using GetStoreApp.Contracts.Services.App;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class DownloadOptionsControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public DownloadOptionsControl()
        {
            ResourceService = App.GetService<IResourceService>();
            this.InitializeComponent();
        }
    }
}
