using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class RequestControl : UserControl
    {
        public RequestViewModel ViewModel { get; }

        public RequestControl()
        {
            ViewModel = App.GetService<RequestViewModel>();
            this.InitializeComponent();
        }
    }
}