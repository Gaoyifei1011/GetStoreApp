using Microsoft.UI.Composition.SystemBackdrops;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Diagnostics;
using Windows.UI.Xaml;

namespace GetStoreAppInstaller.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static bool isInitialized;

        private static string _defaultAppLanguage;
        private static string _currentAppLanguage;

        private static readonly ResourceContext defaultResourceContext = new();
        private static readonly ResourceContext currentResourceContext = new();
        private static readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap;

        public static List<KeyValuePair<string, string>> BackdropList { get; } = [];

        public static List<KeyValuePair<string, string>> ThemeList { get; } = [];

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(string defaultAppLanguage, string currentAppLanguage)
        {
            _defaultAppLanguage = defaultAppLanguage;
            _currentAppLanguage = currentAppLanguage;

            defaultResourceContext.QualifierValues["Language"] = _defaultAppLanguage;
            currentResourceContext.QualifierValues["Language"] = _currentAppLanguage;

            isInitialized = true;
        }

        /// <summary>
        /// 初始化应用本地化信息
        /// </summary>
        public static void LocalizeReosurce()
        {
            InitializeBackdropList();
            InitializeThemeList();
        }

        /// <summary>
        /// 初始化应用背景色信息列表
        /// </summary>
        private static void InitializeBackdropList()
        {
            BackdropList.Add(KeyValuePair.Create(nameof(SystemBackdropTheme.Default), GetLocalized("Settings/BackdropDefault")));
            BackdropList.Add(KeyValuePair.Create(nameof(MicaKind) + nameof(MicaKind.Base), GetLocalized("Settings/BackdropMica")));
            BackdropList.Add(KeyValuePair.Create(nameof(MicaKind) + nameof(MicaKind.BaseAlt), GetLocalized("Settings/BackdropMicaAlt")));
            BackdropList.Add(KeyValuePair.Create(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Default), GetLocalized("Settings/BackdropAcrylic")));
            BackdropList.Add(KeyValuePair.Create(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Base), GetLocalized("Settings/BackdropAcrylicBase")));
            BackdropList.Add(KeyValuePair.Create(nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Thin), GetLocalized("Settings/BackdropAcrylicThin")));
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(KeyValuePair.Create(nameof(ElementTheme.Default), GetLocalized("Settings/ThemeDefault")));
            ThemeList.Add(KeyValuePair.Create(nameof(ElementTheme.Light), GetLocalized("Settings/ThemeLight")));
            ThemeList.Add(KeyValuePair.Create(nameof(ElementTheme.Dark), GetLocalized("Settings/ThemeDark")));
        }

        /// <summary>
        /// 字符串本地化
        /// </summary>
        public static string GetLocalized(string resource)
        {
            if (isInitialized)
            {
                try
                {
                    return resourceMap.GetValue(resource, currentResourceContext).ValueAsString;
                }
                catch (Exception currentResourceException)
                {
                    LogService.WriteLog(LoggingLevel.Warning, string.Format("Get resource context with langauge {0} failed.", _currentAppLanguage), currentResourceException);
                    try
                    {
                        return resourceMap.GetValue(resource, defaultResourceContext).ValueAsString;
                    }
                    catch (Exception defaultResourceException)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, string.Format("Get resource context string with langauge {0} failed.", _defaultAppLanguage), defaultResourceException);
                        return resource;
                    }
                }
            }
            else
            {
                LogService.WriteLog(LoggingLevel.Warning, "Have you forgot to initialize app's resources?", new NullReferenceException());
                return resource;
            }
        }
    }
}
