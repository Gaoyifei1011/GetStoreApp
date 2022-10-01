using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.IO;

namespace GetStoreApp.Converters.Checks
{
    /// <summary>
    /// 检测文件是否存在转换器
    /// </summary>
    public class FileExistVisibilityCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            string FilePath = System.Convert.ToString(value);
            string param = System.Convert.ToString(parameter);

            if (param == "Reverse")
            {
                if (File.Exists(FilePath))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                };
            }
            else
            {
                if(File.Exists(FilePath))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
