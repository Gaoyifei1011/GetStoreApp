using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Converters.Formats
{
    public class DownloadSpeedFormatConverter : IValueConverter
    {
        public List<string> SpeedUnitsList = new List<string>() { "Byte/s","KB/s", "MB/s" };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            int? result = value as int?;

            return string.Format("{0}{1}", result, SpeedUnitsList[1]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
