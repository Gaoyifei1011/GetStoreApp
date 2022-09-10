using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Conversions
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 布尔值与控件显示值转换器
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            bool result = System.Convert.ToBoolean(value);
            string param = System.Convert.ToString(parameter);

            if (!string.IsNullOrEmpty(param) && param == "Reverse")
            {
                return result == false ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return result == true ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
