using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class HistoryLiteConfigControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public HistoryLiteConfigViewModel ViewModel { get; } = IOCHelper.GetService<HistoryLiteConfigViewModel>();

        public HistoryLiteConfigControl()
        {
            InitializeComponent();
        }

        public string GetSelectedHistoryLiteNumName(HistoryLiteNumModel historyLiteItem)
        {
            return historyLiteItem.HistoryLiteNumName;
        }
    }
}
