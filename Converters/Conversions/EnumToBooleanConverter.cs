using GetStoreApp.Services.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Conversions
{
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 将枚举值转换为布尔值
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                if (!Enum.IsDefined(typeof(ElementTheme), value))
                {
                    throw new ArgumentException(LanguageService.GetResources("/Converter/EnumToBooleanTypeArgumentException"));
                }

                var enumValue = Enum.Parse(typeof(ElementTheme), enumString);

                return enumValue.Equals(value);
            }

            throw new ArgumentException(LanguageService.GetResources("/Converter/EnumToBooleanParameterArgumentException"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                return Enum.Parse(typeof(ElementTheme), enumString);
            }

            throw new ArgumentException(LanguageService.GetResources("/Converter/EnumToBooleanParameterArgumentException"));
        }
    }
}
