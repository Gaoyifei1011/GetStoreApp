using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置 WinGet 程序包选项页面
    /// </summary>
    public sealed partial class SettingsWinGetPage : Page, INotifyPropertyChanged
    {
        private readonly string AppInstallerString = ResourceService.GetLocalized("SettingsWinGet/AppInstaller");
        private readonly string BuiltInAppString = ResourceService.GetLocalized("SettingsWinGet/BuiltInApp");

        private KeyValuePair<string, string> _currentWinGetSource;

        public KeyValuePair<string, string> CurrentWinGetSource
        {
            get { return _currentWinGetSource; }

            set
            {
                if (!Equals(_currentWinGetSource, value))
                {
                    _currentWinGetSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentWinGetSource)));
                }
            }
        }

        private KeyValuePair<string, string> _winGetSource;

        public KeyValuePair<string, string> WinGetSource
        {
            get { return _winGetSource; }

            set
            {
                if (!Equals(_winGetSource, value))
                {
                    _winGetSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinGetSource)));
                }
            }
        }

        private List<KeyValuePair<string, string>> WinGetSourceList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsWinGetPage()
        {
            InitializeComponent();
            WinGetSourceList.Add(KeyValuePair.Create(WinGetConfigService.WinGetSourceList[0], BuiltInAppString));
            WinGetSourceList.Add(KeyValuePair.Create(WinGetConfigService.WinGetSourceList[1], AppInstallerString));
            WinGetSource = WinGetSourceList.Find(item => string.Equals(item.Key, WinGetConfigService.WinGetSource, StringComparison.OrdinalIgnoreCase));
            CurrentWinGetSource = WinGetSourceList.Find(item => string.Equals(item.Key, WinGetConfigService.CurrentWinGetSource, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 设置 WinGet 来源
        /// </summary>
        private void OnWinGetSourceSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                WinGetSource = WinGetSourceList[Convert.ToInt32(tag)];
                WinGetConfigService.SetWinGetSource(WinGetSource.Key);
            }
        }

        /// <summary>
        /// 配置 WinGet 数据源
        /// </summary>
        private void OnConfigurationClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                // 导航到 WinGet 数据源配置页面
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
                if (ApplicationData.GetForPackageFamily("Microsoft.DesktopAppInstaller_8wekyb3d8bbwe") is ApplicationData applicationData)
                {
                    string wingetConfigFilePath = Path.Combine(applicationData.LocalFolder.Path, "settings.json");

                    if (File.Exists(wingetConfigFilePath))
                    {
                        await Launcher.LaunchFileAsync(await global::Windows.Storage.StorageFile.GetFileFromPathAsync(wingetConfigFilePath));
                    }
                    else
                    {
                        Shell32Library.ShellExecute(nint.Zero, "open", "winget.exe", "settings", null, WindowShowStyle.SW_HIDE);
                    }
                }
            });
        }
    }
}
