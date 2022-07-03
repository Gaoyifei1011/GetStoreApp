using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class HistoryItemControl : UserControl
    {
        public HistoryItemViewModel ViewModel { get; }

        public HistoryItemControl()
        {
            ViewModel = App.GetService<HistoryItemViewModel>();
            this.InitializeComponent();
        }
    }
}
