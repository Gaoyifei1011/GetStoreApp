using GetStoreApp.Services.Controls.Download;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.UI.Controls.Settings.Experiment
{
    /// <summary>
    /// 实验性功能：打开Aria2配置文件控件
    /// </summary>
    public sealed partial class OpenConfigFileControl : Grid
    {
        public OpenConfigFileControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开配置文件目录
        /// </summary>
        public async void OnOpenConfigFileClicked(object sender, RoutedEventArgs args)
        {
            if (Aria2Service.Aria2ConfPath is not null)
            {
                string filePath = Aria2Service.Aria2ConfPath.Replace(@"\\", @"\");

                // 定位文件，若定位失败，则仅启动资源管理器并打开桌面目录
                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
                        StorageFolder folder = await file.GetParentAsync();
                        FolderLauncherOptions options = new FolderLauncherOptions();
                        options.ItemsToSelect.Add(file);
                        await Launcher.LaunchFolderAsync(folder, options);
                    }
                    catch (Exception)
                    {
                        await Launcher.LaunchFolderPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    }
                }
                else
                {
                    await Launcher.LaunchFolderPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                }
            }
        }
    }
}
