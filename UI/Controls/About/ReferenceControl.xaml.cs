using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class ReferenceControl : UserControl
    {
        public ReferenceViewModel ViewModel { get; }

        public ReferenceControl()
        {
            ViewModel = App.GetService<ReferenceViewModel>();
            this.InitializeComponent();
        }
    }
}
