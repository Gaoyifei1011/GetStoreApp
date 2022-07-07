using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class InstructionsControl : UserControl
    {
        public InstructionsViewModel ViewModel { get; }

        public InstructionsControl()
        {
            ViewModel = App.GetService<InstructionsViewModel>();
            this.InitializeComponent();
        }
    }
}
