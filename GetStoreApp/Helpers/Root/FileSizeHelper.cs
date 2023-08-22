using System;
using System.Collections.Generic;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 文件大小转换辅助类
    /// </summary>
    public static class FileSizeHelper
    {
        public static Dictionary<string, int> SizeDict = new Dictionary<string, int>()
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
            if (size / SizeDict["GB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(size / SizeDict["GB"], 2), "GB");
            }
            else if (size / SizeDict["MB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(size / SizeDict["MB"], 2), "MB");
            }
            else if (size / SizeDict["KB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(size / SizeDict["KB"], 2), "KB");
            }
            else
            {
                return string.Format("{0}{1}", size, "B");
            }
        }
    }
}
