using GetStoreApp.Services.Settings;
using System;

using Windows.UI.Xaml.Data;

namespace GetStoreApp.Converters.General
{
    /// <summary>
    /// 枚举值与布尔值的转换器
    /// A converter that enumerates values with Boolean values
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public Type EnumType { get; set; }

        /// <summary>
        /// 将枚举值转换为布尔值
        /// Converts an enumeration value to a Boolean value
        /// </summary>
        /// <returns>转换完之后的布尔值</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                if (!Enum.IsDefined(EnumType, value))
                {
                    throw new ArgumentException(LanguageSettings.GetResources("/Converter/EnumToBooleanValueArgumentException"));
                }

                object enumValue = Enum.Parse(EnumType, enumString);

                return enumValue.Equals(value);
            }

            throw new ArgumentException(LanguageSettings.GetResources("/Converter/EnumToBooleanParameterArgumentException"));
        }

        /// <summary>
        /// 将布尔值转换为枚举值（单向转换，无需实现）
        /// Converting a Boolean value to an enumeration value (one-way conversion, no implementation required)
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                return Enum.Parse(EnumType, enumString);
            }

            throw new ArgumentException(LanguageSettings.GetResources("EnumToBooleanParameterArgumentException"));
        }
    }
}
