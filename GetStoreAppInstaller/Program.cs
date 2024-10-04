using Mile.Xaml;
using System;
using System.Windows.Forms;

namespace GetStoreAppInstaller
{
    /// <summary>
    /// 获取商店应用 应用安装器
    /// </summary>
    public class Program
    {
        public static Form MainForm { get; private set; }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            XamlIslandsApp app = new();

            MainForm = new Form1();
            Application.Run(MainForm);

            app.Close();
        }
    }

    public partial class Form1 : Form
    {
        private readonly WindowsXamlHost xamlHost = new();

        public Form1()
        {
            CenterToScreen();
            Controls.Add(xamlHost);
            xamlHost.AutoSize = true;
            xamlHost.Dock = DockStyle.Fill;
            xamlHost.Child = new MainPage();
        }
    }
}
