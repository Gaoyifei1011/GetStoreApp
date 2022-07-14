using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class PrecautionControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public PrecautionViewModel ViewModel { get; }

        public PrecautionControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<PrecautionViewModel>();
            this.InitializeComponent();
        }
    }
}
