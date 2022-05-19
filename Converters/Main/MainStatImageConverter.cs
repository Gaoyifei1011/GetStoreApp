using GetStoreApp.Behaviors;

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace GetStoreApp.Converters.Main
{
    /// <summary>
    /// 将后台MainStatImageMode的Enum值转换成Image图片源对应的位置路径
    /// Converts the Enum value of the background MainStatImageMode to the location path corresponding to the Image image source
    /// </summary>
    public class MainStatImageConverter : IValueConverter
    {
        private readonly BitmapImage bitmapImage = new BitmapImage();

        /// <summary>
        /// 将后台MainStatImageMode的Enum值转换成Image图片源对应的位置路径
        /// Converts the Enum value of the background MainStatImageMode to the location path corresponding to the Image image source
        /// </summary>
        /// <param name="value">绑定源生成的值</param>
        /// <param name="targetType">绑定目标属性的类型</param>
        /// <param name="parameter">要使用的转换器参数</param>
        /// <param name="language">要用在转换器中的区域性</param>
        /// <returns>转换后的值</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            MainStatImageMode? result = value as MainStatImageMode?;
            if (result == MainStatImageMode.Error)
            {
                Uri uri = new Uri("ms-appx:///Assets/Logo/Error.png");
                bitmapImage.UriSource = uri;
            }
            else if (result == MainStatImageMode.Warning)
            {
                Uri uri = new Uri("ms-appx:///Assets/Logo/Warning.png");
                bitmapImage.UriSource = uri;
            }
            else if (result == MainStatImageMode.Notification)
            {
                Uri uri = new Uri("ms-appx:///Assets/Logo/Notification.png");
                bitmapImage.UriSource = uri;
            }
            else if (result == MainStatImageMode.Success)
            {
                Uri uri = new Uri("ms-appx:///Assets/Logo/Success.png");
                bitmapImage.UriSource = uri;
            }
            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
