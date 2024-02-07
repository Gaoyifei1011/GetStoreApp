using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Helpers.Converters
{
    /// <summary>
    /// 字符串格式化辅助类
    /// </summary>
    public static class StringConverterHelper
    {
        private static Dictionary<string, int> speedDict = new Dictionary<string, int>()
        {
            { "GB/s",1024*1024*1024 },
            { "MB/s",1024*1024 },
            { "KB/s",1024 }
        };

        /// <summary>
        /// UI字符串本地化（通道）格式化
        /// </summary>
        public static string ChannelNameFormat(string content)
        {
            return ResourceService.ChannelList.Find(item => item.InternalName.Equals(content)).DisplayName;
        }

        /// <summary>
        /// UI字符串本地化（类型）格式化
        /// </summary>
        public static string TypeNameFormat(string content)
        {
            return ResourceService.TypeList.Find(item => item.InternalName.Equals(content)).DisplayName;
        }

        /// <summary>
        /// 文件名称提示格式化
        /// </summary>
        public static string FileNameToolTipFormat(string content)
        {
            if (DownloadOptionsService.DownloadMode.Value.Equals(DownloadOptionsService.DownloadModeList[0].Value))
            {
                return string.Format("{0}{1}{2}", content, Environment.NewLine, ResourceService.GetLocalized("Store/ClickToDownload"));
            }
            else if (DownloadOptionsService.DownloadMode.Value.Equals(DownloadOptionsService.DownloadModeList[1].Value))
            {
                return string.Format("{0}{1}{2}", content, Environment.NewLine, ResourceService.GetLocalized("Store/ClickToOpen"));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 下载速度文字显示格式化
        /// </summary>
        public static string DownloadSpeedFormat(double speed)
        {
            if (speed / speedDict["GB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(speed / speedDict["GB/s"], 2), "GB");
            }
            else if (speed / speedDict["MB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(speed / speedDict["MB/s"], 2), "MB");
            }
            else if (speed / speedDict["KB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(speed / speedDict["KB/s"], 2), "KB");
            }
            else
            {
                return string.Format("{0}{1}", speed, "Byte/s");
            }
        }
    }
}
