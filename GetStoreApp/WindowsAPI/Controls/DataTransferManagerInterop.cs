using GetStoreApp.WindowsAPI.ComTypes;
using System;
using Windows.ApplicationModel.DataTransfer;
using WinRT;

namespace GetStoreApp.WindowsAPI.Controls
{
    /// <summary>
    /// DataTransferManager 的互操作类
    /// </summary>
    public static class DataTransferManagerInterop
    {
        private static readonly Guid riid = new Guid(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);

        private static IDataTransferManagerInterop dataTransferManagerInterop = DataTransferManager.As<IDataTransferManagerInterop>();

        /// <summary>
        /// 返回与当前窗口关联的 DataTransferManager 对象
        /// </summary>
        public static DataTransferManager GetForWindow(IntPtr window)
        {
            return MarshalInterface<DataTransferManager>.FromAbi(dataTransferManagerInterop.GetForWindow(window, riid));
        }

        /// <summary>
        /// 以编程方式启动用户界面，以便与其他应用共享内容
        /// </summary>
        public static void ShowShareUIForWindow(IntPtr window)
        {
            dataTransferManagerInterop.ShowShareUIForWindow(window);
        }
    }
}
