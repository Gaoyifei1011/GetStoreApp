using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;
using static PInvoke.User32;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 标题栏颜色设置
    /// </summary>
    public class TitleBarHelper
    {
        private const int WA_INACTIVE = 0x00;
        private const int WA_ACTIVE = 0x01;

        public static void UpdateTitleBar(ElementTheme theme)
        {
            if (App.MainWindow.ExtendsContentIntoTitleBar)
            {
                if (theme != ElementTheme.Default)
                {
                    Application.Current.Resources["WindowCaptionForeground"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Colors.White),
                        ElementTheme.Light => new SolidColorBrush(Colors.Black),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };

                    Application.Current.Resources["WindowCaptionForegroundDisabled"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF)),
                        ElementTheme.Light => new SolidColorBrush(Color.FromArgb(0x66, 0x00, 0x00, 0x00)),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };

                    Application.Current.Resources["WindowCaptionButtonBackgroundPointerOver"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF)),
                        ElementTheme.Light => new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0x00, 0x00)),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };

                    Application.Current.Resources["WindowCaptionButtonBackgroundPressed"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF)),
                        ElementTheme.Light => new SolidColorBrush(Color.FromArgb(0x66, 0x00, 0x00, 0x00)),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };

                    Application.Current.Resources["WindowCaptionButtonStrokePointerOver"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Colors.White),
                        ElementTheme.Light => new SolidColorBrush(Colors.Black),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };

                    Application.Current.Resources["WindowCaptionButtonStrokePressed"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Colors.White),
                        ElementTheme.Light => new SolidColorBrush(Colors.Black),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };
                }

                Application.Current.Resources["WindowCaptionBackground"] = new SolidColorBrush(Colors.Transparent);
                Application.Current.Resources["WindowCaptionBackgroundDisabled"] = new SolidColorBrush(Colors.Transparent);

                IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                if (hwnd == GetActiveWindow())
                {
                    SendMessage(hwnd, WindowMessage.WM_ACTIVATE,(IntPtr)WA_INACTIVE,IntPtr.Zero);
                    SendMessage(hwnd, WindowMessage.WM_ACTIVATE, (IntPtr)WA_ACTIVE, IntPtr.Zero);
                }
                else
                {
                    SendMessage(hwnd, WindowMessage.WM_ACTIVATE, (IntPtr)WA_ACTIVE, IntPtr.Zero);
                    SendMessage(hwnd, WindowMessage.WM_ACTIVATE, (IntPtr)WA_INACTIVE, IntPtr.Zero);
                }
            }
        }
    }
}
