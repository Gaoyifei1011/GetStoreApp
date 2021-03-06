using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Converters.Formats
{
    /// <summary>
    /// UI字符串本地化（类型）转换器
    /// </summary>
    public class TypeNameFormatConverter : IValueConverter
    {
        private readonly IResourceService ResourceService;

        private List<GetAppTypeModel> TypeList { get; set; }

        public TypeNameFormatConverter()
        {
            ResourceService = App.GetService<IResourceService>();

            TypeList = ResourceService.TypeList;
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
