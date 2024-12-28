using System;

namespace WindowsTools.WindowsAPI.PInvoke.Rstrtmgr
{
    /// <summary>
    /// 重启选项
    /// </summary>
    [Flags]
    public enum RM_SHUTDOWN_TYPE : uint
    {
        /// <summary>
        /// 强制在超时期限后关闭无响应的应用程序和服务。 未响应关闭请求的应用程序在 30 秒内被迫关闭。 不响应关闭请求的服务在 20 秒后被迫关闭。
        /// </summary>
        RmForceShutdown = 0x1,

        /// <summary>
        /// 仅当已使用 RegisterApplicationRestart 函数注册所有应用程序以重启时，才关闭应用程序。 如果无法重启任何进程或服务，则不会关闭任何进程或服务。
        /// </summary>
        RmShutdownOnlyRegistered = 0x10
    }
}
