using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// 注册表保留键值的句柄
    /// </summary>
    public static class ReservedKeyHandles
    {
        /// <summary>
        /// 从属于此键的注册表项定义文档的类型（或类）以及与这些类型关联的属性。 Shell 和 COM 应用程序使用存储在此密钥下的信息。
        /// 此密钥还通过存储 DDE 和 OLE 支持的信息来向后兼容 Windows 3.1 注册数据库。 文件查看器和用户界面扩展将其 OLE 类标识符存储在 HKEY_CLASSES_ROOT中，进程内服务器在此密钥中注册。
        /// 此句柄不应在模拟不同用户的服务或应用程序中使用。
        /// </summary>
        public static readonly nuint HKEY_CLASSES_ROOT = new(0x80000000);

        /// <summary>
        /// 从属此键的注册表项定义当前用户的首选项。 这些首选项包括环境变量的设置、有关程序组的数据、颜色、打印机、网络连接和应用程序首选项。 通过此密钥，可以更轻松地建立当前用户的设置;密钥映射到 HKEY_USERS中的当前用户的分支。 在 HKEY_CURRENT_USER中，软件供应商存储当前用户特定的首选项，供其应用程序使用。 例如，Microsoft 创建 HKEY_CURRENT_USER\Software\Microsoft 密钥供其应用程序使用，每个应用程序在 Microsoft 密钥下创建自己的子项。
        /// HKEY_CURRENT_USER和HKEY_USERS之间的映射是每个进程，并且是在进程首次引用HKEY_CURRENT_USER时建立的。 映射基于第一个线程的安全上下文来引用 HKEY_CURRENT_USER。 如果此安全上下文未在 HKEY_USERS中加载注册表配置单元，则会使用 HKEY_USERS\.Default建立映射。 建立此映射后，即使线程的安全上下文发生更改，它也会保留。
        /// 除HKEY_CURRENT_USER\Software\Classes以下注册表项外，HKEY_CURRENT_USER中的所有注册表项都包含在漫游用户配置文件的每个用户注册表部分中。 若要从漫游用户配置文件中排除其他条目，请将这些条目存储在 HKEY_CURRENT_USER_LOCAL_SETTINGS中。
        /// 此句柄不应在模拟不同用户的服务或应用程序中使用。 请改为调用 RegOpenCurrentUser 函数。
        /// </summary>
        public static readonly nuint HKEY_CURRENT_USER = new(0x80000001);

        /// <summary>
        /// 从属此键的注册表项定义计算机的物理状态，包括有关总线类型、系统内存和已安装硬件和软件的数据。 它包含保存当前配置数据的子项，包括 enum 分支 (即插即用信息，其中包括系统) 、网络登录首选项、网络安全信息、软件相关信息 (（例如服务器名称和服务器) 的位置）和其他系统信息的完整列表。
        /// </summary>
        public static readonly nuint HKEY_LOCAL_MACHINE = new(0x80000002);

        /// <summary>
        /// 从属此键的注册表项为本地计算机上的新用户定义默认用户配置，并为当前用户定义用户配置。
        /// </summary>
        public static readonly nuint HKEY_USERS = new(0x80000003);

        /// <summary>
        /// 从属此键的注册表项允许访问性能数据。 数据实际上未存储在注册表中;注册表函数会导致系统从其源收集数据。
        /// </summary>
        public static readonly nuint HKEY_PERFORMANCE_DATA = new(0x80000004);

        /// <summary>
        /// 包含有关本地计算机系统的当前硬件配置文件的信息。 HKEY_CURRENT_CONFIG下的信息仅描述了当前硬件配置与标准配置之间的差异。 有关标准硬件配置的信息存储在HKEY_LOCAL_MACHINE的软件和系统密钥下。
        /// HKEY_CURRENT_CONFIG 是 HKEY_LOCAL_MACHINE\System\CurrentControlSet\Hardware Profiles\Current的别名。
        /// 有关详细信息，请参阅 HKEY_CURRENT_CONFIG。
        /// </summary>
        public static readonly nuint HKEY_CURRENT_CONFIG = new(0x80000005);

        /// <summary>
        /// HKEY_DYN_DATA是windows 95、windows 98（98se）、windows me以上三种windows操作系统的注册表当中所特有的一个主键。
        /// </summary>
        public static readonly nuint HKEY_DYN_DATA = new(0x80000006);

        /// <summary>
        /// 从属此键的注册表项定义当前用户的本地计算机的首选项。 这些条目不包括在漫游用户配置文件的每个用户注册表部分中。 Windows Server 2008、Windows Vista、Windows Server 2003 和 Windows XP/2000：从 Windows 7 和 Windows Server 2008 R2 开始支持此密钥。
        /// </summary>
        public static readonly nuint HKEY_CURRENT_USER_LOCAL_SETTINGS = new(0x80000007);

        /// <summary>
        /// 从属此键的注册表项引用描述美国英语计数器的文本字符串。 这些条目不适用于Regedit.exe和Regedt32.exe。Windows 2000：不支持此密钥。
        /// </summary>
        public static readonly nuint HKEY_PERFORMANCE_TEXT = new(0x80000050);

        /// <summary>
        /// 从属于此键的注册表项引用文本字符串，这些字符串描述计算机系统所在区域的本地语言的计数器。 这些条目不适用于Regedit.exe和Regedt32.exe。Windows 2000：不支持此密钥。
        /// </summary>
        public static readonly nuint HKEY_PERFORMANCE_NLSTEXT = new(0x80000060);
    }
}
