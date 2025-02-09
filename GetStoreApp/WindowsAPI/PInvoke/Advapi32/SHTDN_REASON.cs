using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// 系统关闭原因代码
    /// </summary>
    [Flags]
    public enum SHTDN_REASON : uint
    {
        /// <summary>
        /// 应用程序问题。
        /// </summary>
        SHTDN_REASON_MAJOR_APPLICATION = 0x00040000,

        /// <summary>
        /// 硬件问题。
        /// </summary>
        SHTDN_REASON_MAJOR_HARDWARE = 0x00010000,

        /// <summary>
        /// 使用 InitiateSystemShutdown 函数而不是 InitiateSystemShutdownEx。
        /// </summary>
        SHTDN_REASON_MAJOR_LEGACY_API = 0x00070000,

        /// <summary>
        /// 操作系统问题。
        /// </summary>
        SHTDN_REASON_MAJOR_OPERATINGSYSTEM = 0x00020000,

        /// <summary>
        /// 其他问题。
        /// </summary>
        SHTDN_REASON_MAJOR_OTHER = 0x00000000,

        /// <summary>
        /// 电源故障。
        /// </summary>
        SHTDN_REASON_MAJOR_POWER = 0x00060000,

        /// <summary>
        /// 软件问题。
        /// </summary>
        SHTDN_REASON_MAJOR_SOFTWARE = 0x00030000,

        /// <summary>
        /// 系统故障。
        /// </summary>
        SHTDN_REASON_MAJOR_SYSTEM = 0x00050000,

        /// <summary>
        /// 蓝屏崩溃事件。
        /// </summary>
        SHTDN_REASON_MINOR_BLUESCREEN = 0x0000000F,

        /// <summary>
        /// 拔出。
        /// </summary>
        SHTDN_REASON_MINOR_CORDUNPLUGGED = 0x0000000b,

        /// <summary>
        /// 磁盘
        /// </summary>
        SHTDN_REASON_MINOR_DISK = 0x00000007,

        /// <summary>
        /// 环境。
        /// </summary>
        SHTDN_REASON_MINOR_ENVIRONMENT = 0x0000000c,

        /// <summary>
        /// 司机。
        /// </summary>
        SHTDN_REASON_MINOR_HARDWARE_DRIVER = 0x0000000d,

        /// <summary>
        /// 热修复。
        /// </summary>
        SHTDN_REASON_MINOR_HOTFIX = 0x00000011,

        /// <summary>
        /// 热修复卸载。
        /// </summary>
        SHTDN_REASON_MINOR_HOTFIX_UNINSTALL = 0x00000017,

        /// <summary>
        /// 反应迟钝。
        /// </summary>
        SHTDN_REASON_MINOR_HUNG = 0x00000005,

        /// <summary>
        /// 安装。
        /// </summary>
        SHTDN_REASON_MINOR_INSTALLATION = 0x00000002,

        /// <summary>
        /// 维护。
        /// </summary>
        SHTDN_REASON_MINOR_MAINTENANCE = 0x00000001,

        /// <summary>
        /// MMC 问题。
        /// </summary>
        SHTDN_REASON_MINOR_MMC = 0x00000019,

        /// <summary>
        /// 网络连接。
        /// </summary>
        SHTDN_REASON_MINOR_NETWORK_CONNECTIVITY = 0x00000014,

        /// <summary>
        /// 网络卡。
        /// </summary>
        SHTDN_REASON_MINOR_NETWORKCARD = 0x00000009,

        /// <summary>
        /// 其他问题。
        /// </summary>
        SHTDN_REASON_MINOR_OTHER = SHTDN_REASON_MAJOR_OTHER,

        /// <summary>
        /// 其他驱动程序事件。
        /// </summary>
        SHTDN_REASON_MINOR_OTHERDRIVER = 0x0000000e,

        /// <summary>
        /// 电源。
        /// </summary>
        SHTDN_REASON_MINOR_POWER_SUPPLY = 0x0000000a,

        /// <summary>
        /// 处理器。
        /// </summary>
        SHTDN_REASON_MINOR_PROCESSOR = 0x00000008,

        /// <summary>
        /// 配置。
        /// </summary>
        SHTDN_REASON_MINOR_RECONFIG = 0x00000004,

        /// <summary>
        /// 安全问题。
        /// </summary>
        SHTDN_REASON_MINOR_SECURITY = 0x00000013,

        /// <summary>
        /// 安全修补程序。
        /// </summary>
        SHTDN_REASON_MINOR_SECURITYFIX = 0x00000012,

        /// <summary>
        /// 安全修补程序卸载。
        /// </summary>
        SHTDN_REASON_MINOR_SECURITYFIX_UNINSTALL = 0x00000018,

        /// <summary>
        /// Service Pack。
        /// </summary>
        SHTDN_REASON_MINOR_SERVICEPACK = 0x00000010,

        /// <summary>
        /// Service Pack 卸载。
        /// </summary>
        SHTDN_REASON_MINOR_SERVICEPACK_UNINSTALL = 0x00000016,

        /// <summary>
        /// 终端服务。
        /// </summary>
        SHTDN_REASON_MINOR_TERMSRV = 0x00000020,

        /// <summary>
        /// 稳定。
        /// </summary>
        SHTDN_REASON_MINOR_UNSTABLE = 0x00000006,

        /// <summary>
        /// 升级。
        /// </summary>
        SHTDN_REASON_MINOR_UPGRADE = 0x00000003,

        /// <summary>
        /// WMI 问题。
        /// </summary>
        SHTDN_REASON_MINOR_WMI = 0x00000015,

        /// <summary>
        /// 原因代码由用户定义。 有关详细信息，请参阅定义自定义原因代码。如果此标志不存在，则原因代码由系统定义。
        /// </summary>
        SHTDN_REASON_FLAG_USER_DEFINED = 0x40000000,

        /// <summary>
        /// 计划关闭。 系统 (SSD) 文件生成系统状态数据。 此文件包含系统状态信息，例如进程、线程、内存使用情况和配置。
        /// 如果此标志不存在，则表示关闭计划外。 通知和报告选项由一组策略控制。 例如，登录后，如果已启用策略，系统将显示一个对话框，报告计划外关闭。 仅当在系统上启用了 SSD 策略时，才会创建 SSD 文件。 管理员可以使用 Windows 错误报告 将 SSD 数据发送到中心位置或 Microsoft。
        /// </summary>
        SHTDN_REASON_FLAG_PLANNED = 0x80000000
    }
}
