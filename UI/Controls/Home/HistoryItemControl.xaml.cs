using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class HistoryItemControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public HistoryItemViewModel ViewModel { get; }

        public HistoryItemControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<HistoryItemViewModel>();
            this.InitializeComponent();
        }
    }
}
