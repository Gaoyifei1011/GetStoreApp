namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// 指向接收以下处置值之一的变量的指针
    /// </summary>
    public enum REG_DISPOSITION
    {
        /// <summary>
        /// 密钥不存在且已创建。
        /// </summary>
        REG_CREATED_NEW_KEY = 0x00000001,

        /// <summary>
        /// 密钥存在，只是打开而不更改。
        /// </summary>
        REG_OPENED_EXISTING_KEY = 0x00000002
    }
}
