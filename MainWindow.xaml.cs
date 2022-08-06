using GetStoreApp.ViewModels.Window;
using System;
using System.IO;
using WinUIEx;

namespace GetStoreApp
{
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow()
        {
            ViewModel = App.GetService<MainWindowViewModel>();
            InitializeComponent();

            AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Logo/GetStoreApp.ico"));
            Content = null;
        }
    }
}
