using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class AppInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string fileVersionProperty = "System.FileVersion";

        private List<string> PropertyNamesList => [fileVersionProperty];

        private string _windowsAppSDKVersion;

        public string WindowsAppSDKVersion
        {
            get { return _windowsAppSDKVersion; }

            set
            {
                if (!Equals(_windowsAppSDKVersion, value))
                {
                    _windowsAppSDKVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowsAppSDKVersion)));
                }
            }
        }

        private string _winUI3Version;

        public string WinUI3Version
        {
            get { return _winUI3Version; }

            set
            {
                if (!Equals(_winUI3Version, value))
                {
                    _winUI3Version = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinUI3Version)));
                }
            }
        }

        private string _windowsUIVersion;

        public string WindowsUIVersion
        {
            get { return _windowsUIVersion; }

            set
            {
                if (!Equals(_windowsUIVersion, value))
                {
                    _windowsUIVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowsUIVersion)));
                }
            }
        }

        private string _doNetVersion;

        public string DoNetVersion
        {
            get { return _doNetVersion; }

            set
            {
                if (!Equals(_doNetVersion, value))
                {
                    _doNetVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoNetVersion)));
                }
            }
        }

        private string _webView2SDKVersion;

        public string WebView2SDKVersion
        {
            get { return _webView2SDKVersion; }

            set
            {
                if (!Equals(_webView2SDKVersion, value))
                {
                    _webView2SDKVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WebView2SDKVersion)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInformationDialog()
        {
            InitializeComponent();

            Task.Run(async () =>
            {
                IReadOnlyList<Package> dependencyPackageList = Package.Current.Dependencies;

                foreach (Package dependencyPackage in dependencyPackageList)
                {
                    if (dependencyPackage.DisplayName.Contains("WindowsAppRuntime"))
                    {
                        // Windows 应用 SDK 版本信息
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WindowsAppSDKVersion = new Version(dependencyPackage.Id.Version.Major, dependencyPackage.Id.Version.Minor, dependencyPackage.Id.Version.Build, dependencyPackage.Id.Version.Revision).ToString();
                        });

                        // WinUI 3 版本信息
                        try
                        {
                            StorageFile winUI3File = await StorageFile.GetFileFromPathAsync(Path.Combine(dependencyPackage.InstalledLocation.Path, "Microsoft.UI.Xaml.Controls.dll"));
                            IDictionary<string, object> winUI3FileProperties = await winUI3File.Properties.RetrievePropertiesAsync(PropertyNamesList);
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                WinUI3Version = winUI3FileProperties[fileVersionProperty] is not null ? Convert.ToString(winUI3FileProperties[fileVersionProperty]) : string.Empty;
                            });
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "Get WinUI3 version failed.", e);
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                WinUI3Version = new Version().ToString();
                            });
                        }
                    }
                }

                // Windows UI 版本信息
                try
                {
                    StorageFile windowsUIFile = await StorageFile.GetFileFromPathAsync(Path.Combine(InfoHelper.SystemDataPath.System, "Windows.UI.Xaml.dll"));
                    IDictionary<string, object> windowsUIFileProperties = await windowsUIFile.Properties.RetrievePropertiesAsync(PropertyNamesList);
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        WindowsUIVersion = windowsUIFileProperties[fileVersionProperty] is not null ? Convert.ToString(windowsUIFileProperties[fileVersionProperty]) : string.Empty;
                    });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Get Windows UI version failed.", e);
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        WindowsUIVersion = new Version().ToString();
                    });
                }

                // WebView2 SDK 版本信息
                try
                {
                    StorageFile webView2CoreFile = await StorageFile.GetFileFromPathAsync(Path.Combine(InfoHelper.AppInstalledLocation, "Microsoft.Web.WebView2.Core.dll"));
                    IDictionary<string, object> webView2CoreFileProperties = await webView2CoreFile.Properties.RetrievePropertiesAsync(PropertyNamesList);
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        WebView2SDKVersion = webView2CoreFileProperties[fileVersionProperty] is not null ? Convert.ToString(webView2CoreFileProperties[fileVersionProperty]) : string.Empty;
                    });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Get WebView2 SDK version failed.", e);
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        WebView2SDKVersion = new Version().ToString();
                    });
                }
            });

            DoNetVersion = Environment.Version.ToString();
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
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/WindowsAppSDKVersion") + WindowsAppSDKVersion);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/WinUI3Version") + WinUI3Version);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/DoNetVersion") + DoNetVersion);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/WebView2SDKVersion") + WebView2SDKVersion);

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
