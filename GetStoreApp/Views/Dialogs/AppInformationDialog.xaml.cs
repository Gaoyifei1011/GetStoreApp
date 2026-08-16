using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
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
    internal sealed partial class AppInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string DoNetVersionString = ResourceService.GetLocalized("Dialog/DoNetVersion");
        private readonly string WebView2SDKVersionString = ResourceService.GetLocalized("Dialog/WebView2SDKVersion");
        private readonly string WindowsAppSDKVersionString = ResourceService.GetLocalized("Dialog/WindowsAppSDKVersion");
        private readonly string WinUIVersionString = ResourceService.GetLocalized("Dialog/WinUIVersion");
        private readonly string fileVersionProperty = "System.FileVersion";

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private bool _isLoadCompleted;

        private bool IsLoadCompleted
        {
            get { return _isLoadCompleted; }

            set
            {
                if (!Equals(_isLoadCompleted, value))
                {
                    _isLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsLoadCompleted)));
                }
            }
        }

        private List<string> PropertyNameList => [fileVersionProperty];

        private ObservableCollection<ContentLinkInfo> AppInformationCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal AppInformationDialog()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 打开内容对话框后发生的事件
        /// </summary>
        private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            IsLoadCompleted = false;
            List<ContentLinkInfo> dependencyInformationList = await GetDependencyInformationListAsync(PropertyNameList);

            foreach (ContentLinkInfo dependencyInformation in dependencyInformationList)
            {
                AppInformationCollection.Add(dependencyInformation);
            }

            IsLoadCompleted = true;
        }

        /// <summary>
        /// 加载完成前禁用关闭对话框
        /// </summary>
        private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (!IsLoadCompleted)
            {
                args.Cancel = true;
            }
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
                List<string> appInformationCopyStringList = await GetAppInformationCopyStringListAsync([.. AppInformationCollection]);
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

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 获取应用依赖信息
        /// </summary>
        private async Task<List<ContentLinkInfo>> GetDependencyInformationListAsync(List<string> propertyNameList)
        {
            if (propertyNameList is not null)
            {
                return await Task.Run(async () =>
                {
                    List<ContentLinkInfo> dependencyInformationList = [];
                    IReadOnlyList<Package> dependencyPackageList = Package.Current.Dependencies;

                    // Windows 应用 SDK 版本信息
                    dependencyInformationList.Add(new()
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
                                IDictionary<string, object> winUI3FileProperties = await winUI3File.Properties.RetrievePropertiesAsync(propertyNameList);
                                dependencyInformationList.Add(new()
                                {
                                    DisplayText = WinUIVersionString,
                                    SecondaryText = Convert.ToString((winUI3FileProperties[fileVersionProperty] is string fileVersionString ? new Version(fileVersionString) : new Version()))
                                });
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationDialog), nameof(GetDependencyInformationListAsync), 1, e);
                                dependencyInformationList.Add(new()
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
                        IDictionary<string, object> webView2CoreFileProperties = await webView2CoreFile.Properties.RetrievePropertiesAsync(propertyNameList);
                        dependencyInformationList.Add(new()
                        {
                            DisplayText = WebView2SDKVersionString,
                            SecondaryText = Convert.ToString((webView2CoreFileProperties[fileVersionProperty] is string fileVersionString ? new Version(fileVersionString) : new Version()))
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationDialog), nameof(GetDependencyInformationListAsync), 2, e);
                        dependencyInformationList.Add(new()
                        {
                            DisplayText = WebView2SDKVersionString,
                            SecondaryText = Convert.ToString(new Version())
                        });
                    }

                    // .NET 版本信息
                    dependencyInformationList.Add(new()
                    {
                        DisplayText = DoNetVersionString,
                        SecondaryText = Convert.ToString(Environment.Version)
                    });

                    return dependencyInformationList;
                });
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取应用信息要准备复制的字符串内容
        /// </summary>
        private async Task<List<string>> GetAppInformationCopyStringListAsync(List<ContentLinkInfo> appInformationList)
        {
            return await Task.Run(() =>
            {
                List<string> appInformationCopyStringList = [];

                foreach (ContentLinkInfo appInformation in AppInformationCollection)
                {
                    appInformationCopyStringList.Add(appInformation.DisplayText + appInformation.SecondaryText);
                }

                return appInformationCopyStringList;
            });
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}
