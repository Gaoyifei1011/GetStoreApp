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
using Windows.UI.Xaml.Media;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace GetStoreAppInstaller.UI.Backdrop
{
    /// <summary>
    /// Desktop Acrylic 背景色
    /// </summary>
    public sealed partial class DesktopAcrylicBackdrop : SystemBackdrop
    {
        private bool isInitialized;
        private bool isWindowClosed;
        private bool isActivated = true;
        private bool useDesktopAcrylicBackdrop;

        private readonly IntPtr windowHandle;
        private readonly FrameworkElement rootElement;
        private readonly UISettings uiSettings = new();
        private readonly AccessibilitySettings accessibilitySettings = new();
        private readonly CompositionCapabilities compositionCapabilities = CompositionCapabilities.GetForCurrentView();
        private readonly DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private readonly float defaultDesktopAcrylicDefaultLightTintOpacity = 0;
        private readonly float defaultDesktopAcrylicDefaultLightLuminosityOpacity = 0.85f;
        private readonly float defaultDesktopAcrylicDefaultDarkTintOpacity = 0.15f;
        private readonly float defaultDesktopAcrylicDefaultDarkLuminosityOpacity = 96;
        private readonly Color defaultDesktopAcrylicDefaultLightTintColor = Color.FromArgb(255, 252, 252, 252);
        private readonly Color defaultDesktopAcrylicDefaultLightFallbackColor = Color.FromArgb(255, 249, 249, 249);
        private readonly Color defaultDesktopAcrylicDefaultDarkTintColor = Color.FromArgb(255, 44, 44, 44);
        private readonly Color defaultDesktopAcrylicDefaultDarkFallbackColor = Color.FromArgb(255, 44, 44, 44);

        private readonly float defaultDesktopAcrylicBaseLightTintOpacity = 0;
        private readonly float defaultDesktopAcrylicBaseLightLuminosityOpacity = 0.9f;
        private readonly float defaultDesktopAcrylicBaseDarkTintOpacity = 0.5f;
        private readonly float defaultDesktopAcrylicBaseDarkLuminosityOpacity = 0.96f;
        private readonly Color defaultDesktopAcrylicBaseLightTintColor = Color.FromArgb(255, 243, 243, 243);
        private readonly Color defaultDesktopAcrylicBaseLightFallbackColor = Color.FromArgb(255, 238, 238, 238);
        private readonly Color defaultDesktopAcrylicBaseDarkTintColor = Color.FromArgb(255, 32, 32, 32);
        private readonly Color defaultDesktopAcrylicBaseDarkFallbackColor = Color.FromArgb(255, 28, 28, 28);

        private readonly float defaultDesktopAcrylicThinLightTintOpacity = 0;
        private readonly float defaultDesktopAcrylicThinLightLuminosityOpacity = 0.44f;
        private readonly float defaultDesktopAcrylicThinDarkTintOpacity = 0.15f;
        private readonly float defaultDesktopAcrylicThinDarkLuminosityOpacity = 0.64f;
        private readonly Color defaultDesktopAcrylicThinLightTintColor = Color.FromArgb(255, 211, 211, 211);
        private readonly Color defaultDesktopAcrylicThinLightFallbackColor = Color.FromArgb(255, 211, 211, 211);
        private readonly Color defaultDesktopAcrylicThinDarkTintColor = Color.FromArgb(255, 84, 84, 84);
        private readonly Color defaultDesktopAcrylicThinDarkFallbackColor = Color.FromArgb(255, 84, 84, 84);

        private SUBCLASSPROC windowSubClassProc;

        public DesktopAcrylicKind Kind { get; set; } = DesktopAcrylicKind.Default;

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

        private bool _useHostBackdropBrush = false;

        public bool UseHostBackdropBrush
        {
            get { return _useHostBackdropBrush; }

            set
            {
                if (!Equals(_useHostBackdropBrush, value))
                {
                    _useHostBackdropBrush = value;
                    UpdateBrush();
                }
            }
        }

        private float _blurAmount = 30f;

        public float BlurAmount
        {
            get { return _blurAmount; }

            set
            {
                if (!Equals(_blurAmount, value))
                {
                    _blurAmount = value;
                    UpdateBrush();
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

        public static bool IsHostBackdropSupported
        {
            get { return Environment.OSVersion.Version >= new Version(10, 0, 22000, 0); }
        }

        public DesktopAcrylicBackdrop(DesktopWindowTarget target, FrameworkElement element, IntPtr handle) : base(target)
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

        ~DesktopAcrylicBackdrop()
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

                if (Kind is DesktopAcrylicKind.Default)
                {
                    _lightTintOpacity = _lightTintOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicDefaultLightTintOpacity : _lightTintOpacity;
                    _lightLuminosityOpacity = _lightLuminosityOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicDefaultLightLuminosityOpacity : _lightLuminosityOpacity;
                    _darkTintOpacity = _darkTintOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicDefaultDarkTintOpacity : _darkTintOpacity;
                    _darkLuminosityOpacity = _darkLuminosityOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicDefaultDarkLuminosityOpacity : _darkLuminosityOpacity;
                    _lightTintColor = _lightTintColor.Equals(defaultColorValue) ? defaultDesktopAcrylicDefaultLightTintColor : _lightTintColor;
                    _lightFallbackColor = _lightFallbackColor.Equals(defaultColorValue) ? defaultDesktopAcrylicDefaultLightFallbackColor : _lightFallbackColor;
                    _darkTintColor = _darkTintColor.Equals(defaultColorValue) ? defaultDesktopAcrylicDefaultDarkTintColor : _darkTintColor;
                    _darkFallbackColor = _darkFallbackColor.Equals(defaultColorValue) ? defaultDesktopAcrylicDefaultDarkFallbackColor : _darkFallbackColor;
                }
                else if (Kind is DesktopAcrylicKind.Base)
                {
                    _lightTintOpacity = _lightTintOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicBaseLightTintOpacity : _lightTintOpacity;
                    _lightLuminosityOpacity = _lightLuminosityOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicBaseLightLuminosityOpacity : _lightLuminosityOpacity;
                    _darkTintOpacity = _darkTintOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicBaseDarkTintOpacity : _darkTintOpacity;
                    _darkLuminosityOpacity = _darkLuminosityOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicBaseDarkLuminosityOpacity : _darkLuminosityOpacity;
                    _lightTintColor = _lightTintColor.Equals(defaultColorValue) ? defaultDesktopAcrylicBaseLightTintColor : _lightTintColor;
                    _lightFallbackColor = _lightFallbackColor.Equals(defaultColorValue) ? defaultDesktopAcrylicBaseLightFallbackColor : _lightFallbackColor;
                    _darkTintColor = _darkTintColor.Equals(defaultColorValue) ? defaultDesktopAcrylicBaseDarkTintColor : _darkTintColor;
                    _darkFallbackColor = _darkFallbackColor.Equals(defaultColorValue) ? defaultDesktopAcrylicBaseDarkFallbackColor : _darkFallbackColor;
                }
                else
                {
                    _lightTintOpacity = _lightTintOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicThinLightTintOpacity : _lightTintOpacity;
                    _lightLuminosityOpacity = _lightLuminosityOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicThinLightLuminosityOpacity : _lightLuminosityOpacity;
                    _darkTintOpacity = _darkTintOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicThinDarkTintOpacity : _darkTintOpacity;
                    _darkLuminosityOpacity = _darkLuminosityOpacity.Equals(defaultOpacityValue) ? defaultDesktopAcrylicThinDarkLuminosityOpacity : _darkLuminosityOpacity;
                    _lightTintColor = _lightTintColor.Equals(defaultColorValue) ? defaultDesktopAcrylicThinLightTintColor : _lightTintColor;
                    _lightFallbackColor = _lightFallbackColor.Equals(defaultColorValue) ? defaultDesktopAcrylicThinLightFallbackColor : _lightFallbackColor;
                    _darkTintColor = _darkTintColor.Equals(defaultColorValue) ? defaultDesktopAcrylicThinDarkTintColor : _darkTintColor;
                    _darkFallbackColor = _darkFallbackColor.Equals(defaultColorValue) ? defaultDesktopAcrylicThinDarkFallbackColor : _darkFallbackColor;
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
            if (Kind is DesktopAcrylicKind.Default)
            {
                _lightTintOpacity = defaultDesktopAcrylicDefaultLightTintOpacity;
                _lightLuminosityOpacity = defaultDesktopAcrylicDefaultLightLuminosityOpacity;
                _darkTintOpacity = defaultDesktopAcrylicDefaultDarkTintOpacity;
                _darkLuminosityOpacity = defaultDesktopAcrylicDefaultDarkLuminosityOpacity;
                _lightTintColor = defaultDesktopAcrylicDefaultLightTintColor;
                _lightFallbackColor = defaultDesktopAcrylicDefaultLightFallbackColor;
                _darkTintColor = defaultDesktopAcrylicDefaultDarkTintColor;
                _darkFallbackColor = defaultDesktopAcrylicDefaultDarkFallbackColor;
            }
            else if (Kind is DesktopAcrylicKind.Base)
            {
                _lightTintOpacity = defaultDesktopAcrylicBaseLightTintOpacity;
                _lightLuminosityOpacity = defaultDesktopAcrylicBaseLightLuminosityOpacity;
                _darkTintOpacity = defaultDesktopAcrylicBaseDarkTintOpacity;
                _darkLuminosityOpacity = defaultDesktopAcrylicBaseDarkLuminosityOpacity;
                _lightTintColor = defaultDesktopAcrylicBaseLightTintColor;
                _lightFallbackColor = defaultDesktopAcrylicBaseLightFallbackColor;
                _darkTintColor = defaultDesktopAcrylicBaseDarkTintColor;
                _darkFallbackColor = defaultDesktopAcrylicBaseDarkFallbackColor;
            }
            else
            {
                _lightTintOpacity = defaultDesktopAcrylicThinLightTintOpacity;
                _lightLuminosityOpacity = defaultDesktopAcrylicThinLightLuminosityOpacity;
                _darkTintOpacity = defaultDesktopAcrylicThinDarkTintOpacity;
                _darkLuminosityOpacity = defaultDesktopAcrylicThinDarkLuminosityOpacity;
                _lightTintColor = defaultDesktopAcrylicThinLightTintColor;
                _lightFallbackColor = defaultDesktopAcrylicThinLightFallbackColor;
                _darkTintColor = defaultDesktopAcrylicThinDarkTintColor;
                _darkFallbackColor = defaultDesktopAcrylicThinDarkFallbackColor;
            }

            _requestedTheme = ElementTheme.Default;
            _isInputActive = false;
            _useHostBackdropBrush = false;

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
                    actualTheme = Windows.UI.Xaml.Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
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

                useDesktopAcrylicBackdrop = IsSupported && uiSettings.AdvancedEffectsEnabled && PowerManager.EnergySaverStatus is not EnergySaverStatus.On && compositionCapabilities.AreEffectsSupported() && (IsInputActive || isActivated);

                if (accessibilitySettings.HighContrast)
                {
                    tintColor = uiSettings.GetColorValue(UIColorType.Background);
                    useDesktopAcrylicBackdrop = false;
                }

                Compositor compositor = DesktopWindowTarget.Compositor;
                CompositionBrush newBrush = useDesktopAcrylicBackdrop ? BuildDesktopAcrylicEffectBrush(compositor, tintColor, tintOpacity, luminosityOpacity) : compositor.CreateColorBrush(fallbackColor);
                CompositionBrush oldBrush = (DesktopWindowTarget.Root as SpriteVisual).Brush;

                if (oldBrush is null || oldBrush.Comment is "Crossfade")
                {
                    // 直接设置新笔刷
                    oldBrush?.Dispose();
                    (DesktopWindowTarget.Root as SpriteVisual).Brush = newBrush;
                    //(DesktopWindowTarget.Root as SpriteVisual).Size = new Vector2(formRoot.Width, formRoot.Height);
                }
                else
                {
                    // 回退色切换时的动画颜色
                    CompositionBrush crossFadeBrush = CreateCrossFadeEffectBrush(compositor, oldBrush, newBrush);
                    ScalarKeyFrameAnimation animation = CreateCrossFadeAnimation(compositor);
                    (DesktopWindowTarget.Root as SpriteVisual).Brush = crossFadeBrush;
                    //(DesktopWindowTarget.Root as SpriteVisual).Size = new Vector2(formRoot.Width, formRoot.Height);

                    CompositionScopedBatch crossFadeAnimationBatch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                    crossFadeBrush.StartAnimation("CrossFade.CrossFade", animation);
                    crossFadeAnimationBatch.End();

                    crossFadeAnimationBatch.Completed += (o, a) =>
                    {
                        crossFadeBrush.Dispose();
                        oldBrush.Dispose();
                        (DesktopWindowTarget.Root as SpriteVisual).Brush = newBrush;
                        //(DesktopWindowTarget.Root as SpriteVisual).Size = new Vector2(formRoot.Width, formRoot.Height);
                    };
                }
            }
        }

        /// <summary>
        /// 创建 DesktopAcrylic 背景色
        /// </summary>
        private CompositionEffectBrush BuildDesktopAcrylicEffectBrush(Compositor compositor, Color tintColor, float tintOpacity, float luminosityOpacity)
        {
            Color convertedLuminosityColor = ColorConversion.GetEffectiveLuminosityColor(tintColor, tintOpacity, luminosityOpacity);
            Color convertedTintColor = ColorConversion.GetEffectiveTintColor(tintColor, tintOpacity, luminosityOpacity);

            // Source 1 : Host backdrop layer effect
            ColorSourceEffect hostBackdropEffect = new()
            {
                Color = Color.FromArgb(255, 0, 0, 0)
            };

            OpacityEffect hostBackdropLayerEffect = new()
            {
                Name = "FixHostBackdropLayer",
                Opacity = IsHostBackdropSupported && UseHostBackdropBrush ? 1 : 0,
                Source = hostBackdropEffect,
            };

            // Source 2 : Tint color effect
            GaussianBlurEffect gaussianBlurEffect = new()
            {
                Name = "GaussianBlurEffect",
                BlurAmount = IsHostBackdropSupported && UseHostBackdropBrush ? Math.Max(BlurAmount - 30, 0) : BlurAmount,
                Source = new CompositionEffectSourceParameter("source"),
                BorderMode = EffectBorderMode.Hard
            };

            BlendEffect luminosityColorEffect = new()
            {
                Mode = BlendEffectMode.Color,
                Foreground = new ColorSourceEffect
                {
                    Name = "LuminosityColorEffect",
                    Color = convertedLuminosityColor,
                },
                Background = gaussianBlurEffect
            };

            ColorSourceEffect tintColorEffect = new()
            {
                Name = "TintColorEffect",
                Color = convertedTintColor,
            };

            BlendEffect tintAndLuminosityColorEffect = new()
            {
                Mode = BlendEffectMode.Luminosity,
                Foreground = tintColorEffect,
                Background = luminosityColorEffect
            };

            OpacityEffect tintColorOpacityEffect = new()
            {
                Name = "TintColorOpacityEffect",
                Opacity = convertedTintColor.A is 255 ? 0f : 1f,
                Source = tintAndLuminosityColorEffect
            };

            // Source 3: Tint color effect without alpha
            ColorSourceEffect tintColorEffectWithoutAlphaEffect = new()
            {
                Name = "TintColorEffectWithoutAlpha",
                Color = convertedTintColor
            };

            OpacityEffect TintColorWithoutAlphaOpacityEffect = new()
            {
                Name = "TintColorWithoutAlphaOpacityEffect",
                Opacity = convertedTintColor.A is 255 ? 1f : 0f,
                Source = tintColorEffectWithoutAlphaEffect,
            };

            // Source 4 : Noise border effect
            BorderEffect noiseBorderEffect = new()
            {
                Source = new CompositionEffectSourceParameter("noise"),
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
            };

            OpacityEffect noiseEffect = new()
            {
                Opacity = 0.02f,
                Source = noiseBorderEffect,
            };

            CompositeEffect compositeEffect = new()
            {
                Mode = CanvasComposite.SourceOver,
                Sources =
                {
                    hostBackdropLayerEffect,
                    tintColorOpacityEffect,
                    TintColorWithoutAlphaOpacityEffect,
                    noiseEffect
                }
            };

            CompositionSurfaceBrush noiseBrush = compositor.CreateSurfaceBrush();
            noiseBrush.Stretch = CompositionStretch.None;
            noiseBrush.Surface = LoadedImageSurface.StartLoadFromUri(new Uri("ms-appx:///Assets/Images/NoiseAsset_256x256_PNG.png"));

            CompositionEffectBrush desktopAcrylicBrush = compositor.CreateEffectFactory(compositeEffect).CreateBrush();

            CompositionBrush backdropBrush = IsHostBackdropSupported && UseHostBackdropBrush ? compositor.CreateHostBackdropBrush() : compositor.CreateBackdropBrush();

            desktopAcrylicBrush.SetSourceParameter("source", backdropBrush);
            desktopAcrylicBrush.SetSourceParameter("noise", noiseBrush);

            return desktopAcrylicBrush;
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
                CrossFade = 0
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
