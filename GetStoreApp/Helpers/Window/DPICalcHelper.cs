using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;

namespace GetStoreApp.Helpers.Window
{
    /// <summary>
    /// DPI（每英寸点数）缩放计算辅助类
    /// </summary>
    public static class DPICalcHelper
    {
        /// <summary>
        /// 有效像素值转换为实际的像素值
        /// </summary>
        public static int ConvertEpxToPixel(IntPtr hwnd, int effectivePixels)
        {
            float scalingFactor = GetScalingFactor(hwnd);
            return Convert.ToInt32(effectivePixels * scalingFactor);
        }

        /// <summary>
        /// 实际的像素值转换为有效像素值
        /// </summary>
        public static int ConvertPixelToEpx(IntPtr hwnd, int pixels)
        {
            float scalingFactor = GetScalingFactor(hwnd);
            return (int)(pixels / scalingFactor);
        }

        /// <summary>
        /// 获取实际的系统缩放比例
        /// </summary>
        private static float GetScalingFactor(IntPtr hwnd)
        {
            int dpi = User32Library.GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            return scalingFactor;
        }
    }
}
