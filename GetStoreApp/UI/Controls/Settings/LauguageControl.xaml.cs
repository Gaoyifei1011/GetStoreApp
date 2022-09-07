using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class LauguageControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public LanguageViewModel ViewModel { get; } = IOCHelper.GetService<LanguageViewModel>();

        public LauguageControl()
        {
            InitializeComponent();
        }

        public string GetSelectedLanguageName(LanguageModel language)
        {
            return language.DisplayName;
        }
    }
}
