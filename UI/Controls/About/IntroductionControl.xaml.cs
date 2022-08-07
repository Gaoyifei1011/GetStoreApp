using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class IntroductionControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public IntroductionControl()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            InitializeComponent();
        }
    }
}
