using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.ComponentModel;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 检测到新更新时是否自动取消
    /// </summary>
    public static class CancelAutoUpdateService
    {
        private static readonly string settingsKey = ConfigKey.CancelAutoUpdateKey;

        private static readonly bool defaultCancelAutoUpdate = false;

        private static bool _cancelAutoUpdate;

        public static bool CancelAutoUpdate
        {
            get { return _cancelAutoUpdate; }

            private set
            {
                if (!Equals(_cancelAutoUpdate, value))
                {
                    _cancelAutoUpdate = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CancelAutoUpdate)));
                }
            }
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的检测到新更新时是否自动取消值
        /// </summary>
        public static void InitializeCancelAutoUpdate()
        {
            CancelAutoUpdate = GetCancelAutoUpdate();
        }

        /// <summary>
        /// 获取设置存储的检测到新更新时是否自动取消值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetCancelAutoUpdate()
        {
            bool? cancelAutoUpdate = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!cancelAutoUpdate.HasValue)
            {
                SetCancelAutoUpdate(defaultCancelAutoUpdate);
                return defaultCancelAutoUpdate;
            }

            return Convert.ToBoolean(cancelAutoUpdate);
        }

        /// <summary>
        /// 使用说明按钮显示发生修改时修改设置存储的检测到新更新时是否自动取消值
        /// </summary>
        public static void SetCancelAutoUpdate(bool cancelAutoUpdate)
        {
            CancelAutoUpdate = cancelAutoUpdate;
            LocalSettingsService.SaveSetting(settingsKey, cancelAutoUpdate);
        }
    }
}
