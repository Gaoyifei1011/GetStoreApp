using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// Windows 应用 SDK 窗口的扩展类
    /// </summary>
    public class WinUIWindow : Microsoft.UI.Xaml.Window
    {
        /// <summary>
        /// 窗口标题
        /// </summary>
        public new string Title
        {
            get => base.Title;
            set => base.Title = value;
        }

        /// <summary>
        /// 窗口类参与数据绑定时的数据上下文
        /// </summary>
        public object DataContext { get; set; }

        /// <summary>
        /// 窗口最小宽度
        /// </summary>
        public int MinWidth { get; set; } = -1;

        /// <summary>
        /// 窗口最小高度
        /// </summary>
        public int MinHeight { get; set; } = -1;

        /// <summary>
        /// 窗口最大宽度
        /// </summary>
        public int MaxWidth { get; set; } = -1;

        /// <summary>
        /// 窗口最大高度
        /// </summary>
        public int MaxHeight { get; set; } = -1;

        /// <summary>
        /// 更改指定窗口的属性
        /// </summary>
        public int GetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex)
        {
            if (IntPtr.Size == 8)
            {
                return User32Library.GetWindowLongPtr(hWnd, nIndex);
            }
            else
            {
                return User32Library.GetWindowLong(hWnd, nIndex);
            }
        }

        /// <summary>
        /// 更改指定窗口的窗口过程
        /// </summary>
        public IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
            }
        }
    }
}
