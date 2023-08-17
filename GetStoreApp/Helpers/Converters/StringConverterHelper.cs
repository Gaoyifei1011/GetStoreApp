using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GetStoreApp.Helpers.Converters
{
    /// <summary>
    /// 字符串格式化辅助类
    /// </summary>
    public static class StringConverterHelper
    {
        private static Dictionary<string, int> SpeedDict = new Dictionary<string, int>()
        {
            { "GB/s",1024*1024*1024 },
            { "MB/s",1024*1024 },
            { "KB/s",1024 }
        };

        public static CultureInfo AppCulture { get; set; }

        /// <summary>
        /// 关于界面项目引用内容格式化
        /// </summary>
        public static string AboutReferenceToolTipFormat(string content)
        {
            return string.Format("{0}\n{1}", content, ResourceService.GetLocalized("About/ReferenceToolTip"));
        }

        /// <summary>
        /// 关于界面感谢介绍内容格式化
        /// </summary>
        public static string AboutThanksToolTipFormat(string content)
        {
            return string.Format("{0}\n{1}", content, ResourceService.GetLocalized("About/ThanksToolTip"));
        }

        /// <summary>
        /// UI字符串本地化（通道）格式化
        /// </summary>
        public static string ChannelNameFormat(string content)
        {
            return ResourceService.ChannelList.Find(item => item.InternalName.Equals(content)).DisplayName;
        }

        public static string CheckBoxToolTipFormat(bool isSelected, string content)
        {
            if (isSelected)
            {
                return ResourceService.GetLocalized(string.Format("/{0}/SelectedToolTip", content));
            }
            else
            {
                return ResourceService.GetLocalized(string.Format("/{0}/UnselectedToolTip", content));
            }
        }

        /// <summary>
        /// 下载文件大小文字显示格式化
        /// </summary>
        public static string DownloadSizeFormat(double size)
        {
            return FileSizeHelper.ConvertFileSizeToString(size);
        }

        /// <summary>
        /// 下载速度文字显示格式化
        /// </summary>
        public static string DownloadSpeedFormat(double speed)
        {
            if (speed / SpeedDict["GB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(speed / SpeedDict["GB/s"], 2), "GB");
            }
            else if (speed / SpeedDict["MB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(speed / SpeedDict["MB/s"], 2), "MB");
            }
            else if (speed / SpeedDict["KB/s"] >= 1)
            {
                return string.Format("{0}{1}", Math.Round(speed / SpeedDict["KB/s"], 2), "KB");
            }
            else
            {
                return string.Format("{0}{1}", speed, "Byte/s");
            }
        }

        /// <summary>
        /// 文件名称提示格式化
        /// </summary>
        public static string FileNameToolTipFormat(string content)
        {
            if (DownloadOptionsService.DownloadMode.SelectedValue == DownloadOptionsService.DownloadModeList[0].SelectedValue)
            {
                return string.Format("{0}\n{1}", content, ResourceService.GetLocalized("Store/ClickToDownload"));
            }
            else if (DownloadOptionsService.DownloadMode.SelectedValue == DownloadOptionsService.DownloadModeList[1].SelectedValue)
            {
                return string.Format("{0}\n{1}", content, ResourceService.GetLocalized("Store/ClickToAccess"));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// GMT时间与当地地区时间格式化
        /// </summary>
        public static string GMTFormat(string content)
        {
            return Convert.ToDateTime(content).ToLocalTime().ToString("G", AppCulture);
        }

        /// <summary>
        /// 安装进度文字提示格式化
        /// </summary>
        public static string InstallValueFormat(double content)
        {
            return string.Format(ResourceService.GetLocalized("Download/InstallValue"), content);
        }

        /// <summary>
        /// UTC标准时间戳与当地地区时间转换内容格式化
        /// </summary>
        public static string TimeStampFormat(long rawDataTime)
        {
            DateTime EpochStartTime = new DateTime(1970, 1, 1, 0, 0, 0);

            TimeSpan UtcOffset = DateTime.Now - DateTime.UtcNow;

            DateTime CurrentTime = EpochStartTime.AddSeconds(rawDataTime + UtcOffset.TotalSeconds);

            return CurrentTime.ToString("G", AppCulture);
        }

        /// <summary>
        /// UI字符串本地化（类型）格式化
        /// </summary>
        public static string TypeNameFormat(string content)
        {
            return ResourceService.TypeList.Find(item => item.InternalName.Equals(content)).DisplayName;
        }

        /// <summary>
        /// WinGet 程序包描述信息格式化
        /// </summary>
        public static string WinGetAppsToolTipFormat(string content, string type)
        {
            if (type is "AppName")
            {
                return string.Format(ResourceService.GetLocalized("WinGet/AppNameToolTip"), content);
            }
            else if (type is "AppVersion")
            {
                return string.Format(ResourceService.GetLocalized("WinGet/AppVersionToolTip"), content);
            }
            else if (type is "AppPublisher")
            {
                return string.Format(ResourceService.GetLocalized("WinGet/AppPublisherToolTip"), content);
            }
            else if (type is "AppCurrentVersion")
            {
                return string.Format(ResourceService.GetLocalized("WinGet/AppCurrentVersionToolTip"), content);
            }
            else if (type is "AppNewestVersion")
            {
                return string.Format(ResourceService.GetLocalized("WinGet/AppNewestVersionToolTip"), content);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
