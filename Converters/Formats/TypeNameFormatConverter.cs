using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Converters.Formats
{
    public class TypeNameFormatConverter : IValueConverter
    {
        private readonly IResourceService resourceService;

        private List<GetAppTypeModel> TypeList { get; set; }

        public TypeNameFormatConverter()
        {
            resourceService = App.GetService<IResourceService>();

            TypeList = resourceService.TypeList;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            string result = value as string;

            return TypeList.Find(item => item.InternalName.Equals(result)).DisplayName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
