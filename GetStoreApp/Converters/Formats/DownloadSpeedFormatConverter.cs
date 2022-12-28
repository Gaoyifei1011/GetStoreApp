using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Converters.Formats
{
    /// <summary>
    /// 下载速度文字显示转换器
    /// </summary>
    public class DownloadSpeedFormatConverter : IValueConverter
    {
        public Dictionary<string, int> SpeedDict = new Dictionary<string, int>()
        {
            { "GB/s",1024*1024*1024 },
            { "MB/s",1024*1024 },
            { "KB/s",1024 }
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return string.Empty;
            }

            double result = System.Convert.ToDouble(value);

            if (result / SpeedDict["GB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(System.Convert.ToDouble(result) / SpeedDict["GB/s"], 2), "GB");
            }
            else if (result / SpeedDict["MB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(System.Convert.ToDouble(result) / SpeedDict["MB/s"], 2), "MB");
            }
            else if (result / SpeedDict["KB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(System.Convert.ToDouble(result) / SpeedDict["KB/s"], 2), "KB");
            }
            else
            {
                return string.Format("{0}{1}", result, "Byte/s");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return default;
        }
    }
}
