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
        public Dictionary<string, int> SizeDict = new Dictionary<string, int>()
        {
            { "GB",1024*1024*1024 },
            { "MB",1024*1024 },
            { "KB",1024 }
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            double result = System.Convert.ToDouble(value);

            if (result / SizeDict["GB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(System.Convert.ToDouble(result) / SizeDict["GB"], 2), "GB");
            }
            else if (result / SizeDict["MB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(System.Convert.ToDouble(result) / SizeDict["MB"], 2), "MB");
            }
            else if (result / SizeDict["KB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(System.Convert.ToDouble(result) / SizeDict["KB"], 2), "KB");
            }
            else
            {
                return string.Format("{0}{1}", result, "B");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
