using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GetStoreApp.Converters.Formats
{
    /// <summary>
    /// 下载文件名称文字提示转换器
    /// </summary>
    public class FileNameToolTipFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (DownloadOptionsService.DownloadMode.InternalName == DownloadOptionsService.DownloadModeList[0].InternalName)
            {
                return string.Format("{0}\n{1}", value, ResourceService.GetLocalized("/Home/ClickToDownload"));
            }
            else if (DownloadOptionsService.DownloadMode.InternalName == DownloadOptionsService.DownloadModeList[1].InternalName)
            {
                return string.Format("{0}\n{1}", value, ResourceService.GetLocalized("/Home/ClickToAccess"));
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
