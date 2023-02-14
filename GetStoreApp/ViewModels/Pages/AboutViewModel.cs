using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.UI.Notifications;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
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
        // 创建应用的桌面快捷方式
        public IRelayCommand CreateDesktopShortcutCommand => new RelayCommand(async () =>
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
            finally
            {
                new QuickOperationNotification(QuickOperationType.DesktopShortcut, IsCreatedSuccessfully).Show();
            }
        });

        // 将应用固定到“开始”屏幕
        public IRelayCommand PinToStartScreenCommand => new RelayCommand(async () =>
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
            finally
            {
                new QuickOperationNotification(QuickOperationType.StartScreen, IsPinnedSuccessfully).Show();
            }
        });

        // 查看更新日志
        public IRelayCommand ShowReleaseNotesCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        });

        // 查看许可证
        public IRelayCommand ShowLicenseCommand => new RelayCommand(async () =>
        {
            await new LicenseDialog().ShowAsync();
        });
    }
}
