﻿using System;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using WinUIEx;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// 应用窗口设置
    /// </summary>
    public static class WindowHelper
    {
        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr hWnd);

        /// <summary>
        /// 显示窗口
        /// </summary>
        public static void ShowAppWindow()
        {
            // 将窗口置于前台前首先获取窗口句柄
            HWND hwnd = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

            // 判断窗口状态是否处于最大化状态，如果是，直接最大化窗口
            if (IsZoomed(hwnd))
            {
                App.MainWindow.Maximize();
            }

            // 其他状态下窗口还原显示状态
            else
            {
                // 还原窗口（如果最小化）时
                App.MainWindow.Restore();
            }

            // 将应用窗口设置到前台
            App.MainWindow.BringToFront();
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public static void HideAppWindow()
        {
            if (App.MainWindow.Visible)
            {
                App.MainWindow.Hide();
            }
        }

        /// <summary>
        /// 设置应用窗口置顶
        /// </summary>
        public static void SetAppTopMost(bool topMostValue)
        {
            App.MainWindow.IsAlwaysOnTop = topMostValue;
        }

        /// <summary>
        /// 关闭应用窗口
        /// </summary>
        public static void CloseWindow()
        {
            App.MainWindow.Close();
        }
    }
}