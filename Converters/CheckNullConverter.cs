using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters
{
    public class CheckNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is not null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
