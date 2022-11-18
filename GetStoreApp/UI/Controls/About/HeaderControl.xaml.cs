using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class HeaderControl : UserControl
    {
        public HeaderControl()
        {
            InitializeComponent();
        }

        private string LocalizeAppVersion(string appVersion)
        {
            return string.Format(ResourceService.GetLocalized("/About/AppVersion"), appVersion);
        }
    }
}
