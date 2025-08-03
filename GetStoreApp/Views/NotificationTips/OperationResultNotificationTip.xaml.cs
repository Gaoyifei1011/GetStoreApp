using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Composition;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.Views.NotificationTips
{
    /// <summary>
    /// 操作完成后应用内通知
    /// </summary>
    public sealed partial class OperationResultNotificationTip : TeachingTip, INotifyPropertyChanged
    {
        private bool isConnected;
        private Grid visualGrid;
        private Grid nonHeroContentRootGrid;
        private ContentExternalBackdropLink contentExternalBackdropLink;

        private bool _isSuccessOperation;

        public bool IsSuccessOperation
        {
            get { return _isSuccessOperation; }

            set
            {
                if (!Equals(_isSuccessOperation, value))
                {
                    _isSuccessOperation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccessOperation)));
                }
            }
        }

        private string _operationContent;

        public string OperationContent
        {
            get { return _operationContent; }

            set
            {
                if (!string.Equals(_operationContent, value))
                {
                    _operationContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OperationContent)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public OperationResultNotificationTip(OperationKind operationKind)
        {
            InitializeComponent();

            if (operationKind is OperationKind.FileLost)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/FileLost");
            }
            else if (operationKind is OperationKind.FolderPicker)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/FolderPickerFailed");
            }
            else if (operationKind is OperationKind.InstallingNotify)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/InstallingNotify");
            }
            else if (operationKind is OperationKind.LanguageChange)
            {
                IsSuccessOperation = true;
                OperationContent = ResourceService.GetLocalized("NotificationTip/LanguageChange");
            }
            else if (operationKind is OperationKind.NotElevated)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/NotElevated");
            }
            else if (operationKind is OperationKind.SelectEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SelectEmpty");
            }
            else if (operationKind is OperationKind.SelectFolderEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SelectFolderEmpty");
            }
            else if (operationKind is OperationKind.SelectPackageVolumeEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SelectPackageVolumeEmpty");
            }
            else if (operationKind is OperationKind.SourceNameEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SourceNameEmpty");
            }
            else if (operationKind is OperationKind.SourceUriEmpty)
            {
                IsSuccessOperation = false;
                OperationContent = ResourceService.GetLocalized("NotificationTip/SourceUriEmpty");
            }
        }

        public OperationResultNotificationTip(OperationKind operationKind, bool operationResult)
        {
            InitializeComponent();

            if (operationKind is OperationKind.CheckUpdate)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/NewestVersion");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/NotNewestVersion");
                }
            }
            else if (operationKind is OperationKind.Desktop)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/DesktopShortcutSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/DesktopShortcutFailed");
                }
            }
            else if (operationKind is OperationKind.DownloadCreate)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/DownloadCreateSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/DownloadCreateFailed");
                }
            }
            else if (operationKind is OperationKind.LogClean)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/LogCleanSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/LogCleanFailed");
                }
            }
            else if (operationKind is OperationKind.StartScreen)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/StartScreenSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/StartScreenFailed");
                }
            }
            else if (operationKind is OperationKind.Taskbar)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/TaskbarSuccessfully");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/TaskbarFailed");
                }
            }
            else if (operationKind is OperationKind.TerminateProcess)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/TerminateSuccess");
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/TerminateFailed");
                }
            }
        }

        public OperationResultNotificationTip(OperationKind operationKind, bool isMultiSelected, int count)
        {
            InitializeComponent();

            if (operationKind is OperationKind.ShareFailed)
            {
                if (isMultiSelected)
                {
                    IsSuccessOperation = false;
                    OperationContent = string.Format(ResourceService.GetLocalized("NotificationTip/ShareSelectedFailed"), count);
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = ResourceService.GetLocalized("NotificationTip/ShareFailed");
                }
            }
        }

        public OperationResultNotificationTip(OperationKind operationKind, bool operationResult, string reason)
        {
            InitializeComponent();

            if (operationKind is OperationKind.WinGetSource)
            {
                if (operationResult)
                {
                    IsSuccessOperation = true;
                    OperationContent = reason;
                }
                else
                {
                    IsSuccessOperation = false;
                    OperationContent = reason;
                }
            }
        }

        ~OperationResultNotificationTip()
        {
            if (nonHeroContentRootGrid is not null)
            {
                try
                {
                    nonHeroContentRootGrid.Loaded -= OnLoaded;
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            }
        }

        /// <summary>
        /// 自定义控件样式
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            nonHeroContentRootGrid = GetTemplateChild("NonHeroContentRootGrid") as Grid;
            if (nonHeroContentRootGrid is not null)
            {
                nonHeroContentRootGrid.Loaded += OnLoaded;
            }
        }

        /// <summary>
        /// 控件加载完成后触发的事件
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!ControlBackdropController.IsClosed && sender is Grid grid)
            {
                if (!isConnected)
                {
                    isConnected = true;
                    visualGrid = new();
                    Canvas.SetZIndex(visualGrid, -9999);
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
