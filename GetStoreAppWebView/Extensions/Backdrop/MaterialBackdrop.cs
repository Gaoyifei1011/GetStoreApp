using GetStoreAppWebView.Services.Root;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Foundation.Diagnostics;

namespace GetStoreAppWebView.Extensions.Backdrop
{
    /// <summary>
    /// 自定义扩展的背景色
    /// </summary>
    public sealed partial class MaterialBackdrop : SystemBackdrop
    {
        private readonly bool isMicaBackdrop;
        private readonly MicaKind micaBackdropKind;
        private readonly DesktopAcrylicKind desktopAcrylicBackdropKind;
        private ISystemBackdropControllerWithTargets systemBackdropController;

        public SystemBackdropConfiguration BackdropConfiguration { get; private set; }

        public MaterialBackdrop(MicaKind micaKind)
        {
            isMicaBackdrop = true;
            micaBackdropKind = micaKind;
        }

        public MaterialBackdrop(DesktopAcrylicKind desktopAcrylicKind)
        {
            isMicaBackdrop = false;
            desktopAcrylicBackdropKind = desktopAcrylicKind;
        }

        /// <summary>
        /// 当此对象附加到有效容器时调用
        /// </summary>
        protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
        {
            base.OnTargetConnected(connectedTarget, xamlRoot);

            if (systemBackdropController is not null)
            {
                LogService.WriteLog(LoggingLevel.Warning, nameof(GetStoreAppWebView), nameof(MaterialBackdrop), nameof(OnTargetConnected), 1, new Exception());
                (Application.Current as WebViewApp).Dispose();
            }

            if (isMicaBackdrop)
            {
                systemBackdropController = new MicaController() { Kind = micaBackdropKind };
                systemBackdropController.AddSystemBackdropTarget(connectedTarget);
                BackdropConfiguration = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
                systemBackdropController.SetSystemBackdropConfiguration(BackdropConfiguration);
            }
            else
            {
                systemBackdropController = new DesktopAcrylicController() { Kind = desktopAcrylicBackdropKind };
                systemBackdropController.AddSystemBackdropTarget(connectedTarget);
                BackdropConfiguration = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
                systemBackdropController.SetSystemBackdropConfiguration(BackdropConfiguration);
            }
        }

        /// <summary>
        /// 重写此方法，以便在 GetDefaultSystemBackdropConfiguration 返回的对象发生变化时调用。
        /// </summary>
        protected override void OnDefaultSystemBackdropConfigurationChanged(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
        {
            // 添加判断代码，防止 SystemBackdrop 在修改的时候触发异常
            if (target is not null)
            { }
        }

        /// <summary>
        /// 从其容器中清除此对象时调用
        /// </summary>
        protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
        {
            base.OnTargetDisconnected(disconnectedTarget);
            systemBackdropController?.RemoveSystemBackdropTarget(disconnectedTarget);
            systemBackdropController = null;
        }
    }
}
