using System;

using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 修改主页面“使用说明”按钮的显示状态
    /// Modify the display state of the Instructions for Use button on the main page
    /// </summary>
    public static class UseInstructionService
    {
        private const string SettingsKey = "UseInsVisibilityValue";

        public static bool UseInsVisValue;

        // 按钮显示控制:默认设置为显示
        private static readonly bool DefaultVisable = true;

        static UseInstructionService()
        {
            UseInsVisValue = GetUseInsVisValue();
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// When the application is initialized, the system information about the key value storage is empty, so you need to determine whether the system stored key value is empty
        /// </summary>
        /// <returns>键值存储的信息是否为空</returns>
        private static bool IsSettingsKeyNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey] == null;
        }

        /// <summary>
        /// 检测存储的键值是否为空,如果不存在,设置默认值
        /// Detects whether the stored key value is empty, and if it does not exist, sets the default value
        /// </summary>
        private static void InitializeSettingsKey()
        {
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = DefaultVisable;
        }

        /// <summary>
        /// 获取当前设置存储的“使用说明”显示状态布尔值
        /// Gets the Usage Instructions display status Boolean value for the current settings store
        /// </summary>
        /// <returns>显示状态布尔值</returns>
        private static bool GetUseInsVisValue()
        {
            if (IsSettingsKeyNullOrEmpty())
            {
                InitializeSettingsKey();
            }
            return Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values[SettingsKey]);
        }

        /// <summary>
        /// 设置主页面“使用说明”的显示状态布尔值
        /// Sets the Boolean display state value for the main page Instructions for Use
        /// </summary>
        /// <param name="useInsVisValue">显示状态布尔值</param>
        public static void SetUseInsVisValue(bool useInsVisValue)
        {
            UseInsVisValue = useInsVisValue;

            ApplicationData.Current.LocalSettings.Values[SettingsKey] = useInsVisValue;
        }
    }
}
