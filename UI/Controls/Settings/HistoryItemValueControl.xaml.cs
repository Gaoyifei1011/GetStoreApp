using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Models;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class HistoryItemValueControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public HistoryItemValueViewModel ViewModel { get; }

        public HistoryItemValueControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<HistoryItemValueViewModel>();
            InitializeComponent();
        }

        public string GetSelectedHistoryItemName(HistoryItemValueModel historyItem)
        {
            return historyItem.HistoryItemName;
        }
    }
}
