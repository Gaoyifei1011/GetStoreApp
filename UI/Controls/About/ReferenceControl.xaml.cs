using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

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
