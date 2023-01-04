using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Conversions
{
    public class BooleanToOppositeValueConverter : IValueConverter
    {
        /// <summary>
        /// 布尔值取反转换器
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
            {
                return false;
            }

            return !System.Convert.ToBoolean(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
