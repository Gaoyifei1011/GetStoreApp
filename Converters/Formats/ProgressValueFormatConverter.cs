using GetStoreApp.Contracts.Services.App;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Formats
{
    /// <summary>
    /// 下载进度文字显示转换器
    /// </summary>
    public class DownloadProgressFormatConverter : IValueConverter
    {
        private readonly IResourceService ResourceService;

        public DownloadProgressFormatConverter()
        {
            ResourceService = App.GetService<IResourceService>();
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            int? result = value as int?;

            return string.Format(ResourceService.GetLocalized("/Download/DownloadProgress"), result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
