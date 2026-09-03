using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.ComponentModel;
using Windows.UI.Notifications;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    internal static class NotificationService
    {
        private static readonly string settingsKey = ConfigKey.NotificationKey;
        private static readonly bool defaultAppNotification = true;

        internal static bool AppNotification { get; private set; }

        private static NotificationSetting _notificationSetting;

        internal static NotificationSetting NotificationSetting
        {
            get { return _notificationSetting; }

            private set
            {
                if (!Equals(_notificationSetting, value))
                {
                    _notificationSetting = value;
                    PropertyChanged?.Invoke(null, new(nameof(NotificationSetting)));
                }
            }
        }

        internal static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的应用通知显示值
        /// </summary>
        internal static void InitializeNotification()
        {
            AppNotification = GetNotification();
            NotificationSetting = ToastNotificationService.AppToastNotifier.Setting;
        }

        /// <summary>
        /// 获取设置存储的应用通知显示值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetNotification()
        {
            bool? appNotification = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!appNotification.HasValue)
            {
                SetNotification(defaultAppNotification);
                return defaultAppNotification;
            }

            return Convert.ToBoolean(appNotification);
        }

        /// <summary>
        /// 应用通知显示发生修改时修改设置存储的使用说明按钮显示值
        /// </summary>
        internal static void SetNotification(bool appNotification)
        {
            AppNotification = appNotification;
            LocalSettingsService.SaveSetting(settingsKey, appNotification);
        }

        /// <summary>
        /// 更新应用的通知状态
        /// </summary>
        internal static void UpdateNotificationSetting()
        {
            NotificationSetting = ToastNotificationService.AppToastNotifier.Setting;
        }
    }
}
