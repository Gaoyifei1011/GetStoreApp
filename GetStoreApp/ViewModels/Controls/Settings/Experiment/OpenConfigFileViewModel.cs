using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Controls.Download;
using GetStoreApp.Helpers.Root;
using GetStoreAppWindowsAPI.PInvoke.Shell32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Storage;

namespace GetStoreApp.ViewModels.Controls.Settings.Experiment
{
    public class OpenConfigFileViewModel : ObservableRecipient
    {
        private IAria2Service Aria2Service { get; } = ContainerHelper.GetInstance<IAria2Service>();

        // 打开配置文件目录
        public IRelayCommand OpenConfigFileCommand => new RelayCommand(async () =>
        {
            if (Aria2Service.Aria2ConfPath is not null)
            {
                string filePath = Aria2Service.Aria2ConfPath.Replace(@"\\", @"\");

                // 判断文件是否存在，文件存在则寻找对应的文件，不存在打开对应的目录；若目录不存在，则仅启动Explorer.exe进程，打开资源管理器的默认文件夹
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    IntPtr pidlList = DllFunctions.ILCreateFromPath(filePath);
                    if (pidlList != IntPtr.Zero)
                    {
                        try
                        {
                            Marshal.ThrowExceptionForHR(DllFunctions.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
                        }
                        finally
                        {
                            DllFunctions.ILFree(pidlList);
                        }
                    }
                }
                else
                {
                    string FileFolderPath = Path.GetDirectoryName(filePath);

                    if (Directory.Exists(FileFolderPath))
                    {
                        await Windows.System.Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(FileFolderPath));
                    }
                    else
                    {
                        Process.Start("explorer.exe");
                    }
                }
            }
        });
    }
}
