using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Root
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
        internal static unsafe WINDOWPLACEMENT? GetWindowPlacement(string windowName)
        {
            if (isInitialized && !string.IsNullOrEmpty(windowName) && windowInfoServiceContainer is not null)
            {
                try
                {
                    if (windowInfoServiceContainer.Values.TryGetValue(windowName, out object windowPlacementObj) && windowPlacementObj is string windowPlacementInfo && JsonObject.TryParse(windowPlacementInfo, out JsonObject windowPlacementRootObject))
                    {
                        List<RECT> monitorInfoList = GetAllMonitorInfoList();

                        // 检查显示器布局自我们记录位置以来是否有变化
                        JsonArray monitorArray = windowPlacementRootObject.GetNamedArray("Monitor");
                        for (uint index = 0; index < monitorArray.Count; index++)
                        {
                            JsonObject monitorObject = monitorArray.GetObjectAt(index);
                            int left = Convert.ToInt32(monitorObject.GetNamedNumber("left"));
                            int top = Convert.ToInt32(monitorObject.GetNamedNumber("top"));
                            int right = Convert.ToInt32(monitorObject.GetNamedNumber("right"));
                            int bottom = Convert.ToInt32(monitorObject.GetNamedNumber("bottom"));

                            foreach (RECT monitorInfo in monitorInfoList)
                            {
                                if (!Equals(monitorInfo.left, left) || !Equals(monitorInfo.top, top) || !Equals(monitorInfo.right, right) || !Equals(monitorInfo.bottom, bottom))
                                {
                                    return null;
                                }
                            }
                        }

                        // 读取窗口位置
                        JsonObject windowPlacementObject = windowPlacementRootObject.GetNamedObject("WindowPlacement");
                        WINDOWPLACEMENT windowPlacement = new()
                        {
                            length = sizeof(WINDOWPLACEMENT)
                        };
                        if (Enum.TryParse(windowPlacementObject.GetNamedString("Flags"), out WINDOWPLACEMENTFLAGS windowPlacementFlags))
                        {
                            windowPlacement.flags = windowPlacementFlags;
                        }
                        if (Enum.TryParse(windowPlacementObject.GetNamedString("ShowCmd"), out SHOWWINDOWCMD showWindowCmd))
                        {
                            windowPlacement.showCmd = showWindowCmd;
                        }
                        windowPlacement.ptMinPosition.X = Convert.ToInt32(windowPlacementObject.GetNamedNumber("MinPositionX"));
                        windowPlacement.ptMinPosition.Y = Convert.ToInt32(windowPlacementObject.GetNamedNumber("MinPositionY"));
                        windowPlacement.ptMaxPosition.X = Convert.ToInt32(windowPlacementObject.GetNamedNumber("MaxPositionX"));
                        windowPlacement.ptMaxPosition.Y = Convert.ToInt32(windowPlacementObject.GetNamedNumber("MaxPositionY"));
                        windowPlacement.rcNormalPosition.left = Convert.ToInt32(windowPlacementObject.GetNamedNumber("RcNormalPositionLeft"));
                        windowPlacement.rcNormalPosition.top = Convert.ToInt32(windowPlacementObject.GetNamedNumber("RcNormalPositionTop"));
                        windowPlacement.rcNormalPosition.right = Convert.ToInt32(windowPlacementObject.GetNamedNumber("RcNormalPositionRight"));
                        windowPlacement.rcNormalPosition.bottom = Convert.ToInt32(windowPlacementObject.GetNamedNumber("RcNormalPositionBottom"));
                        if (windowPlacement.showCmd is SHOWWINDOWCMD.SW_SHOWMINIMIZED && windowPlacement.flags is WINDOWPLACEMENTFLAGS.WPF_RESTORETOMAXIMIZED)
                        {
                            windowPlacement.showCmd = SHOWWINDOWCMD.SW_MAXIMIZE;
                        }
                        else if (windowPlacement.showCmd is not SHOWWINDOWCMD.SW_MAXIMIZE)
                        {
                            windowPlacement.showCmd = SHOWWINDOWCMD.SW_NORMAL;
                        }
                        return windowPlacement;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WindowPlacementService), nameof(GetWindowPlacement), 1, e);
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
                        ["RcNormalPositionBottom"] = JsonValue.CreateNumberValue(windowPlacement.rcNormalPosition.bottom),
                        ["RcNormalPositionRight"] = JsonValue.CreateNumberValue(windowPlacement.rcNormalPosition.right),
                    };
                    windowPlacementRootObject["WindowPlacement"] = windowPlacementObject;
                    windowInfoServiceContainer.Values[windowName] = windowPlacementRootObject.Stringify();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WindowPlacementService), nameof(SaveWindowPlacement), 1, e);
                }
            }
        }

        /// <summary>
        /// 获取所有显示器信息列表
        /// </summary>
        private static unsafe List<RECT> GetAllMonitorInfoList()
        {
            List<RECT> monitorList = [];

            bool MonitorEnumProc(nint hMonitor, nint hdc, nint lprcMonitor, nint dwData)
            {
                monitorList.Add((*(RECT*)lprcMonitor));
                return true;
            }

            User32Library.EnumDisplayMonitors(nint.Zero, nint.Zero, MonitorEnumProc, 0);
            return monitorList;
        }
    }
}
