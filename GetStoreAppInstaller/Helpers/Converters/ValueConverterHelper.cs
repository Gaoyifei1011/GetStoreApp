using Microsoft.UI.Xaml;

namespace GetStoreAppInstaller.Helpers.Converters
{
    /// <summary>
    /// 值类型 / 内容转换辅助类
    /// </summary>
    public static class ValueConverterHelper
    {
        /// <summary>
        /// 检查字符串是否为空
        /// </summary>
        public static bool IsNotEmptyString(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 整数值与控件显示值转换
        /// </summary>
        public static Visibility IntToVisibilityConvert(int value)
        {
            return value is not 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 整数值与控件显示值转换（判断结果相反）
        /// </summary>
        public static Visibility IntToVisibilityReverseConvert(int value)
        {
            return value is 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 布尔值与控件显示值转换（判断结果相反）
        /// </summary>
        public static Visibility BooleanToVisibilityReverseConvert(bool value)
        {
            return value ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
