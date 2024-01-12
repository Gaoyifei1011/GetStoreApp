namespace GetStoreApp.WindowsAPI.PInvoke.Combase
{
    /// <summary>
    /// 确定用于对此线程创建的对象的传入调用的并发模型。
    /// </summary>
    public enum RO_INIT_TYPE
    {
        /// <summary>
        /// 初始化单线程对应的线程。 当前线程在 STA 中初始化。
        /// </summary>
        RO_INIT_SINGLETHREADED = 0,

        /// <summary>
        /// 初始化多线程并发的线程。 当前线程在 MTA 中初始化。
        /// </summary>
        RO_INIT_MULTITHREADED = 1,
    }
}
