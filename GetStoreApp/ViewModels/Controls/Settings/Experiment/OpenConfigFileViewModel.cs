using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.Settings.Experiment
{
    public sealed class OpenConfigFileViewModel
    {
        // 打开配置文件目录
        public IRelayCommand OpenConfigFileCommand => new RelayCommand(async () =>
        {
            if (Aria2Service.Aria2ConfPath is not null)
            {
                string filePath = Aria2Service.Aria2ConfPath.Replace(@"\\", @"\");

                // 判断文件是否存在，文件存在则寻找对应的文件，不存在打开对应的目录；若目录不存在，则仅启动Explorer.exe进程，打开资源管理器的默认文件夹
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    IntPtr pidlList = Shell32Library.ILCreateFromPath(filePath);
                    if (pidlList != IntPtr.Zero)
                    {
                        try
                        {
                            Marshal.ThrowExceptionForHR(Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
                        }
                        finally
                        {
                            Shell32Library.ILFree(pidlList);
                        }
                    }
                }
                else
                {
                    string FileFolderPath = Path.GetDirectoryName(filePath);

                    if (Directory.Exists(FileFolderPath))
                    {
                        await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(FileFolderPath));
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
