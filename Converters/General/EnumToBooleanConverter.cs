using GetStoreApp.Services.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.General
{
    /// <summary>
    /// 枚举值与布尔值的转换器
    /// A converter that enumerates values with Boolean values
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 将枚举值转换为布尔值
        /// Converts an enumeration value to a Boolean value
        /// </summary>
        /// <returns>转换完之后的布尔值</returns>
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

        /// <summary>
        /// 将布尔值转换为枚举值（单向转换，无需实现）
        /// Converting a Boolean value to an enumeration value (one-way conversion, no implementation required)
        /// </summary>
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