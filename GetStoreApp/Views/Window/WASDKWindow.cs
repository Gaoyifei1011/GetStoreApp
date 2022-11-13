using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Extensions.DataType.Events;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Messages;
using GetStoreApp.UI.Dialogs.ContentDialogs.Common;
using GetStoreAppWindowsAPI.PInvoke.User32;
using Microsoft.UI.Dispatching;
using System;
using System.Runtime.InteropServices;
using WinUIEx;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 为WindowEx窗口添加正在关闭窗口事件
    /// </summary>
    public class WASDKWindow : WindowEx
    {
        private WinProc newWndProc = null;
        private IntPtr oldWndProc = IntPtr.Zero;

        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// 是否扩展内容到标题栏
        /// </summary>
        public new bool ExtendsContentIntoTitleBar
        {
            get => base.ExtendsContentIntoTitleBar;
            set => base.ExtendsContentIntoTitleBar = value;
        }

        /// <summary>
        /// 窗口是否关闭的标志
        /// </summary>
        public bool IsClosing { get; set; }

        /// <summary>
        /// 窗口句柄
        /// </summary>
        private IntPtr Hwnd { get; set; } = IntPtr.Zero;

        /// <summary>
        /// 窗口正在关闭事件
        /// </summary>
        public event EventHandler<WindowClosingEventArgs> Closing;

        public WASDKWindow()
        {
            // 获取窗口的句柄
            Hwnd = WindowExtensions.GetWindowHandle(this);
            if (Hwnd == IntPtr.Zero)
            {
                throw new NullReferenceException("The Window Handle is null.");
            }
            newWndProc = new WinProc(NewWindowProc);
            oldWndProc = SetWindowLongPtr(Hwnd, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
        }

        ~WASDKWindow()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        /// <summary>
        /// 更改指定窗口的属性
        /// </summary>
        private IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
        {
            if (IntPtr.Size == 8)
                return User32Library.SetWindowLongPtr64(hWnd, nIndex, newProc);
            else
                return User32Library.SetWindowLongPtr32(hWnd, nIndex, newProc);
        }

        /// <summary>
        /// 窗口消息处理
        /// </summary>
        private IntPtr NewWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 窗口关闭消息
                case WindowMessage.WM_CLOSE:
                    {
                        if (Closing is not null)
                        {
                            if (IsClosing == false)
                            {
                                WindowClosingEventArgs windowClosingEventArgs = new(this);
                                Closing.Invoke(this, windowClosingEventArgs);
                            }
                            return IntPtr.Zero;
                        }
                        break;
                    }
                // 系统设置发生更改时的消息
                case WindowMessage.WM_SETTINGCHANGE:
                    {
                        WeakReferenceMessenger.Default.Send(new SystemSettingsChnagedMessage(true));
                        break;
                    }
                // 窗口接受其他数据消息
                case WindowMessage.WM_COPYDATA:
                    {
                        CopyDataStruct copyDataStruct = (CopyDataStruct)Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));

                        // 没有任何命令参数，正常启动，应用可能被重复启动
                        if (copyDataStruct.dwData == 0)
                        {
                            WindowHelper.ShowAppWindow();

                            if (!App.IsDialogOpening)
                            {
                                App.IsDialogOpening = true;
                                dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
                                {
                                    await new AppRunningDialog().ShowAsync();
                                    App.IsDialogOpening = false;
                                });
                            }
                        }
                        // 获取应用的命令参数
                        else
                        {
                            string[] startupArgs = copyDataStruct.lpData.Split(' ');
                            WeakReferenceMessenger.Default.Send(new CommandMessage(startupArgs));
                        }
                        break;
                    }
            }
            return User32Library.CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }
    }
}
