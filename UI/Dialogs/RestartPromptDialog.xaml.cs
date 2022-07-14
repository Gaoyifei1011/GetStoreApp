using GetStoreApp.Contracts.Services.App;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class RestartPromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; }

        public RestartPromptDialog()
        {
            ResourceService = App.GetService<IResourceService>();
            this.InitializeComponent();
        }
    }
}
