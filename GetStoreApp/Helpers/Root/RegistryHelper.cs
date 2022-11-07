using Microsoft.UI.Xaml;
using Microsoft.Win32;
using System;

namespace GetStoreApp.Helpers.Root
{
    public static class RegistryHelper
    {
        /// <summary>
        /// 获取系统注册表存储的应用主题值
        /// </summary>
        public static ElementTheme GetRegistryAppTheme()
        {
            RegistryKey PersonalizeKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            int value = Convert.ToInt32(PersonalizeKey.GetValue("AppsUseLightTheme", null));

            return value == 0 ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}
