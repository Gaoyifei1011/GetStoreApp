using GetStoreApp.Contracts.Controls.Settings.Common;
using GetStoreApp.Contracts.Root;
using GetStoreApp.Contracts.Window;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Window;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : WASDKWindow
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public INotificationService NotificationService { get; } = ContainerHelper.GetInstance<INotificationService>();

        public INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        public MainWindowViewModel ViewModel { get; } = ContainerHelper.GetInstance<MainWindowViewModel>();

        public MainWindow()
        {
            InitializeComponent();
            NavigationService.NavigationFrame = WindowFrame;
        }
    }
}
