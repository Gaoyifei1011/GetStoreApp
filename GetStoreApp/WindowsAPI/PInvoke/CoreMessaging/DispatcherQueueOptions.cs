using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.CoreMessaging
{
    /// <summary>
    /// 为新的 DispatcherQueueController 指定线程和单元类型。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DispatcherQueueOptions
    {
        /// <summary>
        /// <see cref="DispatcherQueueOptions"> 结构的大小
        /// </summary>
        public int dwSize;

        /// <summary>
        /// 创建的 DispatcherQueueController 的线程关联
        /// </summary>
        public int threadType;

        /// <summary>
        /// 指定是将新线程上的 COM 单元初始化为应用程序单线程单元， (ASTA) 还是单线程单元 (STA) 。
        /// 仅当 threadTypeDQTYPE_THREAD_DEDICATED时，此字段才相关。 当 DispatcherQueueOptions.threadTypeDQTYPE_THREAD_CURRENT时，请使用DQTAT_COM_NONE。
        /// </summary>
        public int apartmentType;
    }
}
