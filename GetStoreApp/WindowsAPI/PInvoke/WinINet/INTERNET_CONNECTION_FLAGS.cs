using System;

namespace GetStoreApp.WindowsAPI.PInvoke.WinINet
{
    [Flags]
    public enum INTERNET_CONNECTION_FLAGS
    {
        /// <summary>
        /// 本地系统使用调制解调器连接到 Internet。
        /// </summary>
        INTERNET_CONNECTION_MODEM = 0x1,

        /// <summary>
        /// 本地系统使用局域网连接到 Internet。
        /// </summary>
        INTERNET_CONNECTION_LAN = 0x2,

        /// <summary>
        /// 本地系统使用代理服务器连接到 Internet。
        /// </summary>
        INTERNET_CONNECTION_PROXY = 0x4,

        /// <summary>
        /// 不再使用。
        /// </summary>
        INTERNET_CONNECTION_MODEM_BUSY = 0x8,

        /// <summary>
        /// 本地系统已安装 RAS。
        /// </summary>
        INTERNET_RAS_INSTALLED = 0x10,

        /// <summary>
        /// 本地系统处于脱机模式。
        /// </summary>
        INTERNET_CONNECTION_OFFLINE = 0x20,

        /// <summary>
        /// 本地系统具有与 Internet 的有效连接，但它可能或当前未连接。
        /// </summary>
        INTERNET_CONNECTION_CONFIGURED = 0x40
    }
}
