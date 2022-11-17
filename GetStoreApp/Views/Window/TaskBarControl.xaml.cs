using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.ViewModels.Window;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.Views.Window
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

            WeakReferenceMessenger.Default.Register<TaskBarControl, WindowClosedMessage>(this, (taskbarControl, windowClosedMessage) =>
            {
                if (windowClosedMessage.Value)
                {
                    Dispose();
                    WeakReferenceMessenger.Default.UnregisterAll(this);
                }
            });
        }
    }
}
