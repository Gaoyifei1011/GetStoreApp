using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Conversions
{
    public class AppTitleBarMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return new Thickness(0);
            }

            if (((NavigationViewPaneDisplayMode)value == NavigationViewPaneDisplayMode.LeftMinimal))
            {
                return new Thickness(96, 0, 0, 0);
            }
            else
            {
                return new Thickness(48, 0, 0, 0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
