using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings;
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
        private static Dictionary<string, int> speedDict = new Dictionary<string, int>()
        {
            { "GB/s",1024*1024*1024 },
            { "MB/s",1024*1024 },
            { "KB/s",1024 }
        };

        public static CultureInfo AppCulture { get; set; }

        /// <summary>
        /// UI字符串本地化（通道）格式化
        /// </summary>
        public static string ChannelNameFormat(string content)
        {
            return ResourceService.ChannelList.Find(item => item.InternalName.Equals(content)).DisplayName;
        }

        /// <summary>
        /// 复选框状态文字提示
        /// </summary>
        public static string CheckBoxToolTipFormat(bool isSelected, string content)
        {
            if (isSelected)
            {
                return ResourceService.GetLocalized(string.Format("{0}/SelectedToolTip", content));
            }
            else
            {
                return ResourceService.GetLocalized(string.Format("{0}/UnselectedToolTip", content));
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
        /// 安装进度文字提示格式化
        /// </summary>
        public static string InstallValueFormat(double content)
        {
            return string.Format(ResourceService.GetLocalized("Download/InstallValue"), content);
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

        /// <summary>
        /// 应用管理描述信息格式化
        /// </summary>
        public static string UwpAppToolTipFormat(string content, string type)
        {
            if (type is "DisplayName")
            {
                return string.Format(ResourceService.GetLocalized("UWPApp/DisplayNameToolTip"), content);
            }
            else if (type is "PublisherName")
            {
                return string.Format(ResourceService.GetLocalized("UWPApp/PublisherToolTip"), content);
            }
            else if (type is "Version")
            {
                return string.Format(ResourceService.GetLocalized("UWPApp/VersionToolTip"), content);
            }
            else if (type is "InstallDate")
            {
                return string.Format(ResourceService.GetLocalized("UWPApp/InstallDateToolTip"), content);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
