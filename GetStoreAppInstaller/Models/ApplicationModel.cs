namespace GetStoreAppInstaller.Models
{
    /// <summary>
    /// 应用程序模型
    /// </summary>
    public class ApplicationModel
    {
        public string AppDescription { get; set; }

        public string EntryPoint { get; set; }

        public string Executable { get; set; }

        public string AppID { get; set; }
    }
}
