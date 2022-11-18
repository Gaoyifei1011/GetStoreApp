using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Services.Controls.Settings.Appearance;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.Views.Window
{
    public sealed partial class TaskBarControl : TaskbarIcon
    {
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
