namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 指定地理位置类。
    /// </summary>
    public enum SYSGEOCLASS
    {
        /// <summary>
        /// 从 Windows 8 开始： 所有地理位置标识符的类。
        /// </summary>
        GEOCLASS_ALL = 0,

        /// <summary>
        /// 区域地理位置标识符的类。
        /// </summary>
        GEOCLASS_REGION = 14,

        /// <summary>
        /// 国家/地区地理位置标识符的类。
        /// </summary>
        GEOCLASS_NATION = 16
    }
}
