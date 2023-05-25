using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;

namespace GetStoreApp.UI.Dialogs.Settings
{
    /// <summary>
    /// 痕迹清理对话框视图
    /// </summary>
    public sealed partial class TraceCleanupPromptDialog : ExtendedContentDialog
    {
        public string CleanFailed { get; } = ResourceService.GetLocalized("Dialog/CleanFailed");

        public TraceCleanupPromptDialog()
        {
            InitializeComponent();
            ViewModel.InitializeTraceCleanupList();
        }

        public bool IsButtonEnabled(bool isSelected, bool isCleaning)
        {
            if (isCleaning)
            {
                return false;
            }
            else
            {
                if (isSelected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
