using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Formats
{
    public class TimeStampFormatConverter : IValueConverter
    {
        /// <summary>
        /// UTC标准时间戳与当地地区时间转换器
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return string.Empty;
            }

            long RawDataTime = System.Convert.ToInt64(value);

            DateTime EpochStartTime = new DateTime(1970, 1, 1, 0, 0, 0);

            TimeSpan UtcOffset = DateTime.Now - DateTime.UtcNow;

            DateTime CurrentTime = EpochStartTime.AddSeconds(RawDataTime + UtcOffset.TotalSeconds);

            return CurrentTime.ToString("F");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
