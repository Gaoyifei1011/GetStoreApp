using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Checks
{
    /// <summary>
    /// 安装文件按钮可用值转换器
    /// </summary>
    public class InstallFileCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }

            string result = System.Convert.ToString(value);

            if (result.EndsWith(".appx", StringComparison.OrdinalIgnoreCase) ||
                result.EndsWith(".msix", StringComparison.OrdinalIgnoreCase) ||
                result.EndsWith(".appxbundle", StringComparison.OrdinalIgnoreCase) ||
                result.EndsWith(".msixbundle", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
