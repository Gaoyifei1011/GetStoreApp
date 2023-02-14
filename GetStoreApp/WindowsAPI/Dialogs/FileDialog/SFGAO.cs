using System;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// 可以在项目 (文件或文件夹) 或一组项上检索的属性。
    /// </summary>
    [Flags]
    public enum SFGAO : uint
    {
        /// <summary>
        /// 可以复制指定的项。
        /// </summary>
        SFGAO_CANCOPY = 0x1,

        /// <summary>
        /// 可以移动指定的项。
        /// </summary>
        SFGAO_CANMOVE = 0x2,

        /// <summary>
        /// 可以为指定的项创建快捷方式。
        /// </summary>
        SFGAO_CANLINK = 0x4,

        /// <summary>
        /// 指定的项可以通过 IShellFolder::BindToObject 绑定到 IStorage 对象。
        /// </summary>
        SFGAO_STORAGE = 0x00000008,

        /// <summary>
        /// 可以重命名指定的项。
        /// </summary>
        SFGAO_CANRENAME = 0x00000010,

        /// <summary>
        /// 可以删除指定的项。
        /// </summary>
        SFGAO_CANDELETE = 0x00000020,

        /// <summary>
        /// 指定的项具有属性表。
        /// </summary>
        SFGAO_HASPROPSHEET = 0x00000040,

        /// <summary>
        /// 指定的项是删除目标。
        /// </summary>
        SFGAO_DROPTARGET = 0x00000100,

        /// <summary>
        /// 此标志是功能属性的掩码：<see cref="SFGAO_CANCOPY">、<see cref="SFGAO_CANMOVE">、<see cref="SFGAO_CANLINK">、<see cref="SFGAO_CANRENAME">、<see cref="SFGAO_CANDELETE">、<see cref="SFGAO_HASPROPSHEET"> 和 <see cref="SFGAO_DROPTARGET">。
        /// </summary>
        SFGAO_CAPABILITYMASK = 0x00000177,

        /// <summary>
        ///
        /// </summary>
        SFGAO_PLACEHOLDER = 0x00000800,

        /// <summary>
        /// Windows 7 及更高版本。 指定的项是系统项。
        /// </summary>
        SFGAO_SYSTEM = 0x00001000,

        /// <summary>
        /// 指定的项已加密，可能需要特殊演示。
        /// </summary>
        SFGAO_ENCRYPTED = 0x00002000,

        /// <summary>
        /// 通过 <see cref="IStream"> 或其他存储接口访问项应该是一个缓慢的操作。 应用程序应避免访问标记为 <see cref="SFGAO_ISSLOW"> 的项目。
        /// </summary>
        SFGAO_ISSLOW = 0x00004000,

        /// <summary>
        /// 指定的项显示为灰色且对用户不可用。
        /// </summary>
        SFGAO_GHOSTED = 0x00008000,

        /// <summary>
        /// 指定的项是快捷方式。
        /// </summary>
        SFGAO_LINK = 0x00010000,

        /// <summary>
        /// 指定的对象是共享的。
        /// </summary>
        SFGAO_SHARE = 0x00020000,

        /// <summary>
        /// 指定的项是只读的。
        /// </summary>
        SFGAO_READONLY = 0x00040000,

        /// <summary>
        /// 项目处于隐藏状态，除非文件夹设置中启用了“显示隐藏文件和文件夹”选项，否则不应显示该项目。
        /// </summary>
        SFGAO_HIDDEN = 0x00080000,

        /// <summary>
        /// 请勿使用。
        /// </summary>
        SFGAO_DISPLAYATTRMASK = 0x000FC000,

        /// <summary>
        /// 项是无数项，应隐藏。 它们不会通过枚举器返回，例如 IShellFolder::EnumObjects 方法创建的枚举器
        /// </summary>
        SFGAO_NONENUMERATED = 0x00100000,

        /// <summary>
        /// 项包含由特定应用程序定义的新内容。
        /// </summary>
        SFGAO_NEWCONTENT = 0x00200000,

        /// <summary>
        /// 不支持。
        /// </summary>
        SFGAO_CANMONIKER = 0x00400000,

        /// <summary>
        /// 不支持。
        /// </summary>
        SFGAO_HASSTORAGE = 0x00400000,

        /// <summary>
        /// 指示该项具有与之关联的流。 可以通过调用 IShellFolder::BindToObject 或 <see cref="IShellItem.BindToHandler"> 访问该流，并在 riid 参数中使用IID_IStream。
        /// </summary>
        SFGAO_STREAM = 0x00400000,

        /// <summary>
        /// 此项目的子级可通过 <see cref="IStream"> 或 IStorage 访问。 这些子项标有 <see cref="SFGAO_STORAGE"> 或 <see cref="SFGAO_STREAM">。
        /// </summary>
        SFGAO_STORAGEANCESTOR = 0x00800000,

        /// <summary>
        /// 指定为输入时，<see cref="SFGAO_VALIDATE"> 指示文件夹验证文件夹或 Shell 项数组中包含的项目是否存在。 如果其中一个或多个项不存在， IShellFolder::GetAttributesOf 和 IShellItemArray::GetAttributes 返回失败代码。 此标志永远不会作为 [out] 值返回。
        /// 与文件系统文件夹一起使用时，<see cref="SFGAO_VALIDATE"> 指示该文件夹放弃 IShellFolder2::GetDetailsEx 客户端检索到的缓存属性，这些属性可能已为指定项累积。
        /// </summary>
        SFGAO_VALIDATE = 0x01000000,

        /// <summary>
        /// 指定的项位于可移动媒体上，或者本身是可移动设备。
        /// </summary>
        SFGAO_REMOVABLE = 0x02000000,

        /// <summary>
        /// 指定的项已压缩。
        /// </summary>
        SFGAO_COMPRESSED = 0x04000000,

        /// <summary>
        /// 指定的项可以托管在 Web 浏览器或Windows资源管理器框架中。
        /// </summary>
        SFGAO_BROWSABLE = 0x08000000,

        /// <summary>
        /// 指定的文件夹是文件系统文件夹，或者包含至少一个子代 (子级、孙子或更高版本) ，即文件系统 (<see cref="SFGAO_FILESYSTEM">) 文件夹。
        /// </summary>
        SFGAO_FILESYSANCESTOR = 0x10000000,

        /// <summary>
        /// 指定的项是文件夹。 某些项可以使用 <see cref="SFGAO_STREAM"> 和 <see cref="SFGAO_FOLDER"> 进行标记，例如具有.zip文件扩展名的压缩文件。 某些应用程序在测试属于文件和容器的项时，可能会包含此标志。
        /// </summary>
        SFGAO_FOLDER = 0x20000000,

        /// <summary>
        /// 指定的文件夹或文件是文件系统 (的一部分，即文件、目录或根目录) 。 可以假定项的已分析名称是有效的 Win32 文件系统路径。 这些路径可以是 UNC 或基于驱动器号。
        /// </summary>
        SFGAO_FILESYSTEM = 0x40000000,

        /// <summary>
        /// 此标志是存储功能属性的掩码：<see cref="SFGAO_STORAGE">、<see cref="SFGAO_LINK">、<see cref="SFGAO_READONLY">、<see cref="SFGAO_STREAM">、<see cref="SFGAO_STORAGEANCESTOR">、<see cref="SFGAO_FILESYSANCESTOR">、<see cref="SFGAO_FOLDER"> 和 <see cref="SFGAO_FILESYSTEM">。 调用方通常不使用此值。
        /// </summary>
        SFGAO_STORAGECAPMASK = 0x70C50008,

        /// <summary>
        /// 指定的文件夹具有子文件夹。 <see cref="SFGAO_HASSUBFOLDER"> 属性只是公告，即使它们不包含子文件夹，Shell 文件夹实现也可能返回。 但是，请注意，相反的（未能返回 <see cref="SFGAO_HASSUBFOLDER"> ）明确指出文件夹对象没有子文件夹。
        /// </summary>
        SFGAO_HASSUBFOLDER = 0x80000000,

        /// <summary>
        /// 此标志是内容属性的掩码，目前仅 <see cref="SFGAO_HASSUBFOLDER">。 调用方通常不使用此值。
        /// </summary>
        SFGAO_CONTENTSMASK = 0x80000000,

        /// <summary>
        /// PKEY_SFGAOFlags属性使用的掩码用于确定被视为导致计算缓慢或缺少上下文的属性：<see cref="SFGAO_ISSLOW">、<see cref="SFGAO_READONLY">、<see cref="SFGAO_HASSUBFOLDER"> 和 <see cref="SFGAO_VALIDATE">。 调用方通常不使用此值。
        /// </summary>
        SFGAO_PKEYSFGAOMASK = 0x81044000,
    }
}
