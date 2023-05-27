using System;
using System.Collections.Generic;

namespace GetStoreApp.Helpers.Root
{
    public static class FileSizeHelper
    {
        public static Dictionary<string, int> SizeDict = new Dictionary<string, int>()
        {
            { "GB",1024*1024*1024 },
            { "MB",1024*1024 },
            { "KB",1024 }
        };

        public static string ConvertFileSizeToString(double size)
        {
            if (size / SizeDict["GB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(System.Convert.ToDouble(size) / SizeDict["GB"], 2), "GB");
            }
            else if (size / SizeDict["MB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(System.Convert.ToDouble(size) / SizeDict["MB"], 2), "MB");
            }
            else if (size / SizeDict["KB"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(System.Convert.ToDouble(size) / SizeDict["KB"], 2), "KB");
            }
            else
            {
                return string.Format("{0}{1}", size, "B");
            }
        }
    }
}
