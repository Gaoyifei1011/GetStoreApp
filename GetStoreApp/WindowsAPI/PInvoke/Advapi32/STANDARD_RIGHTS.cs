using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// 包含对象的标准访问权限
    /// </summary>
    [Flags]
    public enum STANDARD_RIGHTS : uint
    {
        /// <summary>
        /// 删除对象的权限。
        /// </summary>
        DELETE = 0x00010000,

        /// <summary>
        /// 读取对象 安全描述符中的信息的权限，不包括 系统访问控制列表中 的信息 (SACL) 。
        /// </summary>
        READ_CONTROL = 0x00020000,

        /// <summary>
        /// 修改对象安全描述符中 (DACL) 的可自由裁量访问控制列表 的权限。
        /// </summary>
        WRITE_DAC = 0x00040000,

        /// <summary>
        /// 更改对象安全说明符中的所有者的权限。
        /// </summary>
        WRITE_OWNER = 0x00080000,

        /// <summary>
        /// 将对象用于同步的权限。 这使线程可以等待对象处于信号状态。 某些对象类型不支持此访问权限。
        /// </summary>
        SYNCHRONIZE = 0x00100000,

        /// <summary>
        /// 合并 DELETE、READ_CONTROL、WRITE_DAC 和WRITE_OWNER访问。
        /// </summary>
        STANDARD_RIGHTS_REQUIRED = 0x000F0000,

        /// <summary>
        /// 当前定义为等于 <see cref="READ_CONTROL"/>
        /// </summary>
        STANDARD_RIGHTS_READ = READ_CONTROL,

        /// <summary>
        /// 当前定义为等于 <see cref="READ_CONTROL"/>
        /// </summary>
        STANDARD_RIGHTS_WRITE = READ_CONTROL,

        /// <summary>
        /// 当前定义为等于 <see cref="READ_CONTROL"/>
        /// </summary>
        STANDARD_RIGHTS_EXECUTE = READ_CONTROL,

        /// <summary>
        /// 合并 DELETE、READ_CONTROL、WRITE_DAC、WRITE_OWNER 和 SYNCHRONIZE 访问。
        /// </summary>
        STANDARD_RIGHTS_ALL = 0x001F0000,
    }
}
