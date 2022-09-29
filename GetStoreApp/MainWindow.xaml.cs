using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.Window;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Window;

namespace GetStoreApp
{
    public sealed partial class MainWindow : DesktopWindow
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public MainWindowViewModel ViewModel { get; } = IOCHelper.GetService<MainWindowViewModel>();

        public MainWindow()
        {
            Content = null;
            ExtendsContentIntoTitleBar = true;
            InitializeComponent();
        }
    }
}
