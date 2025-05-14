using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Management.Core;
using Windows.Storage;
using Windows.System;

// 抑制 CS8305，IDE0060 警告
#pragma warning disable CS8305,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置 WinGet 程序包选项页面
    /// </summary>
    public sealed partial class SettingsWinGetPage : Page
    {
        public SettingsWinGetPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 配置 WinGet 数据源
        /// </summary>
        private void OnConfigurationClicked(object sender, RoutedEventArgs args)
        {
            if (WinGetConfigService.IsWinGetInstalled && MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                settingsPage.NavigateTo(settingsPage.PageList[1], null, true);
            }
        }

        /// <summary>
        /// 打开 WinGet 程序包设置
        /// </summary>
        private void OnOpenWinGetSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                if (ApplicationDataManager.CreateForPackageFamily("Microsoft.DesktopAppInstaller_8wekyb3d8bbwe") is ApplicationData applicationData)
                {
                    string wingetConfigFilePath = Path.Combine(applicationData.LocalFolder.Path, "settings.json");

                    if (File.Exists(wingetConfigFilePath))
                    {
                        await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(wingetConfigFilePath));
                    }
                    else
                    {
                        Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", "settings", null, WindowShowStyle.SW_HIDE);
                    }
                }
            });
        }
    }
}
