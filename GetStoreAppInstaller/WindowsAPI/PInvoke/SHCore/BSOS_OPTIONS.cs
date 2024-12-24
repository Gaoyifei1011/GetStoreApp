namespace GetStoreAppInstaller.WindowsAPI.PInvoke.SHCore
{
    /// <summary>
    /// 指定将组件对象模型封装 (COM) IStream 的 RandomAccessStream 的行为。
    /// </summary>
    public enum BSOS_OPTIONS
    {
        /// <summary>
        /// 通过流创建 RandomAccessStream 时，请通过 Stat 方法对 STGM 模式使用基本 IRandomAccessStream 行为。
        /// </summary>
        BSOS_DEFAULT = 0,

        /// <summary>
        /// 使用 GetDestinationStream 方法。
        /// </summary>
        BSOS_PREFERDESTINATIONSTREAM,
    }
}
