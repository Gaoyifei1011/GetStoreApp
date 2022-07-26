using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Checks
{
    /// <summary>
    /// 打开文件夹按钮显示值转换器
    /// </summary>
    public class OpenFolderCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            int? result = value as int?;

            return (result == 0 || result == 3) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
