using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Conversions
{
    /// <summary>
    /// 整数值与控件显示值转换器
    /// </summary>
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            int result = System.Convert.ToInt32(value);
            string param = System.Convert.ToString(parameter);

            if (!string.IsNullOrEmpty(param) && param == "Reverse")
            {
                return result == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return result != 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
