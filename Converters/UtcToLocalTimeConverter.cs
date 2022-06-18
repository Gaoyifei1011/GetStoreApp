using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters
{
    public class UtcToLocalTimeConverter : IValueConverter
    {
        /// <summary>
        /// 将数据库存储的long类型的UTC标准时间戳转换为当地地区时间
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return DependencyProperty.UnsetValue;

            long RawDataTime = System.Convert.ToInt64(value);

            DateTime EpochStartTime = new DateTime(1970, 1, 1, 0, 0, 0);

            TimeSpan UtcOffset = DateTime.Now - DateTime.UtcNow;

            DateTime CurrentTime = EpochStartTime.AddSeconds(RawDataTime + UtcOffset.TotalSeconds);

            return CurrentTime.ToString("F");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
