using GetStoreApp.ViewModels.Base;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using System;
using System.Windows.Input;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// Windows 应用 SDK窗口的扩展类
    /// </summary>
    public class WASDKWindow : Microsoft.UI.Xaml.Window
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
        /// 是否扩展内容到标题栏
        /// </summary>
        public new bool ExtendsContentIntoTitleBar
        {
            get => base.ExtendsContentIntoTitleBar;
            set => base.ExtendsContentIntoTitleBar = value;
        }

        /// <summary>
        /// 窗口类参与数据绑定时的数据上下文
        /// </summary>
        public ViewModelBase DataContext { get; set; }

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
        /// 窗口是否关闭的标志
        /// </summary>
        public bool IsClosing { get; set; }

        /// <summary>
        /// 窗口句柄
        /// </summary>
        private IntPtr Hwnd { get; set; } = IntPtr.Zero;

        public ICommand ActivatedCommand
        {
            get { return (ICommand)Content.GetValue(ActivatedCommandProperty); }

            set { Content.SetValue(ActivatedCommandProperty, value); }
        }

        public static readonly DependencyProperty ActivatedCommandProperty = DependencyProperty.Register("ActivatedCommand", typeof(ICommand), typeof(UIElement), new PropertyMetadata(null));

        public ICommand ClosedCommand
        {
            get { return (ICommand)Content.GetValue(ClosedCommandProperty); }

            set { Content.SetValue(ClosedCommandProperty, value); }
        }

        public static readonly DependencyProperty ClosedCommandProperty = DependencyProperty.Register("ClosedCommand", typeof(ICommand), typeof(UIElement), new PropertyMetadata(null));

        public WASDKWindow()
        {
            Activated += OnActivated;
            Closed += OnClosed;
        }

        ~WASDKWindow()
        {
            Activated -= OnActivated;
        }

        public void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            ActivatedCommand?.Execute(args);
        }

        public void OnClosed(object sender, WindowEventArgs args)
        {
            ClosedCommand?.Execute(args);
        }

        /// <summary>
        /// 更改指定窗口的属性
        /// </summary>
        public IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex, WindowProc newProc)
        {
            if (IntPtr.Size == 8)
            {
                return User32Library.SetWindowLongPtr(hWnd, nIndex, newProc);
            }
            else
            {
                return User32Library.SetWindowLong(hWnd, nIndex, newProc);
            }
        }
    }
}
