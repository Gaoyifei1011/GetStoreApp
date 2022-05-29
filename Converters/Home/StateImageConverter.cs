using GetStoreApp.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace GetStoreApp.Converters.Home
{
    /// <summary>
    /// 将后台MainStatImageMode的Enum值转换成Image图片源对应的位置路径
    /// Converts the Enum value of the background StateImageMode to the location path corresponding to the Image image source
    /// </summary>
    public class StateImageConverter : IValueConverter
    {
        private readonly BitmapImage bitmapImage = new();

        /// <summary>
        /// 将后台MainStatImageMode的Enum值转换成Image图片源对应的位置路径
        /// Converts the Enum value of the background StateImageMode to the location path corresponding to the Image image source
        /// </summary>
        /// <returns>转换后的图片路径</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            StateImageMode? result = value as StateImageMode?;

            bitmapImage.UriSource = result switch
            {
                StateImageMode.Error => new("ms-appx:///Assets/Logo/Error.png"),
                StateImageMode.Warning => new("ms-appx:///Assets/Logo/Warning.png"),
                StateImageMode.Notification => new("ms-appx:///Assets/Logo/Notification.png"),
                StateImageMode.Success => new("ms-appx:///Assets/Logo/Success.png"),
                _ => new("ms-appx:///Assets/Logo/Error.png"),
            };
            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
