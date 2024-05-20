using System;
using WinRT;

namespace Windows.ApplicationModel.DataTransfer
{
    /// <summary>
    /// DataTransferManager 互操作辅助类
    /// 允许访问管理多个窗口的 Windows 应用商店应用中的 DataTransferManager 方法。
    /// 临时放在 Microsoft.Management.Deployment.Projection 程序集中
    /// 因为 CS0122，WinRT.FactoryObjectReference 和 WinRT.ActivationFactory 类不可访问，因为它具有一定的保护级别
    /// 等待 CsWinRT 官方仓库合并后，该段临时代码会直接移除
    /// </summary>
    public static class DataTransferManagerInterop
    {
        private static readonly Guid IDataTransferManagerInterop_IID = new("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8");

        private static readonly Guid riid = new("A5CAEE9B-8708-49D1-8D36-67D25A8DA00C");

        private static readonly FactoryObjectReference<IObjectReference> factoryObjectReference;

        static DataTransferManagerInterop()
        {
            // typeName = typeof(Windows.ApplicationModel.DataTransfer.DataTransferManager).FullName
            factoryObjectReference = ActivationFactory.Get<IObjectReference>("Windows.ApplicationModel.DataTransfer.DataTransferManager", IDataTransferManagerInterop_IID);
        }

        /// <summary>
        /// 获取指定窗口的 DataTransferManager 实例。
        /// </summary>
        /// <param name="appWindow">要检索其 DataTransferManager 实例的窗口。</param>
        /// <returns>返回 DataTransferManager 实例。</returns>
        public static DataTransferManager GetForWindow(IntPtr appWindow)
        {
            return IDataTransferManagerInteropMethods.GetForWindow(factoryObjectReference, appWindow, riid);
        }

        /// <summary>
        /// 显示用于共享指定窗口内容的 UI。
        /// </summary>
        /// <param name="appWindow">显示共享 UI 的窗口。</param>
        public static void ShowShareUIForWindow(IntPtr appWindow)
        {
            IDataTransferManagerInteropMethods.ShowShowShareUIForWindow(factoryObjectReference, appWindow);
        }
    }
}
