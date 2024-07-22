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

namespace GetStoreAppWebView.UI.Backdrop
{
    /// <summary>
    /// Mica 背景色
    /// </summary>
    public class MicaBrush : XamlCompositionBrushBase
    {
        private bool isConnected;
        private bool useSolidColorFallback;

        private double tintOpacity;
        private double luminosityOpacity;
        private Color tintColor;
        private Color luminosityColor;

        private readonly UISettings uiSettings = new();
        private readonly AccessibilitySettings accessibilitySettings = new();
        private readonly CompositionCapabilities compositionCapabilities = CompositionCapabilities.GetForCurrentView();
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

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

                FrameworkElement rootElement = Window.Current.Content as FrameworkElement;
                if (rootElement is not null)
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
                uiSettings.ColorValuesChanged -= OnColorValuesChanged;
                Window.Current.CoreWindow.Activated -= OnActivated;
                accessibilitySettings.HighContrastChanged -= OnHighContrastChanged;
                compositionCapabilities.Changed -= OnCompositionCapabilitiesChanged;
                PowerManager.EnergySaverStatusChanged -= OnEnergySaverStatusChanged;

                FrameworkElement rootElement = Window.Current.Content as FrameworkElement;
                if (rootElement is not null)
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

                FrameworkElement rootElement = Window.Current.Content as FrameworkElement;
                if (rootElement is not null)
                {
                    actualTheme = rootElement.ActualTheme;
                }
                else
                {
                    actualTheme = Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                }

                if (actualTheme is ElementTheme.Light)
                {
                    tintColor = luminosityColor = Color.FromArgb(255, 243, 243, 243);
                    tintOpacity = 50;
                    luminosityOpacity = 100;
                }
                else
                {
                    tintColor = luminosityColor = Color.FromArgb(255, 32, 32, 32);
                    tintOpacity = 80;
                    luminosityOpacity = 100;
                }

                useSolidColorFallback = ApiInformation.IsMethodPresent(typeof(Compositor).FullName, nameof(Compositor.TryCreateBlurredWallpaperBackdropBrush)) && uiSettings.AdvancedEffectsEnabled &&
                    (Window.Current.CoreWindow.ActivationMode is CoreWindowActivationMode.Deactivated || compositionCapabilities.AreEffectsSupported() is false || PowerManager.EnergySaverStatus is EnergySaverStatus.On);

                Compositor compositor = Window.Current.Compositor;

                if (accessibilitySettings.HighContrast)
                {
                    tintColor = uiSettings.GetColorValue(UIColorType.Background);
                    useSolidColorFallback = true;
                }

                CompositionBrush newBrush = useSolidColorFallback ? compositor.CreateColorBrush(tintColor) : BuildMicaEffectBrush(compositor, tintColor, (float)(tintOpacity / 100), luminosityColor, (float)(luminosityOpacity / 100));

                CompositionBrush oldBrush = CompositionBrush;

                if (oldBrush is null || CompositionBrush.Comment is "Crossfade")
                {
                    // Set new brush directly
                    oldBrush?.Dispose();
                    CompositionBrush = newBrush;
                }
                else
                {
                    // Crossfade
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

        private static CompositionBrush BuildMicaEffectBrush(Compositor compositor, Color tintColor, float tintOpacity, Color luminosityColor, float luminosityOpacity)
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
                Color = luminosityColor
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

        private CompositionBrush CreateCrossFadeEffectBrush(Compositor compositor, CompositionBrush from, CompositionBrush to)
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
