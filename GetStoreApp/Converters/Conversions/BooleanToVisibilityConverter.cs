using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Conversions
{
    /// <summary>
    /// 布尔值与控件显示值转换器
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
            {
                return Visibility.Collapsed;
            }

            bool result = System.Convert.ToBoolean(value);
            string param = System.Convert.ToString(parameter);

            if (!string.IsNullOrEmpty(param) && param is "Reverse")
            {
                return result is false ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return result is true ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
