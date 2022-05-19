using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GetStoreApp.Converters.General
{
    /// <summary>
    /// 将布尔值转换为控件显示状态的值
    /// Converts a boolean value to a value that displays state for the control
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将布尔值转换为控件显示状态的值
        /// Converts a Boolean value to a value that displays state for the control
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

            bool? result = value as bool?;
            return result == true ? Visibility.Visible : (object)Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
