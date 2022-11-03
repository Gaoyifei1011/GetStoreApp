using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.ViewModels.Shell;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.Views.Shell
{
    public sealed partial class TaskBarControl : TaskbarIcon
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        public TaskBarViewModel ViewModel { get; } = ContainerHelper.GetInstance<TaskBarViewModel>();

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

        // 控件被卸载时，关闭消息服务
        public void TaskBarUnloaded(object sender, RoutedEventArgs args)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
