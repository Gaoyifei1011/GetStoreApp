using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Download;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    public sealed partial class UnfinishedControl : UserControl
    {
        public UnfinishedViewModel ViewModel { get; } = IOCHelper.GetService<UnfinishedViewModel>();

        public UnfinishedControl()
        {
            InitializeComponent();
        }
    }
}
