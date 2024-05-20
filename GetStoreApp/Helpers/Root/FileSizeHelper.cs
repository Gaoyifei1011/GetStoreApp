using System;
using System.Collections.Generic;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 文件大小转换辅助类
    /// </summary>
    public static class FileSizeHelper
    {
        private static readonly Dictionary<string, int> sizeDict = new()
        {
            { "GB",1024*1024*1024 },
            { "MB",1024*1024 },
            { "KB",1024 }
        };

        /// <summary>
        /// 转换为相应格式的文件大小值
        /// </summary>
        public static string ConvertFileSizeToString(double size)
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
