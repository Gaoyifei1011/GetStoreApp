using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class ThemeControl : UserControl
    {
        public ThemeViewModel ViewModel { get; }

        public ThemeControl()
        {
            ViewModel = App.GetService<ThemeViewModel>();
            InitializeComponent();
        }
    }
}