using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Appearance;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    public sealed partial class LauguageControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public LanguageViewModel ViewModel { get; } = ContainerHelper.GetInstance<LanguageViewModel>();

        public LauguageControl()
        {
            InitializeComponent();
        }
    }
}
