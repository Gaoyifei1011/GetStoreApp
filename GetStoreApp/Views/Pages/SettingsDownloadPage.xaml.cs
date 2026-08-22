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
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
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
    internal sealed partial class SettingsDownloadPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string DoEngineAria2String = ResourceService.GetLocalized("SettingsDownload/DoEngineAria2");
        private readonly string DoEngineBitsString = ResourceService.GetLocalized("SettingsDownload/DoEngineBits");
        private readonly string DoEngineDoString = ResourceService.GetLocalized("SettingsDownload/DoEngineDo");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private string _downloadFolder;

        private string DownloadFolder
        {
            get { return _downloadFolder; }

            set
            {
                if (!Equals(_downloadFolder, value))
                {
                    _downloadFolder = value;
                    PropertyChanged?.Invoke(this, new(nameof(DownloadFolder)));
                }
            }
        }

        private bool _manualSetDownloadFolder;

        private bool ManualSetDownloadFolder
        {
            get { return _manualSetDownloadFolder; }

            set
            {
                if (!Equals(_manualSetDownloadFolder, value))
                {
                    _manualSetDownloadFolder = value;
                    PropertyChanged?.Invoke(this, new(nameof(ManualSetDownloadFolder)));
                }
            }
        }

        private ComboBoxItemModel _doEngineMode;

        private ComboBoxItemModel DoEngineMode
        {
            get { return _doEngineMode; }

            set
            {
                if (!Equals(_doEngineMode, value))
                {
                    _doEngineMode = value;
                    PropertyChanged?.Invoke(this, new(nameof(DoEngineMode)));
                }
            }
        }

        private List<ComboBoxItemModel> DoEngineModeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal SettingsDownloadPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面后触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            DownloadFolder = DownloadOptionsService.DownloadFolder;
            ManualSetDownloadFolder = DownloadOptionsService.ManualSetDownloadFolder;
            DoEngineMode = DoEngineModeList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), DownloadOptionsService.DoEngineMode, StringComparison.OrdinalIgnoreCase));
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

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
        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
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
            OpenDeliveryOptimization();
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
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnManualSetDownloadFolderToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(ManualSetDownloadFolder, toggleSwitch.IsOn))
            {
                ManualSetDownloadFolder = toggleSwitch.IsOn;
                DownloadOptionsService.SetManualSetDownloadFolder(toggleSwitch.IsOn);
                ManualSetDownloadFolder = DownloadOptionsService.ManualSetDownloadFolder;
            }
        }

        /// <summary>
        /// 下载引擎方式设置
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnDoEngineModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(DoEngineMode, comboBox.SelectedItem))
            {
                DoEngineMode = comboBox.SelectedItem is ComboBoxItemModel doEngineMode ? doEngineMode : null;

                if (DoEngineMode is not null)
                {
                    DownloadOptionsService.SetDoEngineMode(Convert.ToString(DoEngineMode.SelectedValue));
                }

                DoEngineMode = DoEngineModeList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), DownloadOptionsService.DoEngineMode, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 打开 Aria2 配置文件
        /// </summary>
        private void OnConfigurationClicked(object sender, RoutedEventArgs args)
        {
            OpenAria2Conf();
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            DoEngineModeList.Add(new() { SelectedValue = DownloadOptionsService.DoEngineModeList[0], DisplayMember = DoEngineDoString });
            DoEngineModeList.Add(new() { SelectedValue = DownloadOptionsService.DoEngineModeList[1], DisplayMember = DoEngineBitsString });
            DoEngineModeList.Add(new() { SelectedValue = DownloadOptionsService.DoEngineModeList[2], DisplayMember = DoEngineAria2String });
        }

        /// <summary>
        /// 打开传递优化设置
        /// </summary>
        private void OpenDeliveryOptimization()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("ms-settings:delivery-optimization"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开 Aria2 文件配置
        /// </summary>
        private void OpenAria2Conf()
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

        #endregion 第六部分：数据操作与业务逻辑
    }
}
