using Microsoft.Win32;
using System;
using Windows.UI.Xaml;

namespace GetStoreAppHelper.Helpers
{
    /// <summary>
    /// 注册表读写辅助类
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// 获取系统注册表存储的应用主题值
        /// </summary>
        public static ElementTheme GetRegistrySystemTheme()
        {
            RegistryKey PersonalizeKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            int value = Convert.ToInt32(PersonalizeKey.GetValue("SystemUsesLightTheme", null));

            return value is 0 ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}
