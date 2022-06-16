using GetStoreApp.Models;
using GetStoreApp.Services.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Converters.General
{
    public class TypeNameConverter : IValueConverter
    {
        public List<GetAppTypeModel> TypeList { get; } = new List<GetAppTypeModel>
        {
            new GetAppTypeModel{DisplayName=LanguageService.GetResources("URL"),InternalName="url"},
            new GetAppTypeModel{DisplayName=LanguageService.GetResources("ProductID"),InternalName="ProductId"},
            new GetAppTypeModel{DisplayName=LanguageService.GetResources("PackageFamilyName"),InternalName="PackageFamilyName"},
            new GetAppTypeModel{DisplayName=LanguageService.GetResources("CategoryID"),InternalName="CategoryId"}
        };

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
