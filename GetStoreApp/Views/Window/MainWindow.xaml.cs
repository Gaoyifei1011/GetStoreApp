using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Window;
using WinUIEx;

namespace GetStoreApp
{
    public sealed partial class MainWindow : WindowEx
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public MainWindowViewModel ViewModel { get; } = ContainerHelper.GetInstance<MainWindowViewModel>();

        public MainWindow()
        {
            Content = null;
            ExtendsContentIntoTitleBar = true;
            InitializeComponent();
        }
    }
}
