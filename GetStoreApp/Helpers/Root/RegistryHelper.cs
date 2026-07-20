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
                            value = (T)getValue;
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

        /// <summary>
        /// 保存注册表指定项的内容
        /// </summary>
        public static bool SaveRegistryKey<T>(nuint rootRegistryKey, string rootKey, string key, T value, bool isExpandString = false)
        {
            try
            {
                if ((Equals(rootRegistryKey, ReservedKeyHandles.HKEY_CLASSES_ROOT) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_CURRENT_CONFIG) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_CURRENT_USER) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_LOCAL_MACHINE) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_PERFORMANCE_DATA) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_USERS)) && Advapi32Library.RegOpenKeyEx(rootRegistryKey, rootKey, 0, RegistryAccessRights.KEY_ALL_ACCESS, out nint hKey) is 0)
                {
                    REG_VALUE_TYPE regValueType = REG_VALUE_TYPE.REG_NONE;
                    byte[] saveData = [];
                    int cbData = 0;

                    // 存储布尔值
                    if (Equals(typeof(T), typeof(bool)))
                    {
                        regValueType = REG_VALUE_TYPE.REG_DWORD;
                        saveData = BitConverter.GetBytes(Convert.ToInt32(value));
                        cbData = saveData.Length;
                    }
                    // 存储 32 位整数类型
                    else if (Equals(typeof(T), typeof(int)))
                    {
                        regValueType = REG_VALUE_TYPE.REG_DWORD;
                        saveData = BitConverter.GetBytes(Convert.ToInt32(value));
                        cbData = saveData.Length;
                    }
                    // 存储 32 位整数无符号类型
                    else if (Equals(typeof(T), typeof(uint)))
                    {
                        regValueType = REG_VALUE_TYPE.REG_DWORD;
                        saveData = BitConverter.GetBytes(Convert.ToUInt32(value));
                        cbData = saveData.Length;
                    }
                    // 存储 64 位整数类型
                    else if (Equals(typeof(T), typeof(long)))
                    {
                        regValueType = REG_VALUE_TYPE.REG_QWORD;
                        saveData = BitConverter.GetBytes(Convert.ToInt64(value));
                        cbData = saveData.Length;
                    }
                    // 存储 64 位整数无符号类型
                    else if (Equals(typeof(T), typeof(ulong)))
                    {
                        regValueType = REG_VALUE_TYPE.REG_QWORD;
                        saveData = BitConverter.GetBytes(Convert.ToUInt64(value));
                        cbData = saveData.Length;
                    }
                    // 存储字符串类型
                    else if (Equals(typeof(T), typeof(string)))
                    {
                        regValueType = isExpandString ? REG_VALUE_TYPE.REG_EXPAND_SZ : REG_VALUE_TYPE.REG_SZ;
                        saveData = Encoding.Unicode.GetBytes(Convert.ToString(value) + "\0");
                        cbData = saveData.Length;
                    }
                    // 存储二进制数据
                    else if (Equals(typeof(T), typeof(byte[])))
                    {
                        regValueType = REG_VALUE_TYPE.REG_BINARY;
                        saveData = (byte[])(object)value;
                        cbData = saveData.Length;
                    }
                    // 存储字符串数组
                    else if (Equals(typeof(T), typeof(string[])))
                    {
                        regValueType = REG_VALUE_TYPE.REG_MULTI_SZ;
                        string[] values = (string[])(object)value;
                        string combined = string.Join("\0", values) + "\0\0";
                        saveData = Encoding.Unicode.GetBytes(combined);
                        cbData = saveData.Length;
                    }
                    int Result = Advapi32Library.RegSetValueEx(hKey, key, 0, regValueType, saveData, cbData);
                    Advapi32Library.RegCloseKey(hKey);
                }
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(RegistryHelper), nameof(SaveRegistryKey), 1, e);
                return false;
            }
        }

        /// <summary>
        /// 删除注册表指定项的内容
        /// </summary>
        public static bool RemoveRegistryKey(nuint rootRegistryKey, string rootKey, string key)
        {
            try
            {
                if ((Equals(rootRegistryKey, ReservedKeyHandles.HKEY_CLASSES_ROOT) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_CURRENT_CONFIG) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_CURRENT_USER) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_LOCAL_MACHINE) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_PERFORMANCE_DATA) || Equals(rootRegistryKey, ReservedKeyHandles.HKEY_USERS)) && Advapi32Library.RegOpenKeyEx(rootRegistryKey, rootKey, 0, RegistryAccessRights.KEY_ALL_ACCESS, out nint hKey) is 0)
                {
                    Advapi32Library.RegDeleteValue(hKey, key);
                    Advapi32Library.RegCloseKey(hKey);
                }
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(RegistryHelper), nameof(SaveRegistryKey), 1, e);
                return false;
            }
        }
    }
}
