using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Formats
{
    /// <summary>
    /// 复选框文字提示本地化转换器
    /// </summary>
    public class CheckBoxToolTipFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null && parameter == null)
            {
                return DependencyProperty.UnsetValue;
            }

            bool result = System.Convert.ToBoolean(value);

            if (result)
            {
                return ResourceService.GetLocalized(string.Format("/{0}/SelectedToolTip", parameter));
            }
            else
            {
                return ResourceService.GetLocalized(string.Format("/{0}/UnselectedToolTip", parameter));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
