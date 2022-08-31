using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class LanguageChangeNotification : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public LanguageChangeNotification()
        {
            this.InitializeComponent();
        }
    }
}
