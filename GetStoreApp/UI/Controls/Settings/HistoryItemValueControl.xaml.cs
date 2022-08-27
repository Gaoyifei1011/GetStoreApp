using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class HistoryItemValueControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public HistoryItemValueViewModel ViewModel { get; } = IOCHelper.GetService<HistoryItemValueViewModel>();

        public HistoryItemValueControl()
        {
            InitializeComponent();
        }

        public string GetSelectedHistoryItemName(HistoryLiteNumModel historyItem)
        {
            return historyItem.HistoryLiteNumName;
        }
    }
}
