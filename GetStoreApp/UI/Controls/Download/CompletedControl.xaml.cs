using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Download;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    public sealed partial class CompletedControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public CompletedViewModel ViewModel { get; } = IOCHelper.GetService<CompletedViewModel>();

        public string FileNotExists => ResourceService.GetLocalized("/Download/FileNotExists");

        public string InstallToolTip => ResourceService.GetLocalized("/Download/InstallToolTip");

        public string OpenItemFolderToolTip => ResourceService.GetLocalized("/Download/OpenItemFolderToolTip");

        public string DeleteToolTip => ResourceService.GetLocalized("Download/DeleteToolTip");

        public CompletedControl()
        {
            InitializeComponent();
        }

        public string LocalizedCompletedCountInfo(int count)
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
