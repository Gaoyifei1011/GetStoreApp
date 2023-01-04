using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Conversions
{
    /// <summary>
    /// 下载状态值显示对应内容转换器
    /// </summary>
    public class DownloadFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
            {
                return Visibility.Collapsed;
            }

            int result = System.Convert.ToInt32(value);
            int param = System.Convert.ToInt32(parameter);

            if (result == param)
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
            return default;
        }
    }
}
