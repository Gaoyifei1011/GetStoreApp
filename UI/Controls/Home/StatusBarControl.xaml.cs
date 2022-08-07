using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class StatusBarControl : UserControl
    {
        public StatusBarViewModel ViewModel { get; }

        public StatusBarControl()
        {
            ViewModel = IOCHelper.GetService<StatusBarViewModel>();
            InitializeComponent();
        }
    }
}
