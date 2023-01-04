using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Checks
{
    /// <summary>
    /// 历史记录过滤单选框检查绑定转换器
    /// </summary>
    public class FilterValueCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null || parameter is null)
            {
                return string.Empty;
            }

            return System.Convert.ToString(value) == System.Convert.ToString(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
