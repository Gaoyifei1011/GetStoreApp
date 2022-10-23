using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings.Common;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    public sealed partial class HistoryLiteConfigControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public HistoryLiteConfigViewModel ViewModel { get; } = IOCHelper.GetService<HistoryLiteConfigViewModel>();

        public HistoryLiteConfigControl()
        {
            InitializeComponent();
        }
    }
}
