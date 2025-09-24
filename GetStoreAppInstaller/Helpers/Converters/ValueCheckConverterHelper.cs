using GetStoreAppInstaller.Extensions.DataType.Enums;
using Microsoft.UI.Xaml;

namespace GetStoreAppInstaller.Helpers.Converters
{
    /// <summary>
    /// 值检查辅助类
    /// </summary>
    public static class ValueCheckConverterHelper
    {
        /// <summary>
        /// 检查包文件类型
        /// </summary>
        public static Visibility CheckPackageFileType(PackageFileType packageFileType, PackageFileType comparedPackageFileType)
        {
            return Equals(packageFileType, comparedPackageFileType) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
