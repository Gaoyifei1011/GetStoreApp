using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GetStoreApp.Converters.General
{
    /// <summary>
    /// 将整数值转换为控件显示状态的值
    /// Converts an integer value to a value for the display state of the control
    /// </summary>
    public class IntToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将整数值转换为控件显示状态的值（整数值为0时隐藏，不为0时显示）
        /// Converts an integer value to a value for the control's display state (displayed when the integer value is 0, hidden when not 0)
        /// </summary>
        /// <param name="value">绑定源生成的值</param>
        /// <param name="targetType">绑定目标属性的类型</param>
        /// <param name="parameter">要使用的转换器参数</param>
        /// <param name="language">要用在转换器中的区域性</param>
        /// <returns>转换后的值</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            int? result = value as int?;
            return result != 0 ? Visibility.Visible : (object)Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
