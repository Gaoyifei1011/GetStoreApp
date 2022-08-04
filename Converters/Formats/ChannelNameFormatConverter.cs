using GetStoreApp.Contracts.Services.App;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Formats
{
    /// <summary>
    /// UI字符串本地化（通道）转换器
    /// </summary>
    public class ChannelNameFormatConverter : IValueConverter
    {
        private IResourceService ResourceService { get; } = App.GetService<IResourceService>();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            string result = value as string;

            return ResourceService.ChannelList.Find(item => item.InternalName.Equals(result)).DisplayName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
