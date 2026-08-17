namespace GetStoreAppInstaller.Models
{
    /// <summary>
    /// 应用程序模型
    /// </summary>
    internal class ApplicationModel
    {
        /// <summary>
        /// 应用描述
        /// </summary>
        internal string AppDescription { get; set; }

        /// <summary>
        /// 应用程序用户模型 ID
        /// </summary>
        internal string AppUserModelId { get; set; }

        /// <summary>
        /// 应用入口点
        /// </summary>
        internal string EntryPoint { get; set; }

        /// <summary>
        /// 应用可执行文件
        /// </summary>
        internal string Executable { get; set; }

        /// <summary>
        /// 应用 ID
        /// </summary>
        internal string AppID { get; set; }
    }
}
