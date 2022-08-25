using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
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
        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null && parameter == null)
            {
                return DependencyProperty.UnsetValue;
            }

            bool result = System.Convert.ToBoolean(value);
            string param = parameter as string;

            if (result)
            {
                return ResourceService.GetLocalized(string.Format("/{0}/SelectedDescription", param));
            }
            else
            {
                return ResourceService.GetLocalized(string.Format("/{0}/UnselectedDescription", param));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
