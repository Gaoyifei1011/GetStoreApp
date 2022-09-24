using System;
using System.Diagnostics;

namespace GetStoreAppConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 初始化控制台程序的本地化资源内容
            ConsoleHelper.InitializeLocalization();

            // 初始化控制台程序的信息显示
            ConsoleHelper.InitializeDescription();

            Process.Start("getstoreapp://");

            // 退出应用程序
            Environment.Exit(0);
        }
    }
}
