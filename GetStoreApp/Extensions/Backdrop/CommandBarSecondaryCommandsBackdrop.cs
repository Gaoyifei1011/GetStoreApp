using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using System;
using System.Numerics;
using Windows.Foundation.Diagnostics;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace GetStoreApp.Extensions.Backdrop
{
    /// <summary>
    /// 命令栏浮出控件背景色
    /// </summary>
    public partial class CommandBarSecondaryCommandsBackdrop : IDisposable
    {
        private static bool isInitialized;
        private static DesktopAcrylicController desktopAcrylicController;

        private bool isDisposed;
        private bool isConnected;
        private bool isLoaded;
        private Grid visualGrid;
        private Popup secondaryCommandsPopup;
        private ContentExternalBackdropLink backdropLink;
        private SystemBackdropConfiguration systemBackdropConfiguration;
        private Vector2 cornerRadius = new(8, 8);

        /// <summary>
        /// 初始化命令栏浮出控件背景色
        /// </summary>
        public static void Initialize()
        {
            if (!isInitialized && MainWindow.Current is not null)
            {
                isInitialized = true;
                MainWindow.Current.DispatcherQueue.EnsureSystemDispatcherQueue();
                desktopAcrylicController = new();
            }
        }

        /// <summary>
        /// 卸载浮出控件背景色
        /// </summary>
        public static void Uninitialize()
        {
            if (isInitialized)
            {
                isInitialized = false;
                desktopAcrylicController.Dispose();
                desktopAcrylicController = null;
            }
        }

        /// <summary>
        /// 命令栏初始化完成触发的事件
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isLoaded && sender is CommandBar commandBar && FindDescendant<Popup>(commandBar, "OverflowPopup") is Popup popup)
            {
                isLoaded = true;
                secondaryCommandsPopup = popup;
                secondaryCommandsPopup.Opened += OnOpened;
                secondaryCommandsPopup.ActualThemeChanged += OnActualThemeChanged;
            }
        }

        /// <summary>
        /// 弹出窗口主题发生更改时触发的事件
        /// </summary>

        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            if (isConnected && systemBackdropConfiguration is not null)
            {
                systemBackdropConfiguration.Theme = Enum.TryParse(Convert.ToString(secondaryCommandsPopup.ActualTheme), out SystemBackdropTheme systemBackdropTheme) ? systemBackdropTheme : SystemBackdropTheme.Default;
            }
        }

        /// <summary>
        /// 弹出窗口打开后触发的事件
        /// </summary>
        private void OnOpened(object sender, object args)
        {
            if (secondaryCommandsPopup is not null && secondaryCommandsPopup.FindName("SecondaryItemsControlShadowWrapper") is Grid grid)
            {
                Compositor compositor = ElementCompositionPreview.GetElementVisual(grid).Compositor;

                if (isInitialized)
                {
                    if (!isConnected)
                    {
                        isConnected = true;
                        visualGrid = new();
                        grid.Children.Insert(0, visualGrid);

                        backdropLink = ContentExternalBackdropLink.Create(compositor);
                        backdropLink.ExternalBackdropBorderMode = CompositionBorderMode.Soft;
                        backdropLink.PlacementVisual.Size = grid.ActualSize;
                        backdropLink.PlacementVisual.Clip = compositor.CreateRectangleClip(0, 0, grid.ActualSize.X, grid.ActualSize.Y, cornerRadius, cornerRadius, cornerRadius, cornerRadius);
                        backdropLink.PlacementVisual.BorderMode = CompositionBorderMode.Soft;

                        ElementCompositionPreview.SetElementChildVisual(visualGrid, backdropLink.PlacementVisual);

                        systemBackdropConfiguration = new()
                        {
                            IsInputActive = true,
                            Theme = Enum.TryParse(Convert.ToString(secondaryCommandsPopup.ActualTheme), out SystemBackdropTheme systemBackdropTheme) ? systemBackdropTheme : SystemBackdropTheme.Default
                        };
                        desktopAcrylicController.SetSystemBackdropConfiguration(systemBackdropConfiguration);
                        desktopAcrylicController.AddSystemBackdropTarget(backdropLink);
                    }
                    else if (backdropLink is not null && systemBackdropConfiguration is not null)
                    {
                        backdropLink.PlacementVisual.Size = grid.ActualSize;
                        backdropLink.PlacementVisual.Clip = compositor.CreateRectangleClip(0, 0, grid.ActualSize.X, grid.ActualSize.Y, cornerRadius, cornerRadius, cornerRadius, cornerRadius);
                    }
                }
            }
        }

        /// <summary>
        /// ElementTheme 转换到 SystemBackdropTheme
        /// </summary>

        /// <summary>
        /// 查找 CommandBar 的 OverflowPopup 子控件
        /// </summary>
        public static T FindDescendant<T>(DependencyObject parent, string childName = null) where T : DependencyObject
        {
            if (parent is not null)
            {
                for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, index);

                    if (child is T result && (childName is null || (child is FrameworkElement frameworkElement && string.Equals(frameworkElement.Name, childName))))
                    {
                        return result;
                    }

                    T foundChild = FindDescendant<T>(child, childName);
                    if (foundChild is not null)
                    {
                        return foundChild;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CommandBarSecondaryCommandsBackdrop()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                if (isInitialized && isConnected)
                {
                    desktopAcrylicController.RemoveSystemBackdropTarget(backdropLink);
                    backdropLink.Dispose();
                    backdropLink = null;

                    try
                    {
                        secondaryCommandsPopup.Opened -= OnOpened;
                        secondaryCommandsPopup.ActualThemeChanged -= OnActualThemeChanged;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CommandBarSecondaryCommandsBackdrop), nameof(Dispose), 1, e);
                    }

                    secondaryCommandsPopup = null;
                }

                isConnected = false;
                isDisposed = true;
            }
        }
    }
}
