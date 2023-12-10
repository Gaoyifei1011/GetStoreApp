using Microsoft.UI.Xaml;
using System;
using System.IO;
using TaskbarPinner.Services.Root;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Shell;

namespace TaskbarPinner
{
    /// <summary>
    /// 任务栏固定辅助程序
    /// </summary>
    public sealed partial class App : Application
    {
        private string tempFilePath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "GetStoreAppTemp.txt");

        public CoreApplicationView CoreApplicationView { get; } = CoreApplication.GetCurrentView();

        public App()
        {
            InitializeComponent();
            CoreApplicationView.Activated += OnActivated;
        }

        /// <summary>
        /// 在通过常规启动之外的某种方式激活应用程序时调用，初始化应用内容
        /// </summary>
        private async void OnActivated(CoreApplicationView sender, IActivatedEventArgs args)
        {
            if (args.Kind is ActivationKind.Protocol && File.Exists(tempFilePath))
            {
                try
                {
                    string[] contents = File.ReadAllLines(tempFilePath);

                    PackageManager packageManager = new PackageManager();
                    Package package = packageManager.FindPackageForUser(string.Empty, contents[0]);

                    if (package is not null)
                    {
                        foreach (AppListEntry applistItem in package.GetAppListEntries())
                        {
                            if (applistItem.AppUserModelId.Equals(contents[1]))
                            {
                                bool pinResult = await TaskbarManager.GetDefault().RequestPinAppListEntryAsync(applistItem);

                                if (!pinResult)
                                {
                                    string content = string.Format("{0}\n{1}\n{2}", ResourceService.GetLocalized("Taskbar/TaskbarPinFailedContent1"), ResourceService.GetLocalized("Taskbar/TaskbarPinFailedContent2"), ResourceService.GetLocalized("Taskbar/TaskbarPinFailedContent3"));
                                    MessageDialog messageDialog = new MessageDialog(content, ResourceService.GetLocalized("Taskbar/TaskbarPinFailed"));
                                    messageDialog.Commands.Add(new UICommand(ResourceService.GetLocalized("Taskbar/Exit"), (command) =>
                                    {
                                        Exit();
                                    }));
                                    messageDialog.DefaultCommandIndex = 0;
                                    await messageDialog.ShowAsync();
                                }
                            }
                        }
                    }

                    File.Delete(tempFilePath);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Pin app to taskbar failed", e);
                }
            }

            Exit();
        }
    }
}
