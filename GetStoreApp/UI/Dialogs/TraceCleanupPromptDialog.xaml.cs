using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class TraceCleanupPromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public TraceCleanupPromptViewModel ViewModel { get; } = IOCHelper.GetService<TraceCleanupPromptViewModel>();

        public TraceCleanupPromptDialog()
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            InitializeComponent();
        }

        /// <summary>
        /// 确定清理按钮的可用状态
        /// </summary>
        public bool TraceCleanupSureIsEnabled(bool isCleaning, bool isHistoryClean, bool isDownloadClean, bool isLocalFileClean)
        {
            if (isCleaning)
            {
                if (isHistoryClean || isDownloadClean || isLocalFileClean)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string LocalizeClearStateText(string clearStateText)
        {
            return ResourceService.GetLocalized(string.Format("/Dialog/{0}", clearStateText));
        }
    }
}
