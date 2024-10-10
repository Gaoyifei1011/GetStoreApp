using GetStoreAppInstaller.WindowsAPI.PInvoke.Comctl32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using System;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.System;
using Windows.System.Power;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Composition.Desktop;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace GetStoreAppInstaller.UI.Backdrop
{
    /// <summary>
    /// Mica 背景色
    /// </summary>
    public sealed partial class MicaBackdrop : SystemBackdrop
    {
        private bool isInitialized;
        private bool isWindowClosed;
        private bool isActivated = true;
        private bool useMicaBackdrop;

        private readonly IntPtr windowHandle;
        private readonly FrameworkElement rootElement;
        private readonly UISettings uiSettings = new();
        private readonly AccessibilitySettings accessibilitySettings = new();
        private readonly CompositionCapabilities compositionCapabilities = CompositionCapabilities.GetForCurrentView();
        private readonly DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private readonly float defaultMicaBaseLightTintOpacity = 0.5f;
        private readonly float defaultMicaBaseLightLuminosityOpacity = 1;
        private readonly float defaultMicaBaseDarkTintOpacity = 0.8f;
        private readonly float defaultMicaBaseDarkLuminosityOpacity = 1;
        private readonly Color defaultMicaBaseLightTintColor = Color.FromArgb(255, 243, 243, 243);
        private readonly Color defaultMicaBaseLightFallbackColor = Color.FromArgb(255, 243, 243, 243);
        private readonly Color defaultMicaBaseDarkTintColor = Color.FromArgb(255, 32, 32, 32);
        private readonly Color defaultMicaBaseDarkFallbackColor = Color.FromArgb(255, 32, 32, 32);

        private readonly float defaultMicaAltLightTintOpacity = 0.5f;
        private readonly float defaultMicaAltLightLuminosityOpacity = 1;
        private readonly float defaultMicaAltDarkTintOpacity = 0;
        private readonly float defaultMicaAltDarkLuminosityOpacity = 1;
        private readonly Color defaultMicaAltLightTintColor = Color.FromArgb(255, 218, 218, 218);
        private readonly Color defaultMicaAltLightFallbackColor = Color.FromArgb(255, 218, 218, 218);
        private readonly Color defaultMicaAltDarkTintColor = Color.FromArgb(255, 10, 10, 10);
        private readonly Color defaultMicaAltDarkFallbackColor = Color.FromArgb(255, 10, 10, 10);

        private SUBCLASSPROC windowSubClassProc;

        public MicaKind Kind { get; set; } = MicaKind.Base;

        private float _lightTintOpacity = 0;

        public override float LightTintOpacity
        {
            get { return _lightTintOpacity; }

            set
            {
                if (!Equals(_lightTintOpacity, value))
                {
                    _lightTintOpacity = value;
                    if (value < 0 || value > 1)
                    {
                        throw new ArgumentException("值必须在 0 到 1 之间");
                    }

                    UpdateBrush();
                }
            }
        }

        private float _lightLuminosityOpacity = 0;

        public override float LightLuminosityOpacity
        {
            get { return _lightLuminosityOpacity; }

            set
            {
                if (!Equals(_lightLuminosityOpacity, value))
                {
                    _lightLuminosityOpacity = value;
                    if (value < 0 || value > 1)
                    {
                        throw new ArgumentException("值必须在 0 到 1 之间");
                    }

                    UpdateBrush();
                }
            }
        }

        private float _darkTintOpacity = 0;

        public override float DarkTintOpacity
        {
            get { return _darkTintOpacity; }

            set
            {
                if (!Equals(_darkTintOpacity, value))
                {
                    _darkTintOpacity = value;
                    if (value < 0 || value > 1)
                    {
                        throw new ArgumentException("值必须在 0 到 1 之间");
                    }

                    UpdateBrush();
                }
            }
        }

        private float _darkLuminosityOpacity = 0;

        public override float DarkLuminosityOpacity
        {
            get { return _darkLuminosityOpacity; }

            set
            {
                if (!Equals(_darkLuminosityOpacity, value))
                {
                    _darkLuminosityOpacity = value;
                    if (value < 0 || value > 1)
                    {
                        throw new ArgumentException("值必须在 0 到 1 之间");
                    }

                    UpdateBrush();
                }
            }
        }

        private Color _lightTintColor = Color.FromArgb(0, 0, 0, 0);

        public override Color LightTintColor
        {
            get { return _lightTintColor; }

            set
            {
                if (!Equals(_lightTintColor, value))
                {
                    _lightTintColor = value;
                    UpdateBrush();
                }
            }
        }

        private Color _lightFallbackColor = Color.FromArgb(0, 0, 0, 0);

        public override Color LightFallbackColor
        {
            get { return _lightFallbackColor; }

            set
            {
                if (!Equals(_lightFallbackColor, value))
                {
                    _lightFallbackColor = value;
                    UpdateBrush();
                }
            }
        }

        private Color _darkTintColor = Color.FromArgb(0, 0, 0, 0);

        public override Color DarkTintColor
        {
            get { return _darkTintColor; }

            set
            {
                if (!Equals(_darkTintColor, value))
                {
                    _darkTintColor = value;
                    UpdateBrush();
                }
            }
        }

        private Color _darkFallbackColor = Color.FromArgb(0, 0, 0, 0);

        public override Color DarkFallbackColor
        {
            get { return _darkFallbackColor; }

            set
            {
                if (!Equals(_darkFallbackColor, value))
                {
                    _darkFallbackColor = value;
                    UpdateBrush();
                }
            }
        }

        private ElementTheme _requestedTheme = ElementTheme.Default;

        public override ElementTheme RequestedTheme
        {
            get { return _requestedTheme; }

            set
            {
                if (!Equals(_requestedTheme, value))
                {
                    _requestedTheme = value;
                    UpdateBrush();
                }
            }
        }

        private bool _isInputActive = false;

        public override bool IsInputActive
        {
            get { return _isInputActive; }

            set
            {
                if (!Equals(_isInputActive, value))
                {
                    _isInputActive = value;
                }
            }
        }

        public static bool IsSupported
        {
            get
            {
                return ApiInformation.IsMethodPresent(typeof(Compositor).FullName, nameof(Compositor.TryCreateBlurredWallpaperBackdropBrush));
            }
        }

        public MicaBackdrop(DesktopWindowTarget target, FrameworkElement element, IntPtr handle) : base(target)
        {
            if (target is null)
            {
                throw new ArgumentNullException(string.Format("参数 {0} 不可以为空值", nameof(target)));
            }

            if (handle == IntPtr.Zero)
            {
                throw new NullReferenceException("窗口句柄无效");
            }

            windowHandle = handle;
            rootElement = element;
        }

        ~MicaBackdrop()
        {
            Dispose(false);
        }

        /// <summary>
        /// 初始化系统背景色
        /// </summary>
        public override void InitializeBackdrop()
        {
            if (!isInitialized)
            {
                float defaultOpacityValue = 0;
                Color defaultColorValue = Color.FromArgb(0, 0, 0, 0);

                if (Kind is MicaKind.Base)
                {
                    _lightTintOpacity = _lightTintOpacity.Equals(defaultOpacityValue) ? defaultMicaBaseLightTintOpacity : _lightTintOpacity;
                    _lightLuminosityOpacity = _lightLuminosityOpacity.Equals(defaultOpacityValue) ? defaultMicaBaseLightLuminosityOpacity : _lightLuminosityOpacity;
                    _darkTintOpacity = _darkTintOpacity.Equals(defaultOpacityValue) ? defaultMicaBaseDarkTintOpacity : _darkTintOpacity;
                    _darkLuminosityOpacity = _darkLuminosityOpacity.Equals(defaultOpacityValue) ? defaultMicaBaseDarkLuminosityOpacity : _darkLuminosityOpacity;
                    _lightTintColor = _lightTintColor.Equals(defaultColorValue) ? defaultMicaBaseLightTintColor : _lightTintColor;
                    _lightFallbackColor = _lightFallbackColor.Equals(defaultColorValue) ? defaultMicaBaseLightFallbackColor : _lightFallbackColor;
                    _darkTintColor = _darkTintColor.Equals(defaultColorValue) ? defaultMicaBaseDarkTintColor : _darkTintColor;
                    _darkFallbackColor = _darkFallbackColor.Equals(defaultColorValue) ? defaultMicaBaseDarkFallbackColor : _darkFallbackColor;
                }
                else
                {
                    _lightTintOpacity = _lightTintOpacity.Equals(defaultOpacityValue) ? defaultMicaAltLightTintOpacity : _lightTintOpacity;
                    _lightLuminosityOpacity = _lightLuminosityOpacity.Equals(defaultOpacityValue) ? defaultMicaAltLightLuminosityOpacity : _lightLuminosityOpacity;
                    _darkTintOpacity = _darkTintOpacity.Equals(defaultOpacityValue) ? defaultMicaAltDarkTintOpacity : _darkTintOpacity;
                    _darkLuminosityOpacity = _darkLuminosityOpacity.Equals(defaultOpacityValue) ? defaultMicaAltDarkLuminosityOpacity : _darkLuminosityOpacity;
                    _lightTintColor = _lightTintColor.Equals(defaultColorValue) ? defaultMicaAltLightTintColor : _lightTintColor;
                    _lightFallbackColor = _lightFallbackColor.Equals(defaultColorValue) ? defaultMicaAltLightFallbackColor : _lightFallbackColor;
                    _darkTintColor = _darkTintColor.Equals(defaultColorValue) ? defaultMicaAltDarkTintColor : _darkTintColor;
                    _darkFallbackColor = _darkFallbackColor.Equals(defaultColorValue) ? defaultMicaAltDarkFallbackColor : _darkFallbackColor;
                }

                if (DesktopWindowTarget.Root is null)
                {
                    DesktopWindowTarget.Root = DesktopWindowTarget.Compositor.CreateSpriteVisual();
                }

                uiSettings.ColorValuesChanged += OnColorValuesChanged;
                accessibilitySettings.HighContrastChanged += OnHighContrastChanged;
                compositionCapabilities.Changed += OnCompositionCapabilitiesChanged;
                displayInformation.DpiChanged += OnDpiChanged;
                PowerManager.EnergySaverStatusChanged += OnEnergySaverStatusChanged;

                if (rootElement is not null)
                {
                    rootElement.ActualThemeChanged += OnActualThemeChanged;
                }

                windowSubClassProc = new SUBCLASSPROC(OnWindowSubClassProc);
                Comctl32Library.SetWindowSubclass(windowHandle, windowSubClassProc, 0, IntPtr.Zero);

                isInitialized = true;
                UpdateBrush();
            }
        }

        /// <summary>
        /// 恢复默认值
        /// </summary>
        public override void ResetProperties()
        {
            if (Kind is MicaKind.Base)
            {
                _lightTintOpacity = defaultMicaBaseLightTintOpacity;
                _lightLuminosityOpacity = defaultMicaBaseLightLuminosityOpacity;
                _darkTintOpacity = defaultMicaBaseDarkTintOpacity;
                _darkLuminosityOpacity = defaultMicaBaseDarkLuminosityOpacity;
                _lightTintColor = defaultMicaBaseLightTintColor;
                _lightFallbackColor = defaultMicaBaseLightFallbackColor;
                _darkTintColor = defaultMicaBaseDarkTintColor;
                _darkFallbackColor = defaultMicaBaseDarkFallbackColor;
            }
            else
            {
                _lightTintOpacity = defaultMicaAltLightTintOpacity;
                _lightLuminosityOpacity = defaultMicaAltLightLuminosityOpacity;
                _darkTintOpacity = defaultMicaAltDarkTintOpacity;
                _darkLuminosityOpacity = defaultMicaAltDarkLuminosityOpacity;
                _lightTintColor = defaultMicaAltLightTintColor;
                _lightFallbackColor = defaultMicaAltLightFallbackColor;
                _darkTintColor = defaultMicaAltDarkTintColor;
                _darkFallbackColor = defaultMicaAltDarkFallbackColor;
            }

            _requestedTheme = ElementTheme.Default;
            _isInputActive = false;

            if (isInitialized)
            {
                UpdateBrush();
            }
        }

        /// <summary>
        /// 关闭背景色
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 关闭背景色
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            lock (this)
            {
                if (isInitialized)
                {
                    isInitialized = false;

                    uiSettings.ColorValuesChanged -= OnColorValuesChanged;
                    accessibilitySettings.HighContrastChanged -= OnHighContrastChanged;
                    compositionCapabilities.Changed -= OnCompositionCapabilitiesChanged;
                    displayInformation.DpiChanged -= OnDpiChanged;
                    PowerManager.EnergySaverStatusChanged -= OnEnergySaverStatusChanged;

                    if (rootElement is not null)
                    {
                        rootElement.ActualThemeChanged -= OnActualThemeChanged;
                    }

                    Comctl32Library.RemoveWindowSubclass(windowHandle, windowSubClassProc, 0);

                    if (DesktopWindowTarget.Root is SpriteVisual spriteVisual && spriteVisual.Brush is not null)
                    {
                        spriteVisual.Brush.Dispose();
                        spriteVisual.Brush = null;
                    }
                }
            }
        }

        /// <summary>
        /// 颜色值更改时发生的事件
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            dispatcherQueue.TryEnqueue(UpdateBrush);
        }

        /// <summary>
        /// 当系统高对比度功能打开或关闭时发生的事件
        /// </summary>
        private void OnHighContrastChanged(AccessibilitySettings sender, object args)
        {
            dispatcherQueue.TryEnqueue(UpdateBrush);
        }

        /// <summary>
        /// 当支持的合成功能发生更改时触发的事件
        /// </summary>
        private void OnCompositionCapabilitiesChanged(CompositionCapabilities sender, object args)
        {
            dispatcherQueue.TryEnqueue(UpdateBrush);
        }

        /// <summary>
        /// 显示窗口的屏幕的 DPI 发生更改后触发的事件
        /// </summary>
        private void OnDpiChanged(DisplayInformation sender, object args)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                if (DesktopWindowTarget.Root is SpriteVisual spriteVisual)
                {
                    //spriteVisual.Size = new Vector2(formRoot.Width, formRoot.Height);
                }
            });
        }

        /// <summary>
        /// 在设备的节电模式状态更改时触发的事件
        /// </summary>
        private void OnEnergySaverStatusChanged(object sender, object args)
        {
            dispatcherQueue.TryEnqueue(UpdateBrush);
        }

        /// <summary>
        /// 在 ActualTheme 属性值更改时触发的事件
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            UpdateBrush();
        }

        /// <summary>
        /// 更新应用的背景色
        /// </summary>
        private void UpdateBrush()
        {
            if (isInitialized)
            {
                ElementTheme actualTheme = ElementTheme.Default;

                // 如果传入的 FrameworkElement 为空值，则由系统默认主题色值决定窗口的背景色
                if (rootElement is not null)
                {
                    // 主题值为默认时，窗口背景色主题值则由 FrameworkElement 决定
                    actualTheme = RequestedTheme is ElementTheme.Default ? rootElement.ActualTheme : RequestedTheme;
                }
                else
                {
                    actualTheme = Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                }

                float tintOpacity;
                float luminosityOpacity;
                Color tintColor;
                Color fallbackColor;

                if (actualTheme is ElementTheme.Light)
                {
                    tintOpacity = LightTintOpacity;
                    luminosityOpacity = LightLuminosityOpacity;
                    tintColor = LightTintColor;
                    fallbackColor = LightFallbackColor;
                }
                else
                {
                    tintOpacity = DarkTintOpacity;
                    luminosityOpacity = DarkLuminosityOpacity;
                    tintColor = DarkTintColor;
                    fallbackColor = DarkFallbackColor;
                }

                useMicaBackdrop = IsSupported && uiSettings.AdvancedEffectsEnabled && PowerManager.EnergySaverStatus is not EnergySaverStatus.On && compositionCapabilities.AreEffectsSupported() && (IsInputActive || isActivated);

                if (accessibilitySettings.HighContrast)
                {
                    tintColor = uiSettings.GetColorValue(UIColorType.Background);
                    useMicaBackdrop = false;
                }

                Compositor compositor = DesktopWindowTarget.Compositor;
                CompositionBrush newBrush = useMicaBackdrop ? BuildMicaEffectBrush(compositor, tintColor, tintOpacity, luminosityOpacity) : compositor.CreateColorBrush(fallbackColor);
                CompositionBrush oldBrush = (DesktopWindowTarget.Root as SpriteVisual).Brush;

                if (oldBrush is null || oldBrush.Comment is "Crossfade")
                {
                    // 直接设置新笔刷
                    oldBrush?.Dispose();
                    (DesktopWindowTarget.Root as SpriteVisual).Brush = newBrush;
                    //(DesktopWindowTarget.Root as SpriteVisual).Size = new(formRoot.Width, formRoot.Height);
                }
                else
                {
                    // 回退色切换时的动画颜色
                    CompositionBrush crossFadeBrush = CreateCrossFadeEffectBrush(compositor, oldBrush, newBrush);
                    ScalarKeyFrameAnimation animation = CreateCrossFadeAnimation(compositor);
                    (DesktopWindowTarget.Root as SpriteVisual).Brush = crossFadeBrush;
                    //(DesktopWindowTarget.Root as SpriteVisual).Size = new(formRoot.Width, formRoot.Height);

                    CompositionScopedBatch crossFadeAnimationBatch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                    crossFadeBrush.StartAnimation("CrossFade.CrossFade", animation);
                    crossFadeAnimationBatch.End();

                    crossFadeAnimationBatch.Completed += (o, a) =>
                    {
                        crossFadeBrush.Dispose();
                        oldBrush.Dispose();
                        (DesktopWindowTarget.Root as SpriteVisual).Brush = newBrush;
                        //(DesktopWindowTarget.Root as SpriteVisual).Size = new(formRoot.Width, formRoot.Height);
                    };
                }
            }
        }

        /// <summary>
        /// 创建 Mica 背景色
        /// </summary>
        private CompositionEffectBrush BuildMicaEffectBrush(Compositor compositor, Color tintColor, float tintOpacity, float luminosityOpacity)
        {
            // Tint Color.
            ColorSourceEffect tintColorEffect = new()
            {
                Name = "TintColor",
                Color = tintColor
            };

            // OpacityEffect applied to Tint.
            OpacityEffect tintOpacityEffect = new()
            {
                Name = "TintOpacity",
                Opacity = tintOpacity,
                Source = tintColorEffect
            };

            // Apply Luminosity:

            // Luminosity Color.
            ColorSourceEffect luminosityColorEffect = new()
            {
                Color = tintColor
            };

            // OpacityEffect applied to Luminosity.
            OpacityEffect luminosityOpacityEffect = new()
            {
                Name = "LuminosityOpacity",
                Opacity = luminosityOpacity,
                Source = luminosityColorEffect
            };

            // Luminosity Blend.
            // NOTE: There is currently a bug where the names of BlendEffectMode::Luminosity and BlendEffectMode::Color are flipped.
            BlendEffect luminosityBlendEffect = new()
            {
                Mode = BlendEffectMode.Color,
                Background = new CompositionEffectSourceParameter("BlurredWallpaperBackdrop"),
                Foreground = luminosityOpacityEffect
            };

            // Apply Tint:

            // Color Blend.
            // NOTE: There is currently a bug where the names of BlendEffectMode::Luminosity and BlendEffectMode::Color are flipped.
            BlendEffect colorBlendEffect = new()
            {
                Mode = BlendEffectMode.Luminosity,
                Background = luminosityBlendEffect,
                Foreground = tintOpacityEffect
            };

            CompositionEffectBrush micaEffectBrush = compositor.CreateEffectFactory(colorBlendEffect).CreateBrush();
            micaEffectBrush.SetSourceParameter("BlurredWallpaperBackdrop", compositor.TryCreateBlurredWallpaperBackdropBrush());

            return micaEffectBrush;
        }

        /// <summary>
        /// 创建回退色切换时的动画颜色
        /// </summary>
        private CompositionEffectBrush CreateCrossFadeEffectBrush(Compositor compositor, CompositionBrush from, CompositionBrush to)
        {
            CrossFadeEffect crossFadeEffect = new()
            {
                Name = "Crossfade", // Name to reference when starting the animation.
                Source1 = new CompositionEffectSourceParameter("source1"),
                Source2 = new CompositionEffectSourceParameter("source2"),
                CrossFade = 0,
            };

            CompositionEffectBrush crossFadeEffectBrush = compositor.CreateEffectFactory(crossFadeEffect, ["Crossfade.CrossFade"]).CreateBrush();
            crossFadeEffectBrush.Comment = "Crossfade";

            crossFadeEffectBrush.SetSourceParameter("source1", from);
            crossFadeEffectBrush.SetSourceParameter("source2", to);
            return crossFadeEffectBrush;
        }

        /// <summary>
        /// 为回退色创建动画效果
        /// </summary>
        private ScalarKeyFrameAnimation CreateCrossFadeAnimation(Compositor compositor)
        {
            ScalarKeyFrameAnimation animation = compositor.CreateScalarKeyFrameAnimation();
            LinearEasingFunction linearEasing = compositor.CreateLinearEasingFunction();
            animation.InsertKeyFrame(0.0f, 0.0f, linearEasing);
            animation.InsertKeyFrame(1.0f, 1.0f, linearEasing);
            animation.Duration = TimeSpan.FromMilliseconds(250);
            return animation;
        }

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private IntPtr OnWindowSubClassProc(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            // 窗口大小发生变化时的消息
            if (Msg is WindowMessage.WM_SIZE)
            {
                if (DesktopWindowTarget.Root is SpriteVisual spriteVisual)
                {
                    //spriteVisual.Size = new Vector2(formRoot.Width, formRoot.Height);
                }
            }
            // 窗口关闭时的消息
            else if (Msg is WindowMessage.WM_CLOSE)
            {
                isWindowClosed = true;
                Dispose();
            }
            // 窗口激活状态发生变化时的消息
            else if (Msg is WindowMessage.WM_ACTIVATE)
            {
                isActivated = true;
                UpdateBrush();
            }

            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }
    }
}
