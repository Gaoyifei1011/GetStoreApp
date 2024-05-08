namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 定义指定用于文件传输的代理的常量。 可以为每个作业定义不同的代理设置。
    /// </summary>
    public enum BG_JOB_PROXY_USAGE
    {
        /// <summary>
        /// 使用每个用户定义的代理和代理绕过列表设置来传输文件。 控制面板、Internet 选项、Connections、局域网 (LAN) 设置 (或拨号设置由用户定义，具体取决于网络连接) 。
        /// </summary>
        BG_JOB_PROXY_USAGE_PRECONFIG = 0,

        /// <summary>
        /// 不要使用代理来传输文件。 在 LAN 中传输文件时，请使用此选项。
        /// </summary>
        BG_JOB_PROXY_USAGE_NO_PROXY = 1,

        /// <summary>
        /// 使用应用程序的代理和代理绕过列表传输文件。 如果无法信任系统设置是否正确，请使用此选项。 如果要使用系统设置不适用的特殊帐户（如 LocalSystem）传输文件，也可以使用此选项。
        /// </summary>
        BG_JOB_PROXY_USAGE_OVERRIDE = 2,

        /// <summary>
        /// 自动检测代理设置。 BITS 检测作业中每个文件的代理设置。
        /// </summary>
        BG_JOB_PROXY_USAGE_AUTODETECT = 3
    }
}
