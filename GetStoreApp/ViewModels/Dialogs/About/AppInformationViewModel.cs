using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Notifications;
using GetStoreApp.WindowsAPI.PInvoke.Version;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel;

namespace GetStoreApp.ViewModels.Dialogs.About
{
    /// <summary>
    /// 应用信息对话框视图模型
    /// </summary>
    public sealed class AppInformationViewModel
    {
        public string WindowsAppSDKVersion { get; set; }

        public string WinUI3Version { get; set; }

        public string DoNetVersion { get; set; }

        public string WebView2CoreVersion { get; set; }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        public void CopyAppInformation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/WindowsAppSDKVersion") + WindowsAppSDKVersion);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/WinUI3Version") + WinUI3Version);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/WebView2CoreVersion") + WebView2CoreVersion);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/DoNetVersion") + DoNetVersion);

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            new CopyAppInformationNotification(true).Show();
        }

        /// <summary>
        /// 初始化应用信息
        /// </summary>
        public void InitializeAppInformation()
        {
            IReadOnlyList<Package> DependencyList = Package.Current.Dependencies;

            foreach (Package dependency in DependencyList)
            {
                if (dependency.DisplayName.Contains("WindowsAppRuntime"))
                {
                    // Windows 应用 SDK 版本信息
                    WindowsAppSDKVersion = string.Format("{0}.{1}.{2}.{3}",
                        dependency.Id.Version.Major,
                        dependency.Id.Version.Minor,
                        dependency.Id.Version.Build,
                        dependency.Id.Version.Revision);

                    // WinUI3 版本信息
                    VS_FIXEDFILEINFO WinUI3FileInfo = InfoHelper.GetFileInfo(string.Format(@"{0}\{1}", dependency.InstalledLocation.Path, "Microsoft.ui.xaml.Controls.dll"));
                    WinUI3Version = string.Format("{0}.{1}.{2}.{3}",
                        (WinUI3FileInfo.dwProductVersionMS >> 16) & 0xffff,
                        (WinUI3FileInfo.dwProductVersionMS >> 0) & 0xffff,
                        (WinUI3FileInfo.dwProductVersionLS >> 16) & 0xffff,
                        (WinUI3FileInfo.dwProductVersionLS >> 0) & 0xffff);

                    // WebView2 Core 版本信息
                    VS_FIXEDFILEINFO WebView2CoreFileInfo = InfoHelper.GetFileInfo(string.Format(@"{0}\{1}", dependency.InstalledLocation.Path, "Microsoft.Web.WebView2.Core.dll"));
                    WebView2CoreVersion = string.Format("{0}.{1}.{2}.{3}",
                        (WebView2CoreFileInfo.dwFileVersionMS >> 16) & 0xffff,
                        (WebView2CoreFileInfo.dwFileVersionMS >> 0) & 0xffff,
                        (WebView2CoreFileInfo.dwFileVersionLS >> 16) & 0xffff,
                        (WebView2CoreFileInfo.dwFileVersionLS >> 0) & 0xffff);
                }
            }

            // .NET 版本信息
            DoNetVersion = Environment.Version.ToString();
        }
    }
}
