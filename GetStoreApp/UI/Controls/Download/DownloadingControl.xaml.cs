using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    /// <summary>
    /// 下载页面：正在下载完成用户控件视图
    /// </summary>
    public sealed partial class DownloadingControl : UserControl
    {
        public string WaitDownload { get; } = ResourceService.GetLocalized("Download/WaitDownload");

        public string PauseToolTip { get; } = ResourceService.GetLocalized("Download/PauseToolTip");

        public string DeleteToolTip { get; } = ResourceService.GetLocalized("Download/DeleteToolTip");

        public DownloadingControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 本地化正在下载完成数量统计信息
        /// </summary>
        public string LocalizeDownloadingCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("Download/DownloadingEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("Download/DownloadingCountInfo"), count);
            }
        }
    }
}
