using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Formats
{
    public class GMTFormatConverter : IValueConverter
    {
        /// <summary>
        /// GMT时间与当地地区时间转换器
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string RawDataTime = System.Convert.ToString(value);

            DateTime dt = System.Convert.ToDateTime(RawDataTime).ToLocalTime();

            return dt.ToString("F");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
