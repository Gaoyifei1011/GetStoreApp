using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Conversions
{
    /// <summary>
    /// 安装文件按钮显示值转换器
    /// </summary>
    public class InstallFileVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            string result = System.Convert.ToString(value);

            if (result.EndsWith(".appx", StringComparison.OrdinalIgnoreCase) ||
                result.EndsWith(".msix", StringComparison.OrdinalIgnoreCase) ||
                result.EndsWith(".appxbundle", StringComparison.OrdinalIgnoreCase) ||
                result.EndsWith(".msixbundle", StringComparison.OrdinalIgnoreCase))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
