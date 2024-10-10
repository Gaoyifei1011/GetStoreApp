using GetStoreAppWebView.Helpers.Controls.Extensions;
using GetStoreAppWebView.Models;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.WindowsAPI.ComTypes;
using GetStoreAppWebView.WindowsAPI.PInvoke.User32;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Globalization;
using Windows.Graphics;
using Windows.Graphics.Display;
using Windows.System;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using WinRT;

namespace GetStoreAppWebView.UI.Controls
{
    /// <summary>
    /// 表示一个对象，该对象允许承载 Web 内容。
    /// </summary>
    public partial class WebView2 : ContentControl
    {
        private CoreWebView2EnvironmentOptions options = null;
        private CoreWebView2Environment coreWebViewEnvironment = null;
        private CoreWebView2Controller coreWebViewController = null;
        private CoreWebView2CompositionController coreWebViewCompositionController = null;
        private CoreWebView2 coreWebView = null;

        private bool everHadCoreWebView;
        private bool isLeftMouseButtonPressed;
        private bool isMiddleMouseButtonPressed;
        private bool isRightMouseButtonPressed;
        private bool isXButton1Pressed;
        private bool isXButton2Pressed;
        private bool hasMouseCapture;
        private bool hasPenCapture;
        private bool isRenderedRegistered;
        private bool isTextScaleChangedRegistered;
        private bool isPointerOver;
        private bool webHasFocus;
        private bool isVisible;
        private bool isHostVisible;
        private bool shouldShowMissingAnaheimWarning;
        private bool isCoreFailure_BrowserExited_State;
        private bool isClosed;
        private bool isImplicitCreationInProgress;
        private bool isExplicitCreationInProgress;

        private long manipulationModeChangedToken;
        private long visibilityChangedToken;

        private double rasterizationScale;

        private string stopNavigateOnUriChanged = null;

        private IntPtr xamlHostHwnd = IntPtr.Zero;
        private IntPtr tempHostHwnd = IntPtr.Zero;
        private IntPtr inputWindowHwnd = IntPtr.Zero;

        private readonly Dictionary<uint, bool> hasTouchCapture = [];
        private Point webViewScaledPosition = new();
        private Point webViewScaledSize = new();

        private CoreCursor oldCursor = null;
        private SpriteVisual visual = null;
        private XamlFocusChangeInfoModel xamlFocusChangeInfo = new();
        private readonly UISettings uiSettings = new();
        private PointInt32 hostWindowPosition = new() { X = 0, Y = 0 };

        private WNDPROC delegWndProc;

        public CoreWebView2 CoreWebView2
        {
            get { return coreWebView; }
        }

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }

