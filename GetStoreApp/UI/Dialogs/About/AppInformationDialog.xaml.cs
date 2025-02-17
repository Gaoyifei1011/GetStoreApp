using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.UI.Text;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.UI.Dialogs.About
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ContentDialog
    {
        private bool isInitialized;
        private readonly string fileVersionProperty = "System.FileVersion";

        private List<string> PropertyNamesList => [fileVersionProperty];

        private ObservableCollection<ContentLinkInfo> AppInformationCollection { get; } = [];

        public AppInformationDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 应用信息对话框初始化完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                isInitialized = true;

                List<ContentLinkInfo> dependencyInformationList = [];
                await Task.Run(async () =>
                {
                    IReadOnlyList<Package> dependencyPackageList = Package.Current.Dependencies;

                    // Windows 应用 SDK 版本信息
                    dependencyInformationList.Add(new ContentLinkInfo()
                    {
                        DisplayText = ResourceService.GetLocalized("Dialog/WindowsAppSDKVersion"),
                        SecondaryText = RuntimeInfo.AsString
                    });

                    foreach (Package dependencyPackage in dependencyPackageList)
                    {
                        if (dependencyPackage.DisplayName.Contains("WindowsAppRuntime"))
                        {
                            // WinUI 3 版本信息
                            try
                            {
                                StorageFile winUI3File = await StorageFile.GetFileFromPathAsync(Path.Combine(dependencyPackage.InstalledLocation.Path, "Microsoft.UI.Xaml.dll"));
                                IDictionary<string, object> winUI3FileProperties = await winUI3File.Properties.RetrievePropertiesAsync(PropertyNamesList);
                                dependencyInformationList.Add(new ContentLinkInfo()
                                {
                                    DisplayText = ResourceService.GetLocalized("Dialog/WinUI3Version"),
                                    SecondaryText = (winUI3FileProperties[fileVersionProperty] is not null ? new Version(winUI3FileProperties[fileVersionProperty].ToString()) : new Version()).ToString()
                                });
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Warning, "Get WinUI 3 version failed.", e);
                                dependencyInformationList.Add(new ContentLinkInfo()
                                {
                                    DisplayText = ResourceService.GetLocalized("Dialog/WinUI3Version"),
                                    SecondaryText = new Version().ToString()
                                });
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
                                dependencyInformationList.Add(new ContentLinkInfo()
                                {
                                    DisplayText = ResourceService.GetLocalized("Dialog/WinUI2Version"),
                                    SecondaryText = (winUI2FileProperties[fileVersionProperty] is not null ? new Version(winUI2FileProperties[fileVersionProperty].ToString()) : new Version()).ToString()
                                });
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Warning, "Get WinUI 2 version failed.", e);
                                dependencyInformationList.Add(new ContentLinkInfo()
                                {
                                    DisplayText = ResourceService.GetLocalized("Dialog/WinUI2Version"),
                                    SecondaryText = new Version().ToString()
                                });
                            }
                            break;
                        }
                    }

                    // Windows UI 版本信息
                    try
                    {
                        StorageFile windowsUIFile = await StorageFile.GetFileFromPathAsync(Path.Combine(InfoHelper.SystemDataPath.System, "Windows.UI.Xaml.dll"));
                        IDictionary<string, object> windowsUIFileProperties = await windowsUIFile.Properties.RetrievePropertiesAsync(PropertyNamesList);
                        dependencyInformationList.Add(new ContentLinkInfo()
                        {
                            DisplayText = ResourceService.GetLocalized("Dialog/WindowsUIVersion"),
                            SecondaryText = (windowsUIFileProperties[fileVersionProperty] is not null ? new Version(windowsUIFileProperties[fileVersionProperty].ToString()) : new Version()).ToString()
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Get Windows UI version failed.", e);
                        dependencyInformationList.Add(new ContentLinkInfo()
                        {
                            DisplayText = ResourceService.GetLocalized("Dialog/WindowsUIVersion"),
                            SecondaryText = new Version().ToString()
                        });
                    }

                    // WebView2 SDK 版本信息
                    try
                    {
                        StorageFile webView2CoreFile = await StorageFile.GetFileFromPathAsync(Path.Combine(InfoHelper.AppInstalledLocation, "Microsoft.Web.WebView2.Core.dll"));
                        IDictionary<string, object> webView2CoreFileProperties = await webView2CoreFile.Properties.RetrievePropertiesAsync(PropertyNamesList);
                        dependencyInformationList.Add(new ContentLinkInfo()
                        {
                            DisplayText = ResourceService.GetLocalized("Dialog/WebView2SDKVersion"),
                            SecondaryText = (webView2CoreFileProperties[fileVersionProperty] is not null ? new Version(webView2CoreFileProperties[fileVersionProperty].ToString()) : new Version()).ToString()
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Get WebView2 SDK version failed.", e);
                        dependencyInformationList.Add(new ContentLinkInfo()
                        {
                            DisplayText = ResourceService.GetLocalized("Dialog/WebView2SDKVersion"),
                            SecondaryText = new Version().ToString()
                        });
                    }

                    // .NET 版本信息
                    dependencyInformationList.Add(new ContentLinkInfo()
                    {
                        DisplayText = ResourceService.GetLocalized("Dialog/DoNetVersion"),
                        SecondaryText = Environment.Version.ToString()
                    });
                });

                foreach (ContentLinkInfo dependencyInformation in dependencyInformationList)
                {
                    AppInformationCollection.Add(dependencyInformation);
                }
            }
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private async void OnCopyAppInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            List<string> appInformationCopyStringList = [];

            await Task.Run(() =>
            {
                foreach (ContentLinkInfo appInformationItem in AppInformationCollection)
                {
                    appInformationCopyStringList.Add(appInformationItem.DisplayText + appInformationItem.SecondaryText);
                }
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, appInformationCopyStringList));
            sender.Hide();
            await TeachingTipHelper.ShowAsync(new MainDataCopyTip(DataCopyKind.AppInformation, copyResult));
        }
    }
}
