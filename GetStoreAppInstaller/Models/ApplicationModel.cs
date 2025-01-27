namespace GetStoreAppInstaller.Models
{
    /// <summary>
    /// 应用程序模型
    /// </summary>
    public class ApplicationModel
    {
        /// <summary>
        /// 应用描述
        /// </summary>
        public string AppDescription { get; set; }

        /// <summary>
        /// 应用程序用户模型 ID
        /// </summary>
        public string AppUserModelId { get; set; }

        /// <summary>
        /// 应用入口点
        /// </summary>
        public string EntryPoint { get; set; }

        /// <summary>
        /// 应用可执行文件
        /// </summary>
        public string Executable { get; set; }

        /// <summary>
        /// 应用 ID
        /// </summary>
        public string AppID { get; set; }
    }
}
