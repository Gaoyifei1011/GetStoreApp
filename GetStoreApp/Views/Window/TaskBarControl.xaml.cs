using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
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

            Messenger.Default.Register<bool>(this, MessageToken.WindowClosed, (windowClosedMessage) =>
            {
                if (windowClosedMessage)
                {
                    Dispose();
                    Messenger.Default.Unregister(this);
                }
            });
        }
    }
}
