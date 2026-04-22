using GetStoreApp.Models;
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
using WinRT;

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

        private ComboBoxItemModel _currentWinGetSource;

        public ComboBoxItemModel CurrentWinGetSource
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

        private ComboBoxItemModel _winGetSource;

        public ComboBoxItemModel WinGetSource
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

        private List<ComboBoxItemModel> WinGetSourceList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsWinGetPage()
        {
            InitializeComponent();
            WinGetSourceList.Add(new ComboBoxItemModel() { SelectedValue = WinGetConfigService.WinGetSourceList[0], DisplayMember = BuiltInAppString });
            WinGetSourceList.Add(new ComboBoxItemModel() { SelectedValue = WinGetConfigService.WinGetSourceList[1], DisplayMember = AppInstallerString });
            WinGetSource = WinGetSourceList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), WinGetConfigService.WinGetSource, StringComparison.OrdinalIgnoreCase));
            CurrentWinGetSource = WinGetSourceList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), WinGetConfigService.CurrentWinGetSource, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 设置 WinGet 来源
        /// </summary>
        private void OnWinGetSourceSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel wingetSource && !Equals(WinGetSource, wingetSource))
            {
                WinGetSource = wingetSource;
                WinGetConfigService.SetWinGetSource(Convert.ToString(WinGetSource.SelectedValue));
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
                    string winGetConfigFilePath = Path.Combine(applicationData.LocalFolder.Path, "settings.json");

                    if (File.Exists(winGetConfigFilePath))
                    {
                        await Launcher.LaunchFileAsync(await global::Windows.Storage.StorageFile.GetFileFromPathAsync(winGetConfigFilePath));
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
