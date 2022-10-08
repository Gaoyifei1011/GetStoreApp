using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.ViewModels.Tray;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.Views.Tray
{
    public sealed partial class TaskBarControl : TaskbarIcon
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public TaskBarViewModel ViewModel { get; } = IOCHelper.GetService<TaskBarViewModel>();

        public ElementTheme TrayTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public TaskBarControl()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<TaskBarControl, TrayIconDisposeMessage>(this, (taskbarControl, trayIconDisposeMessage) =>
            {
                if (trayIconDisposeMessage.Value)
                {
                    Dispose();
                }
            });
        }
    }
}
