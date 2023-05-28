using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    /// <summary>
    /// WinGet 程序包设置控件视图模型
    /// </summary>
    public sealed class WinGetConfigViewModel : ViewModelBase
    {
        public bool IsOfficialVersionExisted { get; set; } = WinGetService.IsOfficialVersionExisted;

        public bool IsDevVersionExisted { get; set; } = WinGetService.IsDevVersionExisted;

        private bool _useDevVersion = WinGetConfigService.UseDevVersion;

        public bool UseDevVersion
        {
            get { return _useDevVersion; }

            set
            {
                _useDevVersion = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 安装开发版本
        /// </summary>
        public async void OnDevVersionInstallClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/winget-cli/releases"));
        }

        /// <summary>
        /// 安装官方版本
        /// </summary>
        public async void OnOfficialVersionInstallClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/winget-cli/releases"));
        }

        /// <summary>
        /// 打开 WinGet 程序包设置
        /// </summary>
        public void OnOpenWinGetSettingsClicked(object sender, RoutedEventArgs args)
        {
            unsafe
            {
                Kernel32Library.GetStartupInfo(out STARTUPINFO WinGetSettingsStartupInfo);
                WinGetSettingsStartupInfo.lpReserved = null;
                WinGetSettingsStartupInfo.lpDesktop = null;
                WinGetSettingsStartupInfo.lpTitle = null;
                WinGetSettingsStartupInfo.dwX = 0;
                WinGetSettingsStartupInfo.dwY = 0;
                WinGetSettingsStartupInfo.dwXSize = 0;
                WinGetSettingsStartupInfo.dwYSize = 0;
                WinGetSettingsStartupInfo.dwXCountChars = 500;
                WinGetSettingsStartupInfo.dwYCountChars = 500;
                WinGetSettingsStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                WinGetSettingsStartupInfo.wShowWindow = 0;
                WinGetSettingsStartupInfo.cbReserved2 = 0;
                WinGetSettingsStartupInfo.lpReserved2 = IntPtr.Zero;

                WinGetSettingsStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                Kernel32Library.CreateProcess(null, string.Format("{0} {1}", "winget.exe", "settings"), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref WinGetSettingsStartupInfo, out PROCESS_INFORMATION RebootProcessInformation);

                Kernel32Library.CloseHandle(RebootProcessInformation.hProcess);
                Kernel32Library.CloseHandle(RebootProcessInformation.hThread);
            }
        }

        /// <summary>
        /// 当两个版本共存时，设置是否优先使用开发版本
        /// </summary>
        public async void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                await WinGetConfigService.SetUseDevVersionAsync(toggleSwitch.IsOn);
                UseDevVersion = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// WinGet 程序包配置选项说明
        /// </summary>
        public void OnWinGetConfigInstructionClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }
    }
}
