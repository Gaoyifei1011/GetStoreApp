using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class SelectEmptyPromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; }

        public SelectEmptyPromptDialog()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            InitializeComponent();
        }
    }
}
