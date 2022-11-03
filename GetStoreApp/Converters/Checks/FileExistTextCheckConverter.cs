using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.IO;

namespace GetStoreApp.Converters.Checks
{
    /// <summary>
    /// 检测文件是否存在文字提示转换器
    /// </summary>
    public class FileExistTextCheckConverter : IValueConverter
    {
        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            string FilePath = System.Convert.ToString(value);

            if (File.Exists(FilePath))
            {
                return ResourceService.GetLocalized("/Download/CompleteDownload");
            }
            else
            {
                return ResourceService.GetLocalized("/Download/FileNotExist");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
