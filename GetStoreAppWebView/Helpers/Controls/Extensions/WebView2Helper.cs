using GetStoreAppWebView.WindowsAPI.PInvoke.User32;
using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;

namespace GetStoreAppWebView.Helpers.Controls.Extensions
{
    /// <summary>
    /// 这些实用程序将 WinRT 事件参数转换为 Win32 事件参数，并发送给 WebView
    /// 参考 winuser.h 输入的代码：https://docs.microsoft.com/en-us/windows/desktop/api/winuser/
    /// </summary>
    public static class WebView2Helper
    {
        public static short GetWheelDataWParam(UIntPtr wParam)
        {
            return (short)HiWord(wParam);
        }

        public static short GetXButtonWParam(UIntPtr wParam)
        {
            return (short)HiWord(wParam);
        }

        public static short GetKeystateWParam(UIntPtr wParam)
        {
            return (short)LoWord(wParam);
        }

        public static long LoWord(IntPtr Number)
        {
            return checked((int)Number & 0xffff);
        }

        public static long LoWord(UIntPtr Number)
        {
            return checked((uint)Number & 0xffff);
        }

        public static long HiWord(IntPtr Number)
        {
            return checked(((int)Number >> 16) & 0xffff);
        }

        public static long HiWord(UIntPtr Number)
        {
            return checked(((uint)Number >> 16) & 0xffff);
        }

        public static uint MakeWParam(ushort low, ushort high)
        {
            return ((uint)high << 16) | low;
        }

        public static int MakeLParam(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xffff);
        }

        public static IntPtr PackIntoWin32StylePointerArgs_lparam(Point point)
        {
            // 这些对于基于 WM_POINTER 和 WM_MOUSE 的事件是相同的
            // Pointer: https://msdn.microsoft.com/en-us/ie/hh454929(v=vs.80)
            // Mouse: https://docs.microsoft.com/en-us/windows/desktop/inputdev/wm-mousemove
            IntPtr lParam = new(MakeLParam((int)point.X, (int)point.Y));
            return lParam;
        }

        public static UIntPtr PackIntoWin32StyleMouseArgs_wparam(WindowMessage message, PointerRoutedEventArgs args, PointerPoint pointerPoint)
        {
            ushort lowWord = 0x0;
            ushort highWord = 0x0;

            VirtualKeyModifiers modifiers = args.KeyModifiers;

            // 可以支持像 Ctrl|Alt + Scroll 这样的情况，Alt 将被忽略，它将被视为 Ctrl + Scroll
            if (((int)modifiers & (int)VirtualKeyModifiers.Control) is not 0)
            {
                lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_CONTROL;
            }
            if (((int)modifiers & (int)VirtualKeyModifiers.Shift) is not 0)
            {
                lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_SHIFT;
            }

            PointerPointProperties properties = pointerPoint.Properties;

            if (properties.IsLeftButtonPressed)
            {
                lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_LBUTTON;
            }
            if (properties.IsRightButtonPressed)
            {
                lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_RBUTTON;
            }
            if (properties.IsMiddleButtonPressed)
            {
                lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_MBUTTON;
            }
            if (properties.IsXButton1Pressed)
            {
                lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_XBUTTON1;
            }
            if (properties.IsXButton2Pressed)
            {
                lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_XBUTTON2;
            }

            // 鼠标滚动消息 : https://docs.microsoft.com/en-us/windows/desktop/inputdev/wm-mousewheel
            if (message is WindowMessage.WM_MOUSEWHEEL or WindowMessage.WM_MOUSEHWHEEL)
            {
                highWord = (ushort)properties.MouseWheelDelta;
            }
            else if (message is WindowMessage.WM_XBUTTONDOWN or WindowMessage.WM_XBUTTONUP)
            {
                PointerUpdateKind pointerUpdateKind = properties.PointerUpdateKind;
                if (pointerUpdateKind is PointerUpdateKind.XButton1Pressed ||
                    pointerUpdateKind is PointerUpdateKind.XButton1Released)
                {
                    highWord |= (ushort)MOUSEHOOKSTRUCTEX_MOUSE_DATA.XBUTTON1;
                }
                else if (pointerUpdateKind is PointerUpdateKind.XButton2Pressed ||
                         pointerUpdateKind is PointerUpdateKind.XButton2Released)
                {
                    highWord |= (ushort)MOUSEHOOKSTRUCTEX_MOUSE_DATA.XBUTTON2;
                }
            }

            UIntPtr wParam = new(MakeWParam(lowWord, highWord));
            return wParam;
        }
    }
}
