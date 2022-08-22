using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Converters.Formats
{
    /// <summary>
    /// 下载文件大小文字显示转换器
    /// </summary>
    public class DownloadSizeFormatConverter : IValueConverter
    {
        public List<string> SizeUnitsList = new List<string>()
        {
            "KB",
            "MB",
            "GB"
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            int? result = value as int?;

            return string.Format("{0}{1}", result, SizeUnitsList[1]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
