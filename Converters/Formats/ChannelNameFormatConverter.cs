using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Converters.Formats
{
    /// <summary>
    /// UI字符串本地化（通道）转换器
    /// </summary>
    public class ChannelNameFormatConverter : IValueConverter
    {
        private readonly IResourceService resourceService;

        private List<GetAppChannelModel> ChannelList { get; set; }

        public ChannelNameFormatConverter()
        {
            resourceService = App.GetService<IResourceService>();

            ChannelList = resourceService.ChannelList;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            string result = value as string;

            return ChannelList.Find(item => item.InternalName.Equals(result)).DisplayName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
