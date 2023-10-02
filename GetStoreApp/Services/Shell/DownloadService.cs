using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace GetStoreApp.Services.Shell
{
    /// <summary>
    /// 下载服务
    /// </summary>
    public static class DownloadService
    {
        private static bool IsFileDownloading = false;

        public static STARTUPINFO DownloadProcessStartupInfo;

        public static PROCESS_INFORMATION DownloadProcessInformation;

        /// <summary>
        /// 下载相应的文件
        /// </summary>
        public static async Task QueryDownloadIndexAsync()
        {
            while (true)
            {
                ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/DownloadFile"));

                try
                {
                    List<string> IndexList = ConsoleHelper.ReadLine().Split(',').ToList();
                    bool CheckResult = true;
                    foreach (string indexItem in IndexList)
                    {
                        int index = Convert.ToInt32(indexItem);
                        if (index > ParseService.ResultDataList.Count || index < 1)
                        {
                            CheckResult = false;
                            break;
                        }
                    }

                    if (CheckResult)
                    {
                        for (int index = 0; index < IndexList.Count; index++)
                        {
                            string IndexItem = IndexList[index];
                            if (ConsoleLaunchService.IsAppRunning)
                            {
                                ConsoleHelper.WriteLine(string.Format(ResourceService.GetLocalized("Console/DownloadingInformation"), index + 1, IndexList.Count));
                                DownloadFile(ParseService.ResultDataList[Convert.ToInt32(IndexItem) - 1].FileLink);
                            }
                        }
                        ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/DownloadCompleted"));
                        string InputString = ConsoleHelper.ReadLine();
                        if (InputString is "Y" || InputString is "y")
                        {
                            continue;
                        }
                        else
                        {
                            ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/OpenFolder"));
                            await OpenDownloadFolderAsync();
                            break;
                        }
                    }
                    else
                    {
                        ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/SerialNumberOutRange"));
                        string InputString = ConsoleHelper.ReadLine();
                        if (InputString is "Y" || InputString is "y")
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/SerialNumberError"));
                    string InputString = ConsoleHelper.ReadLine();
                    if (InputString is "Y" || InputString is "y")
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        private static unsafe void DownloadFile(string fileLink)
        {
            byte[] ReadBuff = new byte[101];

            DownloadProcessStartupInfo = new STARTUPINFO();
            DownloadProcessInformation = new PROCESS_INFORMATION();

            SECURITY_ATTRIBUTES DownloadSecurityAttributes = new SECURITY_ATTRIBUTES();
            DownloadSecurityAttributes.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
            DownloadSecurityAttributes.bInheritHandle = true;
            DownloadSecurityAttributes.lpSecurityDescriptor = 0;

            bool PipeCreateResult = Kernel32Library.CreatePipe(out IntPtr hRead, out IntPtr hWrite, &DownloadSecurityAttributes, 0);

            if (PipeCreateResult)
            {
                IntPtr Handle = Kernel32Library.GetStdHandle(StdHandle.STD_OUTPUT_HANDLE);
                Kernel32Library.SetStdHandle(StdHandle.STD_OUTPUT_HANDLE, hWrite);

                Kernel32Library.GetStartupInfo(out DownloadProcessStartupInfo);
                DownloadProcessStartupInfo.lpReserved = IntPtr.Zero;
                DownloadProcessStartupInfo.lpDesktop = IntPtr.Zero;
                DownloadProcessStartupInfo.lpTitle = IntPtr.Zero;
                DownloadProcessStartupInfo.dwX = 0;
                DownloadProcessStartupInfo.dwY = 0;
                DownloadProcessStartupInfo.dwXSize = 0;
                DownloadProcessStartupInfo.dwYSize = 0;
                DownloadProcessStartupInfo.dwXCountChars = 500;
                DownloadProcessStartupInfo.dwYCountChars = 500;
                DownloadProcessStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW | STARTF.STARTF_USESTDHANDLES;
                DownloadProcessStartupInfo.wShowWindow = WindowShowStyle.SW_HIDE;
                DownloadProcessStartupInfo.cbReserved2 = 0;
                DownloadProcessStartupInfo.lpReserved2 = IntPtr.Zero;
                DownloadProcessStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                DownloadProcessStartupInfo.hStdError = hWrite;
                DownloadProcessStartupInfo.hStdOutput = hWrite;

                bool createResult = Kernel32Library.CreateProcess(
                    null,
                    string.Format(
                        @"{0}\{1} --file-allocation=none -d ""{2}"" ""{3}""",
                        InfoHelper.AppInstalledLocation, "Mile.Aria2.exe",
                        DownloadOptionsService.DownloadFolder.Path, fileLink
                        ),
                    IntPtr.Zero,
                    IntPtr.Zero,
                    true,
                    CreateProcessFlags.CREATE_NO_WINDOW,
                    IntPtr.Zero,
                    null,
                    ref DownloadProcessStartupInfo,
                    out DownloadProcessInformation
                    );

                IsFileDownloading = true;
                Kernel32Library.SetStdHandle(StdHandle.STD_OUTPUT_HANDLE, Handle);
                Kernel32Library.CloseHandle(hWrite);

                if (createResult)
                {
                    while (Kernel32Library.ReadFile(hRead, ReadBuff, 100, out uint ReadNum, IntPtr.Zero))
                    {
                        ReadBuff[ReadNum] = (byte)'\0';
                        ConsoleHelper.Write(Encoding.UTF8.GetString(ReadBuff));
                    }

                    if (DownloadProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(DownloadProcessInformation.hProcess);
                    if (DownloadProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(DownloadProcessInformation.hThread);
                    Kernel32Library.CloseHandle(hRead);
                }

                if (hRead != IntPtr.Zero) Kernel32Library.CloseHandle(hRead);
                IsFileDownloading = false;
            }
        }

        /// <summary>
        /// 打开下载完成后保存的目录
        /// </summary>
        private static async Task OpenDownloadFolderAsync()
        {
            await Launcher.LaunchFolderAsync(DownloadOptionsService.DownloadFolder);
        }

        /// <summary>
        /// 程序即将退出，停止下载文件
        /// </summary>
        public static void StopDownloadFile()
        {
            if (IsFileDownloading)
            {
                if (DownloadProcessInformation.dwProcessId is not 0)
                {
                    IntPtr hProcess = Kernel32Library.OpenProcess(EDesiredAccess.PROCESS_TERMINATE, false, DownloadProcessInformation.dwProcessId);
                    if (hProcess != IntPtr.Zero)
                    {
                        Kernel32Library.TerminateProcess(hProcess, 0);
                    }
                    Kernel32Library.CloseHandle(hProcess);
                }
            }
        }
    }
}
