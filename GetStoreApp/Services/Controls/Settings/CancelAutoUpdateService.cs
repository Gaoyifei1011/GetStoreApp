using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.ComponentModel;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 检测到新更新时是否自动取消
    /// </summary>
    public static class CancelAutoUpdateService
    {
        private static readonly string settingsKey = ConfigKey.CancelAutoUpdateKey;

        private static readonly bool defaultCancelAutoUpdateValue = false;

        private static bool _cancelAutoUpdateValue;

        public static bool CancelAutoUpdateValue
        {
            get { return _cancelAutoUpdateValue; }

            private set
            {
                if (!Equals(_cancelAutoUpdateValue, value))
                {
                    _cancelAutoUpdateValue = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CancelAutoUpdateValue)));
                }
            }
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的检测到新更新时是否自动取消值
        /// </summary>
        public static void InitializeCancelAutoUpdate()
        {
            CancelAutoUpdateValue = GetCancelAutoUpdateValue();
        }

        /// <summary>
        /// 获取设置存储的检测到新更新时是否自动取消值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetCancelAutoUpdateValue()
        {
            bool? cancelAutoUpdateValue = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!cancelAutoUpdateValue.HasValue)
            {
                SetCancelAutoUpdateValue(defaultCancelAutoUpdateValue);
                return defaultCancelAutoUpdateValue;
            }

            return Convert.ToBoolean(cancelAutoUpdateValue);
        }

        /// <summary>
        /// 使用说明按钮显示发生修改时修改设置存储的检测到新更新时是否自动取消值
        /// </summary>
        public static void SetCancelAutoUpdateValue(bool cancelAutoUpdateValue)
        {
            CancelAutoUpdateValue = cancelAutoUpdateValue;

            LocalSettingsService.SaveSetting(settingsKey, cancelAutoUpdateValue);
        }
    }
}
