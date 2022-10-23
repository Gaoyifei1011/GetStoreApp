using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Storage;

namespace GetStoreApp.ViewModels.Controls.Settings.Experiment
{
    public class OpenConfigFileViewModel : ObservableRecipient
    {
        private IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern void ILFree(IntPtr pidlList);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);

        // 打开配置文件目录
        public IRelayCommand OpenConfigFileCommand => new RelayCommand(async () =>
        {
            if (Aria2Service.Aria2ConfPath is not null)
            {
                string filePath = Aria2Service.Aria2ConfPath.Replace(@"\\", @"\");

                // 判断文件是否存在，文件存在则寻找对应的文件，不存在打开对应的目录；若目录不存在，则仅启动Explorer.exe进程，打开资源管理器的默认文件夹
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    IntPtr pidlList = ILCreateFromPathW(filePath);
                    if (pidlList != IntPtr.Zero)
                    {
                        try
                        {
                            Marshal.ThrowExceptionForHR(SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
                        }
                        finally
                        {
                            ILFree(pidlList);
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
