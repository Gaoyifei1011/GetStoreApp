using System;

// 获取商店应用辅助程序，创建任务栏菜单图标，使用Mile Xaml(Xaml Islands)实现任务栏右键菜单外观现代化，为获取商店应用程序提供模态对话框支持，该部分未来可能会采用C++开发。（尚未进行）
namespace GetStoreAppHelper
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstanceManager app = new SingleInstanceManager();
            app.Run(args);
            app.Dispose();
        }
    }
}
