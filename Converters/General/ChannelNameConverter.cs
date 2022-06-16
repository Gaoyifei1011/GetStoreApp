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
    public class ChannelNameConverter : IValueConverter
    {
        public List<GetAppChannelModel> ChannelList { get; } = new List<GetAppChannelModel>
        {
            new GetAppChannelModel{ DisplayName=LanguageService.GetResources("Fast"),InternalName="WIF" },
            new GetAppChannelModel{ DisplayName=LanguageService.GetResources("Slow"),InternalName="WIS" },
            new GetAppChannelModel{ DisplayName=LanguageService.GetResources("RP"),InternalName="RP" },
            new GetAppChannelModel{ DisplayName=LanguageService.GetResources("Retail"),InternalName="Retail" }
        };

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
