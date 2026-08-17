using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.Helpers.Converters
{
    /// <summary>
    /// 值类型 / 内容转换辅助类
    /// </summary>
    internal static class ValueConverterHelper
    {
        /// <summary>
        /// 计算当前文件的下载进度
        /// </summary>
        internal static double DownloadProgress(double finishedSize, double totalSize)
        {
            return totalSize is 0 ? 0 : Math.Round(finishedSize / totalSize * 100, 2);
        }

        /// <summary>
        /// 整数值与控件显示值转换
        /// </summary>
        internal static Visibility IntToVisibilityConvert(int value)
        {
            return value is not 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 整数值与控件显示值转换（判断结果相反）
        /// </summary>
        internal static Visibility IntToVisibilityReverseConvert(int value)
        {
            return value is 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 布尔值与控件显示值转换（判断结果相反）
        /// </summary>
        internal static Visibility BooleanToVisibilityReverseConvert(bool value)
        {
            return value ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
