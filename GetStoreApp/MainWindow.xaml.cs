using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Window;
using GetStoreApp.WindowExtensions;
using System;
using System.IO;

namespace GetStoreApp
{
    public sealed partial class MainWindow : DesktopWindow
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow()
        {
            ViewModel = IOCHelper.GetService<MainWindowViewModel>();
            InitializeComponent();

            AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Logo/GetStoreApp.ico"));
            Content = null;
        }
    }
}
