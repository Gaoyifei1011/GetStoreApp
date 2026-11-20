using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.Foundation.Diagnostics;

namespace GetStoreAppInstaller.Services.Root
{
    /// <summary>
    /// 窗口位置服务
    /// </summary>
    internal static class WindowPlacementService
    {
        private static bool isInitialized;
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.GetDefault().LocalSettings;
        private static ApplicationDataContainer windowInfoServiceContainer;

        /// <summary>
        /// 初始化窗口位置服务
        /// </summary>
        internal static void Initialize()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                windowInfoServiceContainer = localSettingsContainer.CreateContainer("WindowInfoServiceContainer", ApplicationDataCreateDisposition.Always);
            }
        }

        /// <summary>
        /// 获取窗口位置
        /// </summary>
        internal static unsafe WINDOWPLACEMENT? GetWindowPlacement(double dpi, string windowName)
        {
            if (isInitialized && !string.IsNullOrEmpty(windowName) && windowInfoServiceContainer is not null)
            {
                try
                {
                    if (windowInfoServiceContainer.Values.TryGetValue(windowName, out object windowPlacementObj) &&
                        windowPlacementObj is string windowPlacementInfo &&
                        JsonObject.TryParse(windowPlacementInfo, out JsonObject windowPlacementRootObject))
                    {
                        List<RECT> monitorInfoList = GetAllMonitorInfoList();

                        // 检查显示器布局自我们记录位置以来是否有变化
                        if (windowPlacementRootObject.TryGetValue("Monitor", out IJsonValue monitorJsonValue) && monitorJsonValue.ValueType is JsonValueType.Array)
                        {
                            JsonArray monitorArray = monitorJsonValue.GetArray();
                            for (uint index = 0; index < monitorArray.Count; index++)
                            {
                                JsonObject monitorObject = monitorArray.GetObjectAt(index);
                                int left = monitorObject.TryGetValue("left", out IJsonValue leftJsonValue) && leftJsonValue.ValueType is JsonValueType.Number ? Convert.ToInt32(leftJsonValue.GetNumber()) : 0;
                                int top = monitorObject.TryGetValue("top", out IJsonValue topJsonValue) && topJsonValue.ValueType is JsonValueType.Number ? Convert.ToInt32(topJsonValue.GetNumber()) : 0;
                                int right = monitorObject.TryGetValue("right", out IJsonValue rightJsonValue) && rightJsonValue.ValueType is JsonValueType.Number ? Convert.ToInt32(rightJsonValue.GetNumber()) : 0;
                                int bottom = monitorObject.TryGetValue("bottom", out IJsonValue bottomJsonValue) && bottomJsonValue.ValueType is JsonValueType.Number ? Convert.ToInt32(bottomJsonValue.GetNumber()) : 0;

                                foreach (RECT monitorInfo in monitorInfoList)
                                {
                                    if (!Equals(monitorInfo.left, left) || !Equals(monitorInfo.top, top) || !Equals(monitorInfo.right, right) || !Equals(monitorInfo.bottom, bottom))
                                    {
                                        return null;
                                    }
                                }
                            }
                        }

                        // 读取窗口位置
                        if (windowPlacementRootObject.TryGetValue("WindowPlacement", out IJsonValue windowPlacementJsonValue) &&
                            windowPlacementJsonValue.ValueType is JsonValueType.Object)
                        {
                            JsonObject windowPlacementObject = windowPlacementJsonValue.GetObject();
                            WINDOWPLACEMENT windowPlacement = new()
                            {
                                length = sizeof(WINDOWPLACEMENT)
                            };
                            if (windowPlacementObject.TryGetValue("Flags", out IJsonValue flagsJsonValue) &&
                                flagsJsonValue.ValueType is JsonValueType.String &&
                                Enum.TryParse(flagsJsonValue.GetString(), out WINDOWPLACEMENTFLAGS windowPlacementFlags))
                            {
                                windowPlacement.flags = windowPlacementFlags;
                            }
                            windowPlacement.showCmd = SHOWWINDOWCMD.SW_SHOWNORMAL;
                            windowPlacement.ptMinPosition.X = windowPlacementObject.TryGetValue("MinPositionX", out IJsonValue minPositionXJsonValue) && minPositionXJsonValue.ValueType is JsonValueType.Number ? Convert.ToInt32(minPositionXJsonValue.GetNumber()) : 0;
                            windowPlacement.ptMinPosition.Y = windowPlacementObject.TryGetValue("MinPositionY", out IJsonValue minPositionYJsonValue) && minPositionYJsonValue.ValueType is JsonValueType.Number ? Convert.ToInt32(minPositionYJsonValue.GetNumber()) : 0;
                            windowPlacement.ptMaxPosition.X = windowPlacementObject.TryGetValue("MaxPositionX", out IJsonValue maxPositionXJsonValue) && maxPositionXJsonValue.ValueType is JsonValueType.Number ? Convert.ToInt32(maxPositionXJsonValue.GetNumber()) : 0;
                            windowPlacement.ptMaxPosition.Y = windowPlacementObject.TryGetValue("MaxPositionY", out IJsonValue maxPositionYJsonValue) && maxPositionYJsonValue.ValueType is JsonValueType.Number ? Convert.ToInt32(maxPositionYJsonValue.GetNumber()) : 0;
                            windowPlacement.rcNormalPosition.left = windowPlacementObject.TryGetValue("RcNormalPositionLeft", out IJsonValue rcNormalPositionLeftJsonValue) && rcNormalPositionLeftJsonValue.ValueType is JsonValueType.Number ? Convert.ToInt32(rcNormalPositionLeftJsonValue.GetNumber()) : 0;
                            windowPlacement.rcNormalPosition.top = windowPlacementObject.TryGetValue("RcNormalPositionTop", out IJsonValue rcNormalPositionTopJsonValue) && rcNormalPositionTopJsonValue.ValueType is JsonValueType.Number ? Convert.ToInt32(rcNormalPositionTopJsonValue.GetNumber()) : 0;
                            windowPlacement.rcNormalPosition.right = Convert.ToInt32(windowPlacement.rcNormalPosition.left + 1000 * dpi);
                            windowPlacement.rcNormalPosition.bottom = Convert.ToInt32(windowPlacement.rcNormalPosition.top + 700 * dpi);
                            return windowPlacement;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(WindowPlacementService), nameof(GetWindowPlacement), 1, e);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 保存窗口位置
        /// </summary>
        internal static void SaveWindowPlacement(string windowName, WINDOWPLACEMENT windowPlacement)
        {
            if (isInitialized && !string.IsNullOrEmpty(windowName) && windowInfoServiceContainer is not null)
            {
                try
                {
                    JsonObject windowPlacementRootObject = [];

                    // 获取显示器信息
                    JsonArray monitorArray = [];
                    List<RECT> monitorInfoList = GetAllMonitorInfoList();
                    foreach (RECT monitorInfo in monitorInfoList)
                    {
                        JsonObject monitorObject = new()
                        {
                            ["left"] = JsonValue.CreateNumberValue(monitorInfo.left),
                            ["top"] = JsonValue.CreateNumberValue(monitorInfo.top),
                            ["right"] = JsonValue.CreateNumberValue(monitorInfo.right),
                            ["bottom"] = JsonValue.CreateNumberValue(monitorInfo.bottom),
                        };
                        monitorArray.Add(monitorObject);
                    }
                    windowPlacementRootObject["Monitor"] = monitorArray;

                    // 存储窗口位置
                    JsonObject windowPlacementObject = new()
                    {
                        ["Flags"] = JsonValue.CreateStringValue(Convert.ToString(windowPlacement.flags)),
                        ["ShowCmd"] = JsonValue.CreateStringValue(Convert.ToString(windowPlacement.showCmd)),
                        ["MinPositionX"] = JsonValue.CreateNumberValue(windowPlacement.ptMinPosition.X),
                        ["MinPositionY"] = JsonValue.CreateNumberValue(windowPlacement.ptMinPosition.Y),
                        ["MaxPositionX"] = JsonValue.CreateNumberValue(windowPlacement.ptMaxPosition.X),
                        ["MaxPositionY"] = JsonValue.CreateNumberValue(windowPlacement.ptMinPosition.Y),
                        ["RcNormalPositionLeft"] = JsonValue.CreateNumberValue(windowPlacement.rcNormalPosition.left),
                        ["RcNormalPositionTop"] = JsonValue.CreateNumberValue(windowPlacement.rcNormalPosition.top),
                    };
                    windowPlacementRootObject["WindowPlacement"] = windowPlacementObject;
                    windowInfoServiceContainer.Values[windowName] = windowPlacementRootObject.Stringify();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(WindowPlacementService), nameof(SaveWindowPlacement), 1, e);
                }
            }
        }

        /// <summary>
        /// 获取所有显示器信息列表
        /// </summary>
        private static unsafe List<RECT> GetAllMonitorInfoList()
        {
            List<RECT> monitorList = [];

            int MonitorEnumProc(nint hMonitor, nint hdc, nint lprcMonitor, nint dwData)
            {
                monitorList.Add((*(RECT*)lprcMonitor));
                return 1;
            }

            User32Library.EnumDisplayMonitors(nint.Zero, nint.Zero, MonitorEnumProc, 0);
            return monitorList;
        }
    }
}
