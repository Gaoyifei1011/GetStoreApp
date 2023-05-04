using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings.Common;
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

        private static HistoryLiteNumModel DefaultHistoryLiteNum { get; set; }

        private static HistoryJumpListNumModel DefaultHistoryJumpListNum { get; set; }

        public static HistoryLiteNumModel HistoryLiteNum { get; set; }

        public static HistoryJumpListNumModel HistoryJumpListNum { get; set; }

        public static List<HistoryLiteNumModel> HistoryLiteNumList { get; set; }

        public static List<HistoryJumpListNumModel> HistoryJumpListNumList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的历史记录显示数量值
        /// </summary>
        public static async Task InitializeAsync()
        {
            HistoryLiteNumList = ResourceService.HistoryLiteNumList;

            HistoryJumpListNumList = ResourceService.HistoryJumpListNumList;

            DefaultHistoryLiteNum = HistoryLiteNumList.Find(item => item.HistoryLiteNumValue is 3);

            DefaultHistoryJumpListNum = HistoryJumpListNumList.Find(item => item.HistoryJumpListNumValue is "Unlimited");

            HistoryLiteNum = await GetHistoryLiteNumAsync();

            HistoryJumpListNum = await GetHistoryJumpListNumAsync();
        }

        /// <summary>
        /// 获取设置存储的微软商店页面历史记录显示的最多项目值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<HistoryLiteNumModel> GetHistoryLiteNumAsync()
        {
            int? historyLiteNumValue = await ConfigService.ReadSettingAsync<int?>(HistoryLiteSettingsKey);

            if (!historyLiteNumValue.HasValue)
            {
                return HistoryLiteNumList.Find(item => item.HistoryLiteNumValue == DefaultHistoryLiteNum.HistoryLiteNumValue);
            }

            return HistoryLiteNumList.Find(item => item.HistoryLiteNumValue == historyLiteNumValue);
        }

        /// <summary>
        /// 获取设置存储的任务栏右键菜单列表历史记录显示的最多项目值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<HistoryJumpListNumModel> GetHistoryJumpListNumAsync()
        {
            string historyJumpListValue = await ConfigService.ReadSettingAsync<string>(HistoryJumpListSettingsKey);

            if (string.IsNullOrEmpty(historyJumpListValue))
            {
                return HistoryJumpListNumList.Find(item => item.HistoryJumpListNumValue.Equals(DefaultHistoryJumpListNum.HistoryJumpListNumValue, StringComparison.OrdinalIgnoreCase));
            }

            return HistoryJumpListNumList.Find(item => item.HistoryJumpListNumValue.Equals(historyJumpListValue, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的微软商店页面历史记录显示数量值
        /// </summary>
        public static async Task SetHistoryLiteNumAsync(HistoryLiteNumModel historyLiteNum)
        {
            HistoryLiteNum = historyLiteNum;

            await ConfigService.SaveSettingAsync(HistoryLiteSettingsKey, historyLiteNum.HistoryLiteNumValue);
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的任务栏右键菜单跳转列表历史记录显示数量值
        /// </summary>
        public static async Task SetHistoryJumpListNumAsync(HistoryJumpListNumModel historyJumpListNum)
        {
            HistoryJumpListNum = historyJumpListNum;

            await ConfigService.SaveSettingAsync(HistoryJumpListSettingsKey, historyJumpListNum.HistoryJumpListNumValue);
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改应用任务栏右键菜单跳转列表的内容
        /// </summary>
        public static async Task UpdateHistoryJumpListAsync(HistoryJumpListNumModel historyJumpListNum)
        {
            if (historyJumpListNum.HistoryJumpListNumValue == "Unlimited")
            {
                return;
            }
            else
            {
                if (Program.ApplicationRoot.TaskbarJumpList is not null)
                {
                    int count = Convert.ToInt32(historyJumpListNum.HistoryJumpListNumValue);
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
