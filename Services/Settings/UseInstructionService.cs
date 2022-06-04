using System;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    public static class UseInstructionService
    {
        private const string SettingsKey = "UseInsVisibilityValue";

        private static readonly bool DefaultValue = true;

        public static bool UseInsVisValue { get; set; }

        static UseInstructionService()
        {
            UseInsVisValue = GetUseInsVisValue();
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// </summary>
        private static bool IsSettingsKeyNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey] == null;
        }

        /// <summary>
        /// 设置默认值
        /// </summary>
        private static void InitializeSettingsKey()
        {
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = DefaultValue;
        }

        /// <summary>
        /// 获取当前设置存储的“使用说明”显示状态布尔值
        /// </summary>
        private static bool GetUseInsVisValue()
        {
            if (IsSettingsKeyNullOrEmpty())
            {
                InitializeSettingsKey();
            }
            return Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values[SettingsKey]);
        }

        /// <summary>
        /// 修改设置
        /// </summary>
        public static void SetUseInsVisValue(bool useInsVisValue)
        {
            UseInsVisValue = useInsVisValue;

            ApplicationData.Current.LocalSettings.Values[SettingsKey] = useInsVisValue;
        }
    }
}
