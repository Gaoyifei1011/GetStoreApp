using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 体积大小转换辅助类
    /// </summary>
    internal static class VolumeSizeHelper
    {
        private static readonly ReadOnlyDictionary<string, int> sizeDict = new Dictionary<string, int>
        {
            { "GB",1024 * 1024 * 1024 },
            { "MB",1024 * 1024 },
            { "KB",1024 }
        }.AsReadOnly();

        /// <summary>
        /// 转换为相应格式的文件大小值
        /// </summary>
        internal static string ConvertVolumeSizeToString(double size)
        {
            if (size / sizeDict["GB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(size / sizeDict["GB"], 2), "GB");
            }
            else if (size / sizeDict["MB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(size / sizeDict["MB"], 2), "MB");
            }
            else if (size / sizeDict["KB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(size / sizeDict["KB"], 2), "KB");
            }
            else
            {
                return string.Format("{0}{1}", size, "B");
            }
        }
    }
}
