using GetStoreApp.WindowsAPI.PInvoke.Advapi32;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 注册表读写辅助类
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// 获取系统注册表存储的应用主题值
        /// </summary>
        public static ElementTheme GetRegistryAppTheme()
        {
            IntPtr hKey = IntPtr.Zero;
            ElementTheme AppsUseLightTheme = ElementTheme.Default;

            if (Advapi32Library.RegOpenKeyEx(ReservedKeyHandles.HKEY_CURRENT_USER, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", 0, RegistryAccessRights.KEY_READ, ref hKey) == 0)
            {
                int dataSize = 4;
                byte[] data = new byte[dataSize];

                if (Advapi32Library.RegQueryValueEx(hKey, "AppsUseLightTheme", IntPtr.Zero, IntPtr.Zero, data, ref dataSize) == 0)
                {
                    AppsUseLightTheme = data[0] is 0 ? ElementTheme.Dark : ElementTheme.Light;
                }
            }
            Advapi32Library.RegCloseKey(hKey);
            return AppsUseLightTheme;
        }
    }
}
