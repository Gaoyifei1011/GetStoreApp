using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
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
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<LanguageViewModel>();
            InitializeComponent();
        }

        public string GetSelectedLanguageName(LanguageModel language)
        {
            return language.DisplayName;
        }
    }
}
