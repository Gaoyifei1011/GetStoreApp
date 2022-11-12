using GetStoreApp.Contracts.Services.Root;
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

        public MainWindowViewModel ViewModel { get; } = ContainerHelper.GetInstance<MainWindowViewModel>();

        public MainWindow()
        {
            Content = null;
            InitializeComponent();
        }
    }
}
