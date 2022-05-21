using System;

using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 主页面“使用说明”按钮的显示状态设置服务
    /// The display status of the "Instructions for Use" button on the main page sets the service
    /// </summary>
    public static class UseInstructionSettings
    {
        /// <summary>
        /// 设置存储时需要使用到的键值
        /// The key value that you need to use when setting the store
        /// </summary>
        private const string SettingsKey = "UseInsVisibilityValue";

        /// <summary>
        /// 设置按钮的默认状态：显示
        /// Sets the default state of the button: Display
        /// </summary>
        private static readonly bool DefaultVisable = true;

        /// <summary>
        /// 主页面按钮的显示状态值
        /// The display status value of the main page button
        /// </summary>
        public static bool UseInsVisValue;

        /// <summary>
        /// 静态资源初始化
        /// </summary>
        static UseInstructionSettings()
        {
            // 从设置存储中加载按钮的显示状态值
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
        /// 设置默认值
        /// Sets the default value
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
