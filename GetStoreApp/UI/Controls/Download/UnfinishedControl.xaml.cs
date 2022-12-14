using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    public sealed partial class UnfinishedControl : UserControl
    {
        public string PauseDownload => ResourceService.GetLocalized("/Download/PauseDownload");

        public string DownloadFailed => ResourceService.GetLocalized("/Download/DownloadFailed");

        public string ContinueToolTip => ResourceService.GetLocalized("/Download/ContinueToolTip");

        public string DeleteToolTip => ResourceService.GetLocalized("/Download/DeleteToolTip");

        public UnfinishedControl()
        {
            InitializeComponent();
        }

        public string LocalizeUnfinishedCountInfo(int count)
        {
            if (count == 0)
            {
                return ResourceService.GetLocalized("/Download/UnfinishedEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("/Download/UnfinishedCountInfo"), count);
            }
        }
    }
}
