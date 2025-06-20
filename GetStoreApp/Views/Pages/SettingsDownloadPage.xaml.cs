using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.System;

// 抑制 CA1822，CS8305，IDE0060 警告
#pragma warning disable CA1822,CS8305,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置下载管理页面
    /// </summary>
    public sealed partial class SettingsDownloadPage : Page, INotifyPropertyChanged
    {
        private readonly string DoEngineAria2String = ResourceService.GetLocalized("SettingsDownload/DoEngineAria2");
        private readonly string DoEngineBitsString = ResourceService.GetLocalized("SettingsDownload/DoEngineBits");
        private readonly string DoEngineDoString = ResourceService.GetLocalized("SettingsDownload/DoEngineDo");

        private StorageFolder _downloadFolder = DownloadOptionsService.DownloadFolder;

        public StorageFolder DownloadFolder
        {
            get { return _downloadFolder; }

            set
            {
                if (!Equals(_downloadFolder, value))
                {
                    _downloadFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFolder)));
                }
            }
        }

        private KeyValuePair<string, string> _doEngineMode;

        public KeyValuePair<string, string> DoEngineMode
        {
            get { return _doEngineMode; }

            set
            {
                if (!Equals(_doEngineMode, value))
                {
                    _doEngineMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoEngineMode)));
                }
            }
        }

        private List<KeyValuePair<string, string>> DoEngineModeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsDownloadPage()
        {
            InitializeComponent();
            DoEngineModeList.Add(KeyValuePair.Create(DownloadOptionsService.DoEngineModeList[2], DoEngineDoString));
            DoEngineModeList.Add(KeyValuePair.Create(DownloadOptionsService.DoEngineModeList[1], DoEngineBitsString));
            DoEngineModeList.Add(KeyValuePair.Create(DownloadOptionsService.DoEngineModeList[0], DoEngineAria2String));
            DoEngineMode = DoEngineModeList.Find(item => string.Equals(item.Key, DownloadOptionsService.DoEngineMode, StringComparison.OrdinalIgnoreCase));
        }

        #region 第一部分：设置下载管理页面——挂载的事件

        /// <summary>
        /// 打开下载文件存放目录
        /// </summary>
        private void OnDownloadOpenFolderClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Task.Run(DownloadOptionsService.OpenFolderAsync);
        }

        /// <summary>
        /// 修改下载文件存放目录
        /// </summary>
        private async void OnDownloadChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string tag)
            {
                switch (tag)
                {
                    case "AppCache":
                        {
                            DownloadFolder = DownloadOptionsService.DefaultDownloadFolder;
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Download":
                        {
                            DownloadFolder = await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Downloads);
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Desktop":
                        {
                            DownloadFolder = await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Desktop);
                            DownloadOptionsService.SetFolder(DownloadFolder);
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
                                    DownloadFolder = await StorageFolder.GetFolderFromPathAsync(pickFolderResult.Path);
                                    DownloadOptionsService.SetFolder(DownloadFolder);
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsDownloadPage), nameof(OnDownloadChangeFolderClicked), 1, e);
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.FolderPicker));
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 打开传递优化设置
        /// </summary>
        private void OnOpenDeliveryOptimizationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:delivery-optimization"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 下载引擎说明
        /// </summary>
        private void OnLearnDoEngineClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                settingsPage.ShowSettingsInstruction();
            }
        }

        /// <summary>
        /// 下载引擎方式设置
        /// </summary>

        private void OnDoEngineModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                DoEngineMode = DoEngineModeList[Convert.ToInt32(tag)];
                DownloadOptionsService.SetDoEngineMode(DoEngineMode.Key);
            }
        }

        /// <summary>
        /// 打开 Aria2 配置文件
        /// </summary>
        private void OnConfigurationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                if (!File.Exists(Aria2Service.Aria2ConfPath))
                {
                    Aria2Service.InitializeAria2Conf();
                }

                try
                {
                    await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(Aria2Service.Aria2ConfPath));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        #endregion 第一部分：设置下载管理页面——挂载的事件
    }
}
