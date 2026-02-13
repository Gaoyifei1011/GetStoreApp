using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Advapi32;
using System;
using System.Text;
using Windows.Foundation.Diagnostics;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 注册表读取辅助类
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// 读取注册表指定项的内容
        /// </summary>
        public static T ReadRegistryKey<T>(nuint rootRegistryKey, string rootKey, string key)
        {
            T value = default;
            try
            {
                if ((Equals(rootRegistryKey, ReservedKeyHandles.HKEY_CLASSES_ROOT) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_CURRENT_CONFIG) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_CURRENT_USER) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_LOCAL_MACHINE) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_PERFORMANCE_DATA) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_USERS)) && Advapi32Library.RegOpenKeyEx(rootRegistryKey, rootKey, 0, RegistryAccessRights.KEY_READ, out nint hKey) is 0)
                {
                    int size = 0;
                    object getValue = null;

                    if (Advapi32Library.RegQueryValueEx(hKey, key, nint.Zero, out REG_VALUE_TYPE type, null, ref size) is 0 && size is not 0)
                    {
                        byte[] data = new byte[size];
                        if (Advapi32Library.RegQueryValueEx(hKey, key, nint.Zero, out type, data, ref size) is 0)
                        {
                            if (type is REG_VALUE_TYPE.REG_SZ || type is REG_VALUE_TYPE.REG_EXPAND_SZ)
                            {
                                getValue = Encoding.Unicode.GetString(data).TrimEnd('\0');
                            }
                            else if (type is REG_VALUE_TYPE.REG_BINARY)
                            {
                                getValue = data;
                            }
                            else if (type is REG_VALUE_TYPE.REG_DWORD || type is REG_VALUE_TYPE.REG_DWORD_LITTLE_ENDIAN)
                            {
                                getValue = BitConverter.ToInt32(data, 0);
                            }
                            else if (type is REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN)
                            {
                                if (data.Length >= 4)
                                {
                                    byte[] reversed = [data[3], data[2], data[1], data[0]];
                                    getValue = BitConverter.ToInt32(reversed, 0);
                                }
                            }
                            else if (type is REG_VALUE_TYPE.REG_LINK)
                            {
                                getValue = Encoding.Unicode.GetString(data).TrimEnd('\0');
                            }
                            else if (type is REG_VALUE_TYPE.REG_MULTI_SZ)
                            {
                                string valueResult = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                getValue = valueResult.Split(['\0'], StringSplitOptions.RemoveEmptyEntries);
                            }
                            else if (type is REG_VALUE_TYPE.REG_QWORD || type is REG_VALUE_TYPE.REG_QWORD_LITTLE_ENDIAN)
                            {
                                getValue = BitConverter.ToInt64(data, 0);
                            }
                        }
                        Advapi32Library.RegCloseKey(hKey);
                    }

                    if (getValue is not null)
                    {
                        if (Equals(typeof(T), typeof(bool)) || Equals(typeof(T), typeof(bool?)))
                        {
                            value = (T)(object)Convert.ToBoolean(getValue);
                        }
                        else if (Equals(typeof(T), typeof(int)) || Equals(typeof(T), typeof(int?)))
                        {
                            value = (T)(object)Convert.ToInt32(getValue);
                        }
                        else if (Equals(typeof(T), typeof(uint)) || Equals(typeof(T), typeof(uint?)))
                        {
                            value = (T)(object)Convert.ToUInt32(getValue);
                        }
                        else if (Equals(typeof(T), typeof(long)) || Equals(typeof(T), typeof(long?)))
                        {
                            value = (T)(object)Convert.ToInt64(getValue);
                        }
                        else if (Equals(typeof(T), typeof(ulong)) || Equals(typeof(T), typeof(ulong?)))
                        {
                            value = (T)(object)Convert.ToUInt64(getValue);
                        }
                        else if (Equals(typeof(T), typeof(string)))
                        {
                            value = (T)(object)Convert.ToString(getValue);
                        }
                        else
                        {
                            value = Equals(typeof(T), typeof(byte[])) || Equals(typeof(T), typeof(string[])) ? (T)getValue : (T)getValue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(RegistryHelper), nameof(ReadRegistryKey), 1, e);
            }

            return value;
        }
    }
}
