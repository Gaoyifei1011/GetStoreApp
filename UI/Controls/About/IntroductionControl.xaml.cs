using GetStoreApp.Contracts.Services.App;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class IntroductionControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public IntroductionControl()
        {
            ResourceService = App.GetService<IResourceService>();
            InitializeComponent();
        }
    }
}
