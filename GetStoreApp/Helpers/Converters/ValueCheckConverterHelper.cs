using Microsoft.UI.Xaml;
using System.IO;
using Windows.ApplicationModel;

namespace GetStoreApp.Helpers.Converters
{
    /// <summary>
    /// 值检查辅助类
    /// </summary>
    public static class ValueCheckConverterHelper
    {
        public static Visibility DownloadFlagCheck(int value, int checkValue)
        {
            return value == checkValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检测文件是否存在（判断结果相反）
        /// </summary>
        public static bool FileExistReverseCheck(string path)
        {
            return !File.Exists(path);
        }

        /// <summary>
        /// 检测当前页面是否为应用列表页面
        /// </summary>
        public static Visibility IsAppListPageCheck(int count)
        {
            return count is 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检测当前应用是否为商店应用
        /// </summary>
        public static Visibility IsStoreAppCheck(PackageSignatureKind packageSignatureKind)
        {
            return packageSignatureKind is PackageSignatureKind.Store ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检测当前应用是否为系统应用（系统应用无法卸载）
        /// </summary>
        public static Visibility IsNotSystemAppCheck(PackageSignatureKind packageSignatureKind)
        {
            return packageSignatureKind is PackageSignatureKind.System ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
