using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters
{
    public class DataTimeFormatConverter : IValueConverter
    {
        /// <summary>
        /// 将字符串中包含的时间转换为DataTime类型值
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            string RawDataTime = value.ToString();

            DateTime dt = System.Convert.ToDateTime(RawDataTime).ToLocalTime();

            return dt.ToString("F");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