            set { SetValue(SourceProperty, value); }
        }

        public static DependencyProperty SourceProperty { get; } =
            DependencyProperty.Register("Source", typeof(Uri), typeof(WebView2), new PropertyMetadata(default, OnSourcePropertyChanged));

        public bool CanGoBack
        {
            get { return (bool)GetValue(CanGoBackProperty); }
            set { SetValue(CanGoBackProperty, value); }
        }

        public static DependencyProperty CanGoBackProperty { get; } =
            DependencyProperty.Register("CanGoBack", typeof(bool), typeof(WebView2), new PropertyMetadata(false));

        public bool CanGoForward
        {
            get { return (bool)GetValue(CanGoForwardProperty); }
            set { SetValue(CanGoForwardProperty, value); }
        }

        public static DependencyProperty CanGoForwardProperty { get; } =
            DependencyProperty.Register("CanGoForward", typeof(bool), typeof(WebView2), new PropertyMetadata(false));

        public event TypedEventHandler<WebView2, CoreWebView2ProcessFailedEventArgs> CoreProcessFailed;

        public event TypedEventHandler<WebView2, CoreWebView2InitializedEventArgs> CoreWebView2Initialized;

        public event TypedEventHandler<WebView2, CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;

        public event TypedEventHandler<WebView2, CoreWebView2NavigationStartingEventArgs> NavigationStarting;

        public event TypedEventHandler<WebView2, CoreWebView2WebMessageReceivedEventArgs> WebMessageReceived;

        /// <summary>
        /// 关闭此元素的 DirectManipulation，以便所有的滚动和手势内部的 WebView 元素将由 Anaheim 处理。
        /// 默认情况下，WebView2 控件不包含在 Tab 导航中
        /// 设置 WebView2 的背景，以确保它在可视化层中的命中测试中是可见的。
        /// </summary>
        public WebView2()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            ManipulationMode = ManipulationModes.None;

            manipulationModeChangedToken = RegisterPropertyChangedCallback(ManipulationModeProperty, OnManipulationModePropertyChanged);
            visibilityChangedToken = RegisterPropertyChangedCallback(VisibilityProperty, OnVisibilityPropertyChanged);
            OnVisibilityPropertyChanged(null, null);

            IsTabStop = true;
            Background = new SolidColorBrush(Colors.Transparent);
        }

        ~WebView2()
        {
            CloseInternal(true);
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 在派生类中重写时，测量子元素在布局中所需的大小，并确定由 FrameworkElement 派生的类的大小。
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// 在派生类中重写时，为 FrameworkElement 派生类定位子元素并确定大小。
        /// 我们可以有一个子 Grid（参考 AddChildPanel）或子 TextBlock （参考 CreateMissingAnaheimWarning),确保它可以被访问。
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Content is FrameworkElement child)
            {
                child.Arrange(new Rect(new Point(0, 0), finalSize));
                return finalSize;
            }

            return base.ArrangeOverride(finalSize);
        }

        #endregion 第一部分：重写父类事件

        #region 第三部分：依赖属性挂载的事件

        /// <summary>
        /// Source 依赖属性发生变化触发的事件
        /// </summary>
        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as WebView2)?.OnPropertyChanged(args);
        }

        /// <summary>
        /// ManipulationMode 依赖属性值发生改变对应的事件
        /// </summary>
        private void OnManipulationModePropertyChanged(DependencyObject sender, DependencyProperty dependencyProperty)
        {
            Environment.FailFast("WebView2.ManipulationMode cannot be set to anything other than \"None\".");
        }

        /// <summary>
        /// Visibility 依赖属性值发生改变对应的事件
        /// </summary>
        private void OnVisibilityPropertyChanged(DependencyObject sender, DependencyProperty dependencyProperty)
        {
            UpdateRenderedSubscriptionAndVisibility();
        }

        #endregion 第三部分：依赖属性挂载的事件

        #region 第四部分：WebView2 控件挂载的事件

        /// <summary>
        /// WebView2 控件加载到可视化树时触发的事件
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            // OnLoaded 和 Onunload 是从 XAML 异步触发的，不幸的是它们可能是乱序的。如果元素从树 A 中移除并添加到树 B 中，我们可以在看到树 A 的 Unloaded 事件之前得到树 B 的 Loaded 事件。
            // 当我们得到一个 Loaded / Unloaded 事件时，检查 IsLoaded 属性。如果它与我们所处的事件不匹配，则不需要做任何事情，因为其他处理程序会处理它。
            // 当我们看到一个加载的事件，当我们已经或已经加载，删除 XamlRootChanged 事件处理程序的旧树，并绑定到新的。
            // 如果我们没有加载，我们就没有什么可做的，因为 Unloaded 已经处理了所有的事情
            if (IsLoaded)
            {
                TryCompleteInitialization();

                if (VisualTreeHelper.GetChildrenCount(this) > 0 && VisualTreeHelper.GetChild(this, 0) is ContentPresenter contentPresenter)
                {
                    contentPresenter.Background = Background;
                    contentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    contentPresenter.VerticalAlignment = VerticalAlignment.Stretch;
                    contentPresenter.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    contentPresenter.VerticalContentAlignment = VerticalAlignment.Stretch;
                }
            }
        }

        /// <summary>
        /// WebView2 控件从可视化树卸载时触发的事件
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            // 注意，在执行卸载操作之前，我们总是检查 IsLoaded 属性，因为我们可能会在新树 B 的加载事件发生后，得到旧树 A 的卸载事件。
            // 参考 WebView2::OnLoaded 中的注释
            if (!IsLoaded)
            {
                UpdateRenderedSubscriptionAndVisibility();

                XamlRoot.Changed -= OnXamlRootChanged;
                Window.Current.VisibilityChanged -= OnVisibilityChanged;

                DisconnectFromRootVisualTarget();
            }
        }

        /// <summary>
        /// WebView2 控件具有焦点时按下键盘键时发生的事件
        /// 由于 WebView 需要 HWND 焦点(通过 OnGotFocus -> MoveFocus)， Xaml 假定焦点是由于外部原因丢失的。当下一个未处理的 TAB KeyDown 到达 XamlRoot 元素时，Xaml 的 FocusManager 将尝试将焦点移动到下一个 Xaml 控件并强制 HWND 焦点回到自身，将 Xaml 焦点从 WebView2 控件中弹出。我们在 KeyDown 处理程序中标记 TAB 已处理，以便 XamlRoot 的选项卡处理忽略它。
        /// 如果 WebView2 已经关闭，那么我们应该让 Xaml 的选项卡处理处理它。
        /// </summary>
        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Tab && !isClosed)
            {
                args.Handled = true;
            }
        }

        /// <summary>
        /// 当指针设备在此元素中启动 按下 操作时发生的事件
        /// </summary>
        private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            // Chromium 处理鼠标消息 WM_MOUSEXXX，处理触摸消息 WM_POINTERXXX
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;
            PointerPoint pointerPoint = args.GetCurrentPoint(this);
            WindowMessage message = WindowMessage.WM_NULL;

            if (deviceType is PointerDeviceType.Mouse)
            {
                // WebView 使用鼠标捕获来避免在元素外发生的指针释放事件，结束指针在 WebView 内部的按下状态。例如，滚动条正在使用，鼠标在被释放之前被移出 WebView 的边界，WebView 将错过释放事件，并且在重新进入 WebView 时，鼠标仍然会使滚动条移动，就像被选中一样。
                hasMouseCapture = CapturePointer(args.Pointer);

                PointerPointProperties properties = pointerPoint.Properties;

                if (properties.IsLeftButtonPressed)
                {
                    message = WindowMessage.WM_LBUTTONDOWN;
                    isLeftMouseButtonPressed = true;
                }
                else if (properties.IsMiddleButtonPressed)
                {
                    message = WindowMessage.WM_MBUTTONDOWN;
                    isMiddleMouseButtonPressed = true;
                }
                else if (properties.IsRightButtonPressed)
                {
                    message = WindowMessage.WM_RBUTTONDOWN;
                    isRightMouseButtonPressed = true;
                }
                else if (properties.IsXButton1Pressed)
                {
                    message = WindowMessage.WM_XBUTTONDOWN;
                    isXButton1Pressed = true;
                }
                else if (properties.IsXButton2Pressed)
                {
                    message = WindowMessage.WM_XBUTTONDOWN;
                    isXButton2Pressed = true;
                }
            }
            else if (deviceType is PointerDeviceType.Touch)
            {
                message = WindowMessage.WM_POINTERDOWN;
                hasTouchCapture.Add(pointerPoint.PointerId, CapturePointer(args.Pointer));
            }
            else if (deviceType is PointerDeviceType.Pen)
            {
                message = WindowMessage.WM_POINTERDOWN;
                hasPenCapture = CapturePointer(args.Pointer);
            }

            // 由于 OnXamlPointerMessage() 将标记已处理的参数，Xaml FocusManager 将忽略这个 Pressed 事件，当它出现到 XamlRoot，而不是按预期设置焦点。
            //因此，我们需要在 WebView2 上手动设置 Xaml Focus (Pointer)。
            if (IsTabStop)
            {
                _ = FocusManager.TryFocusAsync(this, FocusState.Pointer);
            }

            OnXamlPointerMessage(message, args);
        }

        /// <summary>
        /// 在释放之前启动 按下 操作的指针设备时发生的事件
        /// </summary>
        private void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            // Chromium 处理鼠标消息 WM_MOUSEXXX，处理触摸消息 WM_POINTERXXX
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;
            PointerPoint pointerPoint = args.GetCurrentPoint(this);
            WindowMessage message;

            // 获取处理多点触控捕获的指针 id
            uint id = pointerPoint.PointerId;

            if (deviceType is PointerDeviceType.Mouse)
            {
                if (isLeftMouseButtonPressed)
                {
                    message = WindowMessage.WM_LBUTTONUP;
                    isLeftMouseButtonPressed = false;
                }
                else if (isMiddleMouseButtonPressed)
                {
                    message = WindowMessage.WM_MBUTTONUP;
                    isMiddleMouseButtonPressed = false;
                }
                else if (isRightMouseButtonPressed)
                {
                    message = WindowMessage.WM_RBUTTONUP;
                    isRightMouseButtonPressed = false;
                }
                else if (isXButton1Pressed)
                {
                    message = WindowMessage.WM_XBUTTONUP;
                    isXButton1Pressed = false;
                }
                else if (isXButton2Pressed)
                {
                    message = WindowMessage.WM_XBUTTONUP;
                    isXButton2Pressed = false;
                }
                else
                {
                    // 不能保证我们会在 PointerReleased 之前得到一个 PointerPressed。
                    // 例如，可以在 WebView 旁边按下鼠标，拖拽到 WebView 中，然后释放。这是一个有效的案例，不应该崩溃。
                    //因为我们不能总是知道在释放之前哪个按钮被按下了，所以我们不能将此消息转发到 CoreWebView2。
                    return;
                }

                if (hasMouseCapture)
                {
                    ReleasePointerCapture(args.Pointer);
                    hasMouseCapture = false;
                }
            }
            else
            {
                if (hasTouchCapture.ContainsKey(id))
                {
                    ReleasePointerCapture(args.Pointer);
                    hasTouchCapture.Remove(id);
                }

                if (hasPenCapture)
                {
                    ReleasePointerCapture(args.Pointer);
                    hasPenCapture = false;
                }

                message = WindowMessage.WM_POINTERUP;
            }

            OnXamlPointerMessage(message, args);
        }

        /// <summary>
        /// 当指针保持在此元素的命中测试区域内时，指针移动时发生的事件
        /// </summary>

        private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            // Chromium 处理 WM_MOUSEXXX 用于鼠标，WM_POINTERXXX 用于触摸
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;
            WindowMessage message = deviceType is PointerDeviceType.Mouse ? WindowMessage.WM_MOUSEMOVE : WindowMessage.WM_POINTERUPDATE;
            OnXamlPointerMessage(message, args);
        }

        /// <summary>
        /// 当指针滚轮的增量值更改时发生的事件
        /// </summary>
        private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs args)
        {
            // Chromium 处理 WM_MOUSEXXX 用于鼠标，WM_POINTERXXX 用于触摸
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;
            PointerPoint pointerPoint = args.GetCurrentPoint(this);
            PointerPointProperties properties = pointerPoint.Properties;
            WindowMessage message;

            if (deviceType is PointerDeviceType.Mouse)
            {
                message = properties.IsHorizontalMouseWheel ? WindowMessage.WM_MOUSEHWHEEL : WindowMessage.WM_MOUSEWHEEL;
            }
            else
            {
                message = WindowMessage.WM_POINTERWHEEL;
            }

            OnXamlPointerMessage(message, args);
        }

        /// <summary>
        /// 当指针离开此元素的命中测试区域时发生的事件
        /// </summary>
        private void OnPointerExited(object sender, PointerRoutedEventArgs args)
        {
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;
            WindowMessage message;

            if (isPointerOver)
            {
                isPointerOver = false;
                CoreWindow.GetForCurrentThread().PointerCursor = oldCursor;
                oldCursor = null;
            }

            if (deviceType is PointerDeviceType.Mouse)
            {
                message = WindowMessage.WM_MOUSELEAVE;
                if (!hasMouseCapture)
                {
                    ResetMouseInputState();
                }
            }
            else
            {
                message = WindowMessage.WM_POINTERLEAVE;
            }

            OnXamlPointerMessage(message, args);
        }

        /// <summary>
        /// 当指针进入此元素的命中测试区域时发生的事件
        /// </summary>
        private void OnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;
            isPointerOver = true;
            oldCursor = CoreWindow.GetForCurrentThread().PointerCursor;

            UpdateCoreWindowCursor();

            if (deviceType is not PointerDeviceType.Mouse)
            {
                OnXamlPointerMessage(WindowMessage.WM_POINTERENTER, args);
            }
        }

        /// <summary>
        /// 当进行接触的指针异常失去接触时发生的事件
        /// </summary>
        private void OnPointerCanceled(object sender, PointerRoutedEventArgs args)
        {
            ResetPointerHelper(args);
        }

        /// <summary>
        /// 当此元素以前持有的指针捕获移动到另一个元素或其他位置时发生的事件
        /// </summary>
        private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs args)
        {
            ResetPointerHelper(args);
        }

        /// <summary>
        /// WebView2 控件接收焦点之前发生的事件
        /// </summary>
        private void OnGettingFocus(object sender, GettingFocusEventArgs args)
        {
            if (coreWebView is not null)
            {
                CoreWebView2MoveFocusReason moveFocusReason = CoreWebView2MoveFocusReason.Programmatic;

                if (args.InputDevice is FocusInputDeviceKind.Keyboard)
                {
                    if (args.Direction is FocusNavigationDirection.Next)
                    {
                        moveFocusReason = CoreWebView2MoveFocusReason.Next;
                    }
                    else if (args.Direction is FocusNavigationDirection.Previous)
                    {
                        moveFocusReason = CoreWebView2MoveFocusReason.Previous;
                    }
                }

                xamlFocusChangeInfo.storedMoveFocusReason = moveFocusReason;
                xamlFocusChangeInfo.isPending = true;
            }
        }

        /// <summary>
        /// WebView2 控件收到焦点时发生的事件
        /// </summary>
        private void OnGotFocus(object sender, RoutedEventArgs args)
        {
            if (coreWebView is not null && xamlFocusChangeInfo.isPending)
            {
                MoveFocusIntoCoreWebView(xamlFocusChangeInfo.storedMoveFocusReason);
                xamlFocusChangeInfo.isPending = false;
            }
        }

        #endregion 第四部分：WebView2 控件挂载的事件

        #region 第五部分：CoreWebView2 挂载的事件

        /// <summary>
        /// CoreWebView2 导航开始发生的事件
        /// </summary>
        private void OnNavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            UpdateSourceInternal();
            NavigationStarting?.Invoke(this, args);
        }

        /// <summary>
        /// CoreWebView2 Source 属性更改时触发的事件
        /// </summary>
        private void OnSourceChanged(CoreWebView2 sender, CoreWebView2SourceChangedEventArgs args)
        {
            UpdateSourceInternal();
        }

        /// <summary>
        /// CoreWebView2 导航完成发生的事件
        /// </summary>
        private void OnNavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            NavigationCompleted?.Invoke(this, args);
        }

        /// <summary>
        /// CoreWebView2 接收到消息时触发的事件
        /// </summary>
        private void OnWebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            WebMessageReceived?.Invoke(this, args);
        }

        /// <summary>
        /// CoreWebView2 进程意外结束或无响应时触发的事件
        /// </summary>
        private void OnProcessFailed(CoreWebView2 sender, CoreWebView2ProcessFailedEventArgs args)
        {
            CoreWebView2ProcessFailedKind coreProcessFailedKind = args.ProcessFailedKind;
            if (coreProcessFailedKind is CoreWebView2ProcessFailedKind.BrowserProcessExited)
            {
                isCoreFailure_BrowserExited_State = true;

                // CoreWebView2 负责在关闭主机时清除事件处理程序，但我们仍然需要重置事件令牌
                UnregisterCoreEventHandlers();

                // 将它们清空，这样我们就不能再使用它们了
                coreWebViewCompositionController = null;
                coreWebViewController = null;
                coreWebView = null;
                ResetProperties();
            }

            CoreProcessFailed?.Invoke(this, args);
        }

        /// <summary>
        /// 当用户尝试按 Tab 键退出 WebView 时触发的事件
        /// </summary>
        private void OnMoveFocusRequested(object sender, CoreWebView2MoveFocusRequestedEventArgs args)
        {
            CoreWebView2MoveFocusReason moveFocusRequestedReason = args.Reason;

            if (moveFocusRequestedReason is CoreWebView2MoveFocusReason.Next || moveFocusRequestedReason is CoreWebView2MoveFocusReason.Previous)
            {
                FocusNavigationDirection xamlDirection = moveFocusRequestedReason is CoreWebView2MoveFocusReason.Next ? FocusNavigationDirection.Next : FocusNavigationDirection.Previous;
                FindNextElementOptions findNextElementOptions = new();

                UIElement contentRoot = new Func<UIElement>(delegate
                {
                    XamlRoot xamlRoot = XamlRoot;
                    if (xamlRoot is not null)
                    {
                        return xamlRoot.Content;
                    }

                    return Window.Current.Content;
                })();

                if (contentRoot is not null)
                {
                    findNextElementOptions.SearchRoot = contentRoot;
                    if (FocusManager.FindNextElement(xamlDirection, findNextElementOptions) is DependencyObject nextElement)
                    {
                        if (nextElement is WebView2 webView2)
                        {
                            // 如果下一个元素是这个 WebView，那么我们是唯一可聚焦的元素。将焦点移回 WebView，否则我们会被卡在页面的顶部或底部，而不是循环。
                            webView2.MoveFocusIntoCoreWebView(moveFocusRequestedReason);
                            args.Handled = true;
                        }
                        else
                        {
                            // 如果 CoreWebView 也通过 TAB 以外的东西失去焦点（LostFocus事件触发），并且 TAB 处理到达较晚(例如由于较长的延迟)，跳过手动移动 Xaml 焦点到下一个元素。
                            if (webHasFocus)
                            {
                                _ = FocusManager.TryMoveFocusAsync(xamlDirection, findNextElementOptions);
                                webHasFocus = false;
                            }

                            args.Handled = true;
                        }
                    }
                }
            }
            //如果 nextElement 为 null，则通过不标记 Handled 来保持焦点。
        }

        /// <summary>
        /// 当 CoreWebView2 失去焦点时触发的事件
        /// </summary>
        private void OnLostFocus(CoreWebView2Controller sender, object args)
        {
            // 当边缘焦点通过 TAB 导航以外的方式丢失时，取消对边缘焦点的跟踪。
            webHasFocus = false;
        }

        #endregion 第五部分：CoreWebView2 挂载的事件

        #region 第六部分：自定义事件

        /// <summary>
        /// 在核心呈现过程呈现帧后立即发生的事件
        /// </summary>
        private void OnRendered(object sender, object args)
        {
            if (coreWebView is not null)
            {
                // 检查 WebView 在应用中的位置是否发生了变化
                CheckAndUpdateWebViewPosition();
                // 检查窗口本身的位置是否发生了变化
                CheckAndUpdateWindowPosition();
                // 检查父元素的 Visibility 属性是否已更改
                CheckAndUpdateVisibility();
            }
        }

        /// <summary>
        /// 处理 CoreWindow 显示状态发生更改时触发的事件
        /// </summary>
        private void OnVisibilityChanged(object sender, VisibilityChangedEventArgs args)
        {
            XamlRootChangedHelper(false);
        }

        /// <summary>
        /// 控件大小发生改变时触发的事件
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            SetCoreWebViewAndVisualSize(args.NewSize.Width, args.NewSize.Height);
        }

        /// <summary>
        /// XamlRoot 属性更改发生的事件
        /// </summary>
        private void OnXamlRootChanged(XamlRoot sender, XamlRootChangedEventArgs args)
        {
            XamlRootChangedHelper(false);
        }

        /// <summary>
        /// 快捷键被激活（按下或按住）时触发的事件
        /// </summary>
        private void OnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.VirtualKey is VirtualKey.Tab && args.EventType is CoreAcceleratorKeyEventType.KeyDown && webHasFocus && args.Handled)
            {
                UIntPtr wparam = new((int)VirtualKey.Tab);
                IntPtr lparam = new(WebView2Helper.MakeLParam(0x0001, 0x000f));

                if (inputWindowHwnd != IntPtr.Zero)
                {
                    IntPtr inputwindowHwnd = User32Library.GetFocus();
                    if (inputwindowHwnd != IntPtr.Zero)
                    {
                        inputWindowHwnd = inputwindowHwnd;
                    }
                }

                _ = User32Library.SendMessage(inputWindowHwnd, WindowMessage.WM_KEYDOWN, wparam, lparam);
            }
        }

        /// <summary>
        /// 当 WebView 认为应该更改光标时触发的事件
        /// </summary>
        private void OnCursorChanged(CoreWebView2CompositionController sender, object args)
        {
            UpdateCoreWindowCursor();
        }

        /// <summary>
        /// 更改系统文本大小设置时发生的事件
        /// </summary>
        private async void OnTextScaleFactorChanged(UISettings sender, object args)
        {
            // OnTextScaleFactorChanged 发生在非 UI 线程中，使用 Dispatcher 在 UI 线程中调用 UpdateCoreWebViewScale。
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateCoreWebViewScale);
        }

        #endregion 第六部分：自定义事件

        #region 第七部分：公共方法

        /// <summary>
        /// 显式触发控件 CoreWebView2 的初始化。
        /// </summary>
        public async Task EnsureCoreWebView2Async()
        {
            //如果 CoreWebView2 已经存在，立即/同步返回
            if (coreWebView is null)
            {
                isExplicitCreationInProgress = true;
                await CreateCoreObjectsAsync();
            }
        }

        /// <summary>
        /// 在 WebView2 的顶级文档中异步执行提供的脚本。
        /// </summary>
        public async Task<string> ExecuteScriptAsync(string javascriptCode)
        {
            if (coreWebView is not null)
            {
                return await coreWebView.ExecuteScriptAsync(javascriptCode);
            }
            else
            {
                throw new MethodAccessException(string.Format("ExecuteScriptAsync(): {0}", ResourceService.GetLocalized("WebView/WebView2NotPresent")));
            }
        }

        /// <summary>
        /// 重载当前页。
        /// </summary>
        public void Reload()
        {
            if (coreWebView is not null)
            {
                coreWebView.Reload();
            }
            else
            {
                if (everHadCoreWebView)
                {
                    throw new MethodAccessException(string.Format("ExecuteScriptAsync(): {0}", ResourceService.GetLocalized("WebView/WebView2NotPresentClosed")));
                }
                else
                {
                    throw new MethodAccessException(string.Format("ExecuteScriptAsync(): {0}", ResourceService.GetLocalized("WebView/WebView2NotPresent")));
                }
            }
        }

        /// <summary>
        /// 将 WebView 导航到导航历史记录中的下一页。
        /// </summary>
        public void GoForward()
        {
            if (coreWebView is not null && CanGoForward)
            {
                coreWebView.GoForward();
            }
        }

        /// <summary>
        /// 将 WebView 导航到导航历史记录中的上一页。
        /// </summary>
        public void GoBack()
        {
            if (coreWebView is not null && CanGoBack)
            {
                coreWebView.GoBack();
            }
        }

        /// <summary>
        /// 启动指向新 HTML 文档的导航。
        /// </summary>
        public void NavigateToString(string htmlContent)
        {
            if (coreWebView is not null)
            {
                coreWebView.NavigateToString(htmlContent);
            }
            else
            {
                throw new MethodAccessException(string.Format("ExecuteScriptAsync(): {0}", ResourceService.GetLocalized("WebView/WebView2NotPresent")));
            }
        }

        /// <summary>
        /// 关闭 WebView2 对象。
        /// </summary>
        public void Close()
        {
            CloseInternal(false);
        }

        #endregion 第七部分：公共方法

        #region 第八部分：私有方法

        /// <summary>
        /// WebView2 临时窗口的窗口过程
        /// </summary>
        private IntPtr OnWndProc(IntPtr hWnd, WindowMessage msg, UIntPtr wParam, IntPtr lParam)
        {
            return User32Library.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        /// <summary>
        /// Source 依赖属性发生变化时，主动切换网页
        /// </summary>
        private async void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            DependencyProperty property = args.Property;

            if (property == SourceProperty)
            {
                Uri newValueUri = args.NewValue as Uri;

                if (ShouldNavigate(newValueUri))
                {
                    if (!isClosed)
                    {
                        //如果还没有准备好，等待创建 CoreWebView。
                        if (coreWebView is null)
                        {
                            if (isExplicitCreationInProgress)
                            {
                                await EnsureCoreWebView2Async();
                            }
                            else if (isImplicitCreationInProgress)
                            {
                                return;
                            }
                            //首次创建请求
                            else
                            {
                                isImplicitCreationInProgress = true;
                                await CreateCoreObjectsAsync();
                            }
                        }

                        if (coreWebView is not null)
                        {
                            if (!isClosed)
                            {
                                Uri updatedUri = Source;
                                if (!updatedUri.Equals(newValueUri) && ShouldNavigate(updatedUri))
                                {
                                    coreWebView.Navigate(updatedUri.AbsoluteUri);
                                }
                                else
                                {
                                    coreWebView.Navigate(newValueUri.AbsoluteUri);
                                }
                            }
                            else
                            {
                                throw new COMException(ResourceService.GetLocalized("WebView/WebView2Closed"), unchecked((int)0x80000013)); // RO_E_CLOSED
                            }
                        }
                    }
                    else
                    {
                        throw new COMException(ResourceService.GetLocalized("WebView/WebView2Closed"), unchecked((int)0x80000013)); // RO_E_CLOSED
                    }
                }
            }
        }

        /// <summary>
        /// 处理 Xaml 指针消息
        /// </summary>
        private void OnXamlPointerMessage(WindowMessage message, PointerRoutedEventArgs args)
        {
            // 设置 Handled 来防止祖先动作，比如 ScrollViewer 把焦点放在 PointerPressed/PointerReleased 上。
            args.Handled = true;

            if (coreWebView is not null && coreWebViewCompositionController is not null)
            {
                PointerPoint logicalPointerPoint = args.GetCurrentPoint(this);
                Point logicalPoint = logicalPointerPoint.Position;
                Point physicalPoint = new((int)(logicalPoint.X * rasterizationScale), (int)(logicalPoint.Y * rasterizationScale));
                PointerDeviceType deviceType = args.Pointer.PointerDeviceType;

                // 处理鼠标消息
                if (deviceType is PointerDeviceType.Mouse)
                {
                    if (message is WindowMessage.WM_MOUSELEAVE)
                    {
                        coreWebViewCompositionController.SendMouseInput((CoreWebView2MouseEventKind)message, CoreWebView2MouseEventVirtualKeys.None, 0, new Point(0, 0));
                    }
                    else
                    {
                        IntPtr lparam = WebView2Helper.PackIntoWin32StylePointerArgs_lparam(physicalPoint);
                        UIntPtr wparam = WebView2Helper.PackIntoWin32StyleMouseArgs_wparam(message, args, logicalPointerPoint);

                        Point coords_win32 = new((short)WebView2Helper.LoWord(lparam), (short)WebView2Helper.HiWord(lparam));
                        Point coords = new(coords_win32.X, coords_win32.Y);

                        //对于鼠标滚轮滚动和 XBUTTON 事件，鼠标数据是非零的
                        uint mouse_data = 0;
                        if (message is WindowMessage.WM_MOUSEWHEEL || message is WindowMessage.WM_MOUSEHWHEEL)
                        {
                            mouse_data = (uint)WebView2Helper.GetWheelDataWParam(wparam);
                        }
                        else if (message is WindowMessage.WM_XBUTTONDOWN || message is WindowMessage.WM_XBUTTONUP || message is WindowMessage.WM_XBUTTONDBLCLK)
                        {
                            mouse_data = (uint)WebView2Helper.GetXButtonWParam(wparam);
                        }

                        coreWebViewCompositionController.SendMouseInput((CoreWebView2MouseEventKind)message, (CoreWebView2MouseEventVirtualKeys)WebView2Helper.GetKeystateWParam(wparam), mouse_data, coords);
                    }
                }
                else if (deviceType is PointerDeviceType.Touch || deviceType is PointerDeviceType.Pen)
                {
                    CoreWebView2PointerInfo outputPoint = coreWebViewEnvironment.CreateCoreWebView2PointerInfo();
                    PointerPointProperties inputProperties = logicalPointerPoint.Properties;
                    POINTER_FLAGS pointerFlags = POINTER_FLAGS.POINTER_FLAG_NONE;

                    // 处理笔触摸的消息
                    if (deviceType is PointerDeviceType.Pen)
                    {
                        PEN_FLAGS penFlags = PEN_FLAGS.PEN_FLAG_NONE;
                        if (inputProperties.IsBarrelButtonPressed)
                        {
                            penFlags |= PEN_FLAGS.PEN_FLAG_BARREL;
                        }
                        if (inputProperties.IsInverted)
                        {
                            penFlags |= PEN_FLAGS.PEN_FLAG_INVERTED;
                        }
                        if (inputProperties.IsEraser)
                        {
                            penFlags |= PEN_FLAGS.PEN_FLAG_ERASER;
                        }
                        outputPoint.PenFlags = (uint)penFlags;
                        outputPoint.PenMask = (uint)(PEN_MASK.PEN_MASK_PRESSURE | PEN_MASK.PEN_MASK_ROTATION | PEN_MASK.PEN_MASK_TILT_X | PEN_MASK.PEN_MASK_TILT_Y);
                        outputPoint.PenPressure = (uint)inputProperties.Pressure * 1024;
                        outputPoint.PenRotation = (uint)inputProperties.Twist;
                        outputPoint.PenTiltX = (int)inputProperties.XTilt;
                        outputPoint.PenTiltY = (int)inputProperties.YTilt;
                        outputPoint.PointerKind = (uint)POINTER_INPUT_TYPE.PT_PEN;

                        if (logicalPointerPoint.IsInContact)
                        {
                            pointerFlags |= POINTER_FLAGS.POINTER_FLAG_INCONTACT;

                            if (!inputProperties.IsBarrelButtonPressed)
                            {
                                pointerFlags |= POINTER_FLAGS.POINTER_FLAG_FIRSTBUTTON;
                            }
                            else
                            {
                                pointerFlags |= POINTER_FLAGS.POINTER_FLAG_SECONDBUTTON;
                            }
                        }
                    }
                    // 处理触摸屏的消息
                    else if (deviceType is PointerDeviceType.Touch)
                    {
                        outputPoint.TouchFlags = (uint)TOUCH_FLAGS.TOUCH_FLAG_NONE;
                        outputPoint.TouchMask = (uint)(TOUCH_MASK.TOUCH_MASK_CONTACTAREA | TOUCH_MASK.TOUCH_MASK_ORIENTATION | TOUCH_MASK.TOUCH_MASK_PRESSURE);

                        Rect touchContact = new(inputProperties.ContactRect.X * rasterizationScale, inputProperties.ContactRect.Y * rasterizationScale, inputProperties.ContactRect.Width * rasterizationScale, inputProperties.ContactRect.Height * rasterizationScale);
                        outputPoint.TouchContact = touchContact;
                        Rect touchContactRaw = new(inputProperties.ContactRectRaw.X * rasterizationScale, inputProperties.ContactRectRaw.Y * rasterizationScale, inputProperties.ContactRectRaw.Width * rasterizationScale, inputProperties.ContactRectRaw.Height * rasterizationScale);
                        outputPoint.TouchContactRaw = touchContactRaw;
                        outputPoint.TouchOrientation = (uint)inputProperties.Orientation;
                        outputPoint.TouchPressure = (uint)inputProperties.Pressure * 1024;
                        outputPoint.PointerKind = (uint)POINTER_INPUT_TYPE.PT_TOUCH;

                        if (logicalPointerPoint.IsInContact)
                        {
                            pointerFlags |= POINTER_FLAGS.POINTER_FLAG_INCONTACT;
                            pointerFlags |= POINTER_FLAGS.POINTER_FLAG_FIRSTBUTTON;
                        }

                        if (inputProperties.PointerUpdateKind is PointerUpdateKind.LeftButtonPressed)
                        {
                            pointerFlags |= POINTER_FLAGS.POINTER_FLAG_NEW;
                        }
                    }

                    if (inputProperties.IsInRange)
                    {
                        pointerFlags |= POINTER_FLAGS.POINTER_FLAG_INRANGE;
                    }

                    if (inputProperties.IsPrimary)
                    {
                        pointerFlags |= POINTER_FLAGS.POINTER_FLAG_PRIMARY;
                    }

                    if (inputProperties.IsCanceled)
                    {
                        pointerFlags |= POINTER_FLAGS.POINTER_FLAG_CANCELED;
                    }

                    if (inputProperties.TouchConfidence)
                    {
                        pointerFlags |= POINTER_FLAGS.POINTER_FLAG_CONFIDENCE;
                    }

                    if (inputProperties.PointerUpdateKind is PointerUpdateKind.LeftButtonPressed)
                    {
                        pointerFlags |= POINTER_FLAGS.POINTER_FLAG_DOWN;
                    }

                    if (inputProperties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
                    {
                        pointerFlags |= POINTER_FLAGS.POINTER_FLAG_UP;
                    }

                    if (inputProperties.PointerUpdateKind is PointerUpdateKind.Other)
                    {
                        pointerFlags |= POINTER_FLAGS.POINTER_FLAG_UPDATE;
                    }

                    outputPoint.PointerId = args.Pointer.PointerId;
                    outputPoint.FrameId = logicalPointerPoint.FrameId;
                    outputPoint.PointerFlags = (uint)pointerFlags;

                    Point pixelLocation = new((int)(logicalPointerPoint.Position.X * rasterizationScale), (int)(logicalPointerPoint.Position.Y * rasterizationScale));
                    outputPoint.PixelLocation = pixelLocation;
                    Point rawPixelLocation = new((int)(logicalPointerPoint.RawPosition.X * rasterizationScale), (int)(logicalPointerPoint.RawPosition.Y * rasterizationScale));
                    outputPoint.PixelLocationRaw = rawPixelLocation;

                    outputPoint.Time = (uint)(logicalPointerPoint.Timestamp / 1000);
                    outputPoint.HistoryCount = (uint)args.GetIntermediatePoints(this).Count;
                    outputPoint.HistoryCount = (uint)args.GetIntermediatePoints(this).Count;
                    outputPoint.PerformanceCount = (ulong)Stopwatch.Frequency * logicalPointerPoint.Timestamp / 1000000;

                    coreWebViewCompositionController.SendPointerInput((CoreWebView2PointerEventKind)message, outputPoint);
                }
            }
        }

        /// <summary>
        /// 检查所有的祖先控件是否已显示
        /// </summary>
        private bool AreAllAncestorsVisible()
        {
            bool allAncestorsVisible = true;
            DependencyObject parentAsDO = Parent;

            while (parentAsDO is not null)
            {
                if (parentAsDO is UIElement parentAsUIE)
                {
                    Visibility parentVisibility = parentAsUIE.Visibility;

                    if (parentVisibility is Visibility.Collapsed)
                    {
                        allAncestorsVisible = false;
                        break;
                    }
                    parentAsDO = VisualTreeHelper.GetParent(parentAsDO);
                }
            }

            return allAncestorsVisible;
        }

        /// <summary>
        /// 检查 WebView 在应用中的位置是否发生了变化
        /// </summary>
        private void CheckAndUpdateWebViewPosition()
        {
            //如果 WebView2 刚刚从树中删除，则跳过此工作-否则 CoreWebView2.Bounds 更新可能导致闪烁。
            // WebView2 从树中移除后，这个处理程序在帧的渲染通道中再次运行一次（WebView2。HandleRendered()）。被删除元素的 ActualWidth 或 ActualHeight 现在可以计算为零
            //(如果宽度或高度没有明确设置)，导致 0 大小的边界被应用在下面，并清除网页内容，产生一个闪烁，直到 DComp 提交这个帧被合成器处理。
            if (coreWebViewController is not null && IsLoaded)
            {
                // 检查 WebView2 在窗口内的位置是否发生了变化
                bool changed = false;
                GeneralTransform transform = TransformToVisual(null);
                Point topLeft = transform.TransformPoint(new Point(0, 0));

                double scaledTopLeftX = Math.Ceiling(topLeft.X * rasterizationScale);
                double scaledTopLeftY = Math.Ceiling(topLeft.Y * rasterizationScale);

                if (scaledTopLeftX != webViewScaledPosition.X || scaledTopLeftY != webViewScaledPosition.Y)
                {
                    webViewScaledPosition.X = scaledTopLeftX;
                    webViewScaledPosition.Y = scaledTopLeftY;
                    changed = true;
                }

                double scaledSizeX = Math.Ceiling(ActualWidth * rasterizationScale);
                double scaledSizeY = Math.Ceiling(ActualHeight * rasterizationScale);
                if (scaledSizeX != webViewScaledSize.X || scaledSizeY != webViewScaledSize.Y)
                {
                    webViewScaledSize.X = scaledSizeX;
                    webViewScaledSize.Y = scaledSizeY;
                    changed = true;
                }

                if (changed)
                {
                    // 我们使用X, Y，宽度和高度来创建边界
                    coreWebViewController.Bounds = new Rect((int)webViewScaledPosition.X, (int)webViewScaledPosition.Y, (int)webViewScaledSize.X, (int)webViewScaledSize.Y);
                }
            }
        }

        /// <summary>
        /// 检查窗口本身的位置是否发生了变化
        /// </summary>
        private void CheckAndUpdateWindowPosition()
        {
            IntPtr hostWindow = GetHostHwnd();
            if (hostWindow != IntPtr.Zero)
            {
                PointInt32 windowPosition = new()
                {
                    X = 0,
                    Y = 0
                };
                User32Library.ClientToScreen(hostWindow, ref windowPosition);

                if (hostWindowPosition.X != windowPosition.X || hostWindowPosition.Y != windowPosition.Y)
                {
                    hostWindowPosition.X = windowPosition.X;
                    hostWindowPosition.Y = windowPosition.Y;

                    coreWebViewController?.NotifyParentWindowPositionChanged();
                }
            }
        }

        /// <summary>
        /// 检查父元素的 Visibility 属性是否已更改
        /// </summary>
        private void CheckAndUpdateVisibility(bool force = false)
        {
            // //将布尔值按此顺序保存，以避免在不必要的情况下进行昂贵的树遍历。
            bool currentVisibility = Visibility is Visibility.Visible && IsLoaded && isHostVisible && AreAllAncestorsVisible();

            if (isVisible != currentVisibility || force)
            {
                isVisible = currentVisibility;
                UpdateCoreWebViewVisibility();
            }
        }

        /// <summary>
        /// 关闭浏览器
        /// </summary>
        private void CloseInternal(bool inShutdownPath)
        {
            DisconnectFromRootVisualTarget();
            UnregisterCoreEventHandlers();

            XamlRoot.Changed -= OnXamlRootChanged;
            Window.Current.VisibilityChanged -= OnVisibilityChanged;
            Windows.UI.Xaml.Media.CompositionTarget.Rendered -= OnRendered;
            isRenderedRegistered = true;

            if (tempHostHwnd != IntPtr.Zero && CoreWindow.GetForCurrentThread() is not null)
            {
                User32Library.DestroyWindow(tempHostHwnd);
                delegWndProc = null;
                tempHostHwnd = IntPtr.Zero;
            }

            if (manipulationModeChangedToken is not 0)
            {
                UnregisterPropertyChangedCallback(ManipulationModeProperty, manipulationModeChangedToken);
                manipulationModeChangedToken = 0;
            }

            if (visibilityChangedToken is not 0)
            {
                UnregisterPropertyChangedCallback(VisibilityProperty, visibilityChangedToken);
                visibilityChangedToken = 0;
            }

            inputWindowHwnd = IntPtr.Zero;

            if (coreWebView is not null)
            {
                coreWebView = null;
            }

            if (coreWebViewController is not null)
            {
                coreWebViewController.Close();
                coreWebViewController = null;
            }

            if (coreWebViewCompositionController is not null)
            {
                coreWebViewCompositionController = null;
            }

            UnregisterXamlEventHandlers();

            //如果从析构函数调用，跳过ResetProperties()，因为属性值不再重要。（否则，Xaml Core 将断言未能恢复 WebView2 的 DXaml Peer）
            if (!inShutdownPath)
            {
                ResetProperties();
            }

            isClosed = true;
        }

        /// <summary>
        /// 创建有关 CoreWebView 的实现对象
        /// </summary>
        private async Task CreateCoreObjectsAsync()
        {
            if (!isClosed)
            {
                RegisterXamlEventHandlers();

                //我们即将尝试重新创建环境，所以清除之前的环境
                Content = null;

                if (!isCoreFailure_BrowserExited_State)
                {
                    // 创建 CoreWebView 所需要的环境
                    string browserInstall = string.Empty;
                    string useDataFolder = string.Empty;

                    if (options is null)
                    {
                        options = new CoreWebView2EnvironmentOptions();

                        IReadOnlyList<string> applicationLanguagesList = ApplicationLanguages.Languages;
                        if (applicationLanguagesList.Count > 0)
                        {
                            options.Language = applicationLanguagesList[0];
                        }
                    }

                    try
                    {
                        coreWebViewEnvironment = await CoreWebView2Environment.CreateWithOptionsAsync(browserInstall, useDataFolder, options);
                    }
                    catch (Exception e)
                    {
                        int hr = e.HResult;
                        shouldShowMissingAnaheimWarning = hr is unchecked((int)0x80070002);
                        CoreWebView2Initialized?.Invoke(this, new CoreWebView2InitializedEventArgs(e));
                    }
                }

                if (coreWebViewEnvironment is not null)
                {
                    IntPtr hwndParent = GetHostHwnd();

                    if (hwndParent == IntPtr.Zero)
                    {
                        // 如果我们还不知道父节点，要么使用 CoreWindow 作为父节点，或者如果我们没有，创建一个虚拟 hwnd 作为临时父节点。
                        // 一直使用虚拟父类是行不通的，因为我们不能将浏览器从一个非 ShellManaged的 Hwnd (虚拟)重命名为一个 ShellManaged 的 (CoreWindow)。
                        if (CoreWindow.GetForCurrentThread() is CoreWindow coreWindow)
                        {
                            ICoreWindowInterop coreWindowInterop = coreWindow.As<ICoreWindowInterop>();
                            coreWindowInterop.GetWindowHandle(out tempHostHwnd);
                        }
                        else
                        {
                            string classname = "WEBVIEW2_TEMP_PARENT";
                            IntPtr hInstance = NativeLibrary.GetMainProgramHandle();
                            ;
                            delegWndProc = OnWndProc;

                            unsafe
                            {
                                WNDCLASS wc = new()
                                {
                                    lpfnWndProc = Marshal.GetFunctionPointerForDelegate(delegWndProc),
                                    hInstance = hInstance,
                                    lpszClassName = (IntPtr)Utf16StringMarshaller.ConvertToUnmanaged(classname),
                                };
                                User32Library.RegisterClass(wc);
                            }

                            tempHostHwnd = User32Library.CreateWindowEx(0, classname, "WebView2 Temporary Parent", 0x00000000, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
                        }

                        hwndParent = tempHostHwnd;
                    }

                    try
                    {
                        CoreWebView2ControllerWindowReference windowRef = CoreWebView2ControllerWindowReference.CreateFromWindowHandle((ulong)hwndParent);
                        // CreateCoreWebView2CompositionController(Async) 创建一个CompositionController，并处于可视托管模式。
                        // 调用 CreateCoreWebView2Controller 将创建一个控制器，并且将处于窗口模式。
                        coreWebViewCompositionController = await coreWebViewEnvironment.CreateCoreWebView2CompositionControllerAsync(windowRef);
                        coreWebViewController = coreWebViewCompositionController;
                        coreWebViewController.ShouldDetectMonitorScaleChanges = false;
                        coreWebView = coreWebViewController.CoreWebView2;
                        everHadCoreWebView = true;

                        RegisterCoreEventHandlers();
                        // 此时创建被认为已经完成，但是渲染和可访问性连接只会在我们获得 Loaded 事件后运行（参考 TryCompleteInitialization()）
                        CoreWebView2Initialized?.Invoke(this, new CoreWebView2InitializedEventArgs(new Exception()));
                    }
                    catch (Exception e)
                    {
                        CoreWebView2Initialized?.Invoke(this, new CoreWebView2InitializedEventArgs(e));
                    }

                    isImplicitCreationInProgress = false;
                    isExplicitCreationInProgress = false;

                    //执行初始化，包括渲染设置。
                    //现在试试，但如果还没有触发，则推迟到 Loaded 事件。
                    TryCompleteInitialization();
                }
                // CoreWebView2 不存在时，创建提示消息
                else if (shouldShowMissingAnaheimWarning)
                {
                    TextBlock warning = new()
                    {
                        Text = ResourceService.GetLocalized("WebView/WarningSuitableWebView2NotFound")
                    };
                    warning.Inlines.Add(new LineBreak());
                    Run linkText = new()
                    {
                        Text = ResourceService.GetLocalized("WebView/DownloadWebView2Runtime")
                    };
                    Hyperlink hyperlink = new();
                    hyperlink.Inlines.Add(linkText);
                    Uri uri = new("https://aka.ms/winui2/WebView2download/");
                    hyperlink.NavigateUri = uri;
                    warning.Inlines.Add(hyperlink);
                    Content = warning;
                }
            }
            else
            {
                throw new COMException(ResourceService.GetLocalized("WebView/WebView2Closed"), unchecked((int)0x80000013)); // RO_E_CLOSED
            }
        }

        /// <summary>
        /// 删除托管应用的视觉对象树中的根视觉对象。
        /// </summary>
        private void DisconnectFromRootVisualTarget()
        {
            if (coreWebViewCompositionController is not null)
            {
                coreWebViewCompositionController.RootVisualTarget = null;
            }
        }

        /// <summary>
        /// 获取 CoreWindow 的窗口句柄
        /// </summary>
        private IntPtr GetHostHwnd()
        {
            if (xamlHostHwnd == IntPtr.Zero && CoreWindow.GetForCurrentThread() is CoreWindow coreWindow)
            {
                ICoreWindowInterop coreWindowInterop = coreWindow.As<ICoreWindowInterop>();
                coreWindowInterop.GetWindowHandle(out xamlHostHwnd);
            }

            return xamlHostHwnd;
        }

        private void MoveFocusIntoCoreWebView(CoreWebView2MoveFocusReason reason)
        {
            try
            {
                coreWebViewController.MoveFocus(reason);
                webHasFocus = true;
            }
            catch (Exception e)
            {
                //有时，恢复最小化窗口的请求没有完成。这将触发 FocusManager 将 Xaml Focus 设置为 WebView2，从而进入上述 CoreWebView2 MoveFocus() 调用，该调用将在 InputHWND 上尝试 SetFocus()，并且将在 E_INVALIDARG 上失败，因为 HWND 保持最小化。通过忽略这个错误来解决这个问题。由于应用程序被最小化，焦点状态是不相关的-下一次（成功）尝试恢复应用程序
                //将焦点设置为 WebView2 / CoreWebView2 正确。
                if ((uint)e.HResult is not 0x80070057)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 注册与 CoreWebView2 有关的事件
        /// </summary>
        private void RegisterCoreEventHandlers()
        {
            if (coreWebView is not null)
            {
                coreWebView.NavigationStarting += OnNavigationStarting;
                coreWebView.SourceChanged += OnSourceChanged;
                coreWebView.NavigationCompleted += OnNavigationCompleted;
                coreWebView.WebMessageReceived += OnWebMessageReceived;
                coreWebView.ProcessFailed += OnProcessFailed;
            }

            if (coreWebViewController is not null)
            {
                coreWebViewController.MoveFocusRequested += OnMoveFocusRequested;
                coreWebViewController.LostFocus += OnLostFocus;
            }

            if (coreWebViewCompositionController is not null)
            {
                coreWebViewCompositionController.CursorChanged += OnCursorChanged;
            }
        }

        /// <summary>
        /// 注册与 WebView2 控件有关的事件
        /// </summary>
        private void RegisterXamlEventHandlers()
        {
            GettingFocus += OnGettingFocus;
            GotFocus += OnGotFocus;
            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerMoved += OnPointerMoved;
            PointerWheelChanged += OnPointerWheelChanged;
            PointerExited += OnPointerExited;
            PointerEntered += OnPointerEntered;
            PointerCanceled += OnPointerCanceled;
            PointerCaptureLost += OnPointerCaptureLost;
            KeyDown += OnKeyDown;

            // 注意：我们没有在 Islands / win32 中直接模拟AcceleratorKeyActivated 与 DispatcherQueue。
            if (CoreWindow.GetForCurrentThread() is CoreWindow coreWindow)
            {
                coreWindow.Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;
            }

            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// 重置鼠标指针
        /// </summary>
        private void ResetPointerHelper(PointerRoutedEventArgs args)
        {
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;

            if (deviceType is PointerDeviceType.Mouse)
            {
                hasMouseCapture = true;
                ResetMouseInputState();
            }
            else if (deviceType is PointerDeviceType.Touch)
            {
                // 获取处理多点触控捕获的指针 id
                PointerPoint logicalPointerPoint = args.GetCurrentPoint(this);
                uint id = logicalPointerPoint.PointerId;
                hasTouchCapture.Remove(id);
            }
            else if (deviceType is PointerDeviceType.Pen)
            {
                hasPenCapture = false;
            }
        }

        /// <summary>
        /// 重置鼠标输入状态
        /// </summary>
        private void ResetMouseInputState()
        {
            isLeftMouseButtonPressed = false;
            isMiddleMouseButtonPressed = false;
            isRightMouseButtonPressed = false;
            isXButton1Pressed = false;
            isXButton2Pressed = false;
        }

        /// <summary>
        /// 重置属性默认值
        /// </summary>
        private void ResetProperties()
        {
            // 不要重置 Source 属性，因为它对应用开发者恢复状态很有用
            SetCanGoForward(false);
            SetCanGoBack(false);
        }

        /// <summary>
        /// 设置能否向后导航
        /// </summary>
        private void SetCanGoBack(bool canGoBack)
        {
            CanGoBack = canGoBack;
        }

        /// <summary>
        /// 设置能否向前导航
        /// </summary>
        private void SetCanGoForward(bool canGoForward)
        {
            CanGoForward = canGoForward;
        }

        /// <summary>
        /// 检查当前浏览器是否需要进行导航到其他页
        /// </summary>

        private bool ShouldNavigate(Uri uri)
        {
            return uri is not null && uri.AbsoluteUri != stopNavigateOnUriChanged;
        }

        /// <summary>
        /// 设置 CoreWebView 和 SpriteVisual 的大小
        /// </summary>
        private void SetCoreWebViewAndVisualSize(double width, double height)
        {
            if (coreWebView is null && visual is null)
            {
                return;
            }

            if (coreWebView is not null)
            {
                CheckAndUpdateWebViewPosition();
            }

            // CoreWebView2 在桥视图下的视图已经按光栅化比例缩放。为了防止它们从 WebView2 元素上方的比例再次缩放，我们需要在桥的视觉效果上应用一个反向比例。由于反比例会减少桥梁视觉的大小，我们需要通过栅格化比例来补偿。
            if (visual is not null)
            {
                Vector2 newSize = new((float)(width * rasterizationScale), (float)(height * rasterizationScale));
                Vector3 newScale = new((float)(1.0f / rasterizationScale), (float)(1.0f / rasterizationScale), 1.0f);
                visual.Size = newSize;
                visual.Scale = newScale;
            }
        }

        /// <summary>
        /// 完成 CoreWebView 浏览器的初始化
        /// </summary>
        private void TryCompleteInitialization()
        {
            if (!shouldShowMissingAnaheimWarning && IsLoaded && coreWebView is not null)
            {
                // 在一个非 CoreWindow 的场景中，我们可能已经创建了 CoreWebView2，它的父节点是一个虚拟 hwnd (参考 ensuretemporaryhostthwnd())，在这种情况下，我们需要更新以在这里使用真正的父节点。
                // 如果我们使用了一个 CoreWindow 父类，这个 hwnd 不会改变。CoreWebView2 不允许我们进行切换使用 CoreWindow 作为 XamlRoot 的父类。
                if (CoreWindow.GetForCurrentThread() is CoreWindow coreWindow)
                {
                    IntPtr prevParentWindow = xamlHostHwnd;
                    xamlHostHwnd = IntPtr.Zero;
                    IntPtr newParentWindow = GetHostHwnd();
                    UpdateParentWindow(newParentWindow);
                }

                XamlRootChangedHelper(true);
                if (XamlRoot is not null)
                {
                    XamlRoot.Changed += OnXamlRootChanged;
                }
                else
                {
                    Window.Current.VisibilityChanged += OnVisibilityChanged;
                }

                if (!isTextScaleChangedRegistered)
                {
                    uiSettings.TextScaleFactorChanged += OnTextScaleFactorChanged;
                    isTextScaleChangedRegistered = true;
                }

                Content = new Grid()
                {
                    Background = new SolidColorBrush(Colors.Transparent)
                };

                visual ??= Window.Current.Compositor.CreateSpriteVisual();
                SetCoreWebViewAndVisualSize(ActualWidth, ActualHeight);
                ElementCompositionPreview.SetElementChildVisual(this, visual);

                if (coreWebViewCompositionController is not null)
                {
                    coreWebViewCompositionController.RootVisualTarget = visual;
                }

                //如果我们在核心进程失败后重新创建 WebView，表明我们现在已经恢复
                isCoreFailure_BrowserExited_State = false;
            }
        }

        /// <summary>
        /// 更新渲染
        /// </summary>
        private void UpdateRenderedSubscriptionAndVisibility()
        {
            // 当该元素被隐藏或未加载时，渲染订阅被关闭以获得更好的性能。
            // 然而，当该元素由于父元素被隐藏而被有效隐藏时，我们仍然应该订阅
            // 否则，如果祖先再次可见，我们将不会有 HandleRendered 中的检查来通知我们。
            if (IsLoaded && Visibility is Visibility.Visible)
            {
                if (!isRenderedRegistered)
                {
                    Windows.UI.Xaml.Media.CompositionTarget.Rendered += OnRendered;
                    isRenderedRegistered = true;
                }
            }
            else
            {
                Windows.UI.Xaml.Media.CompositionTarget.Rendered -= OnRendered;
                isRenderedRegistered = false;
            }

            CheckAndUpdateVisibility(true);
        }

        /// <summary>
        /// 当从 CoreWebView2 触发 NavigationStarting 事件、SourceChanged 事件或 NavigationCompleted 事件时，不使用导航方法更新源。
        /// </summary>
        private void UpdateSourceInternal()
        {
            string newUri = coreWebView.Source;
            stopNavigateOnUriChanged = newUri;
            Source = new(newUri);
            stopNavigateOnUriChanged = string.Empty;

            if (coreWebView is not null)
            {
                SetCanGoBack(coreWebView.CanGoBack);
                SetCanGoForward(coreWebView.CanGoForward);
            }
        }

        /// <summary>
        /// 注销与 CoreWebView2 有关的事件
        /// </summary>
        private void UnregisterCoreEventHandlers()
        {
            if (coreWebView is not null)
            {
                coreWebView.NavigationStarting -= OnNavigationStarting;
                coreWebView.SourceChanged -= OnSourceChanged;
                coreWebView.NavigationCompleted -= OnNavigationCompleted;
                coreWebView.WebMessageReceived -= OnWebMessageReceived;
                coreWebView.ProcessFailed -= OnProcessFailed;
            }

            if (coreWebViewController is not null)
            {
                coreWebViewController.MoveFocusRequested -= OnMoveFocusRequested;
                coreWebViewController.LostFocus -= OnLostFocus;
            }

            if (coreWebViewCompositionController is not null)
            {
                coreWebViewCompositionController.CursorChanged -= OnCursorChanged;
            }
        }

        /// <summary>
        /// 注销与 WebView2 控件有关的事件
        /// </summary>
        private void UnregisterXamlEventHandlers()
        {
            GettingFocus -= OnGettingFocus;
            GotFocus -= OnGotFocus;
            PointerPressed -= OnPointerPressed;
            PointerReleased -= OnPointerReleased;
            PointerMoved -= OnPointerMoved;
            PointerWheelChanged -= OnPointerWheelChanged;
            PointerExited -= OnPointerExited;
            PointerEntered -= OnPointerEntered;
            PointerCanceled -= OnPointerCanceled;
            PointerCaptureLost -= OnPointerCaptureLost;
            KeyDown -= OnKeyDown;

            if (CoreWindow.GetForCurrentThread() is CoreWindow coreWindow)
            {
                coreWindow.Dispatcher.AcceleratorKeyActivated -= OnAcceleratorKeyActivated;
            }

            SizeChanged -= OnSizeChanged;
        }

        /// <summary>
        /// 更新 CoreWebView 大小
        /// </summary>
        private void UpdateCoreWebViewScale()
        {
            if (coreWebViewController is not null)
            {
                double textScaleFactor = uiSettings.TextScaleFactor;
                coreWebViewController.RasterizationScale = rasterizationScale * textScaleFactor;
            }
        }

        /// <summary>
        /// 当我们过早地隐藏 CoreWebView 时，我们会看到 SystemVisualBridge 的 BackgroundColor 显示引起的闪烁。
        /// 为了解决这个问题，如果 WebView 被隐藏，我们延迟调用隐藏 CoreWebView。
        /// </summary>
        private void UpdateCoreWebViewVisibility()
        {
            if (isVisible && isHostVisible)
            {
                // 给 CreateTimer 的回调在 UI 线程中被调用。
                // 为了使这个有用，我们可以与 XAML 对象进行交互，
                // 在执行 UI 线程之前，我们将使用调度程序先将我们的工作发布到UI线程。
                ThreadPoolTimer timer = ThreadPoolTimer.CreateTimer(async _ => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (coreWebViewController is not null)
                    {
                        coreWebViewController.IsVisible = isVisible;
                    }
                }), TimeSpan.FromMilliseconds(200));
            }
            else
            {
                if (coreWebViewController is not null)
                {
                    coreWebViewController.IsVisible = isVisible;
                }
            }
        }

        /// <summary>
        /// 更新父窗口
        /// </summary>
        private void UpdateParentWindow(IntPtr newParentWindow)
        {
            if (tempHostHwnd != IntPtr.Zero && coreWebViewController is not null)
            {
                CoreWebView2ControllerWindowReference windowRef = CoreWebView2ControllerWindowReference.CreateFromWindowHandle((ulong)newParentWindow);

                //重写 WebView host
                coreWebViewController.ParentWindow = windowRef;

                User32Library.DestroyWindow(tempHostHwnd);
                tempHostHwnd = IntPtr.Zero;
            }
        }

        /// <summary>
        /// 更新 CoreWindow 鼠标指针
        /// </summary>
        private void UpdateCoreWindowCursor()
        {
            if (coreWebViewCompositionController is not null && isPointerOver)
            {
                CoreWindow.GetForCurrentThread().PointerCursor = coreWebViewCompositionController.Cursor;
            }
        }

        /// <summary>
        /// XamlRoot 发生更改时要进行的操作
        /// </summary>
        private void XamlRootChangedHelper(bool forceUpdate)
        {
            (double scale, bool hostVisibility) = new Func<(double, bool)>(delegate
            {
                if (XamlRoot is not null)
                {
                    float scale = (float)XamlRoot.RasterizationScale;
                    bool hostVisibility = XamlRoot.IsHostVisible;

                    return (scale, hostVisibility);
                }

                return (DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel, Window.Current.Visible);
            })();

            if (forceUpdate || (scale != rasterizationScale))
            {
                rasterizationScale = scale;
                // 如果我们执行了 forceUpdate，我们也需要在这里更新主机可见性
                isHostVisible = hostVisibility;
                UpdateCoreWebViewScale();
                SetCoreWebViewAndVisualSize(ActualWidth, ActualHeight);
                CheckAndUpdateWebViewPosition();
                UpdateRenderedSubscriptionAndVisibility();
            }
            else if (hostVisibility != isHostVisible)
            {
                isHostVisible = hostVisibility;
                CheckAndUpdateVisibility();
            }
        }

        #endregion 第八部分：私有方法
    }
}
