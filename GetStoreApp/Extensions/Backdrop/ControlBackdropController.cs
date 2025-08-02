using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Extensions.Backdrop
{
    /// <summary>
    /// 控件背景色控制
    /// </summary>
    public static class ControlBackdropController
    {
        private static DesktopAcrylicController desktopAcrylicController;
        private static SystemBackdropConfiguration systemBackdropConfiguration;
        private static Compositor compositor;
        private static volatile bool isClosed = true;
        private static readonly List<ContentExternalBackdropLink> contentExternalBackdropLinkList = [];

        public static bool IsClosed
        {
            get { return isClosed; }
        }

        /// <summary>
        /// 初始化控件背景色
        /// </summary>
        public static void Initialize(MainWindow mainWindow)
        {
            if (desktopAcrylicController is null && mainWindow is not null && mainWindow.Content is FrameworkElement frameworkElement)
            {
                mainWindow.DispatcherQueue.EnsureSystemDispatcherQueue();
                compositor = mainWindow.Compositor;
                desktopAcrylicController = new();
                systemBackdropConfiguration = new()
                {
                    Theme = Enum.TryParse(Convert.ToString(frameworkElement.ActualTheme), out SystemBackdropTheme systemBackdropTheme) ? systemBackdropTheme : SystemBackdropTheme.Default
                };
                desktopAcrylicController.SetSystemBackdropConfiguration(systemBackdropConfiguration);
                isClosed = false;
            }
        }

        /// <summary>
        /// 卸载控件背景色
        /// </summary>
        public static void UnInitialize()
        {
            if (desktopAcrylicController is not null)
            {
                try
                {
                    compositor = null;
                    systemBackdropConfiguration = null;
                    desktopAcrylicController.RemoveAllSystemBackdropTargets();
                    desktopAcrylicController.Dispose();
                    foreach (ContentExternalBackdropLink contentExternalBackdropLink in contentExternalBackdropLinkList)
                    {
                        contentExternalBackdropLink.Dispose();
                    }
                    contentExternalBackdropLinkList.Clear();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(ControlBackdropController), nameof(UnInitialize), 1, e);
                }

                isClosed = true;
            }
        }

        /// <summary>
        /// 创建 ContentExternalBackdropLink
        /// </summary>
        public static ContentExternalBackdropLink CreateContentExternalBackdropLink()
        {
            if (!IsClosed && desktopAcrylicController is not null && compositor is not null)
            {
                ContentExternalBackdropLink contentExternalBackdropLink = ContentExternalBackdropLink.Create(compositor);
                contentExternalBackdropLinkList.Add(contentExternalBackdropLink);
                desktopAcrylicController.AddSystemBackdropTarget(contentExternalBackdropLink);
                return contentExternalBackdropLink;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更新控件主题色
        /// </summary>
        public static void UpdateControlTheme(ElementTheme elementTheme)
        {
            if (!IsClosed && desktopAcrylicController is not null && systemBackdropConfiguration is not null)
            {
                systemBackdropConfiguration.Theme = Enum.TryParse(Convert.ToString(elementTheme), out SystemBackdropTheme systemBackdropTheme) ? systemBackdropTheme : SystemBackdropTheme.Default;
            }
        }
    }
}
