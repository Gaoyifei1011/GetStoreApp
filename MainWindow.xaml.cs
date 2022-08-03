using System;
using System.IO;
using WinUIEx;

namespace GetStoreApp
{
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            InitializeComponent();

            AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Logo/GetStoreApp.ico"));
            Content = null;
        }
    }
}
