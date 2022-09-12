using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Checks
{
    public class FileExistTextCheckConverter : IValueConverter
    {
        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

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
                return ResourceService.GetLocalized("/Download/FileNotExist");
            }
            else
            {
                return ResourceService.GetLocalized("/Download/CompleteDownload");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
