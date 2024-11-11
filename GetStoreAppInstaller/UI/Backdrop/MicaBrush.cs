using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.System.Power;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace GetStoreAppInstaller.UI.Backdrop
{
    /// <summary>
    /// Mica 背景色
    /// </summary>
    public sealed partial class MicaBrush : XamlCompositionBrushBase
    {
        private bool isConnected;
        private bool useMicaBrush;

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

        private readonly bool isInputActive;
        private readonly float lightTintOpacity;
        private readonly float lightLuminosityOpacity;
        private readonly float darkTintOpacity;
        private readonly float darkLuminosityOpacity;
        private Color lightTintColor = Color.FromArgb(0, 0, 0, 0);
        private Color lightFallbackColor = Color.FromArgb(0, 0, 0, 0);
        private Color darkTintColor = Color.FromArgb(0, 0, 0, 0);
        private Color darkFallbackColor = Color.FromArgb(0, 0, 0, 0);

        private readonly UISettings uiSettings = new();
        private readonly AccessibilitySettings accessibilitySettings = new();
        private readonly CompositionCapabilities compositionCapabilities = CompositionCapabilities.GetForCurrentView();
        private readonly DispatcherQueue dispatcherQueue = Window.Current.CoreWindow.DispatcherQueue;

        public MicaBrush(MicaKind micaKind, bool isInputActive)
        {
            this.isInputActive = isInputActive;

            if (micaKind is MicaKind.Base)
            {
                lightTintOpacity = defaultMicaBaseLightTintOpacity;
                lightLuminosityOpacity = defaultMicaBaseLightLuminosityOpacity;
                darkTintOpacity = defaultMicaBaseDarkTintOpacity;
                darkLuminosityOpacity = defaultMicaBaseDarkLuminosityOpacity;
                lightTintColor = defaultMicaBaseLightTintColor;
                lightFallbackColor = defaultMicaBaseLightFallbackColor;
                darkTintColor = defaultMicaBaseDarkTintColor;
                darkFallbackColor = defaultMicaBaseDarkFallbackColor;
            }
            else
            {
                lightTintOpacity = defaultMicaAltLightTintOpacity;
                lightLuminosityOpacity = defaultMicaAltLightLuminosityOpacity;
                darkTintOpacity = defaultMicaAltDarkTintOpacity;
                darkLuminosityOpacity = defaultMicaAltDarkLuminosityOpacity;
                lightTintColor = defaultMicaAltLightTintColor;
                lightFallbackColor = defaultMicaAltLightFallbackColor;
                darkTintColor = defaultMicaAltDarkTintColor;
                darkFallbackColor = defaultMicaAltDarkFallbackColor;
            }
        }

        /// <summary>
        /// 在屏幕上首次使用画笔绘制元素时调用。
        /// </summary>
        protected override void OnConnected()
        {
            base.OnConnected();

            if (!isConnected)
            {
                isConnected = true;
                UpdateBrush();
                uiSettings.ColorValuesChanged += OnColorValuesChanged;
                Window.Current.CoreWindow.Activated += OnActivated;
                accessibilitySettings.HighContrastChanged += OnHighContrastChanged;
                compositionCapabilities.Changed += OnCompositionCapabilitiesChanged;
                PowerManager.EnergySaverStatusChanged += OnEnergySaverStatusChanged;

                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.ActualThemeChanged += OnActualThemeChanged;
                }
            }
        }

        /// <summary>
        /// 不再使用画笔绘制任何元素时调用。
        /// </summary>
        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            if (isConnected)
            {
                isConnected = false;
                uiSettings.ColorValuesChanged -= OnColorValuesChanged;
                Window.Current.CoreWindow.Activated -= OnActivated;
                accessibilitySettings.HighContrastChanged -= OnHighContrastChanged;
                compositionCapabilities.Changed -= OnCompositionCapabilitiesChanged;
                PowerManager.EnergySaverStatusChanged -= OnEnergySaverStatusChanged;

                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.ActualThemeChanged -= OnActualThemeChanged;
                }

                if (CompositionBrush is not null)
                {
                    CompositionBrush.Dispose();
                    CompositionBrush = null;
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
        /// 在窗口完成激活或停用时触发的事件
        /// </summary>
        private void OnActivated(CoreWindow sender, WindowActivatedEventArgs args)
        {
            UpdateBrush();
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
            if (isConnected)
            {
                ElementTheme actualTheme = ElementTheme.Default;

                actualTheme = Window.Current.Content is FrameworkElement rootElement ? rootElement.ActualTheme : Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;

                float tintOpacity;
                float luminosityOpacity;
                Color tintColor;
                Color fallbackColor;

                if (actualTheme is ElementTheme.Light)
                {
                    tintOpacity = lightTintOpacity;
                    luminosityOpacity = lightLuminosityOpacity;
                    tintColor = lightTintColor;
                    fallbackColor = lightFallbackColor;
                }
                else
                {
                    tintOpacity = darkTintOpacity;
                    luminosityOpacity = darkLuminosityOpacity;
                    tintColor = darkTintColor;
                    fallbackColor = darkFallbackColor;
                }

                useMicaBrush = ApiInformation.IsMethodPresent(typeof(Compositor).FullName, nameof(Compositor.TryCreateBlurredWallpaperBackdropBrush)) && uiSettings.AdvancedEffectsEnabled && PowerManager.EnergySaverStatus is not EnergySaverStatus.On && compositionCapabilities.AreEffectsSupported() && (isInputActive || Window.Current.CoreWindow.ActivationMode is not CoreWindowActivationMode.Deactivated);

                Compositor compositor = Window.Current.Compositor;

                if (accessibilitySettings.HighContrast)
                {
                    tintColor = uiSettings.GetColorValue(UIColorType.Background);
                    useMicaBrush = false;
                }

                CompositionBrush newBrush = useMicaBrush ? BuildMicaEffectBrush(compositor, tintColor, tintOpacity, luminosityOpacity) : compositor.CreateColorBrush(fallbackColor);

                CompositionBrush oldBrush = CompositionBrush;

                if (oldBrush is null || CompositionBrush.Comment is "Crossfade")
                {
                    // 直接设置新笔刷
                    oldBrush?.Dispose();
                    CompositionBrush = newBrush;
                }
                else
                {
                    // 回退色切换时的动画颜色
                    CompositionBrush crossFadeBrush = CreateCrossFadeEffectBrush(compositor, oldBrush, newBrush);
                    ScalarKeyFrameAnimation animation = CreateCrossFadeAnimation(compositor);
                    CompositionBrush = crossFadeBrush;

                    CompositionScopedBatch crossFadeAnimationBatch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                    crossFadeBrush.StartAnimation("CrossFade.CrossFade", animation);
                    crossFadeAnimationBatch.End();

                    crossFadeAnimationBatch.Completed += (o, a) =>
                    {
                        crossFadeBrush.Dispose();
                        oldBrush.Dispose();
                        CompositionBrush = newBrush;
                    };
                }
            }
        }

        /// <summary>
        /// 创建云母背景色
        /// </summary>
        private static CompositionEffectBrush BuildMicaEffectBrush(Compositor compositor, Color tintColor, float tintOpacity, float luminosityOpacity)
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
        /// 创建回退动画
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

            List<string> corssfadeList = ["Crossfade.CrossFade"];
            CompositionEffectBrush crossFadeEffectBrush = compositor.CreateEffectFactory(crossFadeEffect, corssfadeList).CreateBrush();
            crossFadeEffectBrush.Comment = "Crossfade";

            crossFadeEffectBrush.SetSourceParameter("source1", from);
            crossFadeEffectBrush.SetSourceParameter("source2", to);
            return crossFadeEffectBrush;
        }

        private ScalarKeyFrameAnimation CreateCrossFadeAnimation(Compositor compositor)
        {
            ScalarKeyFrameAnimation animation = compositor.CreateScalarKeyFrameAnimation();
            LinearEasingFunction linearEasing = compositor.CreateLinearEasingFunction();
            animation.InsertKeyFrame(0.0f, 0.0f, linearEasing);
            animation.InsertKeyFrame(1.0f, 1.0f, linearEasing);
            animation.Duration = TimeSpan.FromMilliseconds(250);
            return animation;
        }
    }
}
