using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Checks
{
    /// <summary>
    /// 历史记录按时间排序单选框检查绑定转换器
    /// </summary>
    public class TimeSortValueCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return DependencyProperty.UnsetValue;

            return System.Convert.ToBoolean(value) == System.Convert.ToBoolean(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
