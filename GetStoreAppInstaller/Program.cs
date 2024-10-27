using GetStoreAppInstaller.Views.Windows;
using System;
using System.Windows.Forms;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreAppInstaller
{
    /// <summary>
    /// 获取商店应用 应用安装器
    /// </summary>
    public partial class Program
    {
        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();
            new XamlIslandsApp();
            Application.Run(new MainWindow());
        }
    }
}
