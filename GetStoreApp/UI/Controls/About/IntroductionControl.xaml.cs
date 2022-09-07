using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class IntroductionControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IntroductionControl()
        {
            InitializeComponent();
        }
    }
}
