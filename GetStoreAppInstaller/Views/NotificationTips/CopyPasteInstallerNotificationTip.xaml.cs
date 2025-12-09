using GetStoreAppInstaller.Extensions.Backdrop;
using Microsoft.UI.Composition;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using WinRT;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreAppInstaller.Views.NotificationTips
{
    /// <summary>
    /// 复制剪贴应用内通知
    /// </summary>
    public sealed partial class CopyPasteInstallerNotificationTip : TeachingTip, INotifyPropertyChanged
    {
        private bool isConnected;
        private Grid visualGrid;
        private Grid nonHeroContentRootGrid;
        private ContentExternalBackdropLink contentExternalBackdropLink;

        private bool _isSuccessfully;

        public bool IsSuccessfully
        {
            get { return _isSuccessfully; }

            set
            {
                if (!Equals(_isSuccessfully, value))
                {
                    _isSuccessfully = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccessfully)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CopyPasteInstallerNotificationTip(bool isSuccessfully = false)
        {
            InitializeComponent();
            IsSuccessfully = isSuccessfully;
        }

        /// <summary>
        /// 自定义控件样式
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            nonHeroContentRootGrid = GetTemplateChild("NonHeroContentRootGrid").As<Grid>();
            if (nonHeroContentRootGrid is not null)
            {
                nonHeroContentRootGrid.Loaded += OnLoaded;
            }
        }

        /// <summary>
        /// 教学提示关闭后触发的事件
        /// </summary>
        private void OnClosed(object sender, TeachingTipClosedEventArgs args)
        {
            try
            {
                if (nonHeroContentRootGrid is not null)
                {
                    nonHeroContentRootGrid.Loaded -= OnLoaded;
                }

                ControlBackdropController.RemoveContentExternalBackdropLink(contentExternalBackdropLink);
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
        }

        /// <summary>
        /// 控件加载完成后触发的事件
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (ControlBackdropController.IsLoaded && sender.As<Grid>() is Grid grid)
            {
                if (!isConnected)
                {
                    isConnected = true;
                    visualGrid = new();
                    Canvas.SetZIndex(visualGrid, -9999);
                    grid.HorizontalAlignment = HorizontalAlignment.Stretch;
                    grid.VerticalAlignment = VerticalAlignment.Stretch;
                    grid.Children.Add(visualGrid);
                    contentExternalBackdropLink = ControlBackdropController.CreateContentExternalBackdropLink();

                    if (contentExternalBackdropLink is not null)
                    {
                        contentExternalBackdropLink.ExternalBackdropBorderMode = CompositionBorderMode.Soft;
                        contentExternalBackdropLink.PlacementVisual.Size = grid.ActualSize;
                        contentExternalBackdropLink.PlacementVisual.Clip = contentExternalBackdropLink.PlacementVisual.Compositor.CreateRectangleClip(0, 0, grid.ActualSize.X, grid.ActualSize.Y,
                            new Vector2(Convert.ToSingle(CornerRadius.TopLeft), Convert.ToSingle(CornerRadius.TopLeft)),
                            new Vector2(Convert.ToSingle(CornerRadius.TopRight), Convert.ToSingle(CornerRadius.TopRight)),
                            new Vector2(Convert.ToSingle(CornerRadius.BottomRight), Convert.ToSingle(CornerRadius.BottomRight)),
                            new Vector2(Convert.ToSingle(CornerRadius.BottomLeft), Convert.ToSingle(CornerRadius.BottomLeft)));
                        ElementCompositionPreview.SetElementChildVisual(visualGrid, contentExternalBackdropLink.PlacementVisual);
                    }
                }
                else
                {
                    if (contentExternalBackdropLink is not null)
                    {
                        contentExternalBackdropLink.PlacementVisual.Size = grid.ActualSize;
                        contentExternalBackdropLink.PlacementVisual.Clip = contentExternalBackdropLink.PlacementVisual.Compositor.CreateRectangleClip(0, 0, grid.ActualSize.X, grid.ActualSize.Y,
                            new Vector2(Convert.ToSingle(CornerRadius.TopLeft), Convert.ToSingle(CornerRadius.TopLeft)),
                            new Vector2(Convert.ToSingle(CornerRadius.TopRight), Convert.ToSingle(CornerRadius.TopRight)),
                            new Vector2(Convert.ToSingle(CornerRadius.BottomRight), Convert.ToSingle(CornerRadius.BottomRight)),
                            new Vector2(Convert.ToSingle(CornerRadius.BottomLeft), Convert.ToSingle(CornerRadius.BottomLeft)));
                    }
                }
            }
        }
    }
}
