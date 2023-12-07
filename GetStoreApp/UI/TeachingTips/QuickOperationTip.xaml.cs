using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.TeachingTips
{
    /// <summary>
    /// 快捷操作应用内通知
    /// </summary>
    public sealed partial class QuickOperationTip : TeachingTip
    {
        public QuickOperationTip(QuickOperationKind quickOperationKind, bool isPinnedSuccessfully = false)
        {
            InitializeComponent();
            InitializeContent(quickOperationKind, isPinnedSuccessfully);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitializeContent(QuickOperationKind quickOperationKind, bool isPinnedSuccessfully)
        {
            QuickOperationSuccess.Visibility = isPinnedSuccessfully ? Visibility.Visible : Visibility.Collapsed;
            QuickOperationFailed.Visibility = isPinnedSuccessfully ? Visibility.Collapsed : Visibility.Visible;
            if (quickOperationKind is QuickOperationKind.Desktop && isPinnedSuccessfully)
            {
                QuickOperationSuccess.Text = ResourceService.GetLocalized("Notification/DesktopShortcutSuccessfully");
            }
            else if (quickOperationKind is QuickOperationKind.Desktop && !isPinnedSuccessfully)
            {
                QuickOperationFailed.Text = ResourceService.GetLocalized("Notification/DesktopShortFailed");
            }
            else if (quickOperationKind is QuickOperationKind.StartScreen && isPinnedSuccessfully)
            {
                QuickOperationSuccess.Text = ResourceService.GetLocalized("Notification/StartScreenSuccessfully");
            }
            else if (quickOperationKind is QuickOperationKind.StartScreen && !isPinnedSuccessfully)
            {
                QuickOperationFailed.Text = ResourceService.GetLocalized("Notification/StartScreenFailed");
            }
            else if (quickOperationKind is QuickOperationKind.Taskbar && isPinnedSuccessfully)
            {
                QuickOperationSuccess.Text = ResourceService.GetLocalized("Notification/TaskbarSuccessfully");
            }
            else if (quickOperationKind is QuickOperationKind.Taskbar && !isPinnedSuccessfully)
            {
                QuickOperationFailed.Text = ResourceService.GetLocalized("Notification/TaskbarFailed");
            }
        }
    }
}
