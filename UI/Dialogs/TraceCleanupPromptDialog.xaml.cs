using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class TraceCleanupPromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; }

        public TraceCleanupPromptViewModel ViewModel { get; }

        public TraceCleanupPromptDialog()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<TraceCleanupPromptViewModel>();
            InitializeComponent();
        }
    }
}
