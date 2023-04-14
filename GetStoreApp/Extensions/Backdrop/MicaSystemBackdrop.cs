using GetStoreApp.Services.Root;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;

namespace GetStoreApp.Extensions.Backdrop
{
    /// <summary>
    /// 自定义扩展的云母背景色类
    /// </summary>
    public class MicaSystemBackdrop : SystemBackdrop
    {
        private MicaController _micaController;

        public SystemBackdropConfiguration BackdropConfiguration { get; private set; }

        public MicaKind Kind { get; set; } = MicaKind.Base;

        /// <summary>
        /// 在 Window.SystemBackdrop 上设置时触发该方法
        /// </summary>
        protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
        {
            base.OnTargetConnected(connectedTarget, xamlRoot);

            if (_micaController != null)
            {
                throw new ApplicationException(ResourceService.GetLocalized("Resources/SystemBackdropControllerInitializeFailed"));
            }

            _micaController = new MicaController() { Kind = Kind };
            _micaController.AddSystemBackdropTarget(connectedTarget);

            SetControllerConfiguration(connectedTarget, xamlRoot);
        }

        /// <summary>
        /// 从 Window.SystemBackdrop 对象中清除此对象时调用。
        /// </summary>
        protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
        {
            base.OnTargetDisconnected(disconnectedTarget);

            _micaController.RemoveSystemBackdropTarget(disconnectedTarget);
            _micaController = null;
        }

        /// <summary>
        /// 设置应用于系统背景目标的系统策略
        /// </summary>
        private void SetControllerConfiguration(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
        {
            BackdropConfiguration = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
            BackdropConfiguration.IsInputActive = true;
            _micaController.SetSystemBackdropConfiguration(BackdropConfiguration);
        }
    }
}
