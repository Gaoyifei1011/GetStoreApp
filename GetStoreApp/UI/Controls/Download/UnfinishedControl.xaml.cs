using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Download;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    public sealed partial class UnfinishedControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public UnfinishedViewModel ViewModel { get; } = ContainerHelper.GetInstance<UnfinishedViewModel>();

        public string PauseDownload => ResourceService.GetLocalized("/Download/PauseDownload");

        public string InvalidLink => ResourceService.GetLocalized("/Download/InvalidLink");

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
