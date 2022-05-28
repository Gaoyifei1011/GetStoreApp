using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.General
{
    /// <summary>
    /// 布尔值转与Visability值的转换器
    /// A converter for Boolean values to convert versus Visability values
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将布尔值转换为Visability值
        /// Converts the Boolean value to the Visability value
        /// </summary>
        /// <returns>转换完之后的Visability的值</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            bool? result = value as bool?;
            return result == true ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 将转换为Visability值转换为布尔值（单向转换，无需实现）
        /// Converting a Converted Visability Value to a Boolean Value (One-Way Conversion, No Implementation Required)
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}