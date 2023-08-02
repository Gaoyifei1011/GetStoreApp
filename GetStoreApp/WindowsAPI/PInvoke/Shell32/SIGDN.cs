using GetStoreApp.WindowsAPI.ComTypes;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 请求通过 <see cref="IShellItem.GetDisplayName"> 和 SHGetNameFromIDList 检索项的显示名称的形式。
    /// </summary>
    public enum SIGDN : uint
    {
        /// <summary>
        /// 返回相对于父文件夹的显示名称。在 UI 中，此名称通常非常适合向用户显示。
        /// </summary>
        SIGDN_NORMALDISPLAY = 0x00000000,

        /// <summary>
        /// 返回相对于父文件夹的分析名称。此名称不适合在 UI 中使用。
        /// </summary>
        SIGDN_PARENTRELATIVEPARSING = 0x80018001,

        /// <summary>
        /// 返回相对于桌面的分析名称。此名称不适合在 UI 中使用。
        /// </summary>
        SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,

        /// <summary>
        /// 返回相对于父文件夹的编辑名称。在 UI 中，此名称适合向用户显示。
        /// </summary>
        SIGDN_PARENTRELATIVEEDITING = 0x80031001,

        /// <summary>
        /// 返回相对于桌面的编辑名称。在 UI 中，此名称适合向用户显示。
        /// </summary>
        SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,

        /// <summary>
        /// 返回项的文件系统路径（如果有）。只有报告 <see cref="SFGAO.SFGAO_FILESYSTEM"> 具有文件系统路径的项目。当项目没有文件系统路径时，对该项的 <see cref="IShellItem.GetDisplayName"> 的调用将失败。在某些情况下，在 UI 中，此名称适合向用户显示，但请注意，可能未为所有项指定此名称。
        /// </summary>
        SIGDN_FILESYSPATH = 0x80058000,

        /// <summary>
        /// 返回项的 URL（如果有）。某些项没有 URL，在这些情况下，对 <see cref="IShellItem.GetDisplayName"> 的调用将失败。在某些情况下，此名称适合向用户显示，但请注意，可能未为所有项目指定此名称。
        /// </summary>
        SIGDN_URL = 0x80068000,

        /// <summary>
        /// 以地址栏中显示的友好格式返回相对于父文件夹的路径。此名称适合向用户显示。
        /// </summary>
        SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,

        /// <summary>
        /// 返回相对于父文件夹的路径。
        /// </summary>
        SIGDN_PARENTRELATIVE = 0x80080001,

        /// <summary>
        /// 在 Windows 8 中引入。
        /// </summary>
        SIGDN_PARENTRELATIVEFORUI = 0x80094001,
    }
}
