using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.UI.Dialogs.About;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：注意事项用户控件视图模型
    /// </summary>
    public sealed class PrecautionViewModel
    {
        // 区分传统桌面应用
        public IRelayCommand RecognizeCommand => new RelayCommand(async () =>
        {
            await new DesktopAppsDialog().ShowAsync();
        });
    }
}
