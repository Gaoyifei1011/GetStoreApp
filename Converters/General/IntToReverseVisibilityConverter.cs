using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GetStoreApp.Converters.General
{
    /// <summary>
    /// 整数值转与Visability值的转换器（反过来）
    /// Integer value conversion with Visability value converter (in reverse)
    /// </summary>
    public class IntToReverseVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将整数值转换为Visability值
        /// Converts an integer value to a Visability value
        /// </summary>
        /// <returns>转换完之后的Visability的值</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            int? result = value as int?;
            return result == 0 ? Visibility.Visible : (object)Visibility.Collapsed;
        }

        /// <summary>
        /// 将转换为Visability值转换为整数值（单向转换，无需实现）
        /// Converting a Converted Visability Value to an integer Value (One-Way Conversion, No Implementation Required)
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
