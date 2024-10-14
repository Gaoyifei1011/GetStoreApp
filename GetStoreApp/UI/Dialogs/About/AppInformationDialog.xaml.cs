using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreApp.UI.Dialogs.About
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ContentDialog
    {
        private readonly string fileVersionProperty = "System.FileVersion";

        private List<string> PropertyNamesList => [fileVersionProperty];

        private ObservableCollection<DictionaryEntry> AppInformationCollection { get; } = [];

        public AppInformationDialog()
        {
            InitializeComponent();

            Task.Run(async () =>
            {
                IReadOnlyList<Package> dependencyPackageList = Package.Current.Dependencies;
                List<KeyValuePair<string, Version>> dependencyInformationList = [];

                foreach (Package dependencyPackage in dependencyPackageList)
                {
                    if (dependencyPackage.DisplayName.Contains("WindowsAppRuntime"))
                    {
                        // Windows 应用 SDK 版本信息
                        dependencyInformationList.Add(KeyValuePair.Create(ResourceService.GetLocalized("Dialog/WindowsAppSDKVersion"), new Version(dependencyPackage.Id.Version.Major, dependencyPackage.Id.Version.Minor, dependencyPackage.Id.Version.Build, dependencyPackage.Id.Version.Revision)));

                        // WinUI 3 版本信息
                        try
                        {
                            StorageFile winUI3File = await StorageFile.GetFileFromPathAsync(Path.Combine(dependencyPackage.InstalledLocation.Path, "Microsoft.UI.Xaml.dll"));
                            IDictionary<string, object> winUI3FileProperties = await winUI3File.Properties.RetrievePropertiesAsync(PropertyNamesList);
                            dependencyInformationList.Add(KeyValuePair.Create(ResourceService.GetLocalized("Dialog/WinUI3Version"), winUI3FileProperties[fileVersionProperty] is not null ? new Version(winUI3FileProperties[fileVersionProperty].ToString()) : new Version()));
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "Get WinUI 3 version failed.", e);
                            dependencyInformationList.Add(KeyValuePair.Create(ResourceService.GetLocalized("Dialog/WinUI3Version"), new Version()));
                        }
                        break;
                    }
                }

                foreach (Package dependencyPackage in dependencyPackageList)
                {
                    if (dependencyPackage.DisplayName.Contains("Microsoft.UI.Xaml"))
                    {
                        // WinUI 2 版本信息
                        try
                        {
                            StorageFile winUI2File = await StorageFile.GetFileFromPathAsync(Path.Combine(dependencyPackage.InstalledLocation.Path, "Microsoft.UI.Xaml.dll"));
                            IDictionary<string, object> winUI2FileProperties = await winUI2File.Properties.RetrievePropertiesAsync(PropertyNamesList);
                            dependencyInformationList.Add(KeyValuePair.Create(ResourceService.GetLocalized("Dialog/WinUI2Version"), winUI2FileProperties[fileVersionProperty] is not null ? new Version(winUI2FileProperties[fileVersionProperty].ToString()) : new Version()));
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "Get WinUI 2 version failed.", e);
                            dependencyInformationList.Add(KeyValuePair.Create(ResourceService.GetLocalized("Dialog/WinUI2Version"), new Version()));
                        }
                        break;
                    }
                }

                // Windows UI 版本信息
                try
                {
                    StorageFile windowsUIFile = await StorageFile.GetFileFromPathAsync(Path.Combine(InfoHelper.SystemDataPath.System, "Windows.UI.Xaml.dll"));
                    IDictionary<string, object> windowsUIFileProperties = await windowsUIFile.Properties.RetrievePropertiesAsync(PropertyNamesList);
                    dependencyInformationList.Add(KeyValuePair.Create(ResourceService.GetLocalized("Dialog/WindowsUIVersion"), windowsUIFileProperties[fileVersionProperty] is not null ? new Version(windowsUIFileProperties[fileVersionProperty].ToString()) : new Version()));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Get Windows UI version failed.", e);
                    dependencyInformationList.Add(KeyValuePair.Create(ResourceService.GetLocalized("Dialog/WindowsUIVersion"), new Version()));
                }

                // WebView2 SDK 版本信息
                try
                {
                    StorageFile webView2CoreFile = await StorageFile.GetFileFromPathAsync(Path.Combine(InfoHelper.AppInstalledLocation, "Microsoft.Web.WebView2.Core.dll"));
                    IDictionary<string, object> webView2CoreFileProperties = await webView2CoreFile.Properties.RetrievePropertiesAsync(PropertyNamesList);
                    dependencyInformationList.Add(KeyValuePair.Create(ResourceService.GetLocalized("Dialog/WebView2SDKVersion"), webView2CoreFileProperties[fileVersionProperty] is not null ? new Version(webView2CoreFileProperties[fileVersionProperty].ToString()) : new Version()));
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Get WebView2 SDK version failed.", e);
                    dependencyInformationList.Add(KeyValuePair.Create(ResourceService.GetLocalized("Dialog/WebView2SDKVersion"), new Version()));
                }

                // .NET 版本信息
                dependencyInformationList.Add(KeyValuePair.Create(ResourceService.GetLocalized("Dialog/DoNetVersion"), Environment.Version));

                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (KeyValuePair<string, Version> dependencyInformation in dependencyInformationList)
                    {
                        AppInformationCollection.Add(new DictionaryEntry(dependencyInformation.Key, dependencyInformation.Value));
                    }
                });
            });
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private void OnCopyAppInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            Task.Run(() =>
            {
                StringBuilder stringBuilder = new();
                foreach (DictionaryEntry appInformationItem in AppInformationCollection)
                {
                    stringBuilder.Append(appInformationItem.Key);
                    stringBuilder.Append(appInformationItem.Value);
                    stringBuilder.Append(Environment.NewLine);
                }

                DispatcherQueue.TryEnqueue(async () =>
                {
                    bool copyResult = CopyPasteHelper.CopyTextToClipBoard(stringBuilder.ToString());
                    sender.Hide();
                    await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.AppInformation, copyResult));
                });
            });
        }
    }
}
