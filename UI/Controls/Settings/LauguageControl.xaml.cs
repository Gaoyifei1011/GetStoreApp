using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class LauguageControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public LanguageViewModel ViewModel { get; }

        public LauguageControl()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<LanguageViewModel>();
            this.InitializeComponent();
        }
    }
}
