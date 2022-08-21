using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class TitleControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public TitleViewModel ViewModel { get; } = IOCHelper.GetService<TitleViewModel>();

        public TitleControl()
        {
            InitializeComponent();
        }
    }
}
