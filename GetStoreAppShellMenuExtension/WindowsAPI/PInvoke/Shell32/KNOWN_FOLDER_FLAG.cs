namespace WindowsToolsShellExtension.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 定义指定 已知文件夹 的特殊检索选项的常量 (例如，在调用 SHGetKnownFolderIDList 函数以检索已知文件夹) 的路径时使用。 这些值取代了具有并行含义的 CSIDL 值。
    /// </summary>
    public enum KNOWN_FOLDER_FLAG : uint
    {
        /// <summary>
        /// 指定没有特殊检索选项。
        /// </summary>
        KF_FLAG_DEFAULT = 0x00000000,

        /// <summary>
        /// 在 Windows 10 版本 1709 中引入。 从打包应用调用时，指定 LocalAppData/RoamingAppData 文件夹重定向到专用应用位置，这些位置与 LocalFolder 和 RoamingFolder 属性中从 Windows.Storage.ApplicationData.Current 返回的路径相匹配。 其他文件夹将重定向到 LocalAppData 的子目录。
        /// 此标志与 FOLDERID_AppDataDesktop、 FOLDERID_AppDataDocuments、 FOLDERID_AppDataFavorites 和 FOLDERID_AppDataProgramData 一起使用。 它还旨在与 .NET 应用程序兼容，而不是直接从应用程序使用。
        /// </summary>
        KF_FLAG_FORCE_APP_DATA_REDIRECTION = 0x00080000,

        /// <summary>
        /// 在 Windows 10 版本 1703 中引入。 在打包进程中运行时，指定文件系统将某些文件系统位置重定向到特定于包的位置。 此标志会导致返回这些位置的方向目标。 这在文件系统中的实际位置需要知道的情况下非常有用。
        /// </summary>
        KF_FLAG_RETURN_FILTER_REDIRECTION_TARGET = 0x00040000,

        /// <summary>
        /// 在 Windows 10 版本 1703 中引入。 在 AppContainer 进程内运行时或提供 AppContainer 令牌时，指定将某些文件夹重定向到包中特定于 AppContainer 的位置。 此标志强制重定向 (通常不重定向) 用于打包进程的文件夹，并可用于在同一包中的 UWP 和打包应用之间共享文件。 此标志取代已弃用 KF_FLAG_FORCE_APPCONTAINER_REDIRECTION。
        /// </summary>
        KF_FLAG_FORCE_PACKAGE_REDIRECTION = 0x00020000,

        /// <summary>
        /// 在 Windows 10 版本 1703 中引入。 在打包进程内运行时或提供打包进程令牌时，指定某些文件夹重定向到包特定的位置。 此标志在应用它的位置上禁用重定向，而是返回未在打包进程中运行时将返回的路径。 此标志取代已弃用 的KF_FLAG_NO_APPCONTAINER_REDIRECTION。
        /// </summary>
        KF_FLAG_NO_PACKAGE_REDIRECTION = 0x00010000,

        /// <summary>
        /// 在 Windows 8 中引入。 此标志在 Windows 10 版本 1703 中已弃用。 请改用 KF_FLAG_FORCE_PACKAGE_REDIRECTION 。
        /// </summary>
        KF_FLAG_FORCE_APPCONTAINER_REDIRECTION = KF_FLAG_FORCE_PACKAGE_REDIRECTION,

        /// <summary>
        /// 在 Windows 8 中引入。 此标志在 Windows 10 版本 1703 中已弃用。 请改 用 KF_FLAG_NO_PACKAGE_REDIRECTION 。
        /// </summary>
        KF_FLAG_NO_APPCONTAINER_REDIRECTION = KF_FLAG_FORCE_PACKAGE_REDIRECTION,

        /// <summary>
        /// 指定强制创建指定文件夹（如果该文件夹尚不存在）。 应用为该文件夹预定义的安全预配。 如果文件夹不存在且无法创建，则该函数将返回失败代码，并且不会返回任何路径。
        /// </summary>
        KF_FLAG_CREATE = 0x00008000,

        /// <summary>
        /// 指定在尝试检索路径或 IDList 之前不验证文件夹是否存在。 如果未设置此标志，则尝试验证路径中是否确实存在该文件夹。 如果由于文件夹不存在或无法访问而导致验证失败，则该函数将返回失败代码，并且不会返回任何路径。
        /// 如果文件夹位于网络上，则执行函数可能需要更长的时间。 因此，设置此标志可以减少延迟。
        /// </summary>
        KF_FLAG_DONT_VERIFY = 0x00004000,

        /// <summary>
        /// 指定在不使用环境字符串的情况下将完整路径存储在注册表中。 如果未设置此标志，则路径的某些部分可能由环境字符串（如 %USERPROFILE%）表示。 此标志只能与 SHSetKnownFolderPath 和 IKnownFolder：：SetPath 一起使用。
        /// </summary>
        KF_FLAG_DONT_UNEXPAND = 0x00002000,

        /// <summary>
        /// 指定检索文件夹的真实系统路径，其中不含 SHGetKnownFolderIDList 和 IKnownFolder：：GetIDList 返回的任何别名占位符，例如 %USERPROFILE%。 此标志对 SHGetKnownFolderPath 和 IKnownFolder：：GetPath 返回的路径没有影响。 默认情况下，已知文件夹检索函数和方法返回别名路径（如果存在别名）。
        /// </summary>
        KF_FLAG_NO_ALIAS = 0x00001000,

        /// <summary>
        /// 指定使用文件夹 Desktop.ini 的设置初始化文件夹。 如果无法初始化文件夹，则该函数将返回失败代码，并且不会返回任何路径。 此标志应始终与 KF_FLAG_CREATE 结合使用。
        /// </summary>
        KF_FLAG_INIT = 0x00000800,

        /// <summary>
        /// 指定检索已知文件夹的默认路径。 如果未设置此标志，则该函数将检索文件夹的当前路径（可能重定向）。 除非设置了 KF_FLAG_DONT_VERIFY ，否则此标志的执行包括验证文件夹是否存在。
        /// </summary>
        KF_FLAG_DEFAULT_PATH = 0x00000400,

        /// <summary>
        /// 指定检索文件夹的默认路径，而不依赖于其父级的当前位置。 还必须设置KF_FLAG_DEFAULT_PATH 。
        /// </summary>
        KF_FLAG_NOT_PARENT_RELATIVE = 0x00000200,

        /// <summary>
        /// 指定生成简单 IDList (PIDL) 。 如果要检索文件系统路径，则可以使用此值。 但是，如果要检索文件夹的本地化显示名称，请不要指定此值，因为它可能无法正确解析。
        /// </summary>
        KF_FLAG_SIMPLE_IDLIST = 0x00000100,

        /// <summary>
        /// 在 Windows 7 中引入。 指定仅检索别名的 PIDL。 请勿使用文件系统路径。
        /// </summary>
        KF_FLAG_ALIAS_ONLY = 0x80000000
    }
}
