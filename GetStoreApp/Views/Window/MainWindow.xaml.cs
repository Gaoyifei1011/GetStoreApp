using GetStoreApp.Services.Window;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : WASDKWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigationService.NavigationFrame = WindowFrame;
        }
    }
}
