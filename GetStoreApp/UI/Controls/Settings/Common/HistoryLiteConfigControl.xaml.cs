using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Common;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    public sealed partial class HistoryLiteConfigControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public HistoryLiteConfigViewModel ViewModel { get; } = ContainerHelper.GetInstance<HistoryLiteConfigViewModel>();

        public HistoryLiteConfigControl()
        {
            InitializeComponent();
        }
    }
}
