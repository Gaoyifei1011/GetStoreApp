using GetStoreApp.Services.Root;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;

namespace GetStoreApp.Extensions.Backdrop
{
    /// <summary>
    /// 自定义扩展的亚克力背景色类
    /// </summary>
    public class DesktopAcrylicSystemBackdrop : SystemBackdrop
    {
        private DesktopAcrylicController _desktopAcrylicController;

        public SystemBackdropConfiguration BackdropConfiguration { get; private set; }

        /// <summary>
        /// 在 Window.SystemBackdrop 上设置时触发该方法
        /// </summary>
        protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
        {
            base.OnTargetConnected(connectedTarget, xamlRoot);

            if (_desktopAcrylicController is not null)
            {
                throw new ApplicationException(ResourceService.GetLocalized("Resources/SystemBackdropControllerInitializeFailed"));
            }

            _desktopAcrylicController = new DesktopAcrylicController();
            _desktopAcrylicController.AddSystemBackdropTarget(connectedTarget);

            SetControllerConfiguration(connectedTarget, xamlRoot);
        }

        /// <summary>
        /// 从 Window.SystemBackdrop 对象中清除此对象时调用。
        /// </summary>
        protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
        {
            base.OnTargetDisconnected(disconnectedTarget);

            _desktopAcrylicController.RemoveSystemBackdropTarget(disconnectedTarget);
            _desktopAcrylicController = null;
        }

        /// <summary>
        /// 设置应用于系统背景目标的系统策略
        /// </summary>
        private void SetControllerConfiguration(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
        {
            BackdropConfiguration = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
            BackdropConfiguration.IsInputActive = true;
            _desktopAcrylicController.SetSystemBackdropConfiguration(BackdropConfiguration);
        }
    }
}
