using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Collections.Generic;
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
        private static bool isFileDownloading = false;

        public static STARTUPINFO downloadStartupInfo;
        public static PROCESS_INFORMATION downloadInformation;

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
                    List<string> indexList = new List<string>();
                    foreach (string index in ConsoleHelper.ReadLine().Split(','))
                    {
                        indexList.Add(index);
                    }

                    bool checkResult = true;
                    foreach (string indexItem in indexList)
                    {
                        int index = Convert.ToInt32(indexItem);
                        if (index > ParseService._queryLinksList.Count || index < 1)
                        {
                            checkResult = false;
                            break;
                        }
                    }

                    if (checkResult)
                    {
                        for (int index = 0; index < indexList.Count; index++)
                        {
                            string indexItem = indexList[index];
                            if (ConsoleLaunchService.IsAppRunning)
                            {
                                ConsoleHelper.WriteLine(string.Format(ResourceService.GetLocalized("Console/DownloadingInformation"), index + 1, indexList.Count));
                                DownloadFile(ParseService._queryLinksList[Convert.ToInt32(indexItem) - 1].FileName, ParseService._queryLinksList[Convert.ToInt32(indexItem) - 1].FileLink);
                            }
                        }
                        ConsoleHelper.WriteLine(ResourceService.GetLocalized("Console/Completed"));
                        string inputString = ConsoleHelper.ReadLine();
                        if (inputString is "Y" || inputString is "y")
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
                        string inputString = ConsoleHelper.ReadLine();
                        if (inputString is "Y" || inputString is "y")
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
                    string inputString = ConsoleHelper.ReadLine();
                    if (inputString is "Y" || inputString is "y")
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
        private static void DownloadFile(string fileName, string fileLink)
        {
            byte[] readBuff = new byte[101];

            SECURITY_ATTRIBUTES downloadSecurityAttributes = new SECURITY_ATTRIBUTES();
            downloadSecurityAttributes.nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>();
            downloadSecurityAttributes.bInheritHandle = 1;
            downloadSecurityAttributes.lpSecurityDescriptor = 0;

            bool pipeCreateResult = Kernel32Library.CreatePipe(out IntPtr hRead, out IntPtr hWrite, ref downloadSecurityAttributes, 0);

            if (pipeCreateResult)
            {
                IntPtr handle = Kernel32Library.GetStdHandle(STD_HANDLE.STD_OUTPUT_HANDLE);
                Kernel32Library.SetStdHandle(STD_HANDLE.STD_OUTPUT_HANDLE, hWrite);

                Kernel32Library.GetStartupInfo(out STARTUPINFO downloadStartupInfo);
                downloadStartupInfo.lpReserved = IntPtr.Zero;
                downloadStartupInfo.lpDesktop = IntPtr.Zero;
                downloadStartupInfo.lpTitle = IntPtr.Zero;
                downloadStartupInfo.dwX = 0;
                downloadStartupInfo.dwY = 0;
                downloadStartupInfo.dwXSize = 0;
                downloadStartupInfo.dwYSize = 0;
                downloadStartupInfo.dwXCountChars = 500;
                downloadStartupInfo.dwYCountChars = 500;
                downloadStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW | STARTF.STARTF_USESTDHANDLES;
                downloadStartupInfo.wShowWindow = WindowShowStyle.SW_HIDE;
                downloadStartupInfo.cbReserved2 = 0;
                downloadStartupInfo.lpReserved2 = IntPtr.Zero;
                downloadStartupInfo.cb = Marshal.SizeOf<STARTUPINFO>();
                downloadStartupInfo.hStdError = hWrite;
                downloadStartupInfo.hStdOutput = hWrite;

                bool createResult = Kernel32Library.CreateProcess(
                    null,
                    string.Format(
                        @"{0}\{1} --file-allocation=none -d ""{2}"" -o""{3}"" ""{4}""",
                        InfoHelper.AppInstalledLocation, "Mile.Aria2.exe",
                        DownloadOptionsService.DownloadFolder.Path, fileName, fileLink
                        ),
                    IntPtr.Zero,
                    IntPtr.Zero,
                    true,
                    CREATE_PROCESS_FLAGS.CREATE_NO_WINDOW,
                    IntPtr.Zero,
                    null,
                    ref downloadStartupInfo,
                    out downloadInformation
                    );

                isFileDownloading = true;
                Kernel32Library.SetStdHandle(STD_HANDLE.STD_OUTPUT_HANDLE, handle);
                Kernel32Library.CloseHandle(hWrite);

                if (createResult)
                {
                    while (Kernel32Library.ReadFile(hRead, readBuff, 100, out uint ReadNum, IntPtr.Zero))
                    {
                        readBuff[ReadNum] = (byte)'\0';
                        ConsoleHelper.Write(Encoding.UTF8.GetString(readBuff));
                    }

                    if (downloadInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(downloadInformation.hProcess);
                    if (downloadInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(downloadInformation.hThread);
                    Kernel32Library.CloseHandle(hRead);
                }

                if (hRead != IntPtr.Zero) Kernel32Library.CloseHandle(hRead);
                isFileDownloading = false;
                ConsoleHelper.Write(Environment.NewLine);
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
            if (isFileDownloading)
            {
                if (downloadInformation.dwProcessId is not 0)
                {
                    IntPtr hProcess = Kernel32Library.OpenProcess(EDESIREDACCESS.PROCESS_TERMINATE, false, downloadInformation.dwProcessId);
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
