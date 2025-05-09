using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
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
    public sealed partial class SettingsWinGetPage : Page, INotifyPropertyChanged
    {
        private StorageFolder _winGetPackageFolder = WinGetConfigService.DownloadFolder;

        public StorageFolder WinGetPackageFolder
        {
            get { return _winGetPackageFolder; }

            set
            {
                if (!Equals(_winGetPackageFolder, value))
                {
                    _winGetPackageFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinGetPackageFolder)));
                }
            }
        }

        private KeyValuePair<string, string> _winGetInstallMode = WinGetConfigService.WinGetInstallMode;

        public KeyValuePair<string, string> WinGetInstallMode
        {
            get { return _winGetInstallMode; }

            set
            {
                if (!Equals(_winGetInstallMode, value))
                {
                    _winGetInstallMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinGetInstallMode)));
                }
            }
        }

        private List<KeyValuePair<string, string>> WinGetInstallModeList { get; } = WinGetConfigService.WinGetInstallModeList;

        public event PropertyChangedEventHandler PropertyChanged;

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
        /// WinGet 程序包安装方式设置
        /// </summary>
        private void OnWinGetInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                WinGetInstallMode = WinGetInstallModeList[Convert.ToInt32(tag)];
                WinGetConfigService.SetWinGetInstallMode(WinGetInstallMode);
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

        /// <summary>
        /// 打开下载文件存放目录
        /// </summary>
        private async void OnWinGetOpenFolderClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await WinGetConfigService.OpenFolderAsync(WinGetPackageFolder);
        }

        /// <summary>
        /// 修改下载文件存放目录
        /// </summary>
        private async void OnWinGetChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string tag)
            {
                switch (tag)
                {
                    case "AppCache":
                        {
                            WinGetPackageFolder = WinGetConfigService.DefaultDownloadFolder;
                            WinGetConfigService.SetFolder(WinGetPackageFolder);
                            break;
                        }
                    case "Download":
                        {
                            WinGetPackageFolder = await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Downloads);
                            WinGetConfigService.SetFolder(WinGetPackageFolder);
                            break;
                        }
                    case "Desktop":
                        {
                            WinGetPackageFolder = await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Desktop);
                            WinGetConfigService.SetFolder(WinGetPackageFolder);
                            break;
                        }
                    case "Custom":
                        {
                            try
                            {
                                FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                                {
                                    SuggestedStartLocation = PickerLocationId.Downloads
                                };

                                if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                                {
                                    WinGetPackageFolder = await StorageFolder.GetFolderFromPathAsync(pickFolderResult.Path);
                                    WinGetConfigService.SetFolder(WinGetPackageFolder);
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Open folderPicker failed", e);
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.FolderPicker));
                            }

                            break;
                        }
                }
            }
        }
    }
}
