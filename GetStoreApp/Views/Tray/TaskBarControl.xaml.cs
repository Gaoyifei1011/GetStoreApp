using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.ViewModels.Tray;
using H.NotifyIcon;
using System.Threading.Tasks;

namespace GetStoreApp.Views.Tray
{
    public sealed partial class TaskBarControl : TaskbarIcon
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public TaskBarViewModel ViewModel { get; } = IOCHelper.GetService<TaskBarViewModel>();

        public TaskBarControl()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<TaskBarControl, TrayIconDisposeMessage>(this, async (taskbarControl, trayIconDisposeMessage) =>
            {
                if (trayIconDisposeMessage.Value)
                {
                    Dispose();
                }
                await Task.CompletedTask;
            });
        }
    }
}
