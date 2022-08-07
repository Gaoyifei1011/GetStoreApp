using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class RestartPromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; }

        public RestartPromptDialog()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            InitializeComponent();
        }
    }
}
