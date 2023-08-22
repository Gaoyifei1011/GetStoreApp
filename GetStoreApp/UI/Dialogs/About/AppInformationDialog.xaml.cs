using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace GetStoreApp.UI.Dialogs.About
{
    /// <summary>
    /// 应用信息对话框
    /// </summary>
    public sealed partial class AppInformationDialog : ContentDialog, INotifyPropertyChanged
    {
        private string FileVersionProperty { get; } = "System.FileVersion";

        private List<string> PropertyNamesList => new List<string> { FileVersionProperty };

        private string _windowsAppSDKVersion;

        public string WindowsAppSDKVersion
        {
            get { return _windowsAppSDKVersion; }

            set
            {
                _windowsAppSDKVersion = value;
                OnPropertyChanged();
            }
        }

        private string _winUI3Version;

        public string WinUI3Version
        {
            get { return _winUI3Version; }

            set
            {
                _winUI3Version = value;
                OnPropertyChanged();
            }
        }

        private string _doNetVersion;

        public string DoNetVersion
        {
            get { return _doNetVersion; }

            set
            {
                _doNetVersion = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInformationDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        public void OnCopyAppInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            Task.Run(() =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/WindowsAppSDKVersion") + WindowsAppSDKVersion);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/WinUI3Version") + WinUI3Version);
                stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/DoNetVersion") + DoNetVersion);

                DispatcherQueue.TryEnqueue(() =>
                {
                    CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
                    sender.Hide();
                    new AppInformationCopyNotification(this).Show();
                });
            });
        }

        /// <summary>
        /// 初始化应用信息
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                IReadOnlyList<Package> DependencyList = Package.Current.Dependencies;

                foreach (Package dependency in DependencyList)
                {
                    if (dependency.DisplayName.Contains("WindowsAppRuntime"))
                    {
                        // Windows 应用 SDK 版本信息
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WindowsAppSDKVersion = string.Format("{0}.{1}.{2}.{3}",
                                dependency.Id.Version.Major,
                                dependency.Id.Version.Minor,
                                dependency.Id.Version.Build,
                                dependency.Id.Version.Revision);
                        });

                        // WinUI3 版本信息
                        try
                        {
                            StorageFile WinUI3File = await StorageFile.GetFileFromPathAsync(string.Format(@"{0}\{1}", dependency.InstalledLocation.Path, "Microsoft.ui.xaml.Controls.dll"));
                            IDictionary<string, object> WinUI3FileProperties = await WinUI3File.Properties.RetrievePropertiesAsync(PropertyNamesList);
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                WinUI3Version = WinUI3FileProperties[FileVersionProperty] is not null ? Convert.ToString(WinUI3FileProperties[FileVersionProperty]) : string.Empty;
                            });
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogType.WARNING, "Get WinUI3 version failed.", e);
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                WinUI3Version = string.Empty;
                            });
                        }
                    }
                }
            });

            DoNetVersion = Environment.Version.ToString();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
