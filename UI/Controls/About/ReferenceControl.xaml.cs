using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class ReferenceControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public ReferenceViewModel ViewModel { get; }

        public ReferenceControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<ReferenceViewModel>();
            this.InitializeComponent();
        }
    }
}
