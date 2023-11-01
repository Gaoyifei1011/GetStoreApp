using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System.Collections;
using System.Collections.Generic;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用历史记录显示数量设置服务
    /// </summary>
    public static class HistoryRecordService
    {
        private static string HistoryLiteSettingsKey = ConfigKey.HistoryLiteNumKey;

        private static DictionaryEntry DefaultHistoryLiteNum;

        public static DictionaryEntry HistoryLiteNum { get; set; }

        public static List<DictionaryEntry> HistoryLiteNumList { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的历史记录显示数量值
        /// </summary>
        public static void InitializeHistoryRecord()
        {
            HistoryLiteNumList = ResourceService.HistoryLiteNumList;

            DefaultHistoryLiteNum = HistoryLiteNumList.Find(item => item.Value.ToString() is "3");

            HistoryLiteNum = GetHistoryLiteNum();
        }

        /// <summary>
        /// 获取设置存储的微软商店页面历史记录显示的最多项目值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetHistoryLiteNum()
        {
            object historyLiteNumValue = ConfigService.ReadSetting<object>(HistoryLiteSettingsKey);

            if (historyLiteNumValue is null)
            {
                SetHistoryLiteNum(DefaultHistoryLiteNum);
                return HistoryLiteNumList.Find(item => item.Value.Equals(DefaultHistoryLiteNum.Value));
            }

            return HistoryLiteNumList.Find(item => item.Value.Equals(historyLiteNumValue));
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的微软商店页面历史记录显示数量值
        /// </summary>
        public static void SetHistoryLiteNum(DictionaryEntry historyLiteNum)
        {
            HistoryLiteNum = historyLiteNum;

            ConfigService.SaveSetting(HistoryLiteSettingsKey, historyLiteNum.Value);
        }
    }
}
