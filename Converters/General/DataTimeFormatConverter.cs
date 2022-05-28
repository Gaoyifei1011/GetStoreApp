using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.General
{
    /// <summary>
    /// DataTime类型时间格式化转换器
    /// DataTime type time formatting converter
    /// </summary>
    public class DataTimeFormatConverter : IValueConverter
    {
        /// <summary>
        /// 将字符串中包含的时间转换为DataTime类型值
        /// Converts the time contained in the string to a DataTime type value
        /// </summary>
        /// <returns>转换完之后的DataTime类型的值</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            string RawDataTime = value.ToString();

            DateTime dt = System.Convert.ToDateTime(RawDataTime).ToLocalTime();

            return dt.ToString("F");
        }

        /// <summary>
        /// 将转换为DataTime类型值转换为字符串（单向转换，无需实现）
        /// Converting a value of type DataTime to a string (one-way conversion, no implementation required)
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}