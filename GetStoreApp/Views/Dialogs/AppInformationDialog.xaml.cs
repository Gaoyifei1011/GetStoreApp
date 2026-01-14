using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.UI.Text;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string DoNetVersionString = ResourceService.GetLocalized("Dialog/DoNetVersion");
        private readonly string WebView2SDKVersionString = ResourceService.GetLocalized("Dialog/WebView2SDKVersion");
        private readonly string WindowsAppSDKVersionString = ResourceService.GetLocalized("Dialog/WindowsAppSDKVersion");
        private readonly string WinUIVersionString = ResourceService.GetLocalized("Dialog/WinUIVersion");
        private readonly string fileVersionProperty = "System.FileVersion";

        private bool _isLoadCompleted = false;

        public bool IsLoadCompleted
        {
            get { return _isLoadCompleted; }

            set
            {
                if (!Equals(_isLoadCompleted, value))
                {
                    _isLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadCompleted)));
                }
            }
        }

        private List<string> PropertyNamesList => [fileVersionProperty];

        private ObservableCollection<ContentLinkInfo> AppInformationCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInformationDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 应用信息对话框初始化完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            List<ContentLinkInfo> dependencyInformationList = await Task.Run(async () =>
            {
                List<ContentLinkInfo> dependencyInformationList = [];
                IReadOnlyList<Package> dependencyPackageList = Package.Current.Dependencies;

                // Windows 应用 SDK 版本信息
                dependencyInformationList.Add(new ContentLinkInfo()
                {
                    DisplayText = WindowsAppSDKVersionString,
                    SecondaryText = RuntimeInfo.AsString
                });

                foreach (Package dependencyPackage in dependencyPackageList)
                {
                    if (dependencyPackage.DisplayName.Contains("WindowsAppRuntime"))
                    {
                        // WinUI 版本信息
                        try
                        {
                            StorageFile winUI3File = await StorageFile.GetFileFromPathAsync(Path.Combine(dependencyPackage.InstalledLocation.Path, "Microsoft.UI.Xaml.dll"));
                            IDictionary<string, object> winUI3FileProperties = await winUI3File.Properties.RetrievePropertiesAsync(PropertyNamesList);
                            dependencyInformationList.Add(new ContentLinkInfo()
                            {
                                DisplayText = WinUIVersionString,
                                SecondaryText = Convert.ToString((winUI3FileProperties[fileVersionProperty] is string fileVersionString ? new Version(fileVersionString) : new Version()))
                            });
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationDialog), nameof(OnLoaded), 1, e);
                            dependencyInformationList.Add(new ContentLinkInfo()
                            {
                                DisplayText = WinUIVersionString,
                                SecondaryText = Convert.ToString(new Version())
                            });
                        }
                        break;
                    }
                }

                // WebView2 SDK 版本信息
                try
                {
                    StorageFile webView2CoreFile = await StorageFile.GetFileFromPathAsync(Path.Combine(InfoHelper.AppInstalledLocation, "Microsoft.Web.WebView2.Core.dll"));
                    IDictionary<string, object> webView2CoreFileProperties = await webView2CoreFile.Properties.RetrievePropertiesAsync(PropertyNamesList);
                    dependencyInformationList.Add(new ContentLinkInfo()
                    {
                        DisplayText = WebView2SDKVersionString,
                        SecondaryText = Convert.ToString((webView2CoreFileProperties[fileVersionProperty] is string fileVersionString ? new Version(fileVersionString) : new Version()))
                    });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationDialog), nameof(OnLoaded), 4, e);
                    dependencyInformationList.Add(new ContentLinkInfo()
                    {
                        DisplayText = WebView2SDKVersionString,
                        SecondaryText = Convert.ToString(new Version())
                    });
                }

                // .NET 版本信息
                dependencyInformationList.Add(new ContentLinkInfo()
                {
                    DisplayText = DoNetVersionString,
                    SecondaryText = Convert.ToString(Environment.Version)
                });

                return dependencyInformationList;
            });

            foreach (ContentLinkInfo dependencyInformation in dependencyInformationList)
            {
                AppInformationCollection.Add(dependencyInformation);
            }

            IsLoadCompleted = true;
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private async void OnCopyAppInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool copyResult = false;
            ContentDialogButtonClickDeferral contentDialogButtonClickDeferral = args.GetDeferral();

            try
            {
                List<string> appInformationCopyStringList = await Task.Run(() =>
                {
                    List<string> appInformationCopyStringList = [];

                    foreach (ContentLinkInfo appInformationItem in AppInformationCollection)
                    {
                        appInformationCopyStringList.Add(appInformationItem.DisplayText + appInformationItem.SecondaryText);
                    }

                    return appInformationCopyStringList;
                });

                copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, appInformationCopyStringList));
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationDialog), nameof(OnCopyAppInformationClicked), 1, e);
            }
            finally
            {
                contentDialogButtonClickDeferral.Complete();
            }

            await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
        }
    }
}
