using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    public sealed partial class CompletedControl : UserControl
    {
        public string Installing => ResourceService.GetLocalized("/Download/Installing");

        public string InstallError => ResourceService.GetLocalized("/Download/InstallError");

        public string InstallToolTip => ResourceService.GetLocalized("/Download/InstallToolTip");

        public string OpenItemFolderToolTip => ResourceService.GetLocalized("/Download/OpenItemFolderToolTip");

        public string ViewMore => ResourceService.GetLocalized("/Download/ViewMore");

        public string Delete => ResourceService.GetLocalized("/Download/Delete");

        public string DeleteWithFile => ResourceService.GetLocalized("/Download/DeleteWithFile");

        public string DeleteToolTip => ResourceService.GetLocalized("/Download/DeleteToolTip");

        public string DeleteWithFileToolTip => ResourceService.GetLocalized("/Download/DeleteWithFileToolTip");

        public string FileInformation => ResourceService.GetLocalized("/Download/FileInformation");

        public CompletedControl()
        {
            InitializeComponent();
        }

        public string LocalizeCompletedCountInfo(int count)
        {
            if (count == 0)
            {
                return ResourceService.GetLocalized("/Download/CompletedEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("/Download/CompletedCountInfo"), count);
            }
        }
    }
}
