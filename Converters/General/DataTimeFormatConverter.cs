using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GetStoreApp.Converters.General
{
    public class DataTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            //string RawDataTime = value.ToString().Remove(value.ToString().Length - 4, 4);
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
