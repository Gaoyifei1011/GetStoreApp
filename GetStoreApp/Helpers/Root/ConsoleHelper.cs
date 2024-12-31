using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 表示控制台应用程序的标准输入流、输出流和错误流
    /// </summary>
    public static class ConsoleHelper
    {
        public static bool IsExited { get; set; }

        /// <summary>
        /// 将控制台的前景色设置为默认值。
        /// </summary>
        public static int ResetTextColor()
        {
            IntPtr consoleHandle = Kernel32Library.GetStdHandle(STD_HANDLE.STD_OUTPUT_HANDLE);
            return Kernel32Library.SetConsoleTextAttribute(consoleHandle, 0x0F);
        }

        /// <summary>
        /// 设置控制台的前景色
        /// </summary>
        public static int SetTextColor(ushort color)
        {
            IntPtr consoleHandle = Kernel32Library.GetStdHandle(STD_HANDLE.STD_OUTPUT_HANDLE);
            return Kernel32Library.SetConsoleTextAttribute(consoleHandle, color);
        }

        /// <summary>
        /// 设置控制台的标题
        /// </summary>
        public static bool SetTitle(string title)
        {
            return Kernel32Library.SetConsoleTitle(title);
        }

        /// <summary>
        /// 从标准输入流读取下一行字符。
        /// </summary>
        public static string ReadLine()
        {
            IntPtr consoleHandle = Kernel32Library.GetStdHandle(STD_HANDLE.STD_INPUT_HANDLE);

            const int bufferSize = 1024;
            char[] buffer = new char[bufferSize];
            Kernel32Library.ReadConsole(consoleHandle, buffer, bufferSize, out int charsRead, IntPtr.Zero);
            return new string(buffer)[..(charsRead - Environment.NewLine.Length)];
        }

        /// <summary>
        /// 将指定值的文本表示形式写入标准输出流。
        /// </summary>
        public static bool Write(string value)
        {
            if (!IsExited)
            {
                IntPtr consoleHandle = Kernel32Library.GetStdHandle(STD_HANDLE.STD_OUTPUT_HANDLE);
                return Kernel32Library.WriteConsole(consoleHandle, value, value.Length, out _, IntPtr.Zero);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 将指定的数据（后跟当前行终止符）写入标准输出流。
        /// </summary>
        public static bool WriteLine(string value)
        {
            if (!IsExited)
            {
                value += Environment.NewLine;
                IntPtr consoleHandle = Kernel32Library.GetStdHandle(STD_HANDLE.STD_OUTPUT_HANDLE);
                return Kernel32Library.WriteConsole(consoleHandle, value, value.Length, out _, IntPtr.Zero);
            }
            else
            {
                return false;
            }
        }
    }
}
