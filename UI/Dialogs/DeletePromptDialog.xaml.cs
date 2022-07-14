using GetStoreApp.Contracts.Services.App;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class DeletePromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; }

        public DeletePromptDialog()
        {
            ResourceService = App.GetService<IResourceService>();
            this.InitializeComponent();
        }
    }
}
