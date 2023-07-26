using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    /// <summary>
    /// WinGet 程序包设置控件
    /// </summary>
    public sealed partial class WinGetConfigControl : Expander, INotifyPropertyChanged
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

        private WinGetInstallModeModel _winGetInstallMode = WinGetConfigService.WinGetInstallMode;

        public WinGetInstallModeModel WinGetInstallMode
        {
            get { return _winGetInstallMode; }

            set
            {
                _winGetInstallMode = value;
                OnPropertyChanged();
            }
        }

        public List<WinGetInstallModeModel> WinGetInstallModeList => WinGetConfigService.WinGetInstallModeList;

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetConfigControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 判断两个版本是否共同存在
        /// </summary>
        public bool IsBothVersionExisted(bool isOfficialVersionExisted, bool isDevVersionExisted)
        {
            return isOfficialVersionExisted && isDevVersionExisted;
        }

        /// <summary>
        /// 判断 WinGet 程序包是否存在
        /// </summary>
        public bool IsWinGetExisted(bool isOfficialVersionExisted, bool isDevVersionExisted)
        {
            return isOfficialVersionExisted || isDevVersionExisted;
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        /// <summary>
        /// WinGet 程序包安装方式设置
        /// </summary>
        public async void OnWinGetInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender.As<ToggleMenuFlyoutItem>();
            if (item.Tag is not null)
            {
                WinGetInstallMode = WinGetInstallModeList[Convert.ToInt32(item.Tag)];
                await WinGetConfigService.SetWinGetInstallModeAsync(WinGetInstallMode);
            }
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
                WinGetSettingsStartupInfo.wShowWindow = WindowShowStyle.SW_HIDE;
                WinGetSettingsStartupInfo.cbReserved2 = 0;
                WinGetSettingsStartupInfo.lpReserved2 = IntPtr.Zero;

                WinGetSettingsStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                bool createResult = Kernel32Library.CreateProcess(null, string.Format("{0} {1}", "winget.exe", "settings"), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref WinGetSettingsStartupInfo, out PROCESS_INFORMATION WinGetSettingsProcessInformation);

                if (createResult)
                {
                    if (WinGetSettingsProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetSettingsProcessInformation.hProcess);
                    if (WinGetSettingsProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetSettingsProcessInformation.hThread);
                }
            }
        }

        /// <summary>
        /// 当两个版本共存时，设置是否优先使用开发版本
        /// </summary>
        public async void OnToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender.As<ToggleSwitch>();
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

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
