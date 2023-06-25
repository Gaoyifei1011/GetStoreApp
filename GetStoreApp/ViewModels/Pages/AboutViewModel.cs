using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.UI.Notifications;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.StartScreen;

namespace GetStoreApp.ViewModels.Pages
{
    /// <summary>
    /// 关于页面视图模型
    /// </summary>
    public sealed class AboutViewModel
    {
        private static readonly Guid CLSID_TaskbarPin = new Guid("90AA3A4E-1CBA-4233-B8BB-535773D48449");

        // COM接口：IUnknown 接口

        private static readonly Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

        /// <summary>
        /// 创建应用的桌面快捷方式
        /// </summary>
        public async void OnCreateDesktopShortcutClicked(object sender, RoutedEventArgs args)
        {
            bool IsCreatedSuccessfully = false;

            try
            {
                IShellLink AppLink = (IShellLink)new CShellLink();
                IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();
                AppListEntry DefaultEntry = AppEntries[0];
                AppLink.SetPath(string.Format(@"shell:AppsFolder\{0}", DefaultEntry.AppUserModelId));

                IPersistFile PersistFile = (IPersistFile)AppLink;
                PersistFile.Save(string.Format(@"{0}\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ResourceService.GetLocalized("Resources/AppDisplayName")), false);
                IsCreatedSuccessfully = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Create desktop shortcut failed.", e);
            }
            finally
            {
                new QuickOperationNotification(QuickOperationType.DesktopShortcut, IsCreatedSuccessfully).Show();
            }
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        public async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool IsPinnedSuccessfully = false;

            try
            {
                IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();

                AppListEntry DefaultEntry = AppEntries[0];

                if (DefaultEntry is not null)
                {
                    StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                    bool containsEntry = await startScreenManager.ContainsAppListEntryAsync(DefaultEntry);

                    if (!containsEntry)
                    {
                        await startScreenManager.RequestAddAppListEntryAsync(DefaultEntry);
                    }
                }
                IsPinnedSuccessfully = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Pin app to startscreen failed.", e);
            }
            finally
            {
                new QuickOperationNotification(QuickOperationType.StartScreen, IsPinnedSuccessfully).Show();
            }
        }

        // 将应用固定到任务栏
        public async void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            bool IsPinnedSuccessfully = false;

            try
            {
                IShellLink AppLink = (IShellLink)new CShellLink();
                IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();
                AppListEntry DefaultEntry = AppEntries[0];
                AppLink.SetPath(string.Format(@"shell:AppsFolder\{0}", DefaultEntry.AppUserModelId));

                AppLink.GetIDList(out IntPtr pidl);

                if (pidl != IntPtr.Zero)
                {
                    unsafe
                    {
                        fixed (Guid* CLSID_TaskbarPin_Ptr = &CLSID_TaskbarPin, IID_IUnknown_Ptr = &IID_IUnknown)
                        {
                            Ole32Library.CoCreateInstance(CLSID_TaskbarPin_Ptr, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, IID_IUnknown_Ptr, out IntPtr obj);
                            if (obj != IntPtr.Zero)
                            {
                                IPinnedList3 pinnedList = (IPinnedList3)Marshal.GetTypedObjectForIUnknown(obj, typeof(IPinnedList3));

                                if (pinnedList is not null)
                                {
                                    IsPinnedSuccessfully = pinnedList.Modify(IntPtr.Zero, pidl, PLMC.PLMC_EXPLORER) == 0;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Pin app to taskbar failed.", e);
            }
            finally
            {
                new QuickOperationNotification(QuickOperationType.Taskbar, IsPinnedSuccessfully).Show();
            }
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        public async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await new LicenseDialog().ShowAsync();
        }

        /// <summary>
        /// 查看更新日志
        /// </summary>
        public async void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        }
    }
}
