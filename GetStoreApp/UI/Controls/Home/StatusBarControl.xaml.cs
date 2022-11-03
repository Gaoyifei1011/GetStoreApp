using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class StatusBarControl : UserControl
    {
        public StatusBarViewModel ViewModel { get; } = ContainerHelper.GetInstance<StatusBarViewModel>();

        public StatusBarControl()
        {
            InitializeComponent();
        }
    }
}
