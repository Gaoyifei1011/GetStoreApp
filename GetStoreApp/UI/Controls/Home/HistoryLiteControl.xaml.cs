using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class HistoryLiteControl : UserControl
    {
        public string Fillin => ResourceService.GetLocalized("/Home/Fillin");

        public string FillinToolTip => ResourceService.GetLocalized("/Home/FillinToolTip");

        public string Copy => ResourceService.GetLocalized("/Home/Copy");

        public string CopyToolTip => ResourceService.GetLocalized("/Home/CopyToolTip");

        public HistoryLiteControl()
        {
            InitializeComponent();
        }
    }
}
