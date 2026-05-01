using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
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
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

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

        private string _downloadFolder = DownloadOptionsService.DownloadFolder;

        public string DownloadFolder
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

        private bool _manualSetDownloadFolder = DownloadOptionsService.ManualSetDownloadFolder;

        public bool ManualSetDownloadFolder
        {
            get { return _manualSetDownloadFolder; }

            set
            {
                if (!Equals(_manualSetDownloadFolder, value))
                {
                    _manualSetDownloadFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ManualSetDownloadFolder)));
                }
            }
        }

        private ComboBoxItemModel _doEngineMode;

        public ComboBoxItemModel DoEngineMode
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

        private List<ComboBoxItemModel> DoEngineModeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsDownloadPage()
        {
            InitializeComponent();
            DoEngineModeList.Add(new ComboBoxItemModel() { SelectedValue = DownloadOptionsService.DoEngineModeList[0], DisplayMember = DoEngineDoString });
            DoEngineModeList.Add(new ComboBoxItemModel() { SelectedValue = DownloadOptionsService.DoEngineModeList[1], DisplayMember = DoEngineBitsString });
            DoEngineModeList.Add(new ComboBoxItemModel() { SelectedValue = DownloadOptionsService.DoEngineModeList[2], DisplayMember = DoEngineAria2String });
            DoEngineMode = DoEngineModeList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), DownloadOptionsService.DoEngineMode, StringComparison.OrdinalIgnoreCase));
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
            if (sender.As<MenuFlyoutItem>().Tag is string tag)
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
                            DownloadFolder = InfoHelper.UserDataPath.Downloads;
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Desktop":
                        {
                            DownloadFolder = InfoHelper.UserDataPath.Desktop;
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Custom":
                        {
                            try
                            {
                                FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                                {
                                    SuggestedStartFolder = DownloadOptionsService.DownloadFolder
                                };

                                if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                                {
                                    DownloadFolder = pickFolderResult.Path;
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
        /// 设置手动设置下载目录
        /// </summary>
        private void OnManualSetDownloadFolderToggled(object sender, RoutedEventArgs args)
        {
            if (sender.As<ToggleSwitch>() is ToggleSwitch toggleSwitch)
            {
                DownloadOptionsService.SetManualSetDownloadFolder(toggleSwitch.IsOn);
                ManualSetDownloadFolder = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 下载引擎方式设置
        /// </summary>
        private void OnDoEngineModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel doEngineMode && !Equals(DoEngineMode, doEngineMode))
            {
                DoEngineMode = doEngineMode;
                DownloadOptionsService.SetDoEngineMode(Convert.ToString(DoEngineMode.SelectedValue));
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
