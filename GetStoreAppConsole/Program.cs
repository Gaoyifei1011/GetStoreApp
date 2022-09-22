using System;
using System.Diagnostics;
using Windows.Storage;

namespace GetStoreAppConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "获取商店应用控制台程序";
            ConsoleWindow.HideConsoleWindow(Console.Title);
            //Console.WriteLine("GetStoreApp控制台程序目前仅供测试使用。");
            Process.Start("getstoreappdesktop://");
        }
    }
}
