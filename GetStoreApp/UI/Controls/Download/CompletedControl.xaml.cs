using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    /// <summary>
    /// 下载页面：已下载完成用户控件视图
    /// </summary>
    public sealed partial class CompletedControl : UserControl
    {
        public string Installing { get; } = ResourceService.GetLocalized("Download/Installing");

        public string InstallError { get; } = ResourceService.GetLocalized("Download/InstallError");

        public string InstallToolTip { get; } = ResourceService.GetLocalized("Download/InstallToolTip");

        public string OpenItemFolderToolTip { get; } = ResourceService.GetLocalized("Download/OpenItemFolderToolTip");

        public string ViewMore { get; } = ResourceService.GetLocalized("Download/ViewMore");

        public string Delete { get; } = ResourceService.GetLocalized("Download/Delete");

        public string DeleteWithFile { get; } = ResourceService.GetLocalized("Download/DeleteWithFile");

        public string DeleteToolTip { get; } = ResourceService.GetLocalized("Download/DeleteToolTip");

        public string DeleteWithFileToolTip { get; } = ResourceService.GetLocalized("Download/DeleteWithFileToolTip");

        public string ShareFile { get; } = ResourceService.GetLocalized("Download/ShareFile");

        public string FileInformation { get; } = ResourceService.GetLocalized("Download/FileInformation");

        public CompletedControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化已下载完成数量统计信息
        /// </summary>
        public string LocalizeCompletedCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("Download/CompletedEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("Download/CompletedCountInfo"), count);
            }
        }
    }
}
