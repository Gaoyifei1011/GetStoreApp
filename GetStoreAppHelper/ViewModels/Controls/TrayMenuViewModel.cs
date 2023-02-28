using GetStoreAppHelper.Contracts.Command;
using GetStoreAppHelper.Extensions.Command;
using System.Diagnostics;

namespace GetStoreAppHelper.ViewModels.Controls
{
    public class TrayMenuViewModel
    {
        public IRelayCommand ShowOrHideWindowCommand => new RelayCommand(() =>
        {
            // 隐藏窗口
            //if (WindowHelper.IsWindowVisible)
            //{
            //    WindowHelper.HideAppWindow();
            //}
            //// 显示窗口
            //else
            //{
            //    WindowHelper.ShowAppWindow();
            //}
            Debug.WriteLine("ShowOrHideWindowCommand");
        });

        // 打开设置
        public IRelayCommand SettingsCommand => new RelayCommand(() =>
        {
            // 窗口置前端
            //WindowHelper.ShowAppWindow();

            //if (NavigationService.GetCurrentPageType() != typeof(SettingsPage))
            //{
            //    NavigationService.NavigateTo(typeof(SettingsPage));
            //}
            Debug.WriteLine("SettingsCommand");
        });

        // 退出应用
        public IRelayCommand ExitCommand => new RelayCommand(() =>
        {
            Debug.WriteLine("ExitCommand");
        });
    }
}
