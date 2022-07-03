using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class HeaderControl : UserControl
    {
        public HeaderViewModel ViewModel { get; }

        public HeaderControl()
        {
            ViewModel = App.GetService<HeaderViewModel>();
            this.InitializeComponent();
        }
    }
}
