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
    public class StatImageConverter : IValueConverter
    {
        private readonly BitmapImage bitmapImage = new BitmapImage();

        /// <summary>
        /// 将后台MainStatImageMode的Enum值转换成Image图片源对应的位置路径
        /// Converts the Enum value of the background StateImageMode to the location path corresponding to the Image image source
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

            StateImageMode? result = value as StateImageMode?;

            switch (result)
            {
                case StateImageMode.Error:
                    bitmapImage.UriSource = new("ms-appx:///Assets/Logo/Error.png");
                    break;

                case StateImageMode.Warning:
                    bitmapImage.UriSource = new("ms-appx:///Assets/Logo/Warning.png");
                    break;

                case StateImageMode.Notification:
                    bitmapImage.UriSource = new("ms-appx:///Assets/Logo/Notification.png");
                    break;

                case StateImageMode.Success:
                    bitmapImage.UriSource = new("ms-appx:///Assets/Logo/Success.png");
                    break;

                default:
                    bitmapImage.UriSource = new("ms-appx:///Assets/Logo/Error.png");
                    break;
            }

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}