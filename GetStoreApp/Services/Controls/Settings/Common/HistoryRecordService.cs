using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Common
{
    /// <summary>
    /// 应用历史记录显示数量设置服务
    /// </summary>
    public static class HistoryRecordService
    {
        private static string HistoryLiteSettingsKey { get; } = ConfigKey.HistoryLiteNumKey;

        private static string HistoryJumpListSettingsKey { get; } = ConfigKey.HistoryJumpListNumKey;

        private static GroupOptionsModel DefaultHistoryLiteNum { get; set; }

        private static GroupOptionsModel DefaultHistoryJumpListNum { get; set; }

        public static GroupOptionsModel HistoryLiteNum { get; set; }

        public static GroupOptionsModel HistoryJumpListNum { get; set; }

        public static List<GroupOptionsModel> HistoryLiteNumList { get; set; }

        public static List<GroupOptionsModel> HistoryJumpListNumList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的历史记录显示数量值
        /// </summary>
        public static void Initialize()
        {
            HistoryLiteNumList = ResourceService.HistoryLiteNumList;

            HistoryJumpListNumList = ResourceService.HistoryJumpListNumList;

            DefaultHistoryLiteNum = HistoryLiteNumList.Find(item => item.SelectedValue is "3");

            DefaultHistoryJumpListNum = HistoryJumpListNumList.Find(item => item.SelectedValue is "5");

            HistoryLiteNum = GetHistoryLiteNum();

            HistoryJumpListNum = GetHistoryJumpListNum();
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
        /// 获取设置存储的任务栏右键菜单列表历史记录显示的最多项目值，如果设置没有存储，使用默认值
        /// </summary>
        private static GroupOptionsModel GetHistoryJumpListNum()
        {
            string historyJumpListValue = ConfigService.ReadSetting<string>(HistoryJumpListSettingsKey);

            if (string.IsNullOrEmpty(historyJumpListValue))
            {
                return HistoryJumpListNumList.Find(item => item.SelectedValue.Equals(DefaultHistoryJumpListNum.SelectedValue, StringComparison.OrdinalIgnoreCase));
            }

            return HistoryJumpListNumList.Find(item => item.SelectedValue.Equals(historyJumpListValue, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的微软商店页面历史记录显示数量值
        /// </summary>
        public static void SetHistoryLiteNum(GroupOptionsModel historyLiteNum)
        {
            HistoryLiteNum = historyLiteNum;

            ConfigService.SaveSetting(HistoryLiteSettingsKey, historyLiteNum.SelectedValue);
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的任务栏右键菜单跳转列表历史记录显示数量值
        /// </summary>
        public static void SetHistoryJumpListNum(GroupOptionsModel historyJumpListNum)
        {
            HistoryJumpListNum = historyJumpListNum;

            ConfigService.SaveSetting(HistoryJumpListSettingsKey, historyJumpListNum.SelectedValue);
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改应用任务栏右键菜单跳转列表的内容
        /// </summary>
        public static async Task UpdateHistoryJumpListAsync(GroupOptionsModel historyJumpListNum)
        {
            if (historyJumpListNum.SelectedValue is "Unlimited")
            {
                return;
            }
            else
            {
                if (Program.ApplicationRoot.TaskbarJumpList is not null)
                {
                    int count = Convert.ToInt32(historyJumpListNum.SelectedValue);
                    while (Program.ApplicationRoot.TaskbarJumpList.Items.Count > count)
                    {
                        Program.ApplicationRoot.TaskbarJumpList.Items.RemoveAt(0);
                    }
                    await Program.ApplicationRoot.TaskbarJumpList.SaveAsync();
                }
                else
                {
                    return;
                }
            }
        }
    }
}
