using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Root;
using System.Collections.Generic;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用历史记录显示数量设置服务
    /// </summary>
    public static class HistoryRecordService
    {
        private static string HistoryLiteSettingsKey { get; } = ConfigKey.HistoryLiteNumKey;

        private static GroupOptionsModel DefaultHistoryLiteNum { get; set; }

        public static GroupOptionsModel HistoryLiteNum { get; set; }

        public static List<GroupOptionsModel> HistoryLiteNumList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的历史记录显示数量值
        /// </summary>
        public static void InitializeHistoryRecord()
        {
            HistoryLiteNumList = ResourceService.HistoryLiteNumList;

            DefaultHistoryLiteNum = HistoryLiteNumList.Find(item => item.SelectedValue is "3");

            HistoryLiteNum = GetHistoryLiteNum();
        }

        /// <summary>
        /// 获取设置存储的微软商店页面历史记录显示的最多项目值，如果设置没有存储，使用默认值
        /// </summary>
        private static GroupOptionsModel GetHistoryLiteNum()
        {
            string historyLiteNumValue = ConfigService.ReadSetting<string>(HistoryLiteSettingsKey);

            if (string.IsNullOrEmpty(historyLiteNumValue))
            {
                return HistoryLiteNumList.Find(item => item.SelectedValue == DefaultHistoryLiteNum.SelectedValue);
            }

            return HistoryLiteNumList.Find(item => item.SelectedValue == historyLiteNumValue);
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的微软商店页面历史记录显示数量值
        /// </summary>
        public static void SetHistoryLiteNum(GroupOptionsModel historyLiteNum)
        {
            HistoryLiteNum = historyLiteNum;

            ConfigService.SaveSetting(HistoryLiteSettingsKey, historyLiteNum.SelectedValue);
        }
    }
}
