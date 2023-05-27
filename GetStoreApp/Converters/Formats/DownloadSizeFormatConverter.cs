using GetStoreApp.Helpers.Root;
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
        public Dictionary<string, int> SizeDict = new Dictionary<string, int>()
        {
            { "GB",1024*1024*1024 },
            { "MB",1024*1024 },
            { "KB",1024 }
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
            {
                return string.Empty;
            }

            double result = System.Convert.ToDouble(value);
            return FileSizeHelper.ConvertFileSizeToString(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
