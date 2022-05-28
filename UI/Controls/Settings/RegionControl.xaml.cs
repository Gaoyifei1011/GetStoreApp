using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class RegionControl : UserControl
    {
        public RegionViewModel ViewModel { get; }

        public RegionControl()
        {
            ViewModel = App.GetService<RegionViewModel>();
            this.InitializeComponent();
        }
    }
}