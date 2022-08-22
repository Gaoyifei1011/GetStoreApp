using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Conversions
{
    public class IntToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 整数值与控件显示值转换器
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            int? result = value as int?;
            return result != 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
