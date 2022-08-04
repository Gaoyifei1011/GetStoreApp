using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class TitleControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public TitleViewModel ViewModel { get; }

        public TitleControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<TitleViewModel>();
            InitializeComponent();
        }
    }
}
